using System;
using System.Linq;

namespace CG.Proj1.BaseClass
{
    public class OctoNode
    {
        public OctoNode[] Children { get; set; }
        public long ReferenceCount { get; set; }
        public long R { get; set; }
        public long G { get; set; }
        public long B { get; set; }
        public int Level { get; }
        public bool IsLeaf => !Children?.Any() ?? true;
        
        public OctoNode(int level)
        {
            ReferenceCount = 0;
            Level = level;
            if (level < 7)
                Children = Enumerable.Range(0,8).Select(x => new OctoNode(level+1)).ToArray();
        }

        public void InsertColor(byte r, byte g, byte b)
        {
            if (IsLeaf)
            {
                R += r;
                G += g;
                B += b;
                ReferenceCount++;
            }
            else
                Children[Index(r,g,b, Level)].InsertColor(r,g,b);   
        }
        
        private int Index(byte r, byte g, byte b, int level)
        {
            var x = ((byte) (r << level)) >> 7;
            var y = ((byte) (g << level)) >> 7;
            var z = ((byte) (b << level)) >> 7;

            return (x << 2) + (y << 1) + z;
        }

        public void Reduce(int stopLevel)
        {
            if (IsLeaf)
                return;
            
            foreach(var child in Children)
                child.Reduce(stopLevel);
            
            if (Level >= stopLevel)
                SumColorsFromChildren();

            if (Level == stopLevel)
            {
                R = R / (ReferenceCount != 0 ? ReferenceCount : 1);
                G = G / (ReferenceCount != 0 ? ReferenceCount : 1);
                B = B / (ReferenceCount != 0 ? ReferenceCount : 1);
            }
        }

        private void SumColorsFromChildren()
        {
            R = Children.Sum(c => c.R);
            G = Children.Sum(c => c.G);
            B = Children.Sum(c => c.B);
            ReferenceCount = Children.Sum(c => c.ReferenceCount);
            Children = null;
        }

        public (byte, byte, byte) GetColor(byte r, byte g, byte b)
        {
            return IsLeaf 
                ? ((byte)R, (byte)G, (byte)B) 
                : Children[Index(r, g, b, Level)].GetColor(r, g, b);
        }
    }
}