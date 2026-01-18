using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.RedacaoService
{
    public interface IRedacaoInterface
    {
        Task<ServiceResponse<List<RedacaoDto>>> GetEssays();
        Task<ServiceResponse<List<RedacaoDto>>> CreateEssay(RedacaoDto novaRedacaoDto);
        Task<ServiceResponse<RedacaoDto>> GetEssayById(int id);
        Task<ServiceResponse<RedacaoDto>> UpdateEssayAsync(RedacaoDto redacaoAtualizada, IFormFile? novoArquivo);
        Task<ServiceResponse<List<RedacaoDto>>> DeleteEssay(int id);
        Task<ServiceResponse<string>> UploadFileEssay(IFormFile arquivo);
        Task<ServiceResponse<List<RedacaoDto>>> GetEssaysByName(string nomeParcial);
    }
}
