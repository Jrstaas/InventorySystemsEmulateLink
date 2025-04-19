using InventorySystems.Data;
using InventorySystems.Models;
using Microsoft.Maui.Controls;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystems
{
    public partial class SelectUserPage : ContentPage
    {
        private readonly Query _query;

        public SelectUserPage(string dbPath)
        {
            InitializeComponent();
            _query = new Query(dbPath); // Pass the database path to the Query class
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Fetch all users from the database using Query class
            var users = await _query.GetUsersAsync();
            userListView.ItemsSource = users;
        }

        private async void OnUserSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem is User selectedUser)
            {
                // Display prompt to confirm the password for deletion
                string password = await DisplayPromptAsync("Confirm Password", "Enter your password to confirm");

                if (!string.IsNullOrEmpty(password))
                {
                    // Fetch the selected user from the database for password validation
                    var userToDelete = await _query.GetUsersAsync();  // Get all users to find the matching one

                    var validUser = userToDelete.FirstOrDefault(u => u.Username == selectedUser.Username);

                    if (validUser != null && VerifyPassword(password, validUser.PasswordHash))
                    {
                        await _query.DeleteUserAsync(validUser); // Assuming DeleteUserAsync handles the delete in Query class
                        await DisplayAlert("Success", "User account deleted successfully.", "OK");
                        await Navigation.PopToRootAsync(); // Return to login page
                    }
                    else
                    {
                        await DisplayAlert("Error", "Password is incorrect. Please try again.", "OK");
                    }
                }
            }
        }

        // Method to verify the password using hashing
        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            // Assuming you're using SHA256 (replace with your hashing logic)
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(enteredPassword));
                string hashedPassword = Convert.ToBase64String(hashBytes);

                return storedHash == hashedPassword; // Compare the entered password hash with the stored hash
            }
        }
    }
}