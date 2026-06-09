using StudentManagement.domain.Domain;
using StudentManagementSystem.Infrastructure.Data;
using StudentManagementSystem.Infrastructure.Repositories.GenericRepo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentManagementSystem.Infrastructure.Repositories.RolesRepo {
    public class RolesRepository : GenericRepository<Role>, IRolesRepository {
        private readonly ApplicationDbContext _dbContext;

        public RolesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

