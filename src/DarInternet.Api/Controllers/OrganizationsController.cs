using DarInternet.Application.Features.Organizations.Commands.CreateOrganization;
using DarInternet.Application.Features.Organizations.Commands.UpdateOrganization;
using DarInternet.Application.Features.Organizations.Queries.GetOrganizations;
using DarInternet.Application.Features.Organizations.SharedDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DarInternet.Application.Common.Models;
using System;

namespace DarInternet.Api.Controllers
{
    [Authorize]
    public class OrganizationsController : ApiControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<PaginatedList<OrganizationDto>>> MyOrganizations([FromQuery] GetMyOrganizationsQuery query)
        {
            return await Mediator.Send(query);
        }  

        [HttpPost]
        public async Task<ActionResult<Guid>> Create(CreateOrganizationCommand command)
        {
            return await Mediator.Send(command);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, UpdateOrganizationCommand command)
        {
            if (id != command.OrganizationId)
            {
                return BadRequest();
            }

            await Mediator.Send(command);

            return NoContent();
        }
        
    }
}