using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Drawing;
using System.Xml.Linq;

namespace Fullerenes.Server.Objects.CustomStructures.Octree
{
    public class Octree(int maxDepth, int threads, IRegion startRegion) : IOctree
    {
        private class Node(int threads)
        {
            public required IRegion Region { get; set; }
            public required int Depth { get; set; }
            public Node?[] Children { get; init; } = new Node?[8];
            public ICollection<Fullerene?>[] ObjectsArr { get; init; } = new List<Fullerene?>[threads];

            public void SubDivide(int depth, int threads)
            {
                var subRegions = Region.Split8Parts();

                for (int i = 0; i < subRegions.Length; i++)
                    Children[i] = new Node(threads) { Depth = depth, Region = subRegions[i] };
            }
        }

        public int MaxDepth { get; set; } = maxDepth;
        public int ThreadsNumber { get; set; } = threads;

        private readonly Node _root = new Node(threads) { Depth = 0, Region = startRegion};

        public bool Add(Fullerene fullerene, int thread) => Insert(fullerene, thread, _root);

        private bool Insert(Fullerene fullerene, int thread, Node node)
        {
            if (!node.Region.Contains(fullerene))
                return false;

            if (node.ObjectsArr[thread] is not null && 
                node.ObjectsArr[thread]
                .AsParallel()
                .Any(fullerene.Intersect))
                return false;

            if(node.Depth == MaxDepth)
            {
                node.ObjectsArr[thread].Add(fullerene);
                return true;
            }

            if (node.Children.Any(child => child is null))
                node.SubDivide(node.Depth++, ThreadsNumber);

            foreach (var child in node.Children)
                if (Insert(fullerene, thread, child))
                    return true;

            if (node.ObjectsArr[thread] is null)
                node.ObjectsArr[thread] = [fullerene];
            else
                node.ObjectsArr[thread].Add(fullerene);

            return true;
        }


    }
}
