using SQLite;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventorySystems
{
    public class ItemService
    {
        private readonly SQLiteAsyncConnection _database;

        // Constructor to initialize the database connection
        public ItemService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Item>().Wait(); // Ensure the items table is created
        }

        // Method to get all items
        public Task<List<Item>> GetAllItemsAsync()
        {
            return _database.Table<Item>().ToListAsync();
        }

        // Method to get an item by its ID
        public Task<Item> GetItemByIdAsync(int id)
        {
            return _database.Table<Item>().Where(i => i.ProductID == id).FirstOrDefaultAsync();
        }

        // Method to add a new item
        public Task<int> AddItemAsync(Item item)
        {
            return _database.InsertAsync(item);
        }

        // Method to update an existing item
        public Task<int> UpdateItemAsync(Item item)
        {
            return _database.UpdateAsync(item);
        }

        // Method to delete an item
        public Task<int> DeleteItemAsync(Item item)
        {
            return _database.DeleteAsync(item);
        }
    }
}
