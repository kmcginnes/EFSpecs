using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using EFSpecs.Mapping;
using EFSpecs.Reflection;

namespace EFSpecs
{
    public class MappingsValidator<TEntity> where TEntity : class
    {
        private readonly Func<DbContext> _createContext;
        private readonly IEnumerable<IEntityValueAsserter> _mappings;
        private ReadOnlyMetadataCollection<EdmMember> _keyMembers;

        public MappingsValidator(Func<DbContext> createContext, IEnumerable<IEntityValueAsserter> mappings)
        {
            _createContext = createContext;
            _mappings = mappings;
        }

        public void Verify()
        {
            object[] id;
            using (var ctx = _createContext())
            {
                var objectContext = ((IObjectContextAdapter)ctx).ObjectContext;
                var entitySet = objectContext.GetEntitySet<TEntity>();
                _keyMembers = entitySet.ElementType.KeyMembers;

                var expected = objectContext.CreateObject<TEntity>();
                _mappings.ForEach(x => x.SetValue(expected));

                objectContext.AddObject(entitySet.Name, expected);
                objectContext.SaveChanges();

                id = GetKeyValue(expected);
            }

            using (var ctx = _createContext())
            {
                var actual = GetActualEntity(ctx, id);
                _mappings.ForEach(x => x.CheckValue(ctx, actual));
            }
        }

        private object[] GetKeyValue(TEntity expected)
        {
            var keyValues = new List<object>();
            foreach (var keyMember in _keyMembers)
            {
                var accessor = typeof(TEntity).GetPropertyAccessor(keyMember.Name);
                var value = accessor.GetValue(expected);
                keyValues.Add(value);
            }
            return keyValues.ToArray();
        }

        private TEntity GetActualEntity(DbContext ctx, object[] id)
        {
            var dbSet = ctx.Set<TEntity>();
            return dbSet.Find(id);
        }
    }
}