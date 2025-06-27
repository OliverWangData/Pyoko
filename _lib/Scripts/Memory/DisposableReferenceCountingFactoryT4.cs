using Godot;
using SQLib.Memory;
using System;
using System.Collections.Generic;

namespace SQGame.Entities.Physics
{
    public abstract class DisposableReferenceCountingFactory<T, TParameter1, TParameter2, TParameter3> : DisposableReferenceCountingFactory<T, Tuple<TParameter1, TParameter2, TParameter3>> where T : IDisposableReference
    {
        // [Constructor]
        // ****************************************************************************************************
        public DisposableReferenceCountingFactory(Func<TParameter1, TParameter2, TParameter3, T> constructor) : base((paramtuple) => constructor(paramtuple.Item1, paramtuple.Item2, paramtuple.Item3)) { }

        // [Method]
        // ****************************************************************************************************
        public override T Create(Tuple<TParameter1, TParameter2, TParameter3> parameters) => Create(parameters.Item1, parameters.Item2, parameters.Item3);
        public virtual T Create(TParameter1 p1, TParameter2 p2, TParameter3 p3) => Create(new Tuple<TParameter1, TParameter2, TParameter3>(p1, p2, p3));
    }
}