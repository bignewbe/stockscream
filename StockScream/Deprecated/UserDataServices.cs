using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StockScream.Models;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using System.Diagnostics;
using System.Configuration;
using CommonCSharpLibary.Stock;
using System.Threading.Tasks;
using StockScream.DataModels;

namespace StockScream.Services
{
    public interface IDbServices<TEntity, TKey>
    {
        Task<bool> Create(TEntity entity);
        Task<bool> Update(TEntity entity);

        Task<TEntity> GetById(TKey id);
        Task<bool> DeleteById(TKey id);

        Task<TEntity> GetByEmail(string email);
        Task<bool> DeleteByEmail(string email);
    }

    public interface IUserDataServices : IDbServices<UserProfile, string>
    {
    }

    public class UserDataServices : IUserDataServices
    {
        IMongoCollection<UserProfile> _collections;
        public UserDataServices(string connectionStr, string dbName)
        {
            var client = new MongoClient(connectionStr);
            var db = client.GetDatabase(dbName);
            _collections = db.GetCollection<UserProfile>(dbName);
        }

        public async Task<bool> Create(UserProfile entity)
        {
            try
            {
                await _collections.InsertOneAsync(entity);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> Update(UserProfile entity)
        {
            try
            {
                await _collections.ReplaceOneAsync<UserProfile>(x => x.Id == entity.Id, entity);   //replace entire object
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        public async Task<UserProfile> GetById(string id)
        {
            var lst = await(await _collections.FindAsync(u => u.Id == id)).ToListAsync();
            if (lst == null || lst.Count == 0) return null;
            return lst[0];
        }
        public async Task<UserProfile> GetByEmail(string email)
        {
            var lst = await (await _collections.FindAsync(u => u.Email == email)).ToListAsync();
            if (lst == null || lst.Count == 0) return null;
            return lst[0];
        }
        
        public async Task<bool> DeleteById(string id)
        {
            try
            {
                await _collections.DeleteOneAsync<UserProfile>(x => x.Id == id);
                return true;
            }
            catch
            {
                return false;
            }
        }        
        public async Task<bool> DeleteByEmail(string email)
        {
            try {
                await _collections.DeleteOneAsync<UserProfile>(x => x.Email == email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}