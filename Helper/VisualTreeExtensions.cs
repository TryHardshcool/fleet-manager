using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace GestionVehicule.Helper
{
    internal static class VisualTreeExtensions
    {
        /// <summary>
        /// Parcourt récursivement l'arbre visuel pour retrouver tous les éléments
        /// enfants d'un type donné (ex: tous les RadioButton d'une Page).
        /// </summary>
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject parent) where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T typedChild)
                    yield return typedChild;

                foreach (var descendant in FindVisualChildren<T>(child))
                    yield return descendant;
            }
        }
    }
}
