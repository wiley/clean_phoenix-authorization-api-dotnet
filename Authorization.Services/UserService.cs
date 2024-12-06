using Authorization.Services.Interfaces;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Authorization.Domain.User;

namespace Authorization.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _client;
        private readonly ILogger<UserService> _logger;

        public UserService(IHttpClientFactory clientFactory, ILogger<UserService> logger)
        {
            _client = clientFactory.CreateClient("userService");
            _client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("USERS_API_BASE_URL"));
            _client.DefaultRequestHeaders.Add("X-Api-Key", Environment.GetEnvironmentVariable("USERS_API_KEY"));
            _client.Timeout = TimeSpan.FromSeconds(30);

            _logger = logger;
        }

        /// <summary> This method retrieves a user from its service. 
        /// The service is configured through the env variable named USERS_API_BASE_URL
        /// The HttpClient used is configured in Startup.cs
        /// </summary>
        public async Task<UserAPIRepresentation> GetUser(int id)
        {
            try
            {

                UserAPIRepresentation userResponse = null;

                string url = $"{_client.BaseAddress}api/v4/users/{id}";
                HttpResponseMessage response = await _client.GetAsync(url);

                response.EnsureSuccessStatusCode();

                userResponse = await response.Content.ReadFromJsonAsync<UserAPIRepresentation>();
                return userResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"UserService (GetUser) - Unhandled Exception");
                return null;
            }

        }


        public async Task<bool> RequestHealthCheck()
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(_client.BaseAddress + "Healthz"),
                    Headers =
                    {
                        { HttpRequestHeader.Accept.ToString(), "application/json" }
                    }
                };

                HttpResponseMessage message = await _client.SendAsync(httpRequestMessage);

                return (message.StatusCode == HttpStatusCode.OK);
            }
            catch
            {
                return false;
            }
        }
    }
}
