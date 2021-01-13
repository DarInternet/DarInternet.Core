using System.Threading.Tasks;
using DarInternet.Application.Common.Exceptions;
using DarInternet.Application.Features.Organizations.Commands.CreateOrganization;
using DarInternet.Application.IntegrationTests;
using DarInternet.Domain.Entities;
using DarInternet.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace Application.IntegrationTests.Organizations.Commands
{
    using static Testing;

    public class CreateOrganizationTests: TestBase
    {
        [Test]
        public void CreateOrganizationCommand_WithoutTitle_ShouldThrowValidationException()
        {
            var command = new CreateOrganizationCommand();

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public void CreateOrganizationCommand_WithTitleMoreThan200Characters_ShouldThrowValidationException()
        {
            var command = new CreateOrganizationCommand
            {
                Title = "This is a very long title for an organization so we could test Request validator. In this case we expect a validation exception because our title is more that 200 character. so let's test it and see our test result"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }


         [Test]
        public void CreateOrganizationCommand_WithoutCurrentUserId_ShouldThrowDbUpdateException()
        {
            var command = new CreateOrganizationCommand
            {
                Title = "Test"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<DbUpdateException>();
        }


        [Test]
        public async Task CreateOrganizationCommand_WithTitle_ShouldInsertOrganizationAndReturnOrganizationId()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var command = new CreateOrganizationCommand
            {
                Title = "Test"
            };

            var organizationId = await SendAsync(command);

            var organization = await FindAsync<Organization>(organizationId);

            organization.ShouldNotBeNull();
            organization.Title.ShouldBe("Test");
            organization.Id.ShouldBe(organizationId);            

        }

        

        [Test]
        public async Task CreateOrganizationCommand_WithTitle_ShouldInsertCurrentUserAsAdminOfOrganizationAndReturnOrganizationId()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var command = new CreateOrganizationCommand
            {
                Title = "Test"
            };

            var organizationId = await SendAsync(command);

            var organization = await FindAsync<Organization>(organizationId);

            organization.ShouldNotBeNull();
            organization.Title.ShouldBe("Test");
            organization.Id.ShouldBe(organizationId);   

            var context = GetContext();
            var orgnizationUser = await context.OrganizationUsers.SingleOrDefaultAsync(x=>x.OrganizationId==organizationId && x.UserId==defaultUserId && x.Type== UserType.Admin);

            orgnizationUser.ShouldNotBeNull();           

        }

        
    }
}