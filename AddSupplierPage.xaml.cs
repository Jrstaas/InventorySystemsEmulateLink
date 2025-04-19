using Microsoft.Maui.Controls;
using InventorySystems.Models;
using InventorySystems.Data;

namespace InventorySystems
{
    public partial class AddSupplierPage : ContentPage
    {
        private readonly Query _query;

        public AddSupplierPage(Query query)
        {
            InitializeComponent();
            string dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "inventsys.db");
            _query = new Query(dbPath);  // Initialize your Query class with the database path
        }

        // This method will be triggered when the user clicks on the "Save" button
        private async void OnSaveClicked(object sender, EventArgs e)
        {
            // Gather input values from the entry fields
            var supplierName = SupplierNameEntry.Text;
            var contactName = ContactNameEntry.Text;
            var contactEmail = ContactEmailEntry.Text;
            var contactPhone = ContactPhoneEntry.Text;
            var address = AddressEntry.Text;

            // Create a new Supplier object
            var newSupplier = new Supplier
            {
                SupplierName = supplierName,
                ContactName = contactName,
                ContactEmail = contactEmail,
                ContactPhone = contactPhone,
                Address = address
            };

            // Insert the new supplier into the database using the Query class
            await _query.InsertOrUpdateSupplierAsync(newSupplier);

            // After saving, go back to the previous page (Supplier Selection Page)
            await Navigation.PopAsync();
        }

        // Cancel and return to the previous page
        private async void OnCancelClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
