using StudentManagement.domain.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace StudentManagementSystem.Infrastructure.DapperRepositories
{
    public interface IDapperRegistrationRepository
    {
        Task<Registration?> GetByIdAsync(int id);
        Task<IEnumerable<Registration>> GetAllAsync();
    }
}
