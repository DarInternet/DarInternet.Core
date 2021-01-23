using System;
using System.Threading.Tasks;
using DarInternet.Application.Common.Exceptions;
using DarInternet.Application.Features.Tickets.Commands.CreateTicketResponse;
using DarInternet.Application.IntegrationTests;
using DarInternet.Domain.Entities;
using DarInternet.Domain.Enums;
using DarInternet.Infrastructure.Persistence;
using FluentAssertions;
using NUnit.Framework;

namespace Application.IntegrationTests.Tickets.Commands
{
    using static Testing;

    public class CreateTicketResponseCommandTests: TestBase
    {
        [Test]
        public void CreateTicketResponseCommand_WithoutAnyInput_ShouldThrowValidationException()
        {
            var command = new CreateTicketResponseCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }
       
        [Test]
        public async Task CreateTicketResponseCommand_WithEmptyMessage_ShouldThrowValidationException()
        {
            var createdTicketId = await SeedSampleTicket();
            var command = new CreateTicketResponseCommand
            {
                Message= string.Empty,
                ConversationId=createdTicketId
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

         [Test]
        public async Task CreateTicketResponseCommand_WithoutMessage_ShouldThrowValidationException()
        {
            var createdTicketId = await SeedSampleTicket();

            var command = new CreateTicketResponseCommand
            {
                ConversationId=createdTicketId
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }


        [Test]
        public async Task CreateTicketResponseCommand_WithoutConversationId_ShouldThrowValidationException()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var command = new CreateTicketResponseCommand
            {
                Message="Test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task CreateTicketResponseCommand_WithEmptyConversationId_ShouldThrowValidationException()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var command = new CreateTicketResponseCommand
            {
                ConversationId = Guid.Empty,
                Message="Test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public void CreateTicketResponseCommand_WithInvalidConversationId_ShouldThrowNotFoundException()
        {
            var invalidConversationId = new Guid("ef1dc766-98f5-424f-ad4c-0fb42a690140");

            var command = new CreateTicketResponseCommand
            {
                ConversationId = invalidConversationId,
                Message="Test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task CreateTicketResponseCommand_WithDifferentUserThanAdmin_ShouldThrowForbiddenAccessException()
        {
            var createdTicketId = await SeedSampleTicket();

            await RunAsAdministratorAsync();

            var command = new CreateTicketResponseCommand
            {
                ConversationId = createdTicketId,
                Message="Test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        public async Task CreateTicketResponseCommand_ForClosedTicket_ShouldThrowException()
        {
            var createdTicketId = await SeedSampleTicket(true);

            var command = new CreateTicketResponseCommand
            {
                ConversationId = createdTicketId,
                Message="Test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<Exception>().Which.Message.Should().Be("You can't reply to closed ticket");
        }

        [Test]
        public async Task CreateTicketRepsonseCommand_WithValidInput_ShouldInsertTicketMessageAndReturnTicketMessageId()
        {
            var createdTicketId = await SeedSampleTicket();

            var command = new CreateTicketResponseCommand
            {
                ConversationId = createdTicketId,
                Message="Second Response"
            };

            var ticketMessageId = await SendAsync(command);

            var ticketMessage = await FindAsync<ConversationMessage>(ticketMessageId);

            ticketMessage.Should().NotBeNull();
            ticketMessage.Message.Should().Be("Second Response");
            ticketMessage.Id.Should().Be(ticketMessageId);

        }

       
        private static async Task<Guid> SeedSampleTicket(bool isClosedTicket)
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var context = GetContext();

            return await CreateSampleTicket(context, defaultUserId, isClosedTicket);
        }


        private static async Task<Guid> SeedSampleTicket()
        {
            return await SeedSampleTicket(false);
        }

        private static async Task<Guid> CreateSampleTicket(ApplicationDbContext context, string userId,bool isClosedTicket)
        {            
            var createdOrganizationId =await CreateSampleOrganization(context, userId);

            var ticket = new Conversation
            {
                Title= "My First Ticket",
                OrganizationId = createdOrganizationId,
                IsClosed = isClosedTicket
            };

            await context.Conversations.AddAsync(ticket);
            await context.SaveChangesAsync();

            var ticketUser = new ConversationUser
            {
                ConversationId= ticket.Id,
                UserId= userId,
                IsDisabled = false
            };

            await context.ConversationUsers.AddAsync(ticketUser);
            await context.SaveChangesAsync();

            var ticketMessage = new ConversationMessage
            {
                ConversationId = ticket.Id,
                UserId = userId,
                Message = "Hello from first ticket"        
            };

            await context.ConversationMessages.AddAsync(ticketMessage);
            await context.SaveChangesAsync();

            return ticket.Id;
        }

        private static async Task<Guid> CreateSampleOrganization(ApplicationDbContext context, string userId)
        {
            var sampleOrg = new Organization
            {
                Title = "Test Organization"
            };
            await context.AddAsync(sampleOrg);
            await context.SaveChangesAsync();

            var sampleOrgUser = new OrganizationUser
            {
                UserId = userId,
                OrganizationId = sampleOrg.Id,
                Type = UserType.Admin
            };

            await context.AddAsync(sampleOrgUser);
            await context.SaveChangesAsync();

            return sampleOrg.Id;
        }

    }
}