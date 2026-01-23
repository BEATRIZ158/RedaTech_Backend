using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Redatech.DataContext;
using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.CorrecaoService
{
    public class CorrectionService : ICorrectionService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CorrectionService(ApplicationDbContext context, IMapper mapper)
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
                Correction newCorrection = _mapper.Map<Correction>(novaCorrecaoDto);

                _context.Corrections.Add(newCorrection);
                await _context.SaveChangesAsync();

                List<Correction> corrections = _context.Corrections.ToList();
                serviceResponse.Dados = _mapper.Map<List<CorrecaoDto>>(corrections);

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
                Correction correction = _context.Corrections.FirstOrDefault(correction => correction.Id == id);

                if (correction == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Correção não localizada";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                _context.Corrections.Remove(correction);
                await _context.SaveChangesAsync();

                List<Correction> corrections = await _context.Corrections.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<CorrecaoDto>>(corrections);

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
                Correction correction = await _context.Corrections.FirstOrDefaultAsync(correction => correction.Id == id);

                if (correction == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Correção não localizada";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                serviceResponse.Dados = _mapper.Map<CorrecaoDto>(correction);

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
                List<Correction> corrections = await _context.Corrections.ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<CorrecaoDto>>(corrections);

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
                Correction correctionFound = await _context.Corrections
                    .AsNoTracking()
                    .FirstOrDefaultAsync(correction => correction.Id == editadoCorrecaoDto.Id);

                if (correctionFound == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Correção não localizada";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                Correction correctionUpdated = _mapper.Map<Correction>(editadoCorrecaoDto);

                _context.Corrections.Update(correctionUpdated);

                await _context.SaveChangesAsync();

                List<Correction> corrections = await _context.Corrections.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<CorrecaoDto>>(corrections);
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
