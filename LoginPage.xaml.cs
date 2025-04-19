using Microsoft.Maui.Controls;
using InventorySystems.Data;
using InventorySystems.Models;
using System;
using System.Threading.Tasks;

namespace InventorySystems
{
    public partial class LoginPage : ContentPage
    {
        private readonly Query _query;

        public LoginPage(Query query)
        {
            try
            {
                InitializeComponent();
                string dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "inventsys.db");
                _query = new Query(dbPath); // Initialize your Query class with the database path
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in LoginPage constructor: {ex.Message}");
            }
        }

        // Login button clicked
        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var username = Username.Text;
            var password = PasswordHash.Text; // The entered password (no hashing)

            // Call the ValidateUserLogin method in Query class
            var user = await _query.ValidateUserLoginAsync(username, password);

            if (user != null)
            {
                // Successful login, access user properties
                var userId = user.UserID;
                var usernameHash = user.Username;

                // Proceed to the main page, passing user details
                await Navigation.PushAsync(new MainPage(userId, usernameHash, _query));
            }
            else
            {
                // Failed login, show error message
                await DisplayAlert("Error", "Invalid username or password", "OK");
            }
        }

        // Event handler for Entry Completed event (Trigger login on Enter)
        private void OnLoginEntryCompleted(object sender, EventArgs e)
        {
            // Trigger OnLoginButtonClicked when Enter key is pressed
            OnLoginButtonClicked(sender, e);
        }

        // Navigate to the page where users can select and delete their account
        private async void OnRemoveUserClicked(object sender, EventArgs e)
        {
            string dbPath = FileSystem.AppDataDirectory + "/inventsys.db";
            // Navigate to the SelectUserPage where the user can select their account
            await Navigation.PushAsync(new SelectUserPage(dbPath));
        }
    }
}
