using Authorization.Domain;
using Authorization.Domain.Company.OrganizationRole;
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
    public class CompanyService : ICompanyService
    {
        private readonly HttpClient _client;
        private readonly ILogger<CompanyService> _logger;
        private readonly int PageSize = 20;


        public CompanyService(IHttpClientFactory clientFactory, ILogger<CompanyService> logger)
        {
            _client = clientFactory.CreateClient("companyService");
            _client.BaseAddress = new Uri(Environment.GetEnvironmentVariable("COMPANY_API_BASE_URL"));
            _client.DefaultRequestHeaders.Add("X-Api-Key", Environment.GetEnvironmentVariable("COMPANY_API_KEY"));
            _client.Timeout = TimeSpan.FromSeconds(30);

            _logger = logger;
        }

        /// <summary> This method retrieves OrganizationUserRoles from its service. 
        /// The service is configured through the env variable named COMPANY_API_BASE_URL
        /// The HttpClient used is configured in Startup.cs
        /// </summary>
        public async Task<List<OrganizationRoleAPIRepresentation>> GetOrganizationUserRoles(int UserId)
        {
            try
            {
                string url = $"{_client.BaseAddress}api/v4/organization-user-roles";
                var query = new Dictionary<string, string>()
                {
                    ["userId"] = UserId.ToString()
                };
                List<OrganizationRoleAPIRepresentation> organizationUserRoles = await GetListResponse<OrganizationRoleAPIRepresentation>(url, query);
                return organizationUserRoles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CompanyService (GetOrganization) - Unhandled Exception");
                return null;
            }
        }

        /// <summary> This method retrieves Account User Roles from Company API
        /// The service is configured through the env variable named COMPANY_API_BASE_URL
        /// The HttpClient used is configured in Startup.cs
        /// </summary>
        public async Task<List<AccountUserRoleAPIRepresentation>> GetAccountUserRoles(int UserId)
        {
            try
            {
                string url = $"{_client.BaseAddress}api/v4/account-user-roles";
                var query = new Dictionary<string, string>()
                {
                    ["userId"] = UserId.ToString()
                };

                List<AccountUserRoleAPIRepresentation> accountUserRoles = await GetListResponse<AccountUserRoleAPIRepresentation>(url, query);
                return accountUserRoles;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"CompanyService (GetAccountUserRoles) - Unhandled Exception");
                return null;
            }

        }


        private async Task<List<T>> GetListResponse<T>(string url, Dictionary<string, string> query = null) 
        {
            try
            {
                int currentOffset = PageSize;
                List<T> listItems = new();

                HttpResponseMessage response = await _client.GetAsync(QueryHelpers.AddQueryString(url, query));
                response.EnsureSuccessStatusCode();

                ListAPIRepresentation<T> responseList = await response.Content.ReadFromJsonAsync<ListAPIRepresentation<T>>();

                listItems.AddRange(responseList.Items);

                while (currentOffset < responseList.Count)
                {
                    response = await _client.GetAsync(responseList._links.Next.Href);
                    response.EnsureSuccessStatusCode();

                    responseList = await response.Content.ReadFromJsonAsync<ListAPIRepresentation<T>>();

                    listItems.AddRange(responseList.Items);
                    currentOffset += PageSize;
                }
                return listItems;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"CompanyService (GetListResponse) - Unhandled Exception");
                return null;
            }
            
        }
    }
}
