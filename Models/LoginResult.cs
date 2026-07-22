using System;
using System.Collections.Generic;
using System.Text;

namespace GestionVehicule.Models
{
    internal class LoginResult
    {
        public bool Succes { get; set; }
        public string? Erreur { get; set; }
        public Utilisateur? Utilisateur { get; set; }
    }
}
