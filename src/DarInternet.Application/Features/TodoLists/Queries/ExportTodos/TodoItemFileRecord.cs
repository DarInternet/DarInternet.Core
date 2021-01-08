using DarInternet.Application.Common.Mappings;
using DarInternet.Domain.Entities;

namespace DarInternet.Application.Features.TodoLists.Queries.ExportTodos
{
    public class TodoItemRecord : IMapFrom<TodoItem>
    {
        public string Title { get; set; }

        public bool Done { get; set; }
    }
}
