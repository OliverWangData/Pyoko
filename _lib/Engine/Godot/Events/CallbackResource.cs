using Godot;
using SQLib.Events;
using System;

namespace SQLib.GDEngine.Events
{
    [GlobalClass]
    public partial class CallbackResource : Resource
    {
        public Callback Event = new();
    }
}