using GestionVehicule.Services;
using System.Windows;

namespace GestionVehicule.Views
{
    public partial class LoginView : Window
    {
        private readonly LoginService _loginService = new LoginService();

        public LoginView()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var email = emailTextBox.Text.Trim();
            var password = passwordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Merci de remplir tous les champs.", "Champs manquants",
                                 MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            LoginButton.IsEnabled = false; // évite le double-clic pendant la requête

            try
            {
                var result = await _loginService.LoginAsync(email, password);

                if (!result.Succes)
                {
                    MessageBox.Show(result.Erreur, "Connexion refusée",
                                     MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SessionManager.UtilisateurConnecte = result.Utilisateur;

                var dashboardWindow = new DashboardWindow();
                dashboardWindow.Show();
                this.Close();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show("Erreur de connexion : " + ex.Message, "Erreur",
                                 MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                LoginButton.IsEnabled = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
