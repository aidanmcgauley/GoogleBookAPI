namespace GoogleBookAPI.Models.DTOs
{
    public class CustomerBookDTO
    {
        public string Title { get; set; }
        public string? Subtitle { get; set; }
        public string? Description { get; set; }
        public int? PageCount { get; set; }
        public double? AverageRating { get; set; }
        public int? RatingsCount { get; set; }

        public List<AuthorDTO> Authors { get; set; } = new();
    }
}
