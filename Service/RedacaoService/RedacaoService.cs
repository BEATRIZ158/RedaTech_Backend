using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Redatech.DataContext;
using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.RedacaoService
{
    public class RedacaoService : IRedacaoInterface
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RedacaoService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<RedacaoDto>>> CreateEssay(RedacaoDto novaRedacaoDto)
        {
            var response = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                if (novaRedacaoDto == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Dados inválidos.";
                    return response;
                }

                RedacaoModel redacao = _mapper.Map<RedacaoModel>(novaRedacaoDto);
                _context.Redacoes.Add(redacao);
                await _context.SaveChangesAsync();

                var lista = _context.Redacoes.ToList();
                response.Dados = _mapper.Map<List<RedacaoDto>>(lista);
                response.Mensagem = "Redação criada com sucesso!";
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
                response.Mensagem = ex.Message;
            }

            return response;
        }

        public async Task<ServiceResponse<List<RedacaoDto>>> DeleteEssay(int id)
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                RedacaoModel redacao = _context.Redacoes.FirstOrDefault(x => x.Id == id);

                if (redacao == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Redação não localizada";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                _context.Redacoes.Remove(redacao);
                await _context.SaveChangesAsync();

                // Exclui o arquivo da redação da pasta
                if (!string.IsNullOrEmpty(redacao.CaminhoArquivo))
                {
                    var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", redacao.CaminhoArquivo);
                    if (File.Exists(caminhoArquivo))
                        File.Delete(caminhoArquivo);
                }

                List<RedacaoModel> redacoes = await _context.Redacoes.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<RedacaoDto>>(redacoes);

            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<RedacaoDto>> GetEssayById(int id)
        {
            ServiceResponse<RedacaoDto> serviceResponse = new ServiceResponse<RedacaoDto>();

            try
            {
                RedacaoModel redacao = await _context.Redacoes.FirstOrDefaultAsync(x => x.Id == id);

                if (redacao == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Redação não localizado";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                serviceResponse.Dados = _mapper.Map<RedacaoDto>(redacao);

                serviceResponse.Mensagem = "Redação encontrado com sucesso";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<RedacaoDto>>> GetEssays()
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                List<RedacaoModel> redacoes = await _context.Redacoes.ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<RedacaoDto>>(redacoes);
                serviceResponse.Mensagem = "Lista de redações obtida com sucesso";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<RedacaoDto>> UpdateEssayAsync(RedacaoDto redacaoAtualizada, IFormFile? novoArquivo)
        {
            var response = new ServiceResponse<RedacaoDto>();

            try
            {
                var redacaoExistente = await _context.Redacoes.FirstOrDefaultAsync(r => r.Id == redacaoAtualizada.Id);

                if (redacaoExistente == null)
                {
                    response.Mensagem = "Redação não encontrada.";
                    response.Sucesso = false;
                    return response;
                }

                // Atualiza os dados básicos
                redacaoExistente.Descricao = redacaoAtualizada.Descricao;

                // Se um novo arquivo foi enviado
                if (novoArquivo != null && novoArquivo.Length > 0)
                {
                    // Exclui o arquivo antigo
                    if (!string.IsNullOrEmpty(redacaoExistente.CaminhoArquivo))
                    {
                        var caminhoArquivoAntigo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", redacaoExistente.CaminhoArquivo);
                        if (File.Exists(caminhoArquivoAntigo))
                        {
                            File.Delete(caminhoArquivoAntigo);
                        }
                    }

                    // Garante que a pasta existe
                    var pastaDestino = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "redacoes");
                    if (!Directory.Exists(pastaDestino))
                        Directory.CreateDirectory(pastaDestino);

                    // Cria nome único
                    var nomeArquivo = $"{Guid.NewGuid()}_{novoArquivo.FileName}";
                    var caminhoCompleto = Path.Combine(pastaDestino, nomeArquivo);

                    // Salva o novo arquivo
                    using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                    {
                        await novoArquivo.CopyToAsync(stream);
                    }

                    // Atualiza o caminho no banco
                    redacaoExistente.CaminhoArquivo = Path.Combine("redacoes", nomeArquivo);
                }

                await _context.SaveChangesAsync();

                response.Dados = _mapper.Map<RedacaoDto>(redacaoExistente);
                response.Mensagem = "Redação atualizada com sucesso!";
                response.Sucesso = true;
            }
            catch (Exception ex)
            {
                response.Mensagem = $"Erro ao atualizar: {ex.Message}";
                response.Sucesso = false;
            }

            return response;
        }

        public async Task<ServiceResponse<string>> UploadFileEssay(IFormFile arquivo)
        {
            var response = new ServiceResponse<string>();

            try
            {
                if (arquivo == null || arquivo.Length == 0)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Arquivo inválido!";
                    return response;
                }

                var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(arquivo.FileName);
                var caminhoPasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "redacoes");

                if (!Directory.Exists(caminhoPasta))
                    Directory.CreateDirectory(caminhoPasta);

                var caminhoCompleto = Path.Combine(caminhoPasta, nomeArquivo);

                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await arquivo.CopyToAsync(stream);
                }

                string caminhoBanco = Path.Combine("redacoes", nomeArquivo).Replace("\\", "/");
                response.Dados = caminhoBanco;
                response.Mensagem = "Arquivo enviado com sucesso!";
                response.Sucesso = true;
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
                response.Mensagem = $"Erro ao fazer upload: {ex.Message}";
            }

            return response;
        }

        //Editar depois, lista pelos caracteres passados
        public async Task<ServiceResponse<List<RedacaoDto>>> GetEssaysByName(string nome)
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                List<RedacaoModel> redacoes = await _context.Redacoes.ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<RedacaoDto>>(redacoes);
                serviceResponse.Mensagem = "Lista de redações obtida com sucesso";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }
    }
}
