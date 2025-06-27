using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static SQGame.Physics.PhysicsComponent;
using SQGame.Logic.Target;
using System.Runtime.CompilerServices;
using SQLib.Collections.Unmanaged;

namespace SQGame.Physics
{
    public class DiscreteSweepCollisionSystem : IDisposable
    {
        // [Fields]
        // ****************************************************************************************************
        public const int COMPONENT_LIMIT = 16384;
        public const int ITERATION_COUNT = 2; // At iteration 1, there's a fair bit of jiggling at 2k collisions. With 2 iterations, even at 5k the jiggling is minor. Going from 1 to 2 drops around 10% fps. 
        public bool IsProcessing { get; private set; }

        // Components and Indices
        private unsafe PhysicsComponent* components;
        StructReferenceArray<PhysicsComponent> componentsArray;

        private HashSet<Tuple<int, int>> currentOverlaps;
        private HashSet<Tuple<int, int>> previousOverlaps;
        public Action<PhysicsOverlap, int, int> OverlapHandler { private get; set; }

        // [Constructors]
        // ****************************************************************************************************
        public DiscreteSweepCollisionSystem(Action<PhysicsOverlap, int, int> overlapHandler = null)
        {
            unsafe
            {
                componentsArray = new(COMPONENT_LIMIT, COMPONENT_SIZE);
                components = componentsArray.Pointer;
            }

            currentOverlaps = new();
            previousOverlaps = new();
            OverlapHandler = overlapHandler;
        }

        // [Finalizer]
        // ****************************************************************************************************
        private bool disposed = false;

        ~DiscreteSweepCollisionSystem()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing) componentsArray.Dispose();
            disposed = true;
        }

        // [Methods]
        // ****************************************************************************************************
        public int AddComponent(
            PhysicsShape shape, float shapeParameter1, float shapeParameter2,
            float positionX, float positionY, float rotation,
            byte layer, byte mask,
            bool isCollideable, bool enableEnteredMonitoring, bool enableExitedMonitoring, bool enableContinuousMonitoring
        )
        {
            int offset = componentsArray.Add(default);

            unsafe { (components + offset)->Id = offset; }
            SetShape(offset, shape, shapeParameter1, shapeParameter2);
            SetPosition(offset, positionX, positionY);
            SetRotation(offset, rotation);
            SetCollisions(offset, isCollideable, layer, mask);
            SetMonitors(offset, enableEnteredMonitoring, enableExitedMonitoring, enableContinuousMonitoring);
            SetActive(offset, true);
            return offset;
        }

        public int AddComponent(PhysicsBlueprint blueprint, EntityTransform transform) => AddComponent(
            blueprint.Shape, blueprint.ShapeParameter1, blueprint.ShapeParameter2,
            transform.Position.X, transform.Position.Y, transform.Rotation,
            blueprint.CollisionLayers, blueprint.CollisionMasks,
            blueprint.IsCollideable, blueprint.enableEnterMonitor, blueprint.enableExitMonitor, blueprint.enableContinuousMonitor
            );

        public void RemoveComponent(int id)
        {
            SetActive(id, false);
        }


        public void SetShape(int id, PhysicsShape shape, float shapeParameter1, float shapeParameter2)
        {
            unsafe
            {
                PhysicsComponent* component = TryGetComponent(id);
                component->Shape = shape;
                component->ShapeParameter1 = shapeParameter1;
                component->ShapeParameter2 = shapeParameter2;
            }
        }

        public void SetPosition(int id, float positionX, float positionY)
        {
            unsafe
            {
                PhysicsComponent* component = TryGetComponent(id);
                component->PositionX = positionX;
                component->PositionY = positionY;
            }
        }

        public void SetRotation(int id, float rotation)
        {
            unsafe
            {
                TryGetComponent(id)->Rotation = rotation % 6.28318530718f;
            }
        }

        public void SetCollisions(int id, bool isCollideable, byte layer, byte mask)
        {
            unsafe
            {
                PhysicsComponent* component = TryGetComponent(id);
                component->Flags = isCollideable ? component->Flags | PhysicsFlags.IsCollideable : ~(~component->Flags | PhysicsFlags.IsCollideable);
                component->Layer = layer;
                component->Mask = mask;
            }
        }

        public void SetActive(int id, bool isActive)
        {
            unsafe
            {
                PhysicsComponent* component = TryGetComponent(id);
                component->Flags = isActive ? component->Flags | PhysicsFlags.IsActive : ~(~component->Flags | PhysicsFlags.IsActive);
            }
        }

        public void SetMonitors(int id, bool enableEnteredMonitoring, bool enableExitedMonitoring, bool enableContinuousMonitoring)
        {
            unsafe
            {
                PhysicsComponent* component = TryGetComponent(id);
                component->Flags = enableEnteredMonitoring ? component->Flags | PhysicsFlags.IsMonitoringEntered : ~(~component->Flags | PhysicsFlags.IsMonitoringEntered);
                component->Flags = enableContinuousMonitoring ? component->Flags | PhysicsFlags.IsMonitoringExited : ~(~component->Flags | PhysicsFlags.IsMonitoringExited);
                component->Flags = enableExitedMonitoring ? component->Flags | PhysicsFlags.IsMonitoringContinuous : ~(~component->Flags | PhysicsFlags.IsMonitoringContinuous);
            }
        }

        public Vector2 GetPosition(int id)
        {
            Vector2 positions;
            unsafe
            {
                PhysicsComponent* component = TryGetComponent(id);
                positions.X = component->PositionX;
                positions.Y = component->PositionY;
            }
            return positions;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe PhysicsComponent* TryGetComponent(int id)
        {
            if (id < 0 || componentsArray.MaxIndex < id)
            {
                throw new IndexOutOfRangeException($"No component with id {id} exists.");
            }

            return components + *(componentsArray.OffsetPointer + id);
        } 

        // [Methods - Sort And Sweep]
        // ****************************************************************************************************
        // For each component, this method checks for overlapping bounds on the X-Axis. Then runs the collision logic. 
        // This reduces the number of collision pairs that need to be checked, which reduces the amount of collision logic that needs to happen. 
        // This is at the cost of requiring sorting the X-Axis. But insertion sorting can handle near-sorted arrays very performantly. 

        // The system works as follows:
        //      - Applies the target position to the component.
        //      - Checks for overlaps and adds slight offsets to the motion to reduce overlap.
        //      - Writes the result to the position of the component.
        public void Process()
        {
            if (IsProcessing)
            {
                return;
            }

            IsProcessing = true;

            unsafe
            {
                currentOverlaps = new();

                // Overlaps are not fully resolvable in one iteration
                for (int iterations = 0; iterations < ITERATION_COUNT; iterations++)
                {
                    // Calculate bounds
                    Parallel.For(0, componentsArray.MaxIndex + 1, i =>
                    {
                        CalculateBounds(components + i);
                    });

                    // ****** SORTING BY X-AXIS ******
                    // Sorting is needed to make sweeping efficient. Without a sorted axis, sweep will have to compare every component against each other, rather than just extend the sweep sideways until bounds are reached. 
                    // The method used insertion sorting, as most of the components won't be changing movement that much on every process/frame, so array will stay mostly ordered, which is O(n).

                    //  Insertion sort with the following behavior:
                    //
                    //      For each element k in slots 1..n, first check whether el[k] >= el[k-1]. If so, go to next element. (Obviously skip the first element.)
                    //      If not, use binary-search in elements 1..k-1 to determine the insertion location, then scoot the elements over. (You might do this only if k>T where T is some threshold value; with small k this is overkill.)
                    //
                    // Source: https://stackoverflow.com/questions/220044/which-sort-algorithm-works-best-on-mostly-sorted-data

                    for (int i = 1; i <= componentsArray.MaxIndex; i++)
                    {
                        // Hold onto current i value and indice
                        PhysicsComponent current = *(components + i);

                        for (int k = i; k >= 0; k--)
                        {
                            // Left is smaller than held element, or we are at the first element of the array. Or the comparison is inactive.
                            // Insert the held values into this slot.
                            if ((components + k - 1)->LeftBound < current.LeftBound || k == 0)
                            {
                                *(components + k) = current;
                                *(componentsArray.OffsetPointer + current.Id) = k; // The new offset of the held element is at k.
                                break;
                            }
                            // Left is bigger than held element. Move left over to one and iterate.
                            else
                            {
                                *(components + k) = *(components + k - 1);
                                *(componentsArray.OffsetPointer + (components + k - 1)->Id) += 1; // Move one to the right, so increment the previous offset by 1
                            }
                        }
                    }

                    // ****** COLLISION ******
                    // For each component, this sweeps through the sorted X positions, and finds both the leftmost and rightmost components that may be overlapping. 
                    // Then it performs prune 
                    Parallel.For(0, componentsArray.MaxIndex + 1, i =>
                    {
                        PhysicsComponent* component = components + i;

                        // Skips if not IsActive flag
                        if ((component->Flags & PhysicsFlags.IsActive) == 0)
                        {
                            return;
                        }

                        component->CollisionOffsetX = 0;
                        component->CollisionOffsetY = 0;

                        // ****** SWEEP - REQUIRES SORTED COMPONENTS ARRAY ******
                        // 'Sweep' gets a subset of components that are possibly colliding with this one. 

                        for (int left = 1; left < i; left++)
                        {
                            if ((component - left)->RightBound < component->LeftBound)
                            {
                                break;
                            }

                            SolveCollision(component, component - left);
                        }

                        for (int right = 1; right <= componentsArray.MaxIndex - i; right++)
                        {
                            if (component->RightBound < (component + right)->LeftBound)
                            {
                                break;
                            }

                            SolveCollision(component, component + right);
                        }

                        component->PositionX += component->CollisionOffsetX;
                        component->PositionY += component->CollisionOffsetY;
                    });
                }
            }

            IsProcessing = false;

            
            unsafe
            {
                // Overlaps callbacks

                // "Entered" callback (If there was no overlap on previous frame)
                foreach (Tuple<int, int> overlap in currentOverlaps)
                {
                    // Entered 
                    if (!previousOverlaps.Contains(overlap) && ((components + *(componentsArray.OffsetPointer + overlap.Item1))->Flags & PhysicsFlags.IsMonitoringEntered) != 0)
                    {
                        OverlapHandler.Invoke(PhysicsOverlap.Entered, overlap.Item1, overlap.Item2);
                    }
                }

                // "Exited" callback
                foreach (Tuple<int, int> overlap in previousOverlaps)
                {
                    if (!currentOverlaps.Contains(overlap) && ((components + *(componentsArray.OffsetPointer + overlap.Item1))->Flags & PhysicsFlags.IsMonitoringExited) != 0)
                    {
                        OverlapHandler.Invoke(PhysicsOverlap.Exited, overlap.Item1, overlap.Item2);
                    }

                    // Continuous monitoring (Exists in both previous and current)
                    if (currentOverlaps.Contains(overlap) && ((components + *(componentsArray.OffsetPointer + overlap.Item1))->Flags & PhysicsFlags.IsMonitoringContinuous) != 0)
                    {
                        OverlapHandler.Invoke(PhysicsOverlap.Colliding, overlap.Item1, overlap.Item2);
                    }
                }

                previousOverlaps = currentOverlaps;
            }

        }

        /// <summary>
        /// Checks for collision, and resolves the overlap between two components.
        /// Adds the offsets to the offsetX, offsetY of the current component. 
        /// </summary>
        private unsafe void SolveCollision(PhysicsComponent* component, PhysicsComponent* comparison)
        {
            // Skips if not IsActive flag
            if ((comparison->Flags & PhysicsFlags.IsActive) == 0)
            {
                return;
            }

            // Check that the current component's mask overlaps with the comparison's layers
            if ((component->Mask & comparison->Layer) == 0)
            {
                return;
            }

            // Check that Y bounds is also overlapping
            if (comparison->TopBound < component->BottomBound || component->TopBound < comparison->BottomBound)
            {
                return;
            }

            // Offsets is the total displacement (and direction) required for both components to no longer overlap. 
            Vector2 offsets = GetOffsets(component, comparison);

            // Skips if there was no overlap
            if (offsets.X == 0 && offsets.Y == 0)
            {
                return;
            }

            // Adds overlap callback if the component is monitoring
            if (
                (component->Flags & PhysicsFlags.IsMonitoringEntered) != 0 ||
                (component->Flags & PhysicsFlags.IsMonitoringExited) != 0 ||
                (component->Flags & PhysicsFlags.IsMonitoringContinuous) != 0
                )
            {
                lock (currentOverlaps)
                {
                    currentOverlaps.Add(new Tuple<int, int>(component->Id, comparison->Id));
                }
            }

            // Skips applying if the current component is not collideable
            if ((component->Flags & PhysicsFlags.IsCollideable) == 0)
            {
                return;
            }
            // Applies the full offset if the comparison component is not collideable
            else if ((comparison->Flags & PhysicsFlags.IsCollideable) == 0)
            {
                component->CollisionOffsetX += offsets.X;
                component->CollisionOffsetY += offsets.Y;
            }
            // Applies half the offset if both components are collideable (Both will be offset)
            else
            {
                component->CollisionOffsetX += offsets.X / 2;
                component->CollisionOffsetY += offsets.Y / 2;
            }
        }

        private unsafe Vector2 GetOffsets(PhysicsComponent* component, PhysicsComponent* comparison)
        {
            Vector2 offset;
            offset.X = 0;
            offset.Y = 0;

            switch (component->Shape)
            {
                case PhysicsShape.Circle:

                    switch (comparison->Shape)
                    {
                        case PhysicsShape.Circle:
                            return PhysicsCollisionCalculations.CircleCircleOffset(
                                component->PositionX, component->PositionY, component->ShapeParameter1, component->Rotation,
                                comparison->PositionX, comparison->PositionY, comparison->ShapeParameter1, comparison->Rotation
                                );

                        case PhysicsShape.Rectangle:
                            return offset;

                        case PhysicsShape.Cone:
                            return offset;
                    }
                    break;
            }

            return offset;

        }
    }
}