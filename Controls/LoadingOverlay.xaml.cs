using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GestionVehicule.Controls
{
    public partial class LoadingOverlay : UserControl
    {
        // Duree minimale d'affichage, pour que le loader reste visible/perceptible
        // meme si la requete API repond en quelques millisecondes (ex: localhost).
        private const int DureeMinimaleMs = 800;

        private readonly Stopwatch _chrono = new Stopwatch();

        public LoadingOverlay()
        {
            InitializeComponent();
        }

        public void Afficher(string message = "Chargement...")
        {
            MessageText.Text = message;
            Visibility = Visibility.Visible;
            _chrono.Restart();
        }

        /// <summary>
        /// Masque le loader, en attendant si besoin pour respecter une duree
        /// d'affichage minimale (evite le "flash" imperceptible).
        /// </summary>
        public async Task MasquerAsync()
        {
            _chrono.Stop();

            var tempsRestant = DureeMinimaleMs - (int)_chrono.ElapsedMilliseconds;
            if (tempsRestant > 0)
                await Task.Delay(tempsRestant);

            Visibility = Visibility.Collapsed;
        }

        // Conservee pour compatibilite si un appel synchrone est vraiment necessaire,
        // mais prefer MasquerAsync() partout ou c'est possible.
        public void Masquer()
        {
            _chrono.Stop();
            Visibility = Visibility.Collapsed;
        }
    }
}