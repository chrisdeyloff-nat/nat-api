using Xunit;
using Xunit.Extensions.AssertExtensions;
using Test.Helpers;
using nat_api.data.context;
using nat_api.data.models.amount;
using System.Collections.Generic;
using System.Linq;
using Moq;
using nat_api.api.features.amounts.command;
using nat_api.api.features.amounts.query;
using Microsoft.Extensions.Logging;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace nat_api.tests;

public class AmountTests
{
    [Fact]
    public async Task TestGetListOk()
    {
        using (var context = this.CreateContext(this.CreatePostgreSqlUniqueMethodOptions<DataContext>()))
        {
            try
            {
                this.SeedAmounts(context);
                var loggerMock = new Mock<ILogger<GetList.Handler>>();
                var mappingConfig = this.CreateMappingConfiguration();
                var handler = new GetList.Handler(context, loggerMock.Object, mappingConfig);
                var result = (await handler.Handle(new GetList.Query(), System.Threading.CancellationToken.None)).Payload;

                result.Count().ShouldEqual(3);
            }
            finally
            {
                if (context != null)
                { context.Database.EnsureDeleted(); }
            }
        }
    }

    [Fact]
    public async Task TestGetByIdOk()
    {
        using (var context = this.CreateContext(this.CreatePostgreSqlUniqueMethodOptions<DataContext>()))
        {
            try
            {
                const long id = 2;
                this.SeedAmounts(context);
                var loggerMock = new Mock<ILogger<GetById.Handler>>();
                var mappingConfig = this.CreateMappingConfiguration();
                var handler = new GetById.Handler(context, loggerMock.Object, mappingConfig);
                var result = (await handler.Handle(new GetById.Query { Id = id }, System.Threading.CancellationToken.None)).Payload;

                result.Id.ShouldEqual(id);
            }
            finally
            {
                if (context != null)
                { context.Database.EnsureDeleted(); }
            }
        }
    }

    [Fact]
    public async Task TestCreateOneOk()
    {
        await CreateAmount(
            0.99M,
            0,
            1,
            1,
            2,
            0,
            4,
            this.CreatePostgreSqlUniqueMethodOptions<DataContext>()
        );
    }

    [Fact]
    public async Task TestCreateTwoOk()
    {
        await CreateAmount(
            1.56M,
            1,
            1,
            0,
            0,
            1,
            1,
            this.CreatePostgreSqlUniqueMethodOptions<DataContext>()
        );
    }

    [Fact]
    public async Task TestCreateThreeOk()
    {
        await CreateAmount(
            12.85M,
            12,
            1,
            1,
            1,
            0,
            0,
            this.CreatePostgreSqlUniqueMethodOptions<DataContext>()
        );
    }

    private async Task CreateAmount(
        decimal value,
        int shouldSilver,
        int shouldHalf,
        int shouldQuarter,
        int shouldDime,
        int shouldNickel,
        int shouldPenny,
        DbContextOptions<DataContext> options
    )
    {
        using (var context = this.CreateContext(options))
        {
            try
            {
                var loggerMock = new Mock<ILogger<Create.Handler>>();
                var mapper = this.CreateMapper(this.CreateMappingConfiguration());
                var handler = new Create.Handler(context, loggerMock.Object, mapper);
                var result = (await handler.Handle(new Create.Command { Amount = value }, System.Threading.CancellationToken.None)).Payload;

                Assert.True(result.Id > 0, "Created amount id must be greater than 0");
                result.Value.ShouldEqual(value);
                result.SilverDollarValue.ShouldEqual(shouldSilver);
                result.HalfDollarValue.ShouldEqual(shouldHalf);
                result.QuarterValue.ShouldEqual(shouldQuarter);
                result.DimeValue.ShouldEqual(shouldDime);
                result.NickelValue.ShouldEqual(shouldNickel);
                result.PennyValue.ShouldEqual(shouldPenny);
            }
            finally
            {
                if (context != null)
                { context.Database.EnsureDeleted(); }
            }
        }
    }

    [Fact]
    public async Task TestEdittOk()
    {
        using (var context = this.CreateContext(this.CreatePostgreSqlUniqueMethodOptions<DataContext>()))
        {
            try
            {
                var id = 3;
                var val = 18.57M;
                this.SeedAmounts(context);
                var loggerMock = new Mock<ILogger<Edit.Handler>>();
                var mapper = this.CreateMapper(this.CreateMappingConfiguration());
                var handler = new Edit.Handler(context, loggerMock.Object, mapper);
                var result = (await handler.Handle(new Edit.Command { Id = id, Amount = val }, System.Threading.CancellationToken.None)).Payload;

                result.Id.ShouldEqual(id);
                result.Value.ShouldEqual(val);
                result.SilverDollarValue.ShouldEqual(18);
                result.HalfDollarValue.ShouldEqual(1);
                result.QuarterValue.ShouldEqual(0);
                result.DimeValue.ShouldEqual(0);
                result.NickelValue.ShouldEqual(1);
                result.PennyValue.ShouldEqual(2);            }
            finally
            {
                if (context != null)
                { context.Database.EnsureDeleted(); }
            }
        }
    }

    private void SeedAmounts(DataContext context)
    {
        var amounts = new List<Amount>
        {
            new Amount
            {
                Id = 1,
                Value = 13.25M,
                SilverDollarValue = 1,
                HalfDollarValue = 1,
                QuarterValue = 1,
                DimeValue = 1,
                NickelValue = 1,
                PennyValue = 1
            },
            new Amount
            {
                Id = 2,
                Value = 14.25M,
                SilverDollarValue = 1,
                HalfDollarValue = 1,
                QuarterValue = 1,
                DimeValue = 1,
                NickelValue = 1,
                PennyValue = 1
            },
            new Amount
            {
                Id = 3,
                Value = 15.25M,
                SilverDollarValue = 1,
                HalfDollarValue = 1,
                QuarterValue = 1,
                DimeValue = 1,
                NickelValue = 1,
                PennyValue = 1
            }
        };
        context.Amounts.AddRange(amounts);
        context.SaveChanges();
    }

    private IMapper CreateMapper(AutoMapper.IConfigurationProvider mappingConfig)
    {
        return mappingConfig.CreateMapper();
    }
    private AutoMapper.IConfigurationProvider CreateMappingConfiguration()
    {
        return new MapperConfiguration(mc =>
        {
            mc.AddProfile(new GetList.Handler.MappingProfile());
            mc.AddProfile(new GetById.Handler.MappingProfile());
            mc.AddProfile(new Edit.Handler.MappingProfile());
            mc.AddProfile(new Create.Handler.MappingProfile());
        });
    }
}
