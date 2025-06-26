using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
    /// <summary>
    /// Interface for base Service
    /// </summary>
    /// <typeparam name="T">The BaseClass derivative class</typeparam>
    /// <typeparam name="U">The DBContext used for the service</typeparam>
    public interface IBaseService<T,U> where U: DbContext
    {

        /// <summary>
        /// Récupère tous les contrats d'un utilisateur
        /// </summary>
        /// <returns></returns>
        //IEnumerable<T> GetAll();
        IQueryable<T> GetAllQuery();

        /// <summary>
        /// Get an item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T> Get(int id);

        Task<int> Insert(T instance);
        Task Update(T instance);
        void UpdateNoCommit(T instance);
        Task Delete(int id);
        Task Delete(T instance);
        Task DeleteNoCommit(T instance);
        Task InsertNoCommit(T instance);

        void SaveChanges();
        Task SaveChangesAsync();

        Task BeginTransaction();

        Task CommitTransaction();

        Task RollbackTransaction();

        void ExecuteSql(string sql);
    }
}
