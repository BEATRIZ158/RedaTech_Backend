using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.TurmaService
{
    public interface ITurmaInterface
    {
        Task<ServiceResponse<List<TurmaDto>>> GetTurmas();
        Task<ServiceResponse<List<TurmaDto>>> CreateTurma(TurmaDto novaTurmaDto);
        Task<ServiceResponse<TurmaDto>> GetTurmaById(int id);
        Task<ServiceResponse<List<TurmaDto>>> UpdateTurma(TurmaDto editadoTurmaDto);
        Task<ServiceResponse<List<TurmaDto>>> InativaTurma(int id);
        Task<ServiceResponse<List<TurmaDto>>> DeleteTurma(int id);
        Task<ServiceResponse<string>> AdicionarAlunoNaTurma(int turmaId, int alunoId);
        Task<ServiceResponse<string>> RemoverAlunoDaTurma(int turmaId, int alunoId);
        Task<ServiceResponse<List<UsuarioDto>>> ListarAlunosDaTurma(int turmaId);
        Task<ServiceResponse<List<TurmaDto>>> GetTurmasByName(string nomeTurmaParcial);
    }
}
