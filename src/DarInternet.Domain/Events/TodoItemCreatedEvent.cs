using DarInternet.Domain.Common;
using DarInternet.Domain.Entities;

namespace DarInternet.Domain.Events
{
    public class TodoItemCreatedEvent : DomainEvent
    {
        public TodoItemCreatedEvent(TodoItem item)
        {
            Item = item;
        }

        public TodoItem Item { get; }
    }
}
