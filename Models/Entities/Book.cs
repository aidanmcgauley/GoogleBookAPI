namespace GoogleBookAPI.Models.Entities
{
    public class Book
    {
        public int Id { get; set; } // primary key in your DB
        public string GoogleId { get; set; } // maps to "id" from JSON
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public string? Publisher { get; set; }
        public string? PublishedDate { get; set; }
        public int? PageCount { get; set; }
        public string? PrintType { get; set; }
        public string? Language { get; set; }
        public double? AverageRating { get; set; }
        public int? RatingsCount { get; set; }
        public string? MaturityRating { get; set; }

        // Navigation properties
        public ICollection<Author> Authors { get; set; } = new List<Author>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
        public ICollection<IndustryIdentifier> IndustryIdentifiers { get; set; } = new List<IndustryIdentifier>();

    }
}
