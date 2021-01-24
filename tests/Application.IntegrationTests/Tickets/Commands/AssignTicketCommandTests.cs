using System;
using System.Linq;
using System.Threading.Tasks;
using DarInternet.Application.Common.Exceptions;
using DarInternet.Application.Features.Tickets.Commands.AssignTicket;
using DarInternet.Application.IntegrationTests;
using DarInternet.Domain.Entities;
using DarInternet.Domain.Enums;
using DarInternet.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Application.IntegrationTests.Tickets.Commands
{

    using static Testing;

    public class AssignTicketCommandTests: TestBase
    {
        [Test]
        public void AssignTicketCommand_WithoutAnyInput_ShouldThrowValidationException()
        {
            var command = new AssignTicketCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }
       
        [Test]
        public async Task AssignTicketCommand_WithEmptyUserId_ShouldThrowValidationException()
        {
            var createdTicketId = await SeedSampleTicket();

            var command = new AssignTicketCommand
            {
                UserId = string.Empty,
                ShouldRemovePreviousUsers = false,
                ConversationId = createdTicketId
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

         [Test]
        public async Task AssignTicketCommand_WithoutUserId_ShouldThrowValidationException()
        {
             var createdTicketId = await SeedSampleTicket();

            var command = new AssignTicketCommand
            {
                ShouldRemovePreviousUsers = false,
                ConversationId = createdTicketId
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task AssignTicketCommand_WithEmptyConversationId_ShouldThrowValidationException()
        {

            var defaultUserId = await RunAsDefaultUserAsync();

            var createdTicketId = await SeedSampleTicket();

            var command = new AssignTicketCommand
            {
                UserId = defaultUserId,
                ShouldRemovePreviousUsers = false,
                ConversationId = Guid.Empty
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

         [Test]
        public async Task AssignTicketCommand_WithoutConversationId_ShouldThrowValidationException()
        {
            var defaultUserId = await RunAsDefaultUserAsync();
            
            var createdTicketId = await SeedSampleTicket();

            var command = new AssignTicketCommand
            {
                ShouldRemovePreviousUsers = false,
                UserId = defaultUserId
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task AssignTicketCommand_WithInvalidConversationId_ShouldThrowNotFoundException()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var command = new AssignTicketCommand
            {
                ConversationId = Guid.NewGuid(),
                UserId = defaultUserId,
                ShouldRemovePreviousUsers = false
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task AssignTicketCommand_WithDifferentUserThanAdmin_ShouldThrowForbiddenAccessException()
        {            
            var defaultUserId = await RunAsDefaultUserAsync();
            
            var createdTicketId = await SeedSampleTicket();

            await RunAsAdministratorAsync();

            var command = new AssignTicketCommand
            {
                ConversationId = createdTicketId,
                UserId = defaultUserId,
                ShouldRemovePreviousUsers = false
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        public async Task AssignTicketCommand_ForClosedTicket_ShouldThrowException()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var createdTicketId = await SeedSampleTicket(true);

            var command = new AssignTicketCommand
            {
                ConversationId = createdTicketId,
                 UserId = defaultUserId,
                ShouldRemovePreviousUsers = false
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<Exception>().Which.Message.Should().Be("You can't assign new user to closed ticket");
        }

        [Test]
        public async Task AssignTicketCommand_WithShouldRemovePreviousUsersFlag_ShouldAddNewUserToConversationUsersAndSetIsDisabledForPreviousUsers()
        {         
            var createdTicketId = await SeedSampleTicket(false);

            var operator2UserId = await RunAsOperator2UserAsync();

            var command = new AssignTicketCommand
            {
                ConversationId = createdTicketId,
                UserId = operator2UserId,
                ShouldRemovePreviousUsers = true
            };

            await SendAsync(command);

            var context = GetContext();

            var conversationUsers = await context.ConversationUsers.Where(x=>x.ConversationId == createdTicketId).ToListAsync();

            conversationUsers.Count.Should().Be(3);

            conversationUsers.Count(x=>x.IsDisabled).Should().Be(2);

            conversationUsers.SingleOrDefault(x=>x.IsDisabled==false).UserId.Should().Be(operator2UserId);

        }

        [Test]
        public async Task AssignTicketCommand_WithShouldRemovePreviousUsersFlagSetToFalse_ShouldAddNewUserToConversationUsersAndNotDisableAnyPreviousUser()
        {         
            var createdTicketId = await SeedSampleTicket(false);

            var operator2UserId = await RunAsOperator2UserAsync();

            var command = new AssignTicketCommand
            {
                ConversationId = createdTicketId,
                UserId = operator2UserId,
                ShouldRemovePreviousUsers = false
            };

            await SendAsync(command);

            var context = GetContext();

            var conversationUsers = await context.ConversationUsers.Where(x=>x.ConversationId == createdTicketId).ToListAsync();

            conversationUsers.Count.Should().Be(3);

            conversationUsers.Count(x=>x.IsDisabled).Should().Be(0);

            conversationUsers.Any(x=>x.UserId==operator2UserId).Should().BeTrue();
        }
       

       

        private static async Task<Guid> SeedSampleTicket(bool isClosedTicket)
        {
            var operator1UserId = await RunAsOperator1UserAsync();
            var operator2UserId = await RunAsOperator2UserAsync();

            var customerUserId = await RunAsCustomer1UserAsync();

            var context = GetContext();

            return await CreateSampleTicket(context, customerUserId, new string[]{customerUserId, operator1UserId , operator2UserId}, new string[] {customerUserId, operator1UserId} ,isClosedTicket);
        }


        private static async Task<Guid> SeedSampleTicket()
        {
            return await SeedSampleTicket(false);
        }

        private static async Task<Guid> CreateSampleTicket(ApplicationDbContext context, string customerUserId, string[] organizationUserIds, string[] ticketUserIds,bool isClosedTicket)
        {            
            var createdOrganizationId =await CreateSampleOrganization(context, organizationUserIds);

            var ticket = new Conversation
            {
                Title= "My First Ticket",
                OrganizationId = createdOrganizationId,
                IsClosed = isClosedTicket
            };

            await context.Conversations.AddAsync(ticket);
            await context.SaveChangesAsync();

            foreach(var userId in ticketUserIds)
            {
                var ticketUser = new ConversationUser
                {
                    ConversationId= ticket.Id,
                    UserId= userId,
                    IsDisabled = false
                };

                await context.ConversationUsers.AddAsync(ticketUser);
            }
            
            await context.SaveChangesAsync();

            var ticketMessage = new ConversationMessage
            {
                ConversationId = ticket.Id,
                UserId = customerUserId,
                Message = "Hello from first ticket"        
            };

            await context.ConversationMessages.AddAsync(ticketMessage);
            await context.SaveChangesAsync();

            return ticket.Id;
        }

        private static async Task<Guid> CreateSampleOrganization(ApplicationDbContext context, string[] operatorUserIds)
        {
            var sampleOrg = new Organization
            {
                Title = "Test Organization"
            };
            await context.AddAsync(sampleOrg);
            await context.SaveChangesAsync();

            foreach(var userId in operatorUserIds)
            {
                var operatorOrgUser = new OrganizationUser
                {
                    UserId = userId,
                    OrganizationId = sampleOrg.Id,
                    Type = UserType.Operator
                };

                await context.AddAsync(operatorOrgUser);
            }           

            return sampleOrg.Id;
        }
    }
}
