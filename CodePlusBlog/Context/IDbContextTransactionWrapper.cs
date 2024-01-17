using Microsoft.EntityFrameworkCore.Storage;

namespace CodePlusBlog.Context
{
    public interface IDbContextTransactionWrapper: IDisposable
    {
        IDbContextTransaction BeginTransaction();
        void Commit();
        void Rollback();
    }
}
