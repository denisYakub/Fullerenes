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
            public ICollection<Fullerene?>[] ObjectsArr { get; init; }

            public Node(int threads)
            {
                ObjectsArr = new List<Fullerene?>[threads];
            }

            public void SubDivide(int depth, int threads)
            {
                var subRegions = Region.Split8Parts();

                for (int i = 0; i < subRegions.Length; i++)
                    Children[i] = new Node(threads) { Depth = depth + 1, Region = subRegions[i] };
            }
        }

        public int MaxDepth { get; set; }
        public int ThreadsNumber { get; set; }

        private readonly Node _root;

        public Octree(int maxDepth, int threads, CubeRegion startRegion)
        {
            MaxDepth = maxDepth;
            ThreadsNumber = threads;
            _root = new Node(threads) { Depth = 0, Region = startRegion };
        }

        public bool Add(Fullerene fullerene, int thread) => Insert(fullerene, thread);

        private bool Insert(Fullerene fullerene, int thread)
        {
            var stack = new Stack<Node>();
            stack.Push(_root);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (!current.Region.Contains(fullerene))
                    continue;

                if (current.ObjectsArr[thread] is not null && current.ObjectsArr[thread].AsParallel().Any(fullerene.Intersect))
                    return false;

                if (current.Depth == MaxDepth)
                {
                    if (current.ObjectsArr[thread] is null)
                        current.ObjectsArr[thread] = new List<Fullerene>(1000) { fullerene };
                    else
                        current.ObjectsArr[thread].Add(fullerene);

                    return true;
                }

                if (current.Children.Any(child => child is null))
                    current.SubDivide(current.Depth, ThreadsNumber);

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
                    if (current.ObjectsArr[thread] is null)
                        current.ObjectsArr[thread] = new List<Fullerene>(1000) { fullerene };
                    else
                        current.ObjectsArr[thread].Add(fullerene);

                    return true;
                }
            }

            return false;
        }
    }
}