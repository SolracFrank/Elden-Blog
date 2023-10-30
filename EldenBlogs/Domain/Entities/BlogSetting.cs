namespace Domain.Entities
{
    public class BlogSetting
    {
        public Guid Id { get; set; }
        public bool IsMature { get; set; }
        public int Fk_Blog_Id { get; set; }
    }
}
