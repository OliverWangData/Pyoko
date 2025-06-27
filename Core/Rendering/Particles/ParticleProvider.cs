using Godot;
using SQGame.Singletons;

namespace SQGame.Particles
{
    public static class ParticleProvider
    {
        public static Rid Get()
        {
            Rid particles = RenderingServer.ParticlesCreate();
            RenderingServer.ParticlesSetMode(particles, RenderingServer.ParticlesMode.Mode2D);
            return particles;
        }

        public static void Offset(Rid particles, Vector2 offset)
        {
            Basis basis = new(1, 0, 0, 0, 1, 0, 0, 0, 1);
            Transform3D transform = new(basis, new Vector3(offset.X, offset.Y, 0));
            RenderingServer.ParticlesSetEmissionTransform(particles, transform);
        }

        public static void Set(Rid canvas, Rid particles, int id)
        {
            Data.Particles data = GameData.Instance.Get<int, Data.Particles>(id);
            ParticleProcessMaterial processMaterial = GD.Load<ParticleProcessMaterial>(data.ResPPM);
            Texture2D texture = GD.Load<Texture2D>(data.ResTexture);

            Rid mesh = RenderingServer.MeshCreate();
            RenderingServer.MeshClear(mesh);
            Vector2 textureSize = texture.GetSize();
            Vector2[] points = new Vector2[4]
            {
                new Vector2(-textureSize.X / 2.0f, -textureSize.Y / 2.0f),
                new Vector2(textureSize.X / 2.0f, -textureSize.Y / 2.0f),
                new Vector2(textureSize.X / 2.0f, textureSize.Y / 2.0f),
                new Vector2(-textureSize.X / 2.0f, textureSize.Y / 2.0f)
            };
            Vector2[] uvs = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1,1),
                new Vector2(0, 1),
            };

            int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };
            Godot.Collections.Array array = new();
            array.Resize((int)RenderingServer.ArrayType.Max);
            array[(int)RenderingServer.ArrayType.Vertex] = points;
            array[(int)RenderingServer.ArrayType.TexUV] = uvs;
            array[(int)RenderingServer.ArrayType.Index] = indices;

            RenderingServer.MeshAddSurfaceFromArrays(mesh, RenderingServer.PrimitiveType.Triangles, array, compressFormat: RenderingServer.ArrayFormat.FlagUse2DVertices);
            RenderingServer.ParticlesSetTrailBindPoses(particles, new Godot.Collections.Array<Transform3D>());
            RenderingServer.ParticlesSetDrawPasses(particles, 1);
            RenderingServer.ParticlesSetDrawPassMesh(particles, 0, mesh);

            RenderingServer.ParticlesSetProcessMaterial(particles, processMaterial.GetRid());
            RenderingServer.ParticlesSetOneShot(particles, data.OneShot);
            RenderingServer.ParticlesSetAmount(particles, data.Amount);
            RenderingServer.ParticlesSetAmountRatio(particles, 1);
            RenderingServer.ParticlesSetLifetime(particles, data.Lifetime);
            RenderingServer.ParticlesSetFractionalDelta(particles, true);
            RenderingServer.ParticlesSetInterpolate(particles, true);
            RenderingServer.ParticlesSetPreProcessTime(particles, 0);
            RenderingServer.ParticlesSetExplosivenessRatio(particles, data.Explosiveness);
            RenderingServer.ParticlesSetRandomnessRatio(particles, data.Randomness);
            //Aabb aabb = new Aabb(new Vector3(CharacterServer.Visibility.Position.X, CharacterServer.Visibility.Position.Y, 0), new Vector3(CharacterServer.Visibility.Size.X, CharacterServer.Visibility.Size.Y, 0));
            //RenderingServer.ParticlesSetCustomAabb(particles, aabb);
            RenderingServer.ParticlesSetUseLocalCoordinates(particles, data.LocalCoordinates);
            RenderingServer.ParticlesSetDrawOrder(particles, RenderingServer.ParticlesDrawOrder.Index);
            RenderingServer.ParticlesSetSpeedScale(particles, 1);

            RenderingServer.CanvasItemAddParticles(canvas, particles, texture.GetRid());



            /*
            Rid cvit = RenderingServer.CanvasItemCreate();
            cvs.Add(cvit);
            RenderingServer.CanvasItemSetParent(cvit, renderingCanvas);
            Rid particles = RenderingServer.ParticlesCreate();
            particlesCache.Add(particles);
            RenderingServer.ParticlesSetMode(particles, RenderingServer.ParticlesMode.Mode2D);

            ParticleProcessMaterial ppm = GD.Load<ParticleProcessMaterial>("res://Particles/PPM_Fire.tres");
            peepeem.Add(ppm);
            Rid part = ppm.GetRid();
            partsss.Add(part);
            RenderingServer.ParticlesSetProcessMaterial(particles, part);

            Rid mesh = RenderingServer.MeshCreate();
            meshesCache.Add(mesh);
            RenderingServer.MeshClear(mesh);
            Vector2[] points = new Vector2[4]
            {
                new Vector2(-1 / 2.0f, -1 / 2.0f),
                new Vector2(1 / 2.0f, -1 / 2.0f),
                new Vector2(1 / 2.0f, 1 / 2.0f),
                new Vector2(-1 / 2.0f, 1 / 2.0f)
            };
            Vector2[] uvs = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1,1),
                new Vector2(0, 1),
            };

            int[] indices = new int[6] { 0, 1, 2, 0, 2, 3 };
            Godot.Collections.Array array = new();
            array.Resize((int)RenderingServer.ArrayType.Max);
            array[(int)RenderingServer.ArrayType.Vertex] = points;
            array[(int)RenderingServer.ArrayType.TexUV] = uvs;
            array[(int)RenderingServer.ArrayType.Index] = indices;


            RenderingServer.MeshAddSurfaceFromArrays(mesh, RenderingServer.PrimitiveType.Triangles, array, compressFormat: RenderingServer.ArrayFormat.FlagUse2DVertices);
            RenderingServer.ParticlesSetTrailBindPoses(particles, new Godot.Collections.Array<Transform3D>());
            RenderingServer.ParticlesSetDrawPasses(particles, 1);
            RenderingServer.ParticlesSetDrawPassMesh(particles, 0, mesh);



            RenderingServer.ParticlesSetEmitting(particles, true);
            RenderingServer.ParticlesSetOneShot(particles, false);
            RenderingServer.ParticlesSetAmount(particles, 48);
            RenderingServer.ParticlesSetAmountRatio(particles, 1);
            RenderingServer.ParticlesSetLifetime(particles, 0.5f);
            RenderingServer.ParticlesSetFixedFps(particles, 0);
            RenderingServer.ParticlesSetFractionalDelta(particles, true);
            RenderingServer.ParticlesSetInterpolate(particles, true);
            RenderingServer.ParticlesSetPreProcessTime(particles, 0);
            RenderingServer.ParticlesSetExplosivenessRatio(particles, 0);
            RenderingServer.ParticlesSetRandomnessRatio(particles, 0);
            Aabb aabb = new Aabb(new Vector3(CharacterServer.Visibility.Position.X, CharacterServer.Visibility.Position.Y, 0), new Vector3(CharacterServer.Visibility.Size.X, CharacterServer.Visibility.Size.Y, 0));
            RenderingServer.ParticlesSetCustomAabb(particles, aabb);
            RenderingServer.ParticlesSetUseLocalCoordinates(particles, false);
            RenderingServer.ParticlesSetDrawOrder(particles, RenderingServer.ParticlesDrawOrder.Index);
            RenderingServer.ParticlesSetSpeedScale(particles, 1);
            RenderingServer.ParticlesSetFixedFps(particles, 30);

            Transform2D xf2d = new(0, new Vector2(1, -1));
            Transform3D xf = new();
            Basis basis = new();
            basis.Column0 = new Vector3(1, 0, 0);
            basis.Column1 = new Vector3(0, 1, 0);
            basis.Column2 = new Vector3(0, 0, 1);
            xf.Basis = basis;
            xf.Origin = new Vector3(xf2d.Origin.X, xf2d.Origin.Y, 0);
            RenderingServer.ParticlesSetEmissionTransform(particles, xf);

            Texture2D tex = GD.Load<Texture2D>("res://_lib/Sprites/Shapes/SPR_Dot.png");
            tx.Add(tex);
            Rid text = tex.GetRid();
            texturesss.Add(text);
            RenderingServer.CanvasItemAddParticles(cvit, particles, text);
            RenderingServer.ParticlesRestart(particles);
            RenderingServer.ParticlesSetEmitting(particles, true);

            GD.Print(RenderingServer.ParticlesGetEmitting(particles));
            GD.Print(!RenderingServer.ParticlesIsInactive(particles));*/
        }
    }
}