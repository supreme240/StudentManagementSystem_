using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Interfaces
{
    public interface ILogIn
    {
        Task<Registration?> ValidateUserAsync(string userNameOrEmail, string password);
    }
}