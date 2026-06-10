using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IForgotPassword
    {
        Task<int?> ValidateUserAsync(string email, long phoneNumber);
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
    }
}