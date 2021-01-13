using System;
using System.Threading.Tasks;
using DarInternet.Application.Common.Exceptions;
using DarInternet.Application.Features.Organizations.Commands.CreateOrganization;
using DarInternet.Application.Features.Organizations.Commands.UpdateOrganization;
using DarInternet.Application.IntegrationTests;
using DarInternet.Domain.Entities;
using DarInternet.Domain.Enums;
using DarInternet.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Shouldly;

namespace Application.IntegrationTests.Organizations.Commands
{
    using static Testing;

    public class UpdateOrganizationTests
    {
        [Test]
        public async Task UpdateOrganizationCommand_WithoutTitle_ShouldThrowValidationException()
        {
            var createdOrganizationId = await SeedSampleOrganization();
            var command = new UpdateOrganizationCommand { OrganizationId = createdOrganizationId};

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public async Task UpdateOrganizationCommand_WithTitleMoreThan200Characters_ShouldThrowValidationException()
        {
            var createdOrganizationId = await SeedSampleOrganization();

            var command = new UpdateOrganizationCommand
            {
                OrganizationId = createdOrganizationId,
                Title = "This is a very long title for an organization so we could test Request validator. In this case we expect a validation exception because our title is more that 200 character. so let's test it and see our test result"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ValidationException>();
        }

        [Test]
        public void UpdateOrganizationCommand_WithInvalidOrganizationId_ShoulThrowNotFoundException()
        {
            var command = new UpdateOrganizationCommand
            {
                OrganizationId = Guid.Empty,
                Title = "Test 5"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<NotFoundException>();
        }

        [Test]
        public async Task UpdateOrganziationCommand_WithDiffrentUserThanAdmin_ShouldThrowForbiddenAccessException()
        {
            var createdOrganizationId= await SeedSampleOrganization();

            await RunAsAdministratorAsync();

            var command = new UpdateOrganizationCommand
            {
                OrganizationId = createdOrganizationId,
                Title = "Test 5"
            };

            FluentActions.Invoking(() =>
                SendAsync(command)).Should().Throw<ForbiddenAccessException>();
        }

        [Test]
        public async Task UpdateOrganziationCommand_WithValidTitleAndOrganizationId_ShouldUpdateOrganziation()
        {
            var createdOrganizationId= await SeedSampleOrganization();

            await SendAsync(new UpdateOrganizationCommand
            {
                OrganizationId = createdOrganizationId,
                Title = "Test 5"
            });

            var organization = await FindAsync<Organization>(createdOrganizationId);

            organization.Title.Should().Be("Test 5");
        }




         #region Seed Data
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

        #endregion
    }
}