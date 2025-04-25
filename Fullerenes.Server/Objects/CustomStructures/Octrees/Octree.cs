using Fullerenes.Server.Objects.CustomStructures.Octrees.Regions;
using Fullerenes.Server.Objects.Fullerenes;
using System.Drawing;

namespace Fullerenes.Server.Objects.CustomStructures.Octree
{
    public class Octree<TData>(IRegion startRegion, int threadsNumber = 1) : IOctree<TData>
        where TData : Fullerene
    {
        private class Node(Guid id, IRegion region, int threadsNumber)
        {
            private Guid Id { get; } = id;
            public IRegion Region { get; } = region;
            public ICollection<TData>?[] DataCollections { get; } = new ICollection<TData>[threadsNumber];
            public Node?[] Children { get; } = new Node[8];
            public void AddChildRegion(Node child, int id)
            {
                if (child is not null)
                    Children[id] = child;
            }
        }

        private readonly int _threadsNumber = threadsNumber;
        private readonly Node _head = new(Guid.NewGuid(), startRegion, threadsNumber);

        public void StartRegionGeneration(float maxFigureSize)
        {
            var queue = new Queue<Node>();
            queue.Enqueue(_head);

            while (queue.Any())
            {
                var nextInQueue = queue.Dequeue();

                if (nextInQueue.Region.CreateCondition(maxFigureSize))
                {
                    IRegion[] newRegions = nextInQueue.Region.Split8Parts();

                    for (int i = 0; i < newRegions.Length; i++)
                    {
                        var newNode = new Node(Guid.NewGuid(), newRegions[i], _threadsNumber);

                        nextInQueue.AddChildRegion(newNode, i);

                        queue.Enqueue(newNode);
                    }
                }
            }
        }
        public bool AddData(
            TData inputData, int thread,
            Func<TData, bool> checkIfDataCannotBeAdded)
        {
            ArgumentNullException.ThrowIfNull(inputData);
            ArgumentNullException.ThrowIfNull(checkIfDataCannotBeAdded);

            var queue = new Queue<Node>();
            queue.Enqueue(_head);

            var queueToAdd = new Queue<ICollection<TData>>();

            while (queue.Any())
            {
                var region = queue.Dequeue();

                if (region.Region.Contains(inputData) || region.Region.ContainsPart(inputData))
                {
                    var threadDataCollection = region.DataCollections[thread];

                    if (threadDataCollection == null)
                    {
                        threadDataCollection = [inputData];
                    }
                    else if (threadDataCollection.Any(checkIfDataCannotBeAdded))
                    {
                        return false;
                    }
                    else
                    {
                        //threadDataCollection.Add(inputData);
                        queueToAdd.Enqueue(threadDataCollection);
                    }

                    foreach (var child in region.Children)
                        if (child is not null)
                            queue.Enqueue(child);
                }
            }

            while (queueToAdd.Any())
            {
                queueToAdd.Dequeue().Add(inputData);
            }

            return true;
        }
        public void ClearThreadCollection(int thread)
        {
            var queue = new Queue<Node>();
            queue.Enqueue(_head);

            while (queue.Any())
            {
                var region = queue.Dequeue();

                var threadDataCollection = region.DataCollections[thread];

                if (threadDataCollection is not null)
                {
                    threadDataCollection.Clear();
                }

                foreach (var child in region.Children)
                    if (child != null)
                        queue.Enqueue(child);
            }
        }
        ~Octree()
        {
            var queue = new Queue<Node>();
            queue.Enqueue(_head);

            while (queue.Any())
            {
                var nextInQueue = queue.Dequeue();

                foreach (var dataCollection in nextInQueue.DataCollections)
                    dataCollection?.Clear();

                foreach (var child in nextInQueue.Children)
                    if (child is not null)
                        queue.Enqueue(child);
            }
        }
    }
}
