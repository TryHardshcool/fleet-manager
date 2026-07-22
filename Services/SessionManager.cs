using GestionVehicule.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GestionVehicule.Services
{
    internal class SessionManager
    {
        public static Utilisateur? UtilisateurConnecte { get; set; }
    }
}
