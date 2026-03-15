using System.Data;
using System.Data.Common;

namespace TinyUrlApp.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbConnection _connection;
        private DbTransaction _transaction;
        public bool IsDisposed { get; private set; } = false;
        public string ConnectionString { get; set; }
        public UnitOfWork(DbConnection connection) { 
            _connection = connection;
        }
        public DbTransaction Transaction => _transaction;
        public DbConnection GetDbConnection()
        {
            if (string.IsNullOrWhiteSpace(_connection.ConnectionString))
                _connection.ConnectionString = ConnectionString;
            if (_connection.State == ConnectionState.Closed)
                _connection.Open();
            return _connection;
        }
        public DbTransaction Begin()
        {
            _transaction = GetDbConnection().BeginTransaction(IsolationLevel.ReadUncommitted);

            return _transaction;
        }

        public async Task<DbTransaction> BeginAsync()
        {
            _transaction = await GetDbConnection().BeginTransactionAsync();

            return _transaction;
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public async Task CommitAsync()
        {
            await _transaction.CommitAsync();
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            if (_connection != null)
            {
                if (_connection.State == System.Data.ConnectionState.Open)
                    _connection.Close();
                _connection.Dispose();
            }
            IsDisposed = true;
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
        }
    }
}
