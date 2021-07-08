using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;

namespace PasswordChecker.ConsoleApp
{
    class Program
    {
        static void Main()
        {
            bool exit = false;
            Console.WriteLine("Press E+Enter to exit.");
            while (!exit)
            {
                Console.WriteLine("Enter Password to check:");
                var command = Console.ReadLine();
                if (command == "E")
                    exit = true;
                else
                    RunAsync(command).GetAwaiter().GetResult();
            }
        }

        static async Task RunAsync(string pass)
        {
            var result = await CheckPasswordAsync(pass);
            PasswordCheckResponse check= JsonConvert.DeserializeObject<PasswordCheckResponse>(result);
            if (check != null)
            {
                Console.WriteLine($"Password strength is {check.Strength}");
                Console.WriteLine($"Entered Password has been breached {check.BreachCount} times");
            }
        }

        static async Task<string> CheckPasswordAsync(string password)
        {
            var pass = new PasswordRequest() { PasswordToCheck = password};
            string result=string.Empty;
            var content = GetStringContent(pass);
            try
            {
                ServicePointManager.SecurityProtocol =
                    SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                using (var handler = new HttpClientHandler())
                {
                    using (var client = new HttpClient(handler))
                    {
                        client.DefaultRequestHeaders.Add(HttpRequestHeader.ContentType.ToString(), "application/json");
                        Uri requestUri = new Uri("https://localhost:44305/api/password");

                        HttpResponseMessage httpResponseMessage = await client.PostAsync(requestUri, content);
                        httpResponseMessage.EnsureSuccessStatusCode();
                         result = await httpResponseMessage.Content.ReadAsStringAsync();
                    }
                } 
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred: {ex.Message} ");
                return result;
            }
           
        }
        private static StringContent GetStringContent<T>(T load)
        {
            var serializeObject = JsonConvert.SerializeObject(load, new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
            return new StringContent(serializeObject, Encoding.UTF8, "application/json");
        }
    }
    public class PasswordRequest
    {
        public string PasswordToCheck;
    }
    public class PasswordCheckResponse
    {
        public string Strength;
        public int BreachCount;
    }
}
