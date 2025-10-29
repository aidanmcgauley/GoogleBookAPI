using GoogleBookAPI.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GoogleBookAPI.Services
{
    public class BookIngestionService
    {
        private readonly HttpClient _httpClient;
        private readonly BookDbContext _dbContext;

        public BookIngestionService(HttpClient httpClient, BookDbContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task IngestDataFromEndpoint(string endpointUrl)
        {
            try
            {
                var response = await _httpClient.GetStringAsync(endpointUrl);
                var jsonEvents = JsonConvert.DeserializeObject<List<dynamic>>(response);

                foreach (var jsonEvent in jsonEvents)
                {
                    await ProcessBook(jsonEvent);
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ingesting data: {ex.Message}");
                throw;
            }
        }




    }
}
