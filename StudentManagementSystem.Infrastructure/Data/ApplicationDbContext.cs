using Microsoft.EntityFrameworkCore;
using StudentManagement.domain.Domain;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace StudentManagementSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Registration> Registrations { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
    }
}

