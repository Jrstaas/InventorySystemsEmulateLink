using Microsoft.Maui.Controls;
using System;

namespace InventorySystems
{
    public partial class LoginPage : ContentPage
    {
        private UserService _userService;

        public LoginPage()
        {
            InitializeComponent();
            _userService = new UserService(App.DatabasePath); // Set up the UserService with the database path
        }

        private async void OnLoginClicked(object sender, EventArgs e)
        {
            string enteredUsername = usernameEntry.Text;
            string enteredPassword = passwordEntry.Text;

            // Hash the entered password before checking (assuming you hash passwords for security)
            string hashedPassword = HashPassword(enteredPassword);

            var user = await _userService.CheckUserLogin(enteredUsername, hashedPassword);

            if (user != null)
            {
                // If valid, navigate to MainPage, passing UserService
                await Navigation.PushAsync(new MainPage(enteredUsername, _userService));
            }
            else
            {
                // Show error if invalid
                await DisplayAlert("Login Failed", "Invalid username or password. Please try again.", "OK");
            }
        }

        private string HashPassword(string password)
        {
            // For now, just return the password as is, use a secure hash method in production
            return password;
        }

        // Event handler for Entry Completed event
        private void OnLoginEntryCompleted(object sender, EventArgs e)
        {
            // You can either trigger the OnLoginClicked method directly or add other logic
            OnLoginClicked(sender, e);
        }
    }
}
