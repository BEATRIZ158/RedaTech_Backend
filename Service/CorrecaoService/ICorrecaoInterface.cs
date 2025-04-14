using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.CorrecaoService
{
    public interface ICorrecaoInterface
    {
        Task<ServiceResponse<List<CorrecaoDto>>> GetCorrecoes();
        Task<ServiceResponse<List<CorrecaoDto>>> CreateCorrecao(CorrecaoDto novaCorrecaoDto);
        Task<ServiceResponse<CorrecaoDto>> GetCorrecaoById(int id);
        Task<ServiceResponse<List<CorrecaoDto>>> UpdateCorrecao(CorrecaoDto editadoCorrecaoDto);
        Task<ServiceResponse<List<CorrecaoDto>>> DeleteCorrecao(int id);
    }
}
