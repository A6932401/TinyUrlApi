using System.Data.Common;

namespace TinyUrlApp.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        bool IsDisposed { get; }
        string ConnectionString { get; set; }
        DbConnection GetDbConnection();
        DbTransaction Transaction { get; }
        DbTransaction Begin();
        void Commit();
        void Rollback();
        Task<DbTransaction> BeginAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
