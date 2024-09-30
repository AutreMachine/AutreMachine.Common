using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

namespace AutreMachine.Common
{
    /// <summary>
    /// Cette classe est un Helper à hériter pour créer des services plus rapidement - à condition qu'ils soient simples et standards.
    /// Attention : on ne tient pas compte du OwnerId pour les opérations (par exemple, pour JoinService)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseService<T> : IBaseService<T> where T : IBaseClass
    {
        public ILogger<T> logger { get; set; }
        public IRepository<T> repository { get; set; }

        public BaseService(IRepository<T> repository, ILogger<T> logger)
        {
            this.repository = repository;
            this.logger = logger;
        }

        public virtual IQueryable<T> GetAllQuery()
        {
            return repository.GetAllQuery();
        }

        public async Task<T> Get(int id)
        {

            var instance = await repository.Get(id);

            if (instance == null)
                return default(T);

                return instance;
        }

        public async Task<int> Insert(T instance)
        {
            if (instance == null)
                throw new NullReferenceException();


            var ret = await repository.Insert(instance);
            // Need to stop tracking changes - otherwise can't do 1 insert & an update in a row
            if (repository.context != null)
                repository.context.ChangeTracker.Clear();

            return ret;
        }

        public async Task  InsertNoCommit(T instance)
        {

            if (instance == null)
                throw new NullReferenceException();



            await repository.InsertNoCommit(instance);

        }
        public async Task Update(T instance)
        {

            if (instance == null)
                throw new NullReferenceException();


            await repository.Update(instance);
            // Need to stop tracking changes - otherwise can't do 2 updates in a row
            if (repository.context != null)
                repository.context.ChangeTracker.Clear();
        }
        public void UpdateNoCommit(T instance)
        {

            if (instance == null)
                throw new NullReferenceException();


            repository.UpdateNoCommit(instance);
        }

        public async Task Delete(int id)
        {

            var instance = await repository.Get(id);

            await Delete(instance);

        }

        public async Task Delete(T instance)
        {

            if (instance == null)
                throw new Exception("Object not found");


            await repository.Delete(instance);
            // Need to stop tracking changes - otherwise can't do 2 updates in a row
            if (repository.context != null)
                repository.context.ChangeTracker.Clear();

        }

        public async Task DeleteNoCommit(T instance)
        {

            if (instance == null)
                throw new Exception("Object not found");


            repository.Remove(instance);
        }

        public void SaveChanges()
        {
            repository.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await repository.SaveChangesAsync();
        }

        public async Task BeginTransaction()
        {
            await repository.BeginTransaction();
        }

        public async Task CommitTransaction()
        {
            await repository.CommitTransaction();
        }

        public async Task RollbackTransaction()
        {
            await repository.RollbackTransaction();
        }

        public void  ExecuteSql(string sql)
        {
            repository.ExecuteSql(sql);
        }


    }
}
