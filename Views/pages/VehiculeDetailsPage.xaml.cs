using GestionVehicule.Helper;
using GestionVehicule.Models;
using GestionVehicule.Services;
using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GestionVehicule.Views.pages
{
    public partial class VehiculeDetailsPage : Page
    {
        private readonly VehiculeService _vehiculeService = new VehiculeService();
        private readonly int _vehiculeId;
        private Vehicule? _vehiculeActuel;

        public VehiculeDetailsPage(int vehiculeId)
        {
            InitializeComponent();
            _vehiculeId = vehiculeId;
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                var vehicule = await _vehiculeService.GetByIdAsync(_vehiculeId);

                if (vehicule == null)
                {
                    MessageBox.Show("Ce véhicule est introuvable (peut-être déjà supprimé).",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService?.GoBack();
                    return;
                }

                _vehiculeActuel = vehicule;
                AfficherVehicule(vehicule);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible de charger le véhicule : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService?.GoBack();
            }
        }

        private void AfficherVehicule(Vehicule v)
        {
            // ---- Banniere hero ----
            NomCompletText.Text = $"{v.Marque} {v.Modele}";
            ImmatriculationPlateText.Text = v.Immatriculation;
            DateAjoutText.Text = $"Ajouté le {v.CreatedAt:dd/MM/yyyy}";

            var couleurHex = CouleurHelper.ObtenirHex(v.Couleur);
            var couleurBrush = (Brush)new BrushConverter().ConvertFrom(couleurHex)!;
            CouleurPastille.Fill = couleurBrush;
            CouleurText.Text = v.Couleur;

            // ---- Statut : icone, couleurs, libelle ----
            string statutLabel;
            Brush badgeBrush, accentBrush;
            PackIconKind iconeKind;

            switch (v.Statut)
            {
                case "en_service":
                    statutLabel = "Alloué";
                    badgeBrush = (Brush)new BrushConverter().ConvertFrom("#3A2E1E")!;
                    accentBrush = (Brush)new BrushConverter().ConvertFrom("#F5A623")!;
                    iconeKind = PackIconKind.Car;
                    break;

                case "en_maintenance":
                    statutLabel = "Maintenance";
                    badgeBrush = (Brush)new BrushConverter().ConvertFrom("#3A2323")!;
                    accentBrush = (Brush)new BrushConverter().ConvertFrom("#EF5C5C")!;
                    iconeKind = PackIconKind.CarWrench;
                    break;

                default: // disponible
                    statutLabel = "Disponible";
                    badgeBrush = (Brush)new BrushConverter().ConvertFrom("#1E3A34")!;
                    accentBrush = (Brush)new BrushConverter().ConvertFrom("#00C9A7")!;
                    iconeKind = PackIconKind.Car;
                    break;
            }

            StatutText.Text = statutLabel;
            StatutText.Foreground = accentBrush;
            StatutBadge.Background = badgeBrush;

            IconeBadge.Background = badgeBrush;
            IconeVehicule.Foreground = accentBrush;
            IconeVehicule.Kind = iconeKind;

            // ---- Fiche technique ----
            MarqueText.Text = v.Marque;
            ModeleText.Text = v.Modele;
            AnneeText.Text = v.Annee.ToString();

            // ---- Historique ----
            CreeLeText.Text = v.CreatedAt.ToString("dd MMMM yyyy 'à' HH:mm");
            ModifieLeText.Text = v.UpdatedAt.ToString("dd MMMM yyyy 'à' HH:mm");
        }

        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void ModifierButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new AjoutVehiculePage(_vehiculeId));
        }

        private async void SupprimerButton_Click(object sender, RoutedEventArgs e)
        {
            if (_vehiculeActuel == null) return;

            var confirmation = MessageBox.Show(
                $"Supprimer définitivement {_vehiculeActuel.Marque} {_vehiculeActuel.Modele} ({_vehiculeActuel.Immatriculation}) ?",
                "Confirmation",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmation != MessageBoxResult.Yes)
                return;

            var (succes, erreur) = await _vehiculeService.SupprimerAsync(_vehiculeId);

            if (!succes)
            {
                MessageBox.Show(erreur, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            NavigationService?.GoBack();
        }
    }
}