using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.Metadata.Edm;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Transactions;
using EFSpecs.Mapping;
using EFSpecs.Reflection;

namespace EFSpecs
{
    public class PersistenceSpecification<TEntity> 
        where TEntity : class
    {
        private readonly Func<DbContext> _createContext;
        private readonly List<Property> _properties;
        private readonly List<Reference> _references;
	    private ReadOnlyMetadataCollection<EdmMember> _keyMembers;

        public PersistenceSpecification(Func<DbContext> createContext)
        {
            _createContext = createContext;
            _properties = new List<Property>();
            _references = new List<Reference>();
        }

        public PersistenceSpecification<TEntity> CheckProperty<TProperty>(Expression<Func<TEntity, TProperty>> property, object value)
        {
            var accessor = ReflectionHelper.ExtractPropertyInfo(property).ToAccessor();
            _properties.Add(new Property(accessor, value));
            return this;
        }

        public PersistenceSpecification<TEntity> CheckReference<TProperty>(Expression<Func<TEntity, TProperty>> property, object value)
        {
            var accessor = ReflectionHelper.ExtractPropertyInfo(property).ToAccessor();
            _references.Add(new Reference(accessor, value));
            return this;
        }

        public void VerifyMappings()
        {
            try
            {
                using (new TransactionScope())
                {
                    object[] id;
                    using (var ctx = _createContext())
                    {
                        var objectContext = ((IObjectContextAdapter) ctx).ObjectContext;
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
                        foreach (var property in _properties)
                        {
                            property.AssertValue(actual);
                        }
                        foreach (var reference in _references)
                        {
                            reference.AssertValue(ctx, actual);
                        }
                    }

                    throw new CoastIsClearException();
                }
            }
            catch (CoastIsClearException)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Rolling back all transactions");
                Console.ResetColor();
            }
            catch (DbEntityValidationException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                Console.WriteLine("Rolling back all transactions");
                Console.ResetColor();
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: {0}", exception.GetBaseException().Message);
                Console.WriteLine("Rolling back all transactions");
                Console.ResetColor();
            }
        }

        private void SaveEntityToDb(DbContext ctx, TEntity expected)
        {
            var dbSet = GetDbSet(ctx);

            try
            {
                Console.WriteLine("Adding instance to db set");
                dbSet.Add(expected);
                Console.WriteLine("Saving changes to database");
                ctx.SaveChanges();
                Console.WriteLine("Entity saved with id {0}", GetKeyValue(expected));
            }
            catch (Exception exception)
            {
                var baseException = exception.GetBaseException();
                if (baseException is SqlException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(baseException.Message);
                    Console.ResetColor();
                }
                throw;
            }
        }

        private void SetPropertiesOnEntity(TEntity expected)
        {
            foreach (var property in _properties)
            {
                property.SetValue(expected);
            }
            foreach (var reference in _references)
            {
                reference.SetValue(expected);
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
                var propertyInfo = typeof (TEntity).GetProperty(keyMember.Name);
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

    public class CoastIsClearException : Exception
    {
    }
}