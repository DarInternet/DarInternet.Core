using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DarInternet.Application.Common.Models;
using System;
using DarInternet.Application.Features.Tickets.Queries.GetMyTickets;
using DarInternet.Application.Features.Tickets.SharedDto;
using DarInternet.Application.Features.Tickets.Commands.CreateTicket;
using DarInternet.Application.Features.Tickets.Commands.AssignTicket;
using Microsoft.AspNetCore.Authorization;
using DarInternet.Application.Features.Tickets.Queries.GetTicketMessages;

namespace DarInternet.Api.Controllers
{
    [Authorize]
    public class TicketsController: ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedList<TicketDto>>> MyTickets([FromQuery] GetMyTicketsQuery query)
        {
            return await Mediator.Send(query);
        }  

        [HttpGet("GetTicketMessages")]
        public async Task<ActionResult<PaginatedList<TicketMessageDto>>> GetTicketMessages([FromQuery] GetTicketMessagesQuery query)
        {
            return await Mediator.Send(query);
        }  

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateTicketCommand command)
        {
            return await Mediator.Send(command);
        }

        
        [HttpPut("{id}")]
        public async Task<ActionResult> Assign(Guid id, AssignTicketCommand command)
        {
            if (id != command.ConversationId)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            return NoContent();
        }
        
    }
}