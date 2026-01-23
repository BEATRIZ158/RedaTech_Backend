using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.TurmaService
{
    public interface IClassService
    {
        Task<ServiceResponse<List<TurmaDto>>> GetClasses();
        Task<ServiceResponse<List<TurmaDto>>> CreateClass(TurmaDto novaTurmaDto);
        Task<ServiceResponse<TurmaDto>> GetClassById(int id);
        Task<ServiceResponse<List<TurmaDto>>> UpdateClass(TurmaDto editadoTurmaDto);
        Task<ServiceResponse<List<TurmaDto>>> InactiveClass(int id);
        Task<ServiceResponse<List<TurmaDto>>> DeleteClass(int id);
        Task<ServiceResponse<string>> AddStudentToClass(int turmaId, int alunoId);
        Task<ServiceResponse<string>> RemoveStudentToClass(int turmaId, int alunoId);
        Task<ServiceResponse<List<UsuarioDto>>> ListStudentsToClass(int turmaId);
        Task<ServiceResponse<List<TurmaDto>>> GetClassesByName(string nomeTurmaParcial);
    }
}
