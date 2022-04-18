using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotesAPI.Shared.Interfaces
{
    public interface ISettingsService
    {
        string GetDocDbEndpointUri();
        string GetDocDbApiKey();
        string GetDocDbConnectionString();
        string GetDocDbFundooNotesDatabaseName();
        string GetDocDbMainCollectionName();
        int? GetDocDbThroughput();
    }
}
