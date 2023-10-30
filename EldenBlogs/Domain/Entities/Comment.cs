namespace Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public required string CommentContent { get; set; }
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;
        public required Guid Fk_Id_User { get; set; }
    }
}
