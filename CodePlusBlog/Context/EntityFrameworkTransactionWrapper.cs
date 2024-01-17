using Microsoft.EntityFrameworkCore.Storage;

namespace CodePlusBlog.Context
{
    public class EntityFrameworkTransactionWrapper : IDbContextTransactionWrapper
    {
        private readonly RepositoryContext _context;

        public EntityFrameworkTransactionWrapper(RepositoryContext context)
        {
            _context = context;
        }
        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public void Commit()
        {
            _context.SaveChanges();
            _context.Database.CurrentTransaction.Commit();
        }

        public void Dispose()
        {
            _context.Database.CurrentTransaction.Dispose();
        }

        public void Rollback()
        {
            _context.Database.CurrentTransaction.Rollback();
        }
    }
}
