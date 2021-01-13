using DarInternet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarInternet.Infrastructure.Persistence.Configurations
{
    public class OrganizationUserConfiguration: IEntityTypeConfiguration<OrganizationUser>
    {
        public void Configure(EntityTypeBuilder<OrganizationUser> builder)
        {
            builder.Property(t => t.UserId)
                .IsRequired();
        }
    }
}