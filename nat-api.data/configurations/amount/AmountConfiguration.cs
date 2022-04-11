using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nat_api.data.models.amount;

namespace nat_api.data.configurations.amount
{
    public class AmountConfiguration : BaseModelConfiguration<Amount>
    {
        public override void Configure(EntityTypeBuilder<Amount> builder)
        {
            base.Configure(builder);

            builder
                .HasKey(x => x.Id);

            builder
                .Property(x => x.Id)
                .UseIdentityColumn();

            builder
                .Property(x => x.Value)
                .IsRequired(true);

            builder
                .Property(x => x.SilverDollarValue)
                .IsRequired(true);

            builder
                .Property(x => x.HalfDollarValue)
                .IsRequired(true);

            builder
                .Property(x => x.QuarterValue)
                .IsRequired(true);

            builder
                .Property(x => x.DimeValue)
                .IsRequired(true);

            builder
                .Property(x => x.NickelValue)
                .IsRequired(true);

            builder
                .Property(x => x.PennyValue)
                .IsRequired(true);
        }

    }
}
