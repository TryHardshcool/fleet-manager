using GestionVehicule.Models;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace GestionVehicule.Helper

{
    /// <summary>
    /// Enveloppe un Vehicule avec les proprietes pretes a binder dans le DataTemplate
    /// (couleur, icone, libelle) - evite de mettre cette logique dans le XAML.
    /// </summary>
    internal class VehiculeAffichage
    {
        public int Id { get; set; }
        public string NomComplet { get; set; } = "";
        public string Immatriculation { get; set; } = "";
        public string StatutLabel { get; set; } = "";
        public PackIconKind IconKind { get; set; }
        public Brush BadgeBrush { get; set; } = Brushes.Transparent;
        public Brush AccentBrush { get; set; } = Brushes.White;

        public static VehiculeAffichage DepuisVehicule(Vehicule v)
        {
            var affichage = new VehiculeAffichage
            {
                Id = v.Id,
                NomComplet = $"{v.Marque} {v.Modele}",
                Immatriculation = v.Immatriculation
            };

            switch (v.Statut)
            {
                case "en_service":
                    affichage.StatutLabel = "Alloué";
                    affichage.IconKind = PackIconKind.Car;
                    affichage.BadgeBrush = (Brush)new BrushConverter().ConvertFrom("#3A2E1E")!;
                    affichage.AccentBrush = (Brush)new BrushConverter().ConvertFrom("#F5A623")!;
                    break;

                case "en_maintenance":
                    affichage.StatutLabel = "Maintenance";
                    affichage.IconKind = PackIconKind.CarWrench;
                    affichage.BadgeBrush = (Brush)new BrushConverter().ConvertFrom("#3A2323")!;
                    affichage.AccentBrush = (Brush)new BrushConverter().ConvertFrom("#EF5C5C")!;
                    break;

                default: // "disponible"
                    affichage.StatutLabel = "Disponible";
                    affichage.IconKind = PackIconKind.Car;
                    affichage.BadgeBrush = (Brush)new BrushConverter().ConvertFrom("#1E3A34")!;
                    affichage.AccentBrush = (Brush)new BrushConverter().ConvertFrom("#00C9A7")!;
                    break;
            }

            return affichage;
        }
    }
}