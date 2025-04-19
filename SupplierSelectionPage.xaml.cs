using Microsoft.Maui.Controls;
using InventorySystems.Models;
using InventorySystems.Data;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InventorySystems
{
    public partial class SupplierSelectionPage : ContentPage
    {
        private readonly Query _query;
        private List<Supplier>? _suppliers;  // Nullable type

        public SupplierSelectionPage()
        {
            InitializeComponent();  // This will link to the XAML file
            string dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "inventsys.db");
            _query = new Query(dbPath); // Initialize your Query class with the database path
            LoadSuppliers();
        }

        private async void LoadSuppliers()
        {
            _suppliers = (await _query.GetSuppliersAsync()).ToList(); // Fetch the suppliers using the Query class
            SupplierListView.ItemsSource = _suppliers; // Bind ListView to the list of suppliers
        }
    }
}
