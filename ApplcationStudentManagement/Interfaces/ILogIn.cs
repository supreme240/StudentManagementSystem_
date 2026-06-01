using StudentManagement.domain.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudentManagement.Interfaces
{
    public interface ILogIn
    {
        Registration? ValidateUser(string userNameOrEmail, string password);
    }
}