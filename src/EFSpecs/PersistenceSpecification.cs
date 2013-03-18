using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq.Expressions;
using System.Text;
using System.Transactions;
using EFSpecs.Reflection;

namespace EFSpecs
{
    public class PersistenceSpecification<TEntity> 
        where TEntity : class
    {
        private readonly Func<DbContext> _createContext;
        private readonly List<IEntityValueAsserter> _properties;

        public PersistenceSpecification(Func<DbContext> createContext)
        {
            _createContext = createContext;
            _properties = new List<IEntityValueAsserter>();
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
            _properties.Add(new Reference(accessor, value));
            return this;
        }

        public void VerifyMappings()
        {
            try
            {
                using (new TransactionScope())
                {
                    new MappingsValidator<TEntity>(_createContext, _properties).Verify();

                    throw new CoastIsClearException();
                }
            }
            catch (Exception exception)
            {
                if (!HandleException(exception))
                {
                    throw exception;
                }
            }
        }

        private bool HandleException(Exception exception)
        {
            if (exception is CoastIsClearException)
            {
                // Do nothing because all assertions passed
                return true;
            }
            else if (exception is AssertionException)
            {
                return false;
            }
            else if (exception is DbEntityValidationException)
            {
                var e = (DbEntityValidationException) exception;
                var messageBuilder = new StringBuilder();
                foreach (var eve in e.EntityValidationErrors)
                {
                    messageBuilder.AppendFormat(
                        "Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        messageBuilder.AppendFormat(
                            "- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }

                throw new AssertionException(messageBuilder.ToString());
            }
            else
            {
                var message = string.Format("Error: {0}", exception.GetBaseException().Message);

                throw new AssertionException(message);
            }
        }
    }
}