
using StudentManagement.domain.Domain;

namespace ApplicationStudentManagement.Interfaces
{
    public interface IStudentInterface
    {
        Student GetStudentInformation();
        Student GetNewStudentInformation();
    }
}
