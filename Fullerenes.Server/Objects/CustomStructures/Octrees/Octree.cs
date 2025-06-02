using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using Iced.Intel;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Collections.Concurrent;
using System.Drawing;
using System.Xml.Linq;

namespace Fullerenes.Server.Objects.CustomStructures.Octree
{
    public class Octree : IOctree
    {
        private class Node
        {
            public required CubeRegion Region { get; set; }
            public required int Depth { get; set; }
            public Node?[] Children { get; init; } = new Node?[8];
            public ICollection<Fullerene> Objects { get; init; } = new List<Fullerene>(1000);

            public void SubDivide(int depth)
            {
                var subRegions = Region.Split8Parts();

                for (int i = 0; i < subRegions.Length; i++)
                    Children[i] = new Node() { Depth = depth + 1, Region = subRegions[i] };
            }
        }

        public int MaxDepth { get; set; }

        private readonly Node _root;

        public Octree(int maxDepth, CubeRegion startRegion)
        {
            MaxDepth = maxDepth;
            _root = new Node() { Depth = 0, Region = startRegion };
        }

        public bool Add(Fullerene fullerene) => Insert(fullerene);

        private bool Insert(Fullerene fullerene)
        {
            var stack = new Stack<Node>();
            stack.Push(_root);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (!current.Region.Contains(fullerene))
                    continue;

                if (current.Objects is not null && current.Objects.AsParallel().Any(fullerene.Intersect))
                    return false;

                if (current.Depth == MaxDepth)
                {
                    current.Objects.Add(fullerene);

                    return true;
                }

                if (current.Children.Any(child => child is null))
                    current.SubDivide(current.Depth);

                bool pushedToChild = false;
                foreach (var child in current.Children)
                {
                    if (child.Region.Contains(fullerene))
                    {
                        stack.Push(child);
                        pushedToChild = true;
                    }
                }

                if (!pushedToChild)
                {
                    current.Objects.Add(fullerene);

                    return true;
                }
            }

            return false;
        }
    }
}