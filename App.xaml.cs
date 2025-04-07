using Microsoft.Maui.Controls;

namespace InventorySystems
{
    public partial class App : Application
    {
        public static string DatabasePath { get; private set; }

        public App()
        {
            InitializeComponent();

            DatabasePath = Path.Combine(FileSystem.AppDataDirectory, "inventsys.db");
            //database file name via what was sent in the discord//

            DependencyService.Register<UserService>();

            MainPage = new NavigationPage(new LoginPage());
        }
    }

}
