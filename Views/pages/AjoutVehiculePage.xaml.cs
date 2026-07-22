using GestionVehicule.Helper;
using GestionVehicule.Models;
using GestionVehicule.Models.Dto;
using GestionVehicule.Services;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GestionVehicule.Views.pages
{
    public partial class AjoutVehiculePage : Page
    {
        private readonly VehiculeService _vehiculeService = new VehiculeService();

        // Null = mode ajout. Renseigne = mode modification.
        private readonly int? _vehiculeId;

        private bool EstModeModification => _vehiculeId.HasValue;

        // ===================== MODE AJOUT =====================
        public AjoutVehiculePage()
        {
            InitializeComponent();
            _vehiculeId = null;
        }

        // ===================== MODE MODIFICATION =====================
        public AjoutVehiculePage(int vehiculeId)
        {
            InitializeComponent();
            _vehiculeId = vehiculeId;
        }

        // ===================== CHARGEMENT / ETAT INITIAL =====================
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (EstModeModification)
            {
                await ConfigurerModeModificationAsync();
            }
            else
            {
                ConfigurerModeAjout();
            }
        }

        private void ConfigurerModeAjout()
        {
            TitreText.Text = "Ajouter un véhicule";
            SousTitreText.Text = "Renseignez les informations du nouveau véhicule";
            EnregistrerButtonText.Text = "Enregistrer le véhicule";

            // Valeurs par defaut coherentes pour un nouveau vehicule
            CouleurBlanc.IsChecked = true;
            StatutDisponible.IsChecked = true;
        }

        private async Task ConfigurerModeModificationAsync()
        {
            TitreText.Text = "Modifier le véhicule";
            EnregistrerButtonText.Text = "Enregistrer les modifications";

            Loader.Afficher("Chargement du véhicule...");

            try
            {
                var vehicule = await _vehiculeService.GetByIdAsync(_vehiculeId!.Value);

                if (vehicule == null)
                {
                    MessageBox.Show("Ce véhicule est introuvable (peut-être déjà supprimé).",
                        "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                    NavigationService?.GoBack();
                    return;
                }

                SousTitreText.Text = $"{vehicule.Marque} {vehicule.Modele} — {vehicule.Immatriculation}";

                MarqueTextBox.Text = vehicule.Marque;
                ModeleTextBox.Text = vehicule.Modele;
                ImmatriculationTextBox.Text = vehicule.Immatriculation;
                AnneeTextBox.Text = vehicule.Annee.ToString();

                CocherRadioParTag("Couleur", vehicule.Couleur);
                CocherRadioParTag("Statut", vehicule.Statut);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Impossible de charger le véhicule : {ex.Message}",
                    "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                NavigationService?.GoBack();
            }
            finally
            {
                await Loader.MasquerAsync();
            }
        }

        // ===================== VALIDATION SAISIE ANNEE =====================
        private void AnneeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "^[0-9]+$");
        }

        // ===================== ANNULER / RETOUR =====================
        private void AnnulerButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        private void RetourButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.GoBack();
        }

        // ===================== ENREGISTRER (AJOUT OU MODIFICATION) =====================
        private async void EnregistrerButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(MarqueTextBox.Text) ||
                string.IsNullOrWhiteSpace(ModeleTextBox.Text) ||
                string.IsNullOrWhiteSpace(ImmatriculationTextBox.Text) ||
                string.IsNullOrWhiteSpace(AnneeTextBox.Text))
            {
                MessageBox.Show("Merci de remplir tous les champs obligatoires.",
                    "Champs manquants", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(AnneeTextBox.Text.Trim(), out var annee))
            {
                MessageBox.Show("L'année doit être un nombre valide (ex: 2022).",
                    "Année invalide", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var couleur = ObtenirTagCoche("Couleur") ?? "Blanc";
            var statut = ObtenirTagCoche("Statut") ?? "disponible";

            var dto = new VehiculeCreateDto
            {
                Marque = MarqueTextBox.Text.Trim(),
                Modele = ModeleTextBox.Text.Trim(),
                Immatriculation = ImmatriculationTextBox.Text.Trim(),
                Annee = annee,
                Couleur = couleur,
                Statut = statut
            };

            EnregistrerButton.IsEnabled = false;
            Loader.Afficher(EstModeModification ? "Enregistrement des modifications..." : "Ajout du véhicule...");

            try
            {
                if (EstModeModification)
                {
                    var (succes, erreur) = await _vehiculeService.ModifierAsync(_vehiculeId!.Value, dto);

                    if (!succes)
                    {
                        MessageBox.Show(erreur, "Erreur lors de la modification",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    MessageBox.Show($"Véhicule {dto.Marque} {dto.Modele} modifié avec succès.",
                        "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    var (succes, erreur, _) = await _vehiculeService.AjouterAsync(dto);

                    if (!succes)
                    {
                        MessageBox.Show(erreur, "Erreur lors de l'ajout",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    MessageBox.Show($"Véhicule {dto.Marque} {dto.Modele} ajouté avec succès.",
                        "Succès", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                NavigationService?.GoBack();
            }
            finally
            {
                EnregistrerButton.IsEnabled = true;
                await Loader.MasquerAsync();
            }
        }

        // ===================== HELPERS =====================
        private string? ObtenirTagCoche(string groupName)
        {
            var radioCoche = this.FindVisualChildren<RadioButton>()
                .FirstOrDefault(r => r.GroupName == groupName && r.IsChecked == true);

            return radioCoche?.Tag as string;
        }

        private void CocherRadioParTag(string groupName, string valeur)
        {
            var radio = this.FindVisualChildren<RadioButton>()
                .FirstOrDefault(r => r.GroupName == groupName &&
                                      string.Equals(r.Tag as string, valeur, StringComparison.OrdinalIgnoreCase));

            if (radio != null)
                radio.IsChecked = true;
        }
    }
}