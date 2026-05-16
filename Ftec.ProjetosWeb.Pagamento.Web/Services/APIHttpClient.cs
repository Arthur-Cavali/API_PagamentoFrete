using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Ftec.ProjetosWeb.Pagamento.Web.Services
{
    public class APIHttpClient
    {
        private string baseAPI;

        public APIHttpClient(string baseAPI)
        {
            this.baseAPI = baseAPI;
        }

        public Guid Put<T>(string action, Guid id, T data)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PutAsJsonAsync(action + id.ToString(), data).Result;
            if (response.IsSuccessStatusCode)
                return response.Content.ReadFromJsonAsync<Guid>().Result;
            else
                throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public void Put<T>(string fullAction, T data)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PutAsJsonAsync(fullAction, data).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public void Put(string action)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PutAsync(action, null).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public void Post(string action)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PostAsync(action, null).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public void Post<T>(string action, T data)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PostAsJsonAsync(action, data).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public TResult PostRetorno<TBody, TResult>(string action, TBody data)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.PostAsJsonAsync(action, data).Result;
            if (response.IsSuccessStatusCode)
                return response.Content.ReadFromJsonAsync<TResult>().Result!;
            else
                throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public T Get<T>(string actionUri)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync(actionUri).Result;
            if (response.IsSuccessStatusCode)
                return response.Content.ReadFromJsonAsync<T>().Result!;
            else
                throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public T Delete<T>(string action, Guid id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.DeleteAsync(action + id.ToString()).Result;
            if (response.IsSuccessStatusCode)
                return response.Content.ReadFromJsonAsync<T>().Result!;
            else
                throw new Exception(response.Content.ReadAsStringAsync().Result);
        }

        public void Delete(string action, Guid id)
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri(baseAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.DeleteAsync(action + id.ToString()).Result;
            if (!response.IsSuccessStatusCode)
                throw new Exception(response.Content.ReadAsStringAsync().Result);
        }
    }
}
