using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Blog> Blogs { get; }
        IRepository<BlogSetting> BlogSettings { get; }
        IRepository<Comment> Comments { get; }
        IRepository<Entry> Entries { get; }
        IRepository<Media> Medias { get; }
        bool SaveChanges();
        Task<bool> SaveChangesAsync(CancellationToken cancellationToken);
        public Task BeginTransactionAsync();
        public Task CommitTransactionAsync(CancellationToken cancellationToken);
        public void RollbackTransaction();
        public Task<T> ExecuteWithinTransactionAsync<T>(Func<Task<T>> action);

    }
}
