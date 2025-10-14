namespace PROG7312_POE_PART1.Models
{
    public class EventQueue
    {
        public EventQueueNode Push(EventQueueNode head, int eventId, int score)
        {
            var node = new EventQueueNode { EventId = eventId, Score = score };

            if (head == null || score > head.Score)
            {
                node.Next = head;
                return node;
            }

            var current = head;
            while (current.Next != null && current.Next.Score > score)
            {
                current = current.Next;
            }

            node.Next = current.Next;
            current.Next = node;

            return head;
        }

        public EventQueueNode Pop(EventQueueNode head)
        {
            return head?.Next;
        }

        public int Peek(EventQueueNode head)
        {
            return head.EventId;
        }

        public bool IsEmpty(EventQueueNode head) => head == null;
    }
}
