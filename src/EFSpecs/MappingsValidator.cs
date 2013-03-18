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

                var expected = CreateEntity(ctx);
                SetPropertiesOnEntity(expected);

                SaveEntityToDb(ctx, expected);
                id = GetKeyValue(expected);
            }

            using (var ctx = _createContext())
            {
                var actual = GetActualEntity(ctx, id);
                foreach (var property in _mappings)
                {
                    property.AssertValue(ctx, actual);
                }
            }
        }

        private void SaveEntityToDb(DbContext ctx, TEntity expected)
        {
            var dbSet = GetDbSet(ctx);
            dbSet.Add(expected);
            ctx.SaveChanges();
        }

        private void SetPropertiesOnEntity(TEntity expected)
        {
            foreach (var property in _mappings)
            {
                property.SetValue(expected);
            }
        }

        private TEntity CreateEntity(DbContext ctx)
        {
            var entity = GetDbSet(ctx).Create<TEntity>();
            return entity;
        }

        private object[] GetKeyValue(TEntity expected)
        {
            var keyValues = new List<object>();
            foreach (var keyMember in _keyMembers)
            {
                var propertyInfo = typeof(TEntity).GetProperty(keyMember.Name);
                var accessor = new PropertyAccessor(propertyInfo);
                var value = accessor.GetValue(expected);
                keyValues.Add(value);
            }
            return keyValues.ToArray();
        }

        private TEntity GetActualEntity(DbContext ctx, object[] id)
        {
            var dbSet = GetDbSet(ctx);
            return dbSet.Find(id);
        }

        private DbSet<TEntity> GetDbSet(DbContext ctx)
        {
            return ctx.Set<TEntity>();
        }
    }
}