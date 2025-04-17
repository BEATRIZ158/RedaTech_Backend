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

        public async Task<ServiceResponse<List<RedacaoDto>>> UpdateRedacao(RedacaoDto editadaRedacaoDto)
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                RedacaoModel redacaoExistente = await _context.Redacoes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == editadaRedacaoDto.Id);

                if (redacaoExistente == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Redação não localizada";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                RedacaoModel redacaoAtualizada = _mapper.Map<RedacaoModel>(editadaRedacaoDto);

                _context.Redacoes.Update(redacaoAtualizada);

                await _context.SaveChangesAsync();

                List<RedacaoModel> redacoes = await _context.Redacoes.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<RedacaoDto>>(redacoes);
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
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

        //Editar depois, lista pelos caracteres passados
        public async Task<ServiceResponse<List<RedacaoDto>>> GetRedacoesByName(string nome)
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
