using Newtonsoft.Json;
using System;

namespace GestionVehicule.Models
{
    public class Vehicule
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("marque")]
        public string Marque { get; set; } = "";

        [JsonProperty("modele")]
        public string Modele { get; set; } = "";

        [JsonProperty("immatriculation")]
        public string Immatriculation { get; set; } = "";

        [JsonProperty("annee")]
        public int Annee { get; set; }

        [JsonProperty("couleur")]
        public string Couleur { get; set; } = "";

        [JsonProperty("statut")]
        public string Statut { get; set; } = "";

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}