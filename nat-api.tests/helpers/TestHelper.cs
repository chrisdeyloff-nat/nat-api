using Test.Helpers;
using nat_api.data.context;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TestSupport.EfHelpers;
using Moq;

namespace nat_api.tests;

public static class TestHelper
{
    public static DataContext CreateContext(this object source, DbContextOptions<DataContext> options)
    {
        var context = new DataContext(options);
        context.Database.EnsureDeleted();
        context.Database.Migrate();
        return context;
    }

}