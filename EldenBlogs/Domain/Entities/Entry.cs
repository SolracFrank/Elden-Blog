namespace Domain.Entities
{
    public class Entry
    {
        public int Id { get; set; }
        public required string EntryTitle { get; set; }
        public string? EntryBody { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public int Fk_Id_Blog { get; set; }

    }
}
