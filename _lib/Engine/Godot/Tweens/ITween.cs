using Godot;
using SQLib.Events;
using System;

namespace SQLib.GDEngine.Tweens
{
    public interface ITween
    {
        public void Play();
        public Callback Completed { get; }
    }
}