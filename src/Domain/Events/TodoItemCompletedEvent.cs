using DarInternet.Domain.Common;
using DarInternet.Domain.Entities;

namespace DarInternet.Domain.Events
{
    public class TodoItemCompletedEvent : DomainEvent
    {
        public TodoItemCompletedEvent(TodoItem item)
        {
            Item = item;
        }

        public TodoItem Item { get; }
    }
}
