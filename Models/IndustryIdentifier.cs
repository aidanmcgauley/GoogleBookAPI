namespace GoogleBookAPI.Models
{
    public class IndustryIdentifier
    {
        public int Id { get; set; }
        public string Type { get; set; } // ISBN_10, ISBN_13
        public string Identifier { get; set; }

        public int BookId { get; set; }
        public Book Book { get; set; }
    }

}
