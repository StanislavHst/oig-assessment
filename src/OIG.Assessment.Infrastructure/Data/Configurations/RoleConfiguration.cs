using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OIG.Assessment.Domain.Entities;
using System.Text.Json;

namespace OIG.Assessment.Infrastructure.Data.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.HasOne(r => r.Organization)
            .WithMany(o => o.Roles)
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.Property(r => r.Permissions)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => JsonSerializer.Deserialize<ICollection<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>());
        
        builder.Property(r => r.CreatedAt)
            .IsRequired();
        
        builder.Property(r => r.UpdatedAt);
    }
}
