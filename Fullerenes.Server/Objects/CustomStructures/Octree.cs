namespace Fullerenes.Server.Objects.CustomStructures
{
    public class Octree<TRegion, TData>(TRegion startRegion, int threadsNumber)
    {
        private class Node(Guid id, TRegion region, int threadsNumber)
        {
            private Guid Id { get; } = id;
            public TRegion Region { get; } = region;
            public ICollection<TData>?[] DataCollections { get; } = new ICollection<TData>[threadsNumber];
            public Node?[] Children { get; } = new Node[8];
            public void AddChildRegion(Node child, int id) => Children[id] = child;

            public (Guid id, string? region, Guid?[] children) GetRegionInfo()
            {
                ArgumentNullException.ThrowIfNull(Region);

                return (Id, Region.ToString(), Children.Select(child => child?.Id).ToArray());
            }
        }

        private readonly Node _head = new(Guid.NewGuid(), startRegion, threadsNumber);
        //private readonly ICollection<(Guid id, string dataJson, Guid[] children)> _regions = [];
        public int TreeSize { get; private set; }
        public void GenerateRegions(Func<TRegion, TRegion[]> splitRegion, Func<TRegion, bool> generateCondition)
        {
            ArgumentNullException.ThrowIfNull(splitRegion);
            ArgumentNullException.ThrowIfNull(generateCondition);

            var queue = new Queue<Node>();

            queue.Enqueue(_head);
            TreeSize++;

            while (queue.Any())
            {
                var nextInQueue = queue.Dequeue();

                if (generateCondition(nextInQueue.Region))
                {
                    Node[] children = new Node[8];
                    TRegion[] childrenValues = splitRegion(nextInQueue.Region);

                    for (int i = 0; i < childrenValues.Length; i++)
                    {
                        children[i] = new Node(Guid.NewGuid(), childrenValues[i], threadsNumber);

                        nextInQueue.AddChildRegion(children[i], i);

                        TreeSize++;

                        queue.Enqueue(children[i]);
                    }

                    //_regions.Add(nextInQueue.GetRegionInfo());
                }
            }
        }
        public bool AddData(TData inputData, int thread, Func<TData, bool> check, Func<TRegion, bool> inside, Func<TRegion, bool> partInside)
        {
            ArgumentNullException.ThrowIfNull(inputData);
            ArgumentNullException.ThrowIfNull(check);
            ArgumentNullException.ThrowIfNull(inside);
            ArgumentNullException.ThrowIfNull(partInside);

            var regionThatContainsData = _head;

            while (true)
            {
                var innerRegionThatContainsData = regionThatContainsData
                    .Children.FirstOrDefault(child => child != null && inside(child.Region));

                if (innerRegionThatContainsData == null) break;

                regionThatContainsData = innerRegionThatContainsData;
            }

            var queue = new Queue<Node>();
            queue.Enqueue(regionThatContainsData);

            while (queue.Any())
            {
                var region = queue.Dequeue();

                if (partInside(region.Region))
                {
                    region.DataCollections[thread] ??= [];

                    if (region.DataCollections[thread] is not null && region.DataCollections[thread]!.Any(check))
                        return false;

                    region.DataCollections[thread]?.Add(inputData);

                    foreach (var child in region.Children)
                        if (child != null)
                            queue.Enqueue(child);
                }
            }

            return true;
        }
        public void ClearAllThread()
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
        public void ClearSpecificThread(int thread)
        {
            var queue = new Queue<Node>();

            queue.Enqueue(_head);

            while (queue.Any())
            {
                var nextInQueue = queue.Dequeue();

                nextInQueue.DataCollections[thread]?.Clear();

                foreach (var child in nextInQueue.Children)
                    if (child != null)
                        queue.Enqueue(child);
            }
        }
    }
}
