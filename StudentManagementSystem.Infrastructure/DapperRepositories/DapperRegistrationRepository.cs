using Dapper ;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using StudentManagementSystem.Infrastructure.DapperRepositories;
using System.Data;

namespace StudentManagement.domain.Domain
{
    public class DapperRegistrationRepository : IDapperRegistrationRepository
    {
        private readonly string _connectionString;

        public DapperRegistrationRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private IDbConnection CreateConnection()
            => new SqlConnection(_connectionString);

        public async Task<Registration?> GetByIdAsync(int id)
        {
            using var db = CreateConnection();

            // Table name here is Registrations real SQL table

            const string sql = "SELECT * FROM Registrations WHERE Id = @Id";

            return await db.QueryFirstOrDefaultAsync<Registration>(sql, new { Id = id });
        }

        public async Task<IEnumerable<Registration>> GetAllAsync()
        {
            using var db = CreateConnection();

            // Same here  querying the "Registrations" table
            const string sql = "SELECT * FROM Registrations";

            return await db.QueryAsync<Registration>(sql);
        }

        Task<(bool success, string error)> AddRegistrationAsync(Registration registration) => throw new NotImplementedException();

        Task<(bool sucess, string error)> IDapperRegistrationRepository.AddRegistrationAsync(Registration registration)
        {
            return AddRegistrationAsync(registration);
        }
    }
}