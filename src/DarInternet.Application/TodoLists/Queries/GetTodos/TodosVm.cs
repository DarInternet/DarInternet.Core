using System.Collections.Generic;

namespace DarInternet.Application.TodoLists.Queries.GetTodos
{
    public class TodosVm
    {
        public IList<PriorityLevelDto> PriorityLevels { get; set; }

        public IList<TodoListDto> Lists { get; set; }
    }
}
