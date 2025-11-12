namespace PROG7312_POE_PART1.Models
{
    public class PriorityQueue<T>
    {
        // Internal min-heap storage item, priority (Lagu, 2024)
        private readonly List<(T item, int priority)> _heap = new List<(T, int)>();

        // Number of elements in the queue
        public int Count => _heap.Count;

        // Add item with given priority (Lagu, 2024)
        public void Enqueue(T item, int priority)
        {
            _heap.Add((item, priority));
            // Restore heap property (Lagu, 2024)
            HeapifyUp(_heap.Count - 1); 
        }

        // Remove and return item with smallest priority
        public T Dequeue()
        {
            if (_heap.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }

            var root = _heap[0].item;
            var last = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            if (_heap.Count > 0)
            {
                _heap[0] = last;
                // Restoring heap property 
                HeapifyDown(0); 
            }

            return root;
        }

        // Peek at item with smallest priority without removing
        public T Peek()
        {
            if (_heap.Count == 0)
            {
                throw new InvalidOperationException("Queue is empty.");
            }
            return _heap[0].item;
        }

        // Moving element at index up to maintain min-heap (Oumghar, 2025)
        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_heap[index].priority >= _heap[parent].priority)
                {
                    break;
                }
                (_heap[index], _heap[parent]) = (_heap[parent], _heap[index]);
                index = parent;
            }
        }

        // Moving element at index down to maintain min-heap (Oumghar, 2025)
        private void HeapifyDown(int index)
        {
            while (true)
            {
                int left = index * 2 + 1;
                int right = index * 2 + 2;
                int smallest = index;

                if (left < _heap.Count && _heap[left].priority < _heap[smallest].priority)
                {
                    smallest = left;
                }
                if (right < _heap.Count && _heap[right].priority < _heap[smallest].priority)
                {
                    smallest = right;
                }
                if (smallest == index) break;

                (_heap[index], _heap[smallest]) = (_heap[smallest], _heap[index]);
                index = smallest;
            }
        }
    }
}
