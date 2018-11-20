using System;
using System.Collections.Generic;
using System.Data.Entity; // for: DbContext
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
    This code may be DANGEROUS because it was made in mind with
    multiple concurrent database manipulation on the same row for cases of very short peak loads only.
    
    Consider changing loops into delay. Use with care!!!
 */

namespace ServiceLibrary.Tools
{
    /// <remarks>All methods utilise DbUpdateConcurrencyException and re-throws all other exceptions.</remarks>
    public static class SingleChange
    {
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();


        public static async Task<bool> EnsureEntityAsync<EntityClass>(
            DbContext context, IQueryable<EntityClass> isAnyRegisteredQuery, EntityClass entityToEnsure, int maxRetryCount = 5)
            where EntityClass : class, new()
        {
            DbSet<EntityClass> entityObjects = context.Set<EntityClass>();
            int retryCount = 0;

            do
            {
                try
                {
                    if (isAnyRegisteredQuery != null && await isAnyRegisteredQuery.AnyAsync())
                        return true;

                    entityObjects.Add(entityToEnsure);


                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                ++retryCount;

            } while (retryCount < maxRetryCount);

            return false;
        }

        public static async Task<bool> InsertEntityAsync<EntityClass>(
            DbContext context, EntityClass newEntity, int maxRetryCount = 5)
            where EntityClass : class, new()
        {
            DbSet<EntityClass> entityObjects = context.Set<EntityClass>();
            int retryCount = 0;

            do
            {
                try
                {
                    entityObjects.Add(newEntity);

                    await context.SaveChangesAsync();
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }

                ++retryCount;

            } while (retryCount < maxRetryCount);

            return false;
        }

        public static async Task<bool> UpdateEntityAsync<EntityClass>(
            DbContext context, IQueryable<EntityClass> singleRowQuery, Action<EntityClass> propertyAction, int maxRetryCount = 5)
            where EntityClass : class, new()
        {
            System.Data.Entity.Infrastructure.DbUpdateConcurrencyException exception;
            int retryCount = 0;
            EntityClass entity;

            entity = await singleRowQuery.FirstOrDefaultAsync();
            if (entity == null)
                return false;

            do
            {
                try
                {
                    propertyAction(entity);

                    context.Entry(entity).State = System.Data.Entity.EntityState.Modified;


                    await context.SaveChangesAsync();
                    return true;
                }
                catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
                {
                    exception = ex;
                }
                catch (Exception)
                {
                    throw;
                }

                await exception.Entries.Single().ReloadAsync();
                ++retryCount;

            } while (retryCount < maxRetryCount);

            return false;
        }


        #region Guarded save changes

        public static bool Save(System.Data.Entity.DbContext context)
        {
            try
            {
                context.SaveChanges();
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {
                ex.Entries.Single().Reload();
            }
            catch (Exception)
            {
                throw;
            }
            return false;
        }

        public static async Task<bool> SaveAsync(System.Data.Entity.DbContext context)
        {
            System.Data.Entity.Infrastructure.DbUpdateConcurrencyException exception;

            try
            {
                await context.SaveChangesAsync();
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {
                exception = ex;
            }
            catch (Exception)
            {
                throw;
            }

            await exception.Entries.Single().ReloadAsync();
            return false;
        }

        public static async Task<bool> SaveAsync(System.Data.Entity.DbContext context, System.Threading.CancellationToken cancellationToken)
        {
            System.Data.Entity.Infrastructure.DbUpdateConcurrencyException exception;

            try
            {
                await context.SaveChangesAsync(cancellationToken);
                return true;
            }
            catch (System.Data.Entity.Infrastructure.DbUpdateConcurrencyException ex)
            {
                exception = ex;
            }
            catch (Exception)
            {
                throw;
            }

            await exception.Entries.Single().ReloadAsync(cancellationToken);
            return false;
        }

        #endregion // Guarded save changes
    }
}
