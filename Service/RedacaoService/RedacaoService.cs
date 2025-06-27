using AutoMapper;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<ServiceResponse<List<RedacaoDto>>> CreateRedacao(RedacaoDto novaRedacaoDto)
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

        public async Task<ServiceResponse<List<RedacaoDto>>> DeleteRedacao(int id)
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
                    if (System.IO.File.Exists(caminhoArquivo))
                    {
                        System.IO.File.Delete(caminhoArquivo);
                    }
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

        public async Task<ServiceResponse<RedacaoDto>> GetRedacaoById(int id)
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

        public async Task<ServiceResponse<List<RedacaoDto>>> GetRedacoes()
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

        public async Task<ServiceResponse<RedacaoDto>> UpdateRedacaoAsync(RedacaoDto redacaoAtualizada, IFormFile? novoArquivo)
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
                redacaoExistente.Titulo = redacaoAtualizada.Titulo;

                // Se um novo arquivo foi enviado
                if (novoArquivo != null && novoArquivo.Length > 0)
                {
                    // Exclui o arquivo antigo
                    if (!string.IsNullOrEmpty(redacaoExistente.CaminhoArquivo))
                    {
                        var caminhoArquivoAntigo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", redacaoExistente.CaminhoArquivo);
                        if (System.IO.File.Exists(caminhoArquivoAntigo))
                        {
                            System.IO.File.Delete(caminhoArquivoAntigo);
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

        public async Task<ServiceResponse<string>> UploadArquivoRedacao(IFormFile arquivo)
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

        public async Task<ServiceResponse<List<RedacaoDto>>> GetRedacoesByTitulo(string tituloParcial)
        {
            var serviceResponse = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                if (string.IsNullOrEmpty(tituloParcial))
                {
                    serviceResponse.Sucesso = false;
                    serviceResponse.Mensagem = "Informe um título para buscar.";
                    return serviceResponse;
                }

                var redacoes = await _context.Redacoes
                    .Where(u => u.Titulo.StartsWith(tituloParcial))
                    .ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<RedacaoDto>>(redacoes);
                serviceResponse.Sucesso = true;
                serviceResponse.Mensagem = "Redação encontrada com sucesso!";
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<RedacaoDto>>> CreateRedacaoComUpload(RedacaoUploadDto dto)
        {
            var response = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                if (dto == null || dto.Arquivo == null || dto.Arquivo.Length == 0)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Dados ou arquivo inválido.";
                    return response;
                }

                // 1. Faz upload do arquivo
                var nomeArquivo = Guid.NewGuid().ToString() + Path.GetExtension(dto.Arquivo.FileName);
                var caminhoPasta = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "redacoes");

                if (!Directory.Exists(caminhoPasta))
                    Directory.CreateDirectory(caminhoPasta);

                var caminhoCompleto = Path.Combine(caminhoPasta, nomeArquivo);
                using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
                {
                    await dto.Arquivo.CopyToAsync(stream);
                }

                var caminhoBanco = Path.Combine("redacoes", nomeArquivo).Replace("\\", "/");

                // 2. Cria a Redação com o caminho do arquivo
                var redacao = new RedacaoModel
                {
                    AlunoId = dto.AlunoId,
                    Titulo = dto.Titulo,
                    Descricao = dto.Descricao,
                    CaminhoArquivo = caminhoBanco
                };

                _context.Redacoes.Add(redacao);
                await _context.SaveChangesAsync();

                response.Dados = _mapper.Map<List<RedacaoDto>>(_context.Redacoes.ToList());
                response.Mensagem = "Redação criada com sucesso!";
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
                response.Mensagem = ex.Message;

                if (ex.InnerException != null)
                {
                    response.Mensagem += " Detalhes: " + ex.InnerException.Message;
                }
            }

            return response;
        }

        public async Task<ServiceResponse<List<RedacaoDto>>> ListarRedacoesPorTurma(int idTurma)
        {
            var serviceResponse = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                var redacoes = await _context.Redacoes
                    .FromSqlInterpolated($@"
                        SELECT r.*
                        FROM Redacoes r
                        WHERE r.AlunoId IN (
                            SELECT AlunoId FROM TurmasAlunos WHERE TurmaId = {idTurma}
                        )
                    ")
                    .ToListAsync();

                var redacoesDto = _mapper.Map<List<RedacaoDto>>(redacoes);

                serviceResponse.Dados = redacoesDto;
                serviceResponse.Sucesso = true;
                serviceResponse.Mensagem = "Redações da turma listadas com sucesso.";
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro ao listar redações por turma: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<RedacaoDto>>> ListarRedacoesComCorrecao(int idTurma)
        {
            var serviceResponse = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                var redacoesComCorrecao = await _context.Redacoes
                    .FromSqlInterpolated($@"
                        SELECT r.*
                        FROM Redacoes r
                        INNER JOIN Correcoes c ON c.RedacaoId = r.Id
                        WHERE r.UsuarioId IN (
                            SELECT AlunoId FROM TurmasAlunos WHERE TurmaId = {idTurma}
                        )
                    ")
                    .ToListAsync();

                var redacoesDto = _mapper.Map<List<RedacaoDto>>(redacoesComCorrecao);

                serviceResponse.Dados = redacoesDto;
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro ao buscar redações com correção: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<RedacaoDto>>> ListarRedacoesSemCorrecao(int idTurma)
        {
            var serviceResponse = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                var redacoesSemCorrecao = await _context.Redacoes
                    .FromSqlInterpolated($@"
                        SELECT r.*
                        FROM Redacoes r
                        WHERE r.UsuarioId IN (
                            SELECT AlunoId FROM TurmasAlunos WHERE TurmaId = {idTurma}
                        )
                        AND r.Id NOT IN (
                            SELECT RedacaoId FROM Correcoes
                        )
                    ")
                    .ToListAsync();

                var redacoesDto = _mapper.Map<List<RedacaoDto>>(redacoesSemCorrecao);

                serviceResponse.Dados = redacoesDto;
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro ao buscar redações sem correção: {ex.Message}";
            }

            return serviceResponse;
        }
    }
}
