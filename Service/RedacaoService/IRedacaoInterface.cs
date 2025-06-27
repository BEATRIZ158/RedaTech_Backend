using Microsoft.AspNetCore.Mvc;
using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.RedacaoService
{
    public interface IRedacaoInterface
    {
        Task<ServiceResponse<List<RedacaoDto>>> GetRedacoes();
        Task<ServiceResponse<List<RedacaoDto>>> CreateRedacao(RedacaoDto novaRedacaoDto);
        Task<ServiceResponse<RedacaoDto>> GetRedacaoById(int id);
        Task<ServiceResponse<RedacaoDto>> UpdateRedacaoAsync(RedacaoDto redacaoAtualizada, IFormFile? novoArquivo);
        Task<ServiceResponse<List<RedacaoDto>>> DeleteRedacao(int id);
        Task<ServiceResponse<string>> UploadArquivoRedacao(IFormFile arquivo);
        Task<ServiceResponse<List<RedacaoDto>>> GetRedacoesByTitulo(string tituloParcial);
        Task<ServiceResponse<List<RedacaoDto>>> CreateRedacaoComUpload(RedacaoUploadDto dto);
        Task<ServiceResponse<List<RedacaoDto>>> ListarRedacoesPorTurma(int idTurma);
        Task<ServiceResponse<List<RedacaoDto>>> ListarRedacoesComCorrecao(int idTurma);
        Task<ServiceResponse<List<RedacaoDto>>> ListarRedacoesSemCorrecao(int idTurma);
    }
}
