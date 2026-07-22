using GestionVehicule.Services;
using GestionVehicule.Views.pages;
using System.Windows;

namespace GestionVehicule.Views
{
    /// <summary>
    /// Interaction logic for DashboardWindow.xaml
    /// </summary>
    public partial class DashboardWindow : Window
    {

        private readonly LoginService _loginService = new LoginService();
        public DashboardWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new DashboardPage());
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var confirmation = MessageBox.Show(
               "Voulez-vous vraiment vous déconnecter ?",
               "Confirmation",
               MessageBoxButton.YesNo,
               MessageBoxImage.Question);

            if (confirmation != MessageBoxResult.Yes)
                return;

            Loader.Afficher("Déconnexion en cours...");

            await _loginService.LogoutAsync();

            await Loader.MasquerAsync();

            var loginView = new LoginView();
            loginView.Show();

            // On ferme la fenêtre Dashboard qui héberge cette page
            this.Close();
        }
    }
}
