using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TaskAdministratorUWP.Models;

namespace TaskAdministratorUWP.Services
{
    class RequestHandler
    {
        public async Task<IEnumerable<T>> GetDataFromAPI<T>(string table)
        {
            IEnumerable<T> result = null;

            using (var client = UWPHttpClient.GetRequest())
            {
                HttpResponseMessage response = await client.GetAsync("api/" + table);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<IEnumerable<T>>(content);
                }
                else
                {
                    throw new Exception((int)response.StatusCode + "-" + response.StatusCode.ToString());
                }
            }
            return result;
        }

        public async Task PostDataToAPI(AssignmentsClient assignmentToPost)
        {
            using (var client = UWPHttpClient.GetRequest())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(assignmentToPost), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("api/Assignments", content);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception((int)response.StatusCode + "-" + response.StatusCode.ToString());
                }
            }
        }

        public async Task DeleteDataToAPI(string assignmentToDelete)
        {
            using (var client = UWPHttpClient.GetRequest())
            {
                HttpResponseMessage response = await client.DeleteAsync("api/Assignments/" + assignmentToDelete);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception((int)response.StatusCode + "-" + response.StatusCode.ToString());
                }
            }
        }
    }
}
