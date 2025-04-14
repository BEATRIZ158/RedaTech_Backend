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

        public async Task<ServiceResponse<List<RedacaoDto>>> CreateRedacao(RedacaoDto novaRedacaoDto)
        {
            ServiceResponse<List<RedacaoDto>> serviceResponse = new ServiceResponse<List<RedacaoDto>>();

            try
            {
                if (novaRedacaoDto == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Informar dados!";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                RedacaoModel novaRedacao = _mapper.Map<RedacaoModel>(novaRedacaoDto);

                _context.Redacoes.Add(novaRedacao);
                await _context.SaveChangesAsync();

                List<RedacaoModel> redacoes = _context.Redacoes.ToList();
                serviceResponse.Dados = _mapper.Map<List<RedacaoDto>>(redacoes);

                serviceResponse.Mensagem = "Redação criada com sucesso!";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }
            return serviceResponse;
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
    }
}

