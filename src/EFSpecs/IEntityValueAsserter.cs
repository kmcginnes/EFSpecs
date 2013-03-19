using System.Data.Entity;

namespace EFSpecs
{
    public interface IEntityValueAsserter
    {
        void SetValue(object entity);
        void CheckValue(DbContext ctx, object entity);
    }
}