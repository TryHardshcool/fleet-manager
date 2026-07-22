using GestionVehicule.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using System.Net.Http;
using System.Text;

namespace GestionVehicule.Services
{
    internal class LoginService
    {
        private readonly HttpClient _client = ApiClientProvider.Client;

        public async Task<LoginResult> LoginAsync(string email, string password)
        {
            Log.Information("Tentative de connexion pour l'utilisateur: {Email}", email);

            var payload = new { email, mot_de_passe = password };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _client.PostAsync("Auth/Login.php", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            var parsed = JObject.Parse(responseBody);

            if (!response.IsSuccessStatusCode)
            {
                var message = parsed["erreur"]?.ToString() ?? "Erreur inconnue";
                Log.Warning("Échec de connexion pour {Email} : {Message}", email, message);
                return new LoginResult { Succes = false, Erreur = message };
            }

            var utilisateur = parsed["utilisateur"]?.ToObject<Utilisateur>();
            Log.Information("Connexion réussie pour {Email}", email);
            return new LoginResult { Succes = true, Utilisateur = utilisateur };
        }

        public async Task<bool> LogoutAsync()
        {
            Log.Information("Déconnexion de l'utilisateur: {Email}",
                SessionManager.UtilisateurConnecte?.Email ?? "inconnu");

            try
            {
                // POST vide : pas de body attendu par Logout.php
                var response = await _client.PostAsync("Auth/Logout.php", null);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    Log.Warning("Échec de la déconnexion : {Body}", responseBody);
                    return false;
                }

                Log.Information("Déconnexion réussie");
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Erreur réseau lors de la déconnexion");
                return false;
            }
            finally
            {
                // On nettoie la session côté client dans tous les cas,
                // même si l'appel réseau échoue (l'utilisateur veut partir).
                SessionManager.UtilisateurConnecte = null;
            }
        }
    }
}