using Newtonsoft.Json;

namespace GestionVehicule.Models
{
    internal class Utilisateur
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nom")]
        public string Nom { get; set; }
        [JsonProperty("email")]
        public string Email { get; set; }
        [JsonProperty("mot_de_passe")]
        public string MotDePasse { get; set; }

        [JsonProperty("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
