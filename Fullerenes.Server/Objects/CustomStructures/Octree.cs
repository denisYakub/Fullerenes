using System.Drawing;
using System.Threading;

namespace Fullerenes.Server.Objects.CustomStructures
{
    public class Octree<TRegion, TData>(TRegion startRegion, int threadsNumber = 1)
    {
        private class Node(Guid id, TRegion region, int threadsNumber)
        {
            private Guid Id { get; } = id;
            public TRegion Region { get; } = region;
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
        public int CountNodes { get; private set; }
        public int CountElements { get; private set; }

        public void GenerateRegions(
            Func<TRegion, TRegion[]> splitRegion, 
            Func<TRegion, bool> generateRegionCondition)
        {
            ArgumentNullException.ThrowIfNull(splitRegion);
            ArgumentNullException.ThrowIfNull(generateRegionCondition);

            var queue = new Queue<Node>();
            queue.Enqueue(_head);

            CountNodes++;

            while (queue.Any())
            {
                var nextInQueue = queue.Dequeue();

                if (generateRegionCondition(nextInQueue.Region))
                {
                    TRegion[] newRegions = splitRegion(nextInQueue.Region);

                    for (int i = 0; i < newRegions.Length; i++)
                    {
                        var newNode = new Node(Guid.NewGuid(), newRegions[i], _threadsNumber);

                        nextInQueue.AddChildRegion(newNode, i); 
                        CountNodes++;

                        queue.Enqueue(newNode);
                    }
                }
            }
        }
        public bool AddData(
            TData inputData,
            int thread,
            Func<TData, bool> checkIfDataCannotBeAdded,
            Func<TRegion, bool> checkIfDataInsideRegion,
            Func<TRegion, bool> checkIfDatasPartInsideRegion)
        {
            ArgumentNullException.ThrowIfNull(inputData);
            ArgumentNullException.ThrowIfNull(checkIfDataCannotBeAdded);
            ArgumentNullException.ThrowIfNull(checkIfDataInsideRegion);
            ArgumentNullException.ThrowIfNull(checkIfDatasPartInsideRegion);

            var regionThatContainsData = _head;

            while (true)
            {
                var nextRegionThatContainsData = regionThatContainsData
                    .Children.FirstOrDefault(child => 
                    child != null && 
                    checkIfDataInsideRegion(child.Region));

                if (nextRegionThatContainsData is null) break;

                regionThatContainsData = nextRegionThatContainsData;
            }

            var queue = new Queue<Node>();
            queue.Enqueue(regionThatContainsData);

            while (queue.Any())
            {
                var region = queue.Dequeue();

                if (checkIfDatasPartInsideRegion(region.Region))
                {
                    var threadDataCollection = region.DataCollections[thread];

                    if (threadDataCollection == null)
                    {
                        threadDataCollection = [ inputData ];
                        CountElements++;

                    } else if (threadDataCollection.Any(checkIfDataCannotBeAdded))
                    {
                        return false;

                    } else
                    {
                        threadDataCollection.Add(inputData);
                    }

                    foreach (var child in region.Children)
                        if (child is not null)
                            queue.Enqueue(child);
                }
            }

            return true;
        }
        public void ClearCurrentThreadCollection(int thread)
        {
            var queue = new Queue<Node>();
            queue.Enqueue(_head);

            while (queue.Any())
            {
                var region = queue.Dequeue();

                var threadDataCollection = region.DataCollections[thread];

                if (threadDataCollection is not null)
                {
                    CountElements -= threadDataCollection.Count;
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

            CountElements = 0;
        }
    }
}
