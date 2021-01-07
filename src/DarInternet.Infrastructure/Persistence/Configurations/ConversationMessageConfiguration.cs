using DarInternet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DarInternet.Infrastructure.Persistence.Configurations
{
    public class ConversationMessageConfiguration: IEntityTypeConfiguration<ConversationMessage>
    {
        public void Configure(EntityTypeBuilder<ConversationMessage> builder)
        {
            builder.Property(t => t.Message)
                .HasColumnType("text")
                .IsRequired();
        }
    }
}