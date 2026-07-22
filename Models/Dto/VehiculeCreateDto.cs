using Newtonsoft.Json;

namespace GestionVehicule.Models.Dto
{
    public class VehiculeCreateDto
    {
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
        public string Statut { get; set; } = "disponible";
    }
}