using Godot;
using SQLib.Events;
using System;

namespace SQLib.GDEngine.Events
{
    [GlobalClass]
    public partial class CallbackResourceInt : Resource
    {
        public Callback<int> Event = new();
    }
}