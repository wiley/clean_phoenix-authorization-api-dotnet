using Authorization.Domain.Entitlements;
using Authorization.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Authorization.Services
{
    public class EntitlementService : IEntitlementService
    {
        private readonly HttpClient _client;
        private readonly ILogger<CompanyService> _logger;
        private readonly int PageSize = 20;
        public EntitlementService(IHttpClientFactory clientFactory, ILogger<CompanyService> logger)
        {
            _client = clientFactory.CreateClient("entitlementService");
            _client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("ENTITLEMENTS_API_BASE_URL"));
            _client.DefaultRequestHeaders.Add("X-Api-Key", Environment.GetEnvironmentVariable("ENTITLEMENTS_API_KEY"));
            _client.Timeout = TimeSpan.FromSeconds(30);

            _logger = logger;
        }

        /// <summary> This method retrieves all entitlements related to an User from its service. 
        /// The service is configured through the env variable named COMPANY_API_BASE_URL
        /// The HttpClient used is configured in Startup.cs
        /// </summary>
        public async Task<List<EntitlementAPIRepresentation>> GetUserEntitlements(int userId, List<Guid> userAccounts = null, List<int> userOrganizations = null)
        {
            List<EntitlementAPIRepresentation> userEntitlements = new();
            try
            {
                userEntitlements.AddRange(await GetEntitlementsByUserId(userId));
                
                // Get the entitlements associated to all accounts of the current User
                userAccounts?.ForEach(async accountId =>
                {
                    userEntitlements.AddRange(await GetEntitlementsByAccountId(accountId));
                });

                // Get the entitlements associated to all organizations of the current User
                userOrganizations?.ForEach(async organizationId =>
                {
                    userEntitlements.AddRange(await GetEntitlementsByOrganizationId(organizationId));
                });

                return userEntitlements;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"EntitlementService (GetUserEntitlements) - Unhandled Exception");
                return null;
            }
        }

        private async Task<List<EntitlementAPIRepresentation>> GetEntitlementsByUserId(int userId)
        {
            List<EntitlementAPIRepresentation> entitlements = new();
            
            int currentOffset = PageSize;
            EntitlementListAPIRepresentation entitlementsResponse = null;


            string url = $"{_client.BaseAddress}v4/entitlements";
            var query = new Dictionary<string, string>()
            {
                ["userId"] = userId.ToString()
            };

            HttpResponseMessage response = await _client.GetAsync(QueryHelpers.AddQueryString(url, query));
            response.EnsureSuccessStatusCode();

            entitlementsResponse = await response.Content.ReadFromJsonAsync<EntitlementListAPIRepresentation>();

            entitlements.AddRange(entitlementsResponse.Items);

            while (currentOffset < entitlementsResponse.Count)
            {
                response = await _client.GetAsync(entitlementsResponse._links.Next.Href);
                response.EnsureSuccessStatusCode();

                entitlementsResponse = await response.Content.ReadFromJsonAsync<EntitlementListAPIRepresentation>();

                entitlements.AddRange(entitlementsResponse.Items);
                currentOffset += PageSize;
            }

            return entitlements;
        }

        private async Task<List<EntitlementAPIRepresentation>> GetEntitlementsByAccountId(Guid accountId)
        {
            List<EntitlementAPIRepresentation> entitlements = new();

            int currentOffset = PageSize;
            EntitlementListAPIRepresentation entitlementsResponse = null;


            string url = $"{_client.BaseAddress}v4/entitlements";
            var query = new Dictionary<string, string>()
            {
                ["accountId"] = accountId.ToString()
            };

            HttpResponseMessage response = await _client.GetAsync(QueryHelpers.AddQueryString(url, query));
            response.EnsureSuccessStatusCode();

            entitlementsResponse = await response.Content.ReadFromJsonAsync<EntitlementListAPIRepresentation>();

            entitlements.AddRange(entitlementsResponse.Items);

            while (currentOffset < entitlementsResponse.Count)
            {
                response = await _client.GetAsync(entitlementsResponse._links.Next.Href);
                response.EnsureSuccessStatusCode();

                entitlementsResponse = await response.Content.ReadFromJsonAsync<EntitlementListAPIRepresentation>();

                entitlements.AddRange(entitlementsResponse.Items);
                currentOffset += PageSize;
            }

            return entitlements;
        }

        private async Task<List<EntitlementAPIRepresentation>> GetEntitlementsByOrganizationId(int organizationId)
        {
            List<EntitlementAPIRepresentation> entitlements = new();

            int currentOffset = PageSize;
            EntitlementListAPIRepresentation entitlementsResponse = null;


            string url = $"{_client.BaseAddress}v4/entitlements";
            var query = new Dictionary<string, string>()
            {
                ["organizationId"] = organizationId.ToString()
            };

            HttpResponseMessage response = await _client.GetAsync(QueryHelpers.AddQueryString(url, query));
            response.EnsureSuccessStatusCode();

            entitlementsResponse = await response.Content.ReadFromJsonAsync<EntitlementListAPIRepresentation>();

            entitlements.AddRange(entitlementsResponse.Items);

            while (currentOffset < entitlementsResponse.Count)
            {
                response = await _client.GetAsync(entitlementsResponse._links.Next.Href);
                response.EnsureSuccessStatusCode();

                entitlementsResponse = await response.Content.ReadFromJsonAsync<EntitlementListAPIRepresentation>();

                entitlements.AddRange(entitlementsResponse.Items);
                currentOffset += PageSize;
            }

            return entitlements;
        }
    }
}
