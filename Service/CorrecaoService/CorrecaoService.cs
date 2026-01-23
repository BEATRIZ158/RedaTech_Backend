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

        public async Task<ServiceResponse<List<CorrecaoDto>>> CreateCorrection(CorrecaoDto novaCorrecaoDto)
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
                Correction novaCorrecao = _mapper.Map<Correction>(novaCorrecaoDto);

                _context.Corrections.Add(novaCorrecao);
                await _context.SaveChangesAsync();

                List<Correction> correcoes = _context.Corrections.ToList();
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

        public async Task<ServiceResponse<List<CorrecaoDto>>> DeleteCorrection(int id)
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = new ServiceResponse<List<CorrecaoDto>>();

            try
            {
                Correction correcao = _context.Corrections.FirstOrDefault(x => x.Id == id);

                if (correcao == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Correção não localizada";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                _context.Corrections.Remove(correcao);
                await _context.SaveChangesAsync();

                List<Correction> correcoes = await _context.Corrections.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<CorrecaoDto>>(correcoes);

            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<CorrecaoDto>> GetCorrectionById(int id)
        {
            ServiceResponse<CorrecaoDto> serviceResponse = new ServiceResponse<CorrecaoDto>();

            try
            {
                Correction correcao = await _context.Corrections.FirstOrDefaultAsync(x => x.Id == id);

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

        public async Task<ServiceResponse<List<CorrecaoDto>>> GetCorrections()
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = new ServiceResponse<List<CorrecaoDto>>();

            try
            {
                List<Correction> correcoes = await _context.Corrections.ToListAsync();

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

        public async Task<ServiceResponse<List<CorrecaoDto>>> UpdateCorrection(CorrecaoDto editadoCorrecaoDto)
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = new ServiceResponse<List<CorrecaoDto>>();

            try
            {
                Correction correcaoExistente = await _context.Corrections
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == editadoCorrecaoDto.Id);

                if (correcaoExistente == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Correção não localizada";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                Correction correcaoAtualizada = _mapper.Map<Correction>(editadoCorrecaoDto);

                _context.Corrections.Update(correcaoAtualizada);

                await _context.SaveChangesAsync();

                List<Correction> correcoes = await _context.Corrections.ToListAsync();
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
