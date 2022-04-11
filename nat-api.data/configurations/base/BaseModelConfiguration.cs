using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using nat_api.data.models;
using System;

namespace nat_api.data.configurations
{
    public class BaseModelConfiguration<TModel> : IEntityTypeConfiguration<TModel>
        where TModel : BaseModel
    {
        public virtual void Configure(EntityTypeBuilder<TModel> builder)
        {
            builder
                .Property(x => x.Created)
                .HasDefaultValueSql("(now() at time zone 'utc')");

            builder
                .Property(x => x.Updated)
                .HasDefaultValueSql("(now() at time zone 'utc')");
        }

        public virtual DateTime InitialSeedTime => DateTime.UtcNow;

        public TModel Add(TModel tModel)
        {
            tModel.Created = InitialSeedTime;
            tModel.Updated = InitialSeedTime;

            return tModel;
        } 
    }
}