using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AutreMachine.Common
{
    public class Repository<T, U> : IRepository<T, U> where T : BaseClass where U:DbContext
    {
        public U context { get; set; }

        private DbSet<T> entities;
        string errorMessage = string.Empty;

        private string typeName;

        public Repository(U context)
        {
            this.context = context;
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            entities = context.Set<T>();

            this.typeName = typeof(T).Name;
        }

        public IEnumerable<T> GetAll()
        {
            return entities.Select(x => x).AsEnumerable();
        }

        public IQueryable<T> GetAllQuery()
        {
            return entities.Select(x => x).AsQueryable();
        }

        public async Task<T> Get(int Id)
        {
            return await entities.SingleOrDefaultAsync(s => s.Id == Id);
        }

        /// <summary>
        /// Insère l'entité en base, et renvoie une entité avec Id rempli
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<int> Insert(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }


            // Date Created
            // ------------
            entity.DateCreated = DateTime.UtcNow;


            entities.Add(entity);
            await context.SaveChangesAsync();
            return entity.Id;
        }

        /// <summary>
        /// Insère l'entité en base SANS COMMIT
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task InsertNoCommit(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }


            // Date Created
            // ------------
            entity.DateCreated = DateTime.UtcNow;


            await entities.AddAsync(entity);

        }

        public async Task Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            context.Update(entity);

            await context.SaveChangesAsync();

        }

        public void UpdateNoCommit(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            context.Update(entity);

        }

        public async Task Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            entities.Remove(entity);
            await context.SaveChangesAsync();
        }

        public void Remove(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");
            entities.Remove(entity);

        }

       
        public async Task DeleteRange(IEnumerable<T> instances)
        {
            if (instances == null)
                throw new ArgumentNullException("entity");
            
            entities.RemoveRange(instances);
            await context.SaveChangesAsync();

        }

        public void SaveChanges()
        {
            context.SaveChanges();

            
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task BeginTransaction()
        {
            await context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransaction()
        {
            await context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransaction()
        {
            await context.Database.RollbackTransactionAsync();
        }
        public int ExecuteSql(string sql)
        {
            return this.context.Database.ExecuteSqlRaw(sql);
        }
        public async Task<int> ExecuteSql(string sql, params object[] parameters)
        {
            return await this.context.Database.ExecuteSqlRawAsync(sql, parameters);
        }
    }

}
