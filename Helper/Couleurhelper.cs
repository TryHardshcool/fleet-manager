using System;
using System.Collections.Generic;
using System.Text;

namespace GestionVehicule.Helper
{
    /// <summary>
    /// Fait le lien entre le libelle couleur stocke en base ("Rouge", "Bleu"...)
    /// et le code hexadecimal utilise pour l'affichage (pastille, badge...).
    /// Les valeurs correspondent exactement aux ColorSwatchButton du formulaire.
    /// </summary>
    internal static class CouleurHelper
    {
        private static readonly Dictionary<string, string> _couleurs = new()
        {
            { "Blanc", "#FFFFFF" },
            { "Noir",  "#1A1A1A" },
            { "Gris",  "#8C8C8C" },
            { "Rouge", "#D32F2F" },
            { "Bleu",  "#1565C0" },
            { "Vert",  "#2E7D32" }
        };

        public static string ObtenirHex(string libelle) =>
            _couleurs.TryGetValue(libelle, out var hex) ? hex : "#8C8C8C";
    }
}
