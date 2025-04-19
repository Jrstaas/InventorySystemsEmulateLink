using Microsoft.Maui.Controls;
using InventorySystems.Data;
using InventorySystems.Models;
using System;

namespace InventorySystems
{
    public partial class RegistrationPage : ContentPage
    {
        private readonly Query _query; // Assuming Query handles DB operations

        public RegistrationPage(string dbPath)
        {
            InitializeComponent();
            _query = new Query(dbPath);
        }

        private async void OnRegisterClicked(object sender, EventArgs e)
        {
            string username = usernameEntry.Text;
            string password = passwordEntry.Text;
            string email = emailEntry.Text;
            string role = "User"; // You can adjust role logic as needed (e.g., Admin, User, etc.)

            // Hash the password (use a secure hashing method in production)
            string hashedPassword = HashPassword(password);

            // Create a new User object
            var newUser = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                Email = email,
                Role = role
            };

            // Save the new user to the database
            await _query.InsertUserAsync(newUser); // Use the InsertUserAsync method from Query class

            // Navigate back to the LoginPage
            await DisplayAlert("Registration Successful", "You can now log in with your new account.", "OK");
            await Navigation.PopAsync(); // Return to the login page
        }

        private string HashPassword(string password)
        {
            // For now, just return the password as is
            return password; 
        }
    }
}
