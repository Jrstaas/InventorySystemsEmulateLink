using Microsoft.Maui.Controls;
using InventorySystems.Models;
using System.Collections.ObjectModel;
using InventorySystems.Data;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace InventorySystems
{
    public partial class MainPage : ContentPage
    {
        private ObservableCollection<Product> products = new();
        private Query _query;
        private int _userId;
        private string _username;

        public MainPage(int userId, string username, Query query)
        {
            InitializeComponent();
            _userId = userId;
            _username = username;
            string dbPath = System.IO.Path.Combine(FileSystem.AppDataDirectory, "inventsys.db");
            _query = new Query(dbPath);
            LoadProducts();
        }

        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            string keyword = e.NewTextValue?.ToLower() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(keyword))
            {
                ProductCollectionView.ItemsSource = products;
            }
            else
            {
                var filtered = products.Where(p =>
                    (p.ProductName?.ToLower().Contains(keyword) ?? false) ||
                    (p.Description?.ToLower().Contains(keyword) ?? false) ||
                    (p.Category?.ToLower().Contains(keyword) ?? false)).ToList();

                ProductCollectionView.ItemsSource = filtered;
            }
        }

        private async void LoadProducts()
        {
            var allProducts = await _query.GetAllProductsAsync();  // Changed to load all products
            products = new ObservableCollection<Product>(allProducts);
            ProductCollectionView.ItemsSource = products;
        }

        private async void AddProductClicked(object sender, EventArgs e)
        {
            var name = ProductNameEntry.Text?.Trim();
            var description = DescriptionEntry.Text?.Trim();
            var category = CategoryEntry.Text?.Trim();
            var supplierId = int.TryParse(SupplierIdEntry.Text, out var sId) ? sId : 0;
            var unitPrice = decimal.TryParse(UnitPriceEntry.Text, out var price) ? price : 0;
            var quantity = int.TryParse(StockQuantityEntry.Text, out var qty) ? qty : 0;

            if (string.IsNullOrWhiteSpace(name)) return;

            var newProduct = new Product
            {
                ProductName = name,
                Description = description,
                Category = category,
                UnitPrice = unitPrice,
                StockQuantity = quantity,
                SupplierID = supplierId,
                UserID = _userId
            };

            await _query.AddProductToDatabaseAsync(newProduct);
            LoadProducts();
        }

        private async void AdjustValue(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Product product)
            {
                var parent = button.Parent as StackLayout;
                var quantityEntry = parent?.Children
                    .OfType<Entry>()
                    .FirstOrDefault(e => e.ClassId == "QuantityEntry");

                int quantity = 1;
                if (quantityEntry != null && int.TryParse(quantityEntry.Text, out int parsedQty))
                {
                    quantity = parsedQty;
                }

                if (quantity <= 0)
                {
                    await DisplayAlert("Invalid Quantity", "Please enter a valid quantity.", "OK");
                    return;
                }

                bool isBuy = button.Text.Equals("Buy", StringComparison.OrdinalIgnoreCase);
                int delta = isBuy ? quantity : -quantity;

                if (!isBuy && product.StockQuantity < quantity)
                {
                    await DisplayAlert("Error", "Not enough stock to sell.", "OK");
                    return;
                }

                product.StockQuantity += delta;

                await _query.UpdateProductInDatabaseAsync(product);

                var order = new Order
                {
                    OrderDate = DateTime.Now,
                    ProductID = product.ProductID,
                    Quantity = quantity,
                    Price = product.UnitPrice,
                    SubTotal = product.UnitPrice * quantity,
                    TotalAmount = product.UnitPrice * quantity,
                    UserID = _userId,
                    SupplierID = product.SupplierID
                };

                await _query.InsertOrderAsync(order);
                await SendOrderEmailAsync(order, product);
                await DisplayAlert("Success", isBuy ? "Purchase complete!" : "Sale recorded!", "OK");
                LoadProducts();
            }
        }



        private async void DeleteProductClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Product product)
            {
                await _query.DeleteProductInDatabaseAsync(product.ProductID);
                LoadProducts();
            }
        }
        private async Task SendOrderEmailAsync(Order order, Product product)
        {
            try
            {
                string fromEmail = "2810285@vikes.csuohio.edu";     // Your sender email
                string password = "Ryan2004";           // Your sender password or app password (hack me)
                string toEmail = "2810285@vikes.csuohio.edu";          // Who receives the email (maybe the user or fixed email)

                var smtpClient = new SmtpClient("smtp.gmail.com")  // Use your SMTP server
                {
                    Port = 587,
                    Credentials = new NetworkCredential(fromEmail, password),
                    EnableSsl = true,
                };

                string subject = $"Order Confirmation - Product ID: {product.ProductID}";
                string body = $@"
Order Confirmation

Product: {product.ProductName}
Description: {product.Description}
Category: {product.Category}
Price per Unit: {product.UnitPrice:C}
Quantity: {order.Quantity}
Subtotal: {order.SubTotal:C}
Total: {order.TotalAmount:C}
Date: {order.OrderDate:g}

Thank you for using InventorySystems.";

                var mailMessage = new MailMessage(fromEmail, toEmail, subject, body);

                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }
}
