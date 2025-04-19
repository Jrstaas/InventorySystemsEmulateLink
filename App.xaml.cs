using Microsoft.Maui.Controls;
using InventorySystems.Data;

namespace InventorySystems
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Initialize the Query class with the database path
            string dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "inventsys.db");
            var query = new Query(dbPath);  // Use the correct path for the database file

            // Use the query object to initialize the LoginPage without DatabaseContext
            MainPage = new NavigationPage(new LoginPage(query));  // Pass query instead of databaseContext
        }
    }
}
