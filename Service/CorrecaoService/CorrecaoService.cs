using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Redatech.DataContext;
using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.CorrecaoService
{
    public class CorrecaoService : ICorrecaoInterface
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CorrecaoService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ServiceResponse<List<CorrecaoDto>>> CreateCorrecao(CorrecaoDto novaCorrecaoDto)
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = new ServiceResponse<List<CorrecaoDto>>();

            try
            {
                if (novaCorrecaoDto == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Informar dados!";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                // Mapeia o DTO para a entidade que será salva no banco
                CorrecaoModel novaCorrecao = _mapper.Map<CorrecaoModel>(novaCorrecaoDto);

                _context.Correcoes.Add(novaCorrecao);
                await _context.SaveChangesAsync();

                List<CorrecaoModel> correcoes = _context.Correcoes.ToList();
                serviceResponse.Dados = _mapper.Map<List<CorrecaoDto>>(correcoes);

                serviceResponse.Mensagem = "Correção criada com sucesso!";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<CorrecaoDto>>> DeleteCorrecao(int id)
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = new ServiceResponse<List<CorrecaoDto>>();

            try
            {
                CorrecaoModel correcao = _context.Correcoes.FirstOrDefault(x => x.Id == id);

                if (correcao == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Correção não localizada";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                _context.Correcoes.Remove(correcao);
                await _context.SaveChangesAsync();

                List<CorrecaoModel> correcoes = await _context.Correcoes.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<CorrecaoDto>>(correcoes);

            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<CorrecaoDto>> GetCorrecaoById(int id)
        {
            ServiceResponse<CorrecaoDto> serviceResponse = new ServiceResponse<CorrecaoDto>();

            try
            {
                CorrecaoModel correcao = await _context.Correcoes.FirstOrDefaultAsync(x => x.Id == id);

                if (correcao == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Correção não localizada";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                serviceResponse.Dados = _mapper.Map<CorrecaoDto>(correcao);

                serviceResponse.Mensagem = "Correção encontrada com sucesso";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<CorrecaoDto>>> GetCorrecoes()
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = new ServiceResponse<List<CorrecaoDto>>();

            try
            {
                List<CorrecaoModel> correcoes = await _context.Correcoes.ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<CorrecaoDto>>(correcoes);

                serviceResponse.Mensagem = "Lista de correções obtida com sucesso";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<CorrecaoDto>>> UpdateCorrecao(CorrecaoDto editadoCorrecaoDto)
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = new ServiceResponse<List<CorrecaoDto>>();

            try
            {
                CorrecaoModel correcaoExistente = await _context.Correcoes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == editadoCorrecaoDto.Id);

                if (correcaoExistente == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Correção não localizada";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                CorrecaoModel correcaoAtualizada = _mapper.Map<CorrecaoModel>(editadoCorrecaoDto);

                _context.Correcoes.Update(correcaoAtualizada);

                await _context.SaveChangesAsync();

                List<CorrecaoModel> correcoes = await _context.Correcoes.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<CorrecaoDto>>(correcoes);
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
