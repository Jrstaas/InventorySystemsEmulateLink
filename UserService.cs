using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
//THIS IS EMULATING BACKEND FOR RIGHT NOW//
namespace InventorySystems
{
    public class UserService
    {
        private SQLiteConnection _dbConnection;

        public UserService(string databasePath)
        {
            _dbConnection = new SQLiteConnection(databasePath);
            _dbConnection.CreateTable<User>(); // Ensure the User table is created
            AddTestUsers();
        }

        // Method to check if user credentials are correct
        public async Task<User> CheckUserLogin(string username, string passwordHash)
        {
            var user = _dbConnection.Table<User>().FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
            return user;
        }

        // Other methods to interact with users (e.g., add, update, delete)
        public void AddUser(User user)
        {
            _dbConnection.Insert(user);
        }

        public User GetUserByUsername(string username)
        {
            return _dbConnection.Table<User>().FirstOrDefault(u => u.Username == username);
        }
        public void AddTestUsers()
        {
            var existingUser = _dbConnection.Table<User>().FirstOrDefault(u => u.Username == "testuser");
            if (existingUser == null)
            {
                _dbConnection.Insert(new User
                {
                    Username = "jake",
                    PasswordHash = "staas" // Make sure to use the same hash method as in the login check
                });
            }
        }
    }
}
