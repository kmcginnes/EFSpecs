using System.Data.Entity;

namespace EFSpecs
{
    public interface IEntityValueAsserter
    {
        void SetValue(object entity);
        void AssertValue(DbContext ctx, object entity);
    }
}