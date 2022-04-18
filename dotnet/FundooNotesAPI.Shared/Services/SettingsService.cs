using FundooNotesAPI.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotesAPI.Shared.Services
{
    public class SettingsService : ISettingsService
    {
        // Cosmos DB
        private const string DocDbEndpointUriKey = "DocDbEndpointUri";
        private const string DocDbApiKey = "DocDbApiKey";
        private const string DocDbConnectionStringKey = "CosmosDBConnection";
        private const string DocDbFundooNotesDatabaseNameKey = "DocDbFundooNotesDatabaseName";
        private const string DocDbFundooNotesMainCollectionNameKey = "DocDbFundooNotesMainCollectionName";
        private const string DocDbThroughput = "DocDbThroughput"; 

        //*** PRIVATE ***//
        private static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

        // Cosmos
        public string GetDocDbEndpointUri()
        {
            return GetEnvironmentVariable(DocDbEndpointUriKey);
        }

        public string GetDocDbApiKey()
        {
            return GetEnvironmentVariable(DocDbApiKey);
        }

        public string GetDocDbConnectionString()
        {
            return GetEnvironmentVariable(DocDbConnectionStringKey);
        }

        public string GetDocDbFundooNotesDatabaseName()
        {
            return GetEnvironmentVariable(DocDbFundooNotesDatabaseNameKey);
        }

        public string GetDocDbMainCollectionName()
        {
            return GetEnvironmentVariable(DocDbFundooNotesMainCollectionNameKey);
        }

        public int? GetDocDbThroughput()
        {
            if (int.TryParse(GetEnvironmentVariable(DocDbThroughput), out int throughput)) return throughput;
            return null;
        }
    }
}
