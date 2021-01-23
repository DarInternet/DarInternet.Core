using System;
using System.Threading.Tasks;
using DarInternet.Application.Common.Exceptions;
using DarInternet.Application.Features.Tickets.Commands.CreateTicket;
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

    public class CreateTicketCommandTests: TestBase
    {
        [Test]
        public void CreateTicketCommand_WithoutAnyInput_ShouldThrowValidationException()
        {
            var command = new CreateTicketCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task CreateTicketCommand_WithEmptyTitle_ShouldThrowValidationException()
        {
            var createdOrganizationId = await SeedSampleOrganization();
            var command = new CreateTicketCommand
            {
                Title=string.Empty,
                Message="Test",
                OrganizationId=createdOrganizationId
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

         [Test]
        public async Task CreateTicketCommand_WithoutTitle_ShouldThrowValidationException()
        {
            var createdOrganizationId = await SeedSampleOrganization();

            var command = new CreateTicketCommand
            {
                Message="Test",
                OrganizationId=createdOrganizationId
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }


        [Test]
        public async Task CreateTicketCommand_WithTitleMoreThan1000Characters_ShouldThrowValidationException()
        {
            var createdOrganizationId = await SeedSampleOrganization();

            string veryLongTitle = new String('A', 1001);

            var command = new CreateTicketCommand
            {
                Title = veryLongTitle,
                Message= "Test",
                OrganizationId= createdOrganizationId
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task CreateTicketCommand_WithEmptyMessage_ShouldThrowValidationException()
        {
            var createdOrganizationId = await SeedSampleOrganization();
            var command = new CreateTicketCommand
            {
                Title= "Test",
                Message= string.Empty,
                OrganizationId=createdOrganizationId
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

         [Test]
        public async Task CreateTicketCommand_WithoutMessage_ShouldThrowValidationException()
        {
            var createdOrganizationId = await SeedSampleOrganization();

            var command = new CreateTicketCommand
            {
                Title="Test",
                OrganizationId=createdOrganizationId
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }


        [Test]
        public async Task CreateTicketCommand_WithoutOrganizationId_ShouldThrowValidationException()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var command = new CreateTicketCommand
            {
                Title = "Test",
                Message="Test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task CreateTicketCommand_WithEmptyOrganizationId_ShouldThrowValidationException()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var command = new CreateTicketCommand
            {
                OrganizationId = Guid.Empty,
                Title = "Test",
                Message="Test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public void CreateTicketCommand_WithInvalidOrganizationId_ShouldThrowNotFoundException()
        {
            var invalidOrganizationId = new Guid("ef1dc766-98f5-424f-ad4c-0fb42a690140");

            var command = new CreateTicketCommand
            {
                OrganizationId = invalidOrganizationId,
                Title = "Test",
                Message="Test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<NotFoundException>();
        }

         [Test]
        public async Task CreateTicketCommand_WithDifferentUserThanAdmin_ShouldThrowException()
        {
            var createdOrganizationId = await SeedSampleOrganization();

            await RunAsAdministratorAsync();

            var command = new CreateTicketCommand
            {
                OrganizationId = createdOrganizationId,
                Title = "Test",
                Message="Test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        public async Task CreateTicketCommand_WithValidInput_ShouldInsertTicketAndReturnTicketId()
        {
            var createdOrganizationId = await SeedSampleOrganization();

            var command = new CreateTicketCommand
            {
                OrganizationId = createdOrganizationId,
                Title = "Test Ticket Title",
                Message="First Ticket"
            };

            var ticketId = await SendAsync(command);

            var ticket = await FindAsync<Conversation>(ticketId);

            ticket.Should().NotBeNull();
            ticket.Title.Should().Be("Test Ticket Title");
            ticket.Id.Should().Be(ticketId);            

        }

        [Test]
        public async Task CreateTicketCommand_WithValidInput_ShouldAddCurrentUserToConversationUserAndReturnTicketId()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var createdOrganizationId = await SeedSampleOrganization();

            var command = new CreateTicketCommand
            {
                OrganizationId = createdOrganizationId,
                Title = "Test Ticket Title",
                Message="First Ticket"
            };

            var ticketId = await SendAsync(command);

            var ticket = await FindAsync<Conversation>(ticketId);

            ticket.Should().NotBeNull();
            ticket.Title.Should().Be("Test Ticket Title");
            ticket.Id.Should().Be(ticketId);        

            var context = GetContext();
            var conversationUser = await context.ConversationUsers.SingleOrDefaultAsync(x=>x.ConversationId==ticketId && x.UserId==defaultUserId);

            conversationUser.Should().NotBeNull();                       

        }

         [Test]
        public async Task CreateTicketCommand_WithValidInput_ShouldAddMessageToConversationMessagesAndReturnTicketId()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var createdOrganizationId = await SeedSampleOrganization();

            var command = new CreateTicketCommand
            {
                OrganizationId = createdOrganizationId,
                Title = "Test Ticket Title",
                Message="First Ticket"
            };

            var ticketId = await SendAsync(command);

            var ticket = await FindAsync<Conversation>(ticketId);

            ticket.Should().NotBeNull();
            ticket.Title.Should().Be("Test Ticket Title");
            ticket.Id.Should().Be(ticketId);        

            var context = GetContext();
            var conversationMessage = await context.ConversationMessages.FirstOrDefaultAsync(x=>x.ConversationId==ticketId && x.UserId==defaultUserId);

            conversationMessage.Should().NotBeNull();                       
            conversationMessage.Message.Should().Be("First Ticket");                       


        }


        private static async Task<Guid> SeedSampleOrganization()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var context = GetContext();

            return await CreateSampleOrganization(context,"Test1",defaultUserId);
        }

        private static async Task<Guid> CreateSampleOrganization(ApplicationDbContext context,string title, string userId)
        {
            var sampleOrg = new Organization
            {
                Title = title
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