using CookItUniversal.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace CookItUniversal.SQLiteController
{
    public class SQLiteController<T> 
        where T : DBBaseModel, new()
    {
        private const string dbName = "Recipies.db";

        public async Task<bool> CheckDbAsync()
        {
            bool dbExist = true;

            try
            {
                StorageFile sf = await ApplicationData.Current.LocalFolder.GetFileAsync(dbName);
            }
            catch (Exception)
            {
                dbExist = false;
            }

            return dbExist;
        }

        public async Task CreateTableIfNotExist()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);
            await conn.CreateTableAsync<T>();
        }

        public async Task AddItemAsync(T item)
        {
            if (item == null)
            {
                return;
            }

            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);
            await conn.InsertAsync(item);
        }

        public async Task AddItemsAsync(IList<T> list)
        {
            if (list == null || list.Count == 0)
            {
                return;
            }

            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);
            await conn.InsertAllAsync(list);
        }

        public async Task<IList<T>> FetchItems()
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);

            AsyncTableQuery<T> query = conn.Table<T>();
            List<T> result = await query.ToListAsync();

            return result;
        }

        public async Task<IList<T>> FetchItemsByRecipeId(string recipeId)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);

            AsyncTableQuery<T> query = conn.Table<T>().Where(item => item.RecipeId == recipeId);
            List<T> result = await query.ToListAsync();

            return result;
        }

        public async Task<T> FindItemById(string id)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);

            AsyncTableQuery<T> query = conn.Table<T>().Where(item => item.Id == id);
            T result = await query.FirstOrDefaultAsync();

            return result;
        }

        public async Task DeleteItemsAsync(string recipeId)
        {
            SQLiteAsyncConnection conn = new SQLiteAsyncConnection(dbName);

            var items = await conn.Table<T>().Where(x => x.Id == recipeId || x.RecipeId == recipeId).ToListAsync();
            if (items != null && items.Count>0)
            {
                foreach (var item in items)
                {
                    await conn.DeleteAsync(item);
                }                
            }
        }
    }
}
