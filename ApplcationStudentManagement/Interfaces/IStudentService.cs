
using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IStudentService
    {
        Student GetStudentInformation();
        Student GetNewStudentInformation();
    }
}
