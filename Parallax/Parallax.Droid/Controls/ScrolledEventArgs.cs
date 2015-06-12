using System;

namespace Parallax.Droid.Controls
{
    public class ScrolledEventArgs : EventArgs
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int OldX { get; set; }
        public int OldY { get; set; }
    }
}