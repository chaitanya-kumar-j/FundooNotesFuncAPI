using FundooNotesAPI.Models.RequestModels;
using FundooNotesAPI.Shared.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FundooNotesAPI.Shared.Services
{
    public class NotesService : INotesService
    {
        private readonly IJWTService _jWTService;
        ISettingsService _settingsService;

        // Cosmos DocDB API database
        private string _docDbEndpointUri;
        private string _docDbPrimaryKey;
        private string _docDbDatabaseName;

        // Doc DB Collections
        private string _docDbDigitalMainCollectionName;

        private static CosmosClient _docDbSingletonClient;
        private readonly Container _cosmosContainer;



        public NotesService(IJWTService jWTService, ISettingsService settingsService)
        {
            _settingsService = settingsService;
            _jWTService = jWTService;
            _docDbEndpointUri = _settingsService.GetDocDbEndpointUri();
            _docDbPrimaryKey = _settingsService.GetDocDbApiKey();

            _docDbDatabaseName = _settingsService.GetDocDbFundooNotesDatabaseName();
            _docDbDigitalMainCollectionName = _settingsService.GetDocDbMainCollectionName();
            _docDbSingletonClient = new CosmosClient(_settingsService.GetDocDbEndpointUri(), settingsService.GetDocDbApiKey());
            _cosmosContainer = _docDbSingletonClient.GetContainer(_docDbDatabaseName, _docDbDigitalMainCollectionName);
            
        }

        //**** PRIVATE METHODS ****//
        private static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }

        public async Task<FundooNote> CreateFundooNote(string email, FundooNote newFundooNote)
        {
            if (string.IsNullOrEmpty(newFundooNote.NoteId) || newFundooNote.NoteId == "string" )
            {
                newFundooNote.CreatedAt = Convert.ToString(DateTime.Now);
                newFundooNote.NoteId = Guid.NewGuid().ToString();
                newFundooNote.Collaborations.Add(email);
            }

            try
            {
                using (var response = _cosmosContainer.CreateItemAsync(newFundooNote, new PartitionKey(newFundooNote.NoteId)))
                {
                    return response.Result.Resource;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Task<List<FundooNote>> DeleteFundooNoteById(string email, string id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<FundooNote>> GetAllFundooNotes(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(_docDbDigitalMainCollectionName))
                    throw new Exception("No Digital Main collection defined!");

                using (var query = _cosmosContainer.GetItemLinqQueryable<FundooNote>()
                                .Where(n => n.Collaborations.Contains(email))
                                .ToFeedIterator())
                {
                    List<FundooNote> allDocuments = new List<FundooNote>();
                    while (query.HasMoreResults)
                    {
                        var queryResult = await query.ReadNextAsync();

                        allDocuments.AddRange(queryResult.ToList());
                    }

                    return allDocuments;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task<FundooNote> GetFundooNoteById(string email, string id)
        {
            try
            {
                if (string.IsNullOrEmpty(_docDbDigitalMainCollectionName))
                    throw new Exception("No Digital Main collection defined!");

                FundooNote document = await _cosmosContainer.ReadItemAsync<FundooNote>(id, new PartitionKey(id));

                return document;
                
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public Task<FundooNote> UpdateFundooNote(string email, FundooNote updatedFundooNote)
        {
            throw new NotImplementedException();
        }
    }
}
