using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.CorrecaoService
{
    public interface ICorrectionService
    {
        Task<ServiceResponse<List<CorrecaoDto>>> GetCorrections();
        Task<ServiceResponse<List<CorrecaoDto>>> CreateCorrection(CorrecaoDto novaCorrecaoDto);
        Task<ServiceResponse<CorrecaoDto>> GetCorrectionById(int id);
        Task<ServiceResponse<List<CorrecaoDto>>> UpdateCorrection(CorrecaoDto editadoCorrecaoDto);
        Task<ServiceResponse<List<CorrecaoDto>>> DeleteCorrection(int id);
    }
}
