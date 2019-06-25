using System;
using System.Threading.Tasks;
using Apache.Ignite.Core.Transactions;

namespace Sedio.Ignite
{
    public sealed class IgniteTransaction : IDisposable,IAsyncDisposable
    {
        private readonly ITransaction transaction;
        private bool wasCommitted;

        internal IgniteTransaction(ITransaction transaction)
        {
            this.transaction = transaction;
        }

        public void Commit()
        {
            transaction.Commit();
            wasCommitted = true;
        }

        public async Task CommitAsync()
        {
            await transaction.CommitAsync().ConfigureAwait(false);
            wasCommitted = true;
        }

        public void Dispose()
        {
            try
            {
                if (!wasCommitted)
                {
                    transaction.Rollback();
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                if (!wasCommitted)
                {
                    await transaction.RollbackAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                transaction.Dispose();
            }
        }
    }
}