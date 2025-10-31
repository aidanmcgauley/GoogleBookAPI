namespace GoogleBookAPI.Models.DTOs
{
    public class ManagerBookDTO
    {
        public string GoogleId { get; set; }
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

        public List<AuthorDTO> Authors { get; set; } = new();
        public List<CategoryDTO> Categories { get; set; } = new();
        public List<IndustryIdentifierDTO> IndustryIdentifiers { get; set; } = new();
    }
}
