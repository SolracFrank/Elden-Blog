namespace Domain.Entities
{
    public class Media
    {
        public int Id_Media { get; set; }
        public enum Type { Video,Image}
        public required string MediaType { get; set; } 
        public required string MediaPath { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int Fk_Id_User { get; set; }
    }
}
