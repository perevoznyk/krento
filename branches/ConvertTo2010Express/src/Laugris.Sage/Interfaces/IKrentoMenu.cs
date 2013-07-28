using System;
using System.Collections.Generic;
using System.Text;

namespace Laugris.Sage
{
    public interface IKrentoMenu
    {
        IKrentoMenuItem AddMenuItem(EventHandler clickHandler);
        IKrentoMenuItem InsertMenuItem(int index, EventHandler clickHandler);
        int Count { get; }
        void Clear();
        IKrentoMenuItem this[int index] { get; }
        IKrentoMenuItem this[string name] { get; }
        int IndexOf(IKrentoMenuItem item);
        void Remove(IKrentoMenuItem item);
        void RemoveAt(int index);
    }
}
