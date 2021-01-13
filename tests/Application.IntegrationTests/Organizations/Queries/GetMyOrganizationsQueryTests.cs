using DarInternet.Application.Features.Organizations.Queries.GetOrganizations;
using DarInternet.Application.IntegrationTests;
using DarInternet.Domain.Entities;
using DarInternet.Domain.Enums;
using DarInternet.Infrastructure.Persistence;
using FluentAssertions;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace Application.IntegrationTests.Organizations.Queries
{
    using static Testing;

    public class GetMyOrganizationsQueryTests: TestBase
    {       

        [Test]
        public async Task GetMyOrganizationsQuery_WithoutParameter_ShouldReturnPaginatedOrganizations()
        {
            await SeedSampleOrganizations();

            var query = new GetMyOrganizationsQuery {PageNumber=1 , PageSize=2};

            var result = await SendAsync(query);

            result.TotalCount.Should().Be(3);
            result.TotalPages.Should().Be(2);
            result.Items.Any(x=>x.Title=="Test1").Should().BeTrue();
        }


        #region Seed Data
        private static async Task SeedSampleOrganizations()
        {
            var defaultUserId = await RunAsDefaultUserAsync();

            var context = GetContext();

            await CreateSampleOrganization(context,"Test1",defaultUserId);
            await CreateSampleOrganization(context,"Test2",defaultUserId);
            await CreateSampleOrganization(context,"Test3",defaultUserId);
        }

        private static async Task CreateSampleOrganization(ApplicationDbContext context,string title, string userId)
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
        }

        #endregion
    }
}
