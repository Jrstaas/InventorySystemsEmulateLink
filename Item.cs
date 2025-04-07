using SQLite;
using System;

namespace InventorySystems
{
    public class Item
    {
        [PrimaryKey, AutoIncrement]
        public int ProductID { get; set; }

        public string ProductName { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public decimal UnitPrice { get; set; }

        public int StockQuantity { get; set; }

        //public int? ReorderLevel { get; set; } not sure on plan for this

        public string ImageUrl { get; set; }  // New property to store the image URL

        public Item() { }

        // Updated constructor to include ImageUrl
        public Item(string productName, string description, string category, decimal unitPrice, int stockQuantity, int? reorderLevel, string imageUrl)
        {
            ProductName = productName;
            Description = description;
            Category = category;
            UnitPrice = unitPrice;
            StockQuantity = stockQuantity;
            //ReorderLevel = reorderLevel; not sure on plan for this
            ImageUrl = imageUrl; 
        }
    }
}
