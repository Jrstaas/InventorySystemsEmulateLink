using Microsoft.Maui.Controls;
using Microsoft.Maui.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySystems
{
    public partial class MainPage : ContentPage
    {
        private UserService _userService; // User-related service (for handling users)
        private ItemService _itemService; // Service for interacting with items (added ItemService for items)
        private List<Item> _items; // Store the fetched items
        private Layout _originalLayout;

        public MainPage(string username, UserService userService)
        {
            InitializeComponent();
            _userService = userService; // Initialize our user serviceing
            _originalLayout = this.Content as Layout;
            WelcomeLabel.Text = $"Welcome, {username}!";

            LoadItems(); // Load items when the page is initialized
        }

        // Method to load items from the database
        private async void LoadItems()
        {
            _items = await _itemService.GetAllItemsAsync(); // Fetch all items from the database
            PopulateGrid(_items); // Display the items in the grid
        }

        // Method to populate the grid with items
        private void PopulateGrid(List<Item> items)
        {
            ItemGridContent.Children.Clear(); // Clear existing content

            int row = 0, col = 0;

            // Ensure the ItemGridContent grid has enough rows and columns
            ItemGridContent.RowDefinitions.Clear();
            ItemGridContent.ColumnDefinitions.Clear();

            // Create 8 rows and 5 columns based on the grid size
            for (int i = 0; i < 8; i++) // Assuming 8 rows
            {
                ItemGridContent.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }

            for (int i = 0; i < 5; i++) // Assuming 5 columns
            {
                ItemGridContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            }

            foreach (var item in items)
            {
                if (col == 5) // Move to next row after 5 items
                {
                    col = 0;
                    row++;
                }

                // Create a Grid for each item to hold BoxView and StackLayout
                var itemGrid = new Grid
                {
                    RowDefinitions =
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto }
                    },
                    ColumnDefinitions =
                    {
                        new ColumnDefinition { Width = GridLength.Auto },
                        new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                        new ColumnDefinition { Width = GridLength.Auto }
                    },
                    Padding = new Thickness(10), // Padding around each item grid
                    Margin = new Thickness(10)  // Margin around the grid cells
                };

                // Add rectangle (BoxView)
                var boxView = new BoxView
                {
                    Color = Colors.LightGray,
                    HeightRequest = 50, // Keep the height fixed
                    CornerRadius = 10,
                    HorizontalOptions = LayoutOptions.FillAndExpand // Stretch across the entire row
                };

                // Add image
                var image = new Image
                {
                    Source = item.ImageUrl,
                    WidthRequest = 60,
                    HeightRequest = 60,
                    GestureRecognizers =
                    {
                        new TapGestureRecognizer
                        {
                            Command = new Command(() => ShowImagePopup(item.ImageUrl))
                        }
                    }
                };

                // Add labels and buttons
                var nameLabel = new Label
                {
                    Text = item.ProductName, // Using ProductName from the database
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Start
                };

                var valueLabel = new Label
                {
                    Text = item.StockQuantity.ToString(), // Using StockQuantity from the database
                    FontSize = 18,  // Increase by 1.25x (default ~14)
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalOptions = LayoutOptions.Center,
                    Margin = new Thickness(10, 0, 0, 0) // Space between - button and value
                };

                var plusButton = new Button
                {
                    Text = "+",
                    Command = new Command(() => AdjustValue(item, 1)),
                    BackgroundColor = Colors.Blue,
                    TextColor = Colors.White,
                    FontSize = 18, // Increase by 1.25x
                    WidthRequest = 40,  // Adjust if needed
                    HeightRequest = 40
                };

                var minusButton = new Button
                {
                    Text = "-",
                    Command = new Command(() => AdjustValue(item, -1)),
                    BackgroundColor = Colors.Blue,
                    TextColor = Colors.White,
                    FontSize = 18, // Increase by 1.25x
                    WidthRequest = 40,
                    HeightRequest = 40
                };

                // Create and add the layout for each item in the grid
                var stackLayout = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    Children = { image, nameLabel, plusButton, minusButton, valueLabel }
                };

                // Add the BoxView and StackLayout to the itemGrid
                itemGrid.Children.Add(boxView); // Add BoxView at (0, 0) by default
                Grid.SetColumnSpan(boxView, 5); // Ensures the BoxView spans across all columns
                itemGrid.Children.Add(stackLayout); // Add StackLayout at (0, 1) by default

                // Set the Grid.Row and Grid.Column properties for itemGrid before adding it to the parent grid
                Grid.SetRow(itemGrid, row);
                Grid.SetColumn(itemGrid, col);

                // Add the itemGrid to the parent grid (ItemGridContent)
                ItemGridContent.Children.Add(itemGrid); // Now only passing the element to Add()

                col++; // Move to next column
            }
        }

        // Method to adjust the value when + or - button is clicked
        private void AdjustValue(Item item, int change)
        {
            // Prevent the value from going below 0
            if (item.StockQuantity + change >= 0)
            {
                item.StockQuantity += change;
                // Update the item in the database
                _itemService.UpdateItemAsync(item);
            }
            PopulateGrid(_items); // Update grid with the new value
        }

        // Search functionality
        private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var filteredItems = _items.Where(i => i.ProductName.ToLower().Contains(e.NewTextValue.ToLower())).ToList();
            PopulateGrid(filteredItems);
        }

        private void ShowImagePopup(string imageUrl)
        {
            var popupBackground = new BoxView
            {
                BackgroundColor = Colors.Black.MultiplyAlpha(0.8f), // Dim the background
                Opacity = 0.8,
                HorizontalOptions = LayoutOptions.Fill,
                VerticalOptions = LayoutOptions.Fill
            };

            var popupImage = new Image
            {
                Source = imageUrl,
                WidthRequest = 300, // Adjust size
                HeightRequest = 300,
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            var popupLayout = new AbsoluteLayout();

            AbsoluteLayout.SetLayoutBounds(popupBackground, new Rect(0, 0, 1, 1));
            AbsoluteLayout.SetLayoutFlags(popupBackground, AbsoluteLayoutFlags.All);

            AbsoluteLayout.SetLayoutBounds(popupImage, new Rect(0.5, 0.5, -1, -1));
            AbsoluteLayout.SetLayoutFlags(popupImage, AbsoluteLayoutFlags.PositionProportional);

            popupLayout.Children.Add(popupBackground);
            popupLayout.Children.Add(popupImage);

            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += (s, e) => this.Content = _originalLayout; // Restore the original layout
            popupBackground.GestureRecognizers.Add(tapGesture);
            popupImage.GestureRecognizers.Add(tapGesture);

            this.Content = popupLayout; // Show the popup
        }


    }
}
