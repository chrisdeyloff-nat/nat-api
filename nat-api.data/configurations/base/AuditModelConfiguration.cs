using nat_api.data.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace nat_api.data.configurations
{
    public class AuditModelConfiguration : IEntityTypeConfiguration<Audit>
    {
        public virtual void Configure(EntityTypeBuilder<Audit> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id);
            builder.Property(e => e.AuditDateTimeUtc);
            builder.Property(e => e.AuditType);
            // builder.Property(e => e.AuditUser);        
            builder.Property(e => e.TableName);
            builder.Property(e => e.KeyValues);
            builder.Property(e => e.OldValues)
                .IsRequired(false);
            builder.Property(e => e.NewValues);
            builder.Property(e => e.ChangedColumns)
                .IsRequired(false);
        }
    }
}