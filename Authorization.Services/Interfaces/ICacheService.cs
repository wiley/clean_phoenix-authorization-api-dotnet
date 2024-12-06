using Authorization.Domain.Company;
using Authorization.Domain.Pagination;
using Authorization.Domain.User;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Authorization.Services.Interfaces
{
    public interface ICacheService
    {
        int TotalFound { get; }

        Task<User> GetUser(int UserId);

        Task CreateUser(int UserId);

        Task<User> UpdateUser(int id, User user);
    }
}
