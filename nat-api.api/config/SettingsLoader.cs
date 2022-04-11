using Microsoft.Extensions.Configuration;
using System;
using System.Text;

namespace nat_api.config
{
    public class SettingsLoader
    {
        public static void LoadSettings(IConfiguration configuration)
        {
            Settings.Environment = configuration["ASPNETCORE_ENVIRONMENT"];
            Settings.DefaultConnection = configuration["ConnectionStrings:Default"];
            Settings.SerilogConnection = configuration["Serilog:WriteTo:1:Args:connectionString"];
            Settings.UIOriginURL = configuration["UIOriginURL"];
        }
    }
}
