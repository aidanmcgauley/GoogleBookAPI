using GoogleBookAPI.Data;
using GoogleBookAPI.Models;
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
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response);
                var jsonBooks = jsonResponse.items;

                if(jsonBooks != null)
                {
                    foreach (var jsonBook in jsonBooks)
                    {
                        await ProcessBook(jsonBook);
                    }

                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    Console.WriteLine("No books found in JSON response");
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error ingesting data: {ex.Message}");
                throw;
            }
        }

        private async Task ProcessBook(dynamic jsonBook)
        {
            Console.WriteLine($"Processing book {jsonBook.id} - {jsonBook.volumeInfo.title}");

            string googleId = jsonBook.id;

            var existingBook = await _dbContext.Books
                .Include(b => b.Authors)
                .Include(b => b.Categories)
                .Include(b => b.IndustryIdentifiers)
                .FirstOrDefaultAsync(b => b.GoogleId == googleId);

            if (existingBook != null)
            {
                UpdateBook(existingBook, jsonBook);
            }
            else
            {
                CreateBook(jsonBook);
            }

        }

        private void CreateBook(dynamic jsonBook)
        {
            // Defensive check — sometimes API returns incomplete objects
            if (jsonBook == null || jsonBook.volumeInfo == null)
                return;

            var volumeInfo = jsonBook.volumeInfo;

            var book = new Book
            {
                GoogleId = jsonBook.id,
                Title = (string)volumeInfo.title,
                Subtitle = (string?)volumeInfo.subtitle,
                Description = (string?)volumeInfo.description,
                Publisher = (string?)volumeInfo.publisher,
                PublishedDate = (string?)volumeInfo.publishedDate,
                PageCount = (int?)volumeInfo.pageCount,
                PrintType = (string?)volumeInfo.printType,
                Language = (string?)volumeInfo.language,
                AverageRating = (double?)volumeInfo.averageRating,
                RatingsCount = (int?)volumeInfo.ratingsCount,
                MaturityRating = (string?)volumeInfo.maturityRating,
                Authors = new List<Author>(),
                Categories = new List<Category>(),
                IndustryIdentifiers = new List<IndustryIdentifier>()
            };

            // --- Authors ---
            if (volumeInfo.authors != null)
            {
                foreach (var authorNameDynamic in volumeInfo.authors)
                {
                    var authorName = (string)authorNameDynamic;
                    // Option to check for existing names
                    var existingAuthor = _dbContext.Authors.FirstOrDefault(a => a.Name == authorName);
                    if (existingAuthor != null)
                        book.Authors.Add(existingAuthor);
                    else
                        book.Authors.Add(new Author { Name = authorName });
                }

            }

            // --- Categories ---
            if (volumeInfo.categories != null)
            {
                foreach (var categoryName in volumeInfo.categories)
                {
                    book.Categories.Add(new Category
                    {
                        Name = (string)categoryName
                    });
                }
            }

            // --- Industry Identifiers ---
            if (volumeInfo.industryIdentifiers != null)
            {
                foreach (var identifier in volumeInfo.industryIdentifiers)
                {
                    book.IndustryIdentifiers.Add(new IndustryIdentifier
                    {
                        Type = (string)identifier.type,
                        Identifier = (string)identifier.identifier
                    });
                }
            }

            _dbContext.Books.Add(book);
        }

        private void UpdateBook(Book existingBook, dynamic jsonBook)
        {
            if (jsonBook == null || jsonBook.volumeInfo == null)
                return;

            var volumeInfo = jsonBook.volumeInfo;

            // --- Update only dynamic fields ---
            existingBook.Description = (string?)volumeInfo.description;
            existingBook.AverageRating = (double?)volumeInfo.averageRating;
            existingBook.RatingsCount = (int?)volumeInfo.ratingsCount;
            existingBook.MaturityRating = (string?)volumeInfo.maturityRating;

            // --- Sync Authors (in case names were corrected or added) ---
            if (volumeInfo.authors != null)
            {
                var jsonAuthors = ((IEnumerable<dynamic>)volumeInfo.authors)
                                  .Select(a => (string)a)
                                  .ToList();

                // Remove authors no longer in JSON
                var authorsToRemove = existingBook.Authors
                               .Where(a => !jsonAuthors.Contains(a.Name))
                               .ToList(); // materialize to avoid modifying while iterating

                foreach (var author in authorsToRemove)
                {
                    existingBook.Authors.Remove(author);
                }

                // Add missing authors
                foreach (var authorName in jsonAuthors)
                {
                    if (!existingBook.Authors.Any(a => a.Name == authorName))
                    {
                        var existingAuthor = _dbContext.Authors.FirstOrDefault(a => a.Name == authorName);
                        existingBook.Authors.Add(existingAuthor ?? new Author { Name = authorName });
                    }
                }
            }

            // --- Sync Categories ---
            if (volumeInfo.categories != null)
            {
                var jsonCategories = ((IEnumerable<dynamic>)volumeInfo.categories)
                                     .Select(c => (string)c)
                                     .ToList();

                // Remove categories no longer in the json
                var catsToRemove = existingBook.Categories
                            .Where(c => !jsonCategories.Contains(c.Name))
                            .ToList();

                foreach (var cat in catsToRemove)
                {
                    existingBook.Categories.Remove(cat);
                }


                foreach (var categoryName in jsonCategories)
                {
                    if (!existingBook.Categories.Any(c => c.Name == categoryName))
                        existingBook.Categories.Add(new Category { Name = categoryName });
                }
            }

            // --- Sync Industry Identifiers ---
            if (volumeInfo.industryIdentifiers != null)
            {
                var jsonIdentifiers = ((IEnumerable<dynamic>)volumeInfo.industryIdentifiers)
                                      .Select(ii => new { Type = (string)ii.type, Identifier = (string)ii.identifier })
                                      .ToList();

                // Remove IndustryIdentifiers no longer in JSON
                var identifiersToRemove = existingBook.IndustryIdentifiers
                    .Where(ii => !jsonIdentifiers.Any(j => j.Type == ii.Type && j.Identifier == ii.Identifier))
                    .ToList(); // materialize to avoid modifying while iterating

                foreach (var ii in identifiersToRemove)
                {
                    existingBook.IndustryIdentifiers.Remove(ii);
                }

                foreach (var ii in jsonIdentifiers)
                {
                    if (!existingBook.IndustryIdentifiers.Any(existing => existing.Type == ii.Type && existing.Identifier == ii.Identifier))
                    {
                        existingBook.IndustryIdentifiers.Add(new IndustryIdentifier
                        {
                            Type = ii.Type,
                            Identifier = ii.Identifier
                        });
                    }
                }
            }

            // No need to call _dbContext.Books.Add(existingBook)
            // EF is already tracking the entity; changes will be saved via SaveChangesAsync in ProcessBook
        }

    }
}