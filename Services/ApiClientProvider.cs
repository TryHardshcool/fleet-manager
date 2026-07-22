using System;
using System.Net;
using System.Net.Http;

namespace GestionVehicule.Services
{
    internal static class ApiClientProvider
    {
        private const string BaseUrl = "http://localhost/Examen_Api_WPF/";

        // Un seul CookieContainer partagé par TOUTE l'application
        private static readonly CookieContainer _cookies = new CookieContainer();

        private static readonly Lazy<HttpClient> _client = new Lazy<HttpClient>(() =>
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookies,
                UseCookies = true
            };

            return new HttpClient(handler)
            {
                BaseAddress = new Uri(BaseUrl)
            };
        });

        public static HttpClient Client => _client.Value;
    }
}