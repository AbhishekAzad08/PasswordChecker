using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PasswordChecker.Services.Enums;

namespace PasswordChecker.Services
{
    public class PasswordHelper:IPasswordHelper
    {
        private readonly AppSettings _appSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public PasswordHelper(IOptions<AppSettings> appSettings,IHttpClientFactory httpClientFactory)
        {
            _appSettings = appSettings.Value;
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Gets the Password strength based on the total score
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public PasswordScore GetPasswordScore(string password)
        {
            if (password.Length == 0)
                return PasswordScore.NoScore;
            int passwordScore=0;
            CheckForMinLength(password,ref passwordScore);
            CheckForAtleastOneUpperCaseLetter(password, ref passwordScore);
            CheckForAtleastOneLowerCaseLetter(password, ref passwordScore);
            CheckForAtleastOneNumber(password, ref passwordScore);
            CheckForAtleastOneSpecialCharacter(password, ref passwordScore);

            return (PasswordScore) passwordScore;
        }

        /// <summary>
        /// Checks if the password has minimum length of 8 and increment the score
        /// </summary>
        /// <param name="password"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        private int CheckForMinLength(string password,ref int score)
        {
            return password.Length > 8?++score:score;
        }

        /// <summary>
        /// Checks if the password has atleast one upper case character and increment the score
        /// </summary>
        /// <param name="password"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        private int CheckForAtleastOneUpperCaseLetter(string password,ref int score)
        {
            return password.Any(char.IsUpper) ? ++score : score;
        }

        /// <summary>
        /// Checks if the password has atleast one lower case character and increment the score
        /// </summary>
        /// <param name="password"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        private int CheckForAtleastOneLowerCaseLetter(string password,ref int score)
        {
            return password.Any(char.IsLower) ? ++score : score;
        }

        /// <summary>
        /// Checks if the password has atleast one number and increment the score
        /// </summary>
        /// <param name="password"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        private int CheckForAtleastOneNumber(string password,ref int score)
        {
            return password.Any(char.IsNumber) ? ++score : score;
        }

        /// <summary>
        /// Checks if the password has atleast one special character and increment the score
        /// </summary>
        /// <param name="password"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        private int CheckForAtleastOneSpecialCharacter(string password,ref int score)
        {
            return password.Any(ch => !char.IsLetterOrDigit(ch)) ? ++score : score;
        }

        /// <summary>
        /// Method to call the Pwned Password API
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        private async Task<string> GetPasswordBreachesAsync(string uri)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            HttpResponseMessage response = new HttpResponseMessage();
            using (var client = _httpClientFactory.CreateClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                Uri requestUri = new Uri(uri);
                 response =
                    await client.GetAsync(requestUri, HttpCompletionOption.ResponseContentRead);

                response.EnsureSuccessStatusCode();
            }

            if (response.IsSuccessStatusCode)
            {
                return response.Content.ReadAsStringAsync().Result;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Call the Pwned Password API and return the breach count
        /// </summary>
        /// <param name="passwordToCheck"></param>
        /// <returns></returns>
        public async Task<int> GetPasswordBreachCount(string passwordToCheck)
        {
            var hashedInput = GetHashedString(passwordToCheck);
            var apiUri = String.Format(String.Concat(_appSettings.PwnedApiBaseUrl, _appSettings.BreachEndpoint),
                hashedInput.Substring(0, 5));
            
            var data = await GetPasswordBreachesAsync(apiUri);
            string[] passwordData = data.Split("\r\n");
            var matchedPassword = passwordData.FirstOrDefault (x => x.Split(':')[0].Equals(hashedInput.Substring(5)));

            return matchedPassword != null ? Convert.ToInt32(matchedPassword.Split(':')[1]) : 0;
            
        }
        /// <summary>
        /// Get SHA1 hash of the  UTF8 encoded string for the input password
        /// </summary>
        /// <param name="passwordInput"></param>
        /// <returns></returns>
        private string GetHashedString(string passwordInput )
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(passwordInput));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
