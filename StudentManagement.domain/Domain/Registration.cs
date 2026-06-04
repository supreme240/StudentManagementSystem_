using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace StudentManagement.domain.Domain
{
    public class Registration
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;
        public string Gender { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role {get; set; } = "Student";
    }
}
