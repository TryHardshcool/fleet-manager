using GestionVehicule.Models;
using GestionVehicule.Models.Dto;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GestionVehicule.Services
{
    internal class VehiculeService
    {
        private readonly HttpClient _client = ApiClientProvider.Client;
        private const string Endpoint = "Vehicules.php";

        // ===================== LISTE COMPLETE =====================
        public async Task<List<Vehicule>> GetAllAsync()
        {
            var response = await _client.GetAsync(Endpoint);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("Échec de récupération des véhicules : {Body}", body);
                return new List<Vehicule>();
            }

            var vehicules = JsonConvert.DeserializeObject<List<Vehicule>>(body) ?? new List<Vehicule>();
            Log.Information("{Count} véhicule(s) récupéré(s)", vehicules.Count);
            return vehicules;
        }

        // ===================== UN SEUL VEHICULE (?id=...) =====================
        public async Task<Vehicule?> GetByIdAsync(int id)
        {
            var response = await _client.GetAsync($"{Endpoint}?id={id}");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("Échec de récupération du véhicule {Id} : {Body}", id, body);
                return null;
            }
            if (body.Trim() == "false")
            {
                Log.Warning("Aucun véhicule trouvé pour l'id {Id}", id);
                return null;
            }

            return JsonConvert.DeserializeObject<Vehicule>(body);
        }

        // ===================== RECHERCHE (?recherche=...) =====================
        public async Task<List<Vehicule>> RechercherAsync(string terme)
        {
            var response = await _client.GetAsync($"{Endpoint}?recherche={Uri.EscapeDataString(terme)}");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                Log.Warning("Échec de la recherche '{Terme}' : {Body}", terme, body);
                return new List<Vehicule>();
            }

            return JsonConvert.DeserializeObject<List<Vehicule>>(body) ?? new List<Vehicule>();
        }

        // ===================== AJOUT (POST) =====================
        public async Task<(bool Succes, string? Erreur, Vehicule? Vehicule)> AjouterAsync(VehiculeCreateDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            System.Diagnostics.Debug.WriteLine("JSON envoyé : " + json);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync(Endpoint, content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                var message = parsed["erreur"]?.ToString() ?? "Erreur lors de l'ajout";
                Log.Warning("Échec de l'ajout du véhicule {Immatriculation} : {Message}",
                    dto.Immatriculation, message);
                return (false, message, null);
            }

            var resultat = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
            var nouvelId = Convert.ToInt32(resultat["id"]);

            var vehiculeCree = new Vehicule
            {
                Id = nouvelId,
                Marque = dto.Marque,
                Modele = dto.Modele,
                Immatriculation = dto.Immatriculation,
                Annee = dto.Annee,
                Couleur = dto.Couleur,
                Statut = dto.Statut
            };

            Log.Information("Véhicule {Immatriculation} ajouté avec succès (id: {Id})",
                dto.Immatriculation, nouvelId);
            return (true, null, vehiculeCree);
        }

        // ===================== MODIFICATION (PUT reel) =====================
        public async Task<(bool Succes, string? Erreur)> ModifierAsync(int id, VehiculeCreateDto dto)
        {
            var json = JsonConvert.SerializeObject(dto);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PutAsync($"{Endpoint}?id={id}", content);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                var message = parsed["erreur"]?.ToString() ?? "Erreur lors de la modification";
                Log.Warning("Échec de la modification du véhicule {Id} : {Message}", id, message);
                return (false, message);
            }

            Log.Information("Véhicule {Id} modifié avec succès", id);
            return (true, null);
        }

        // ===================== SUPPRESSION (DELETE reel) =====================
        public async Task<(bool Succes, string? Erreur)> SupprimerAsync(int id)
        {
            var response = await _client.DeleteAsync($"{Endpoint}?id={id}");
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                var parsed = JsonConvert.DeserializeObject<Dictionary<string, object>>(body);
                var message = parsed["erreur"]?.ToString() ?? "Erreur lors de la suppression";
                Log.Warning("Échec de la suppression du véhicule {Id} : {Message}", id, message);
                return (false, message);
            }

            Log.Information("Véhicule {Id} supprimé avec succès", id);
            return (true, null);
        }
    }
}