using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
    /// <summary>
    /// Classe de base pour tous les Repository en Generic
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T, U> where U:DbContext
    {
        U context { get; set; }
        //IEnumerable<T> GetAll();
        IQueryable<T> GetAllQuery();
        Task<T> Get(int Id);
        Task<int> Insert(T entity);
        Task InsertNoCommit(T entity);
        Task Update(T entity);
        void UpdateNoCommit(T entity);
        /// <summary>
        /// Delete = Remove + context.SaveChanges()
        /// </summary>
        /// <param name="entity"></param>
        Task Delete(T entity);

        /// <summary>
        /// Remove = Pas de commit en base
        /// </summary>
        /// <param name="entity"></param>
        void Remove(T entity);

        Task DeleteRange(IEnumerable<T> instances);
        void SaveChanges();
        Task SaveChangesAsync();

        Task BeginTransaction();

        Task CommitTransaction();

        Task RollbackTransaction();

        int ExecuteSql(string sql);
        Task<int> ExecuteSql(string sql, params object[] parameters);
    }
}