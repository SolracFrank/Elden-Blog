namespace Domain.Entities
{
    public class Blog
    {
        public int Id { get; set; }
        public string? BannerPath { get; set; }
        public required string BlogTitle { get; set; }
        public string? BlogDescription { get; set; }
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; }
        public Guid Fk_Id_User { get; set; }

    }
}
