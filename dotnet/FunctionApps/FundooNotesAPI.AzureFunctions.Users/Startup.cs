using FundooNotesAPI.AzureFunctions.Users;
using FundooNotesAPI.Shared.Interfaces;
using FundooNotesAPI.Shared.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(Startup))]

namespace FundooNotesAPI.AzureFunctions.Users
{
    public class Startup : FunctionsStartup
    {
        private static readonly IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("AppSettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        public override void Configure(IFunctionsHostBuilder builder)
        {

            builder.Services.AddSingleton<IUserService, UserService>();

            builder.Services.AddSingleton<IJWTService, JWTService>();

            builder.Services.AddSingleton<ISettingsService, SettingsService>();
        }
    }
}
