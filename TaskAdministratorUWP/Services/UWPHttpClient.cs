using System;
using System.Net.Http;

namespace TaskAdministratorUWP.Services
{
    public static class UWPHttpClient
    {
        public static HttpClient GetRequest()
        {
            HttpClient client = new HttpClient { BaseAddress = new Uri("http://localhost:59946/") };

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }
    }
}
