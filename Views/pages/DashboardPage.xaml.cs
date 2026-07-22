using GestionVehicule.Helper;
using GestionVehicule.Models;
using GestionVehicule.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionVehicule.Views.pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        private readonly VehiculeService _vehiculeService = new VehiculeService();

        public DashboardPage()
        {
            InitializeComponent();

            var utilisateur = SessionManager.UtilisateurConnecte;
            WelcomeText.Text = utilisateur != null
                ? $"Bienvenue, {utilisateur.Nom}"
                : "Bienvenue";
        }

        // Chargement initial une fois que la page est reellement affichee
        // (evite d'appeler l'API dans le constructeur, avant que la fenetre soit prete)
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await ChargerVehiculesAsync();
        }

        private async Task ChargerVehiculesAsync()
        {
            Loader.Afficher("Chargement des véhicules...");

            try
            {
                var vehicules = await _vehiculeService.GetAllAsync();
                AfficherVehicules(vehicules.Select(VehiculeAffichage.DepuisVehicule).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible de charger les véhicules : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                await Loader.MasquerAsync();
            }
        }

        private void AfficherVehicules(System.Collections.Generic.List<VehiculeAffichage> vehicules)
        {
            VehiculesItemsControl.ItemsSource = vehicules;
            AucunResultatText.Visibility = vehicules.Count == 0 ? Visibility.Visible : Visibility.Collapsed;

            // Mise a jour des cartes statistiques a partir des donnees reelles
            TotalVehiculesText.Text = vehicules.Count.ToString();
            DisponiblesText.Text = vehicules.Count(v => v.StatutLabel == "Disponible").ToString();
            AllouesText.Text = vehicules.Count(v => v.StatutLabel == "Alloué").ToString();
            MaintenanceText.Text = vehicules.Count(v => v.StatutLabel == "Maintenance").ToString();
        }

        // ===================== RECHERCHE =====================
        private async void RechercheTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            var terme = RechercheTextBox.Text.Trim();

            try
            {
                var vehicules = string.IsNullOrEmpty(terme)
                    ? await _vehiculeService.GetAllAsync()
                    : await _vehiculeService.RechercherAsync(terme);

                AfficherVehicules(vehicules.Select(VehiculeAffichage.DepuisVehicule).ToList());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors de la recherche : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ===================== AJOUTER =====================
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AjoutVehiculePage());
        }

        // ===================== VOIR =====================
        private void VoirButton_Click(object sender, RoutedEventArgs e)
        {
            var vehicule = (VehiculeAffichage)((Button)sender).Tag;
            NavigationService.Navigate(new VehiculeDetailsPage(vehicule.Id));
        }

        // ===================== MODIFIER =====================
        private void ModifierButton_Click(object sender, RoutedEventArgs e)
        {
            var vehicule = (VehiculeAffichage)((Button)sender).Tag;
            NavigationService.Navigate(new AjoutVehiculePage(vehicule.Id));
        }

        // ===================== SUPPRIMER =====================
        private async void SupprimerButton_Click(object sender, RoutedEventArgs e)
        {
            var vehicule = (VehiculeAffichage)((Button)sender).Tag;

            var confirmation = MessageBox.Show(
                $"Supprimer définitivement {vehicule.NomComplet} ({vehicule.Immatriculation}) ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmation != MessageBoxResult.Yes)
                return;

            Loader.Afficher("Suppression en cours...");

            var (succes, erreur) = await _vehiculeService.SupprimerAsync(vehicule.Id);

            if (!succes)
            {
                await Loader.MasquerAsync();
                MessageBox.Show(erreur, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // On recharge la liste plutot que de retirer manuellement l'element,
            // pour rester synchronise avec les compteurs (Disponibles/Alloués/Maintenance)
            // ChargerVehiculesAsync() gere elle-meme l'affichage/masquage du loader
            await ChargerVehiculesAsync();
        }
    }
}