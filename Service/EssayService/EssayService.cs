using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Redatech.DataContext;
using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.RedacaoService
{
    public class EssayService : IEssayService
    {

        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public EssayService(ApplicationDbContext context, IMapper mapper)
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

                Essay redacao = _mapper.Map<Essay>(novaRedacaoDto);
                _context.Essays.Add(redacao);
                await _context.SaveChangesAsync();

                var lista = _context.Essays.ToList();
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
                Essay essay = _context.Essays.FirstOrDefault(x => x.Id == id);

                if (essay == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Redação não localizada";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                _context.Essays.Remove(essay);
                await _context.SaveChangesAsync();

                // Exclui o arquivo da redação da pasta
                if (!string.IsNullOrEmpty(essay.FilePath))
                {
                    var caminhoArquivo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", essay.FilePath);
                    if (File.Exists(caminhoArquivo))
                        File.Delete(caminhoArquivo);
                }

                List<Essay> essays = await _context.Essays.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<RedacaoDto>>(essays);
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
                Essay essay = await _context.Essays.FirstOrDefaultAsync(essay => essay.Id == id);

                if (essay == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Redação não localizado";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                serviceResponse.Dados = _mapper.Map<RedacaoDto>(essay);

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
                List<Essay> essays = await _context.Essays.ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<RedacaoDto>>(essays);
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
                var essay = await _context.Essays.FirstOrDefaultAsync(essay => essay.Id == redacaoAtualizada.Id);

                if (essay == null)
                {
                    response.Mensagem = "Redação não encontrada.";
                    response.Sucesso = false;
                    return response;
                }

                // Atualiza os dados básicos
                essay.Description = redacaoAtualizada.Descricao;

                // Se um novo arquivo foi enviado
                if (novoArquivo != null && novoArquivo.Length > 0)
                {
                    // Exclui o arquivo antigo
                    if (!string.IsNullOrEmpty(essay.FilePath))
                    {
                        var caminhoArquivoAntigo = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", essay.FilePath);
                        if (File.Exists(caminhoArquivoAntigo))
                            File.Delete(caminhoArquivoAntigo);
                    }

                    // Garante que a pasta existe
                    var pathDestiny = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "redacoes");
                    if (!Directory.Exists(pathDestiny))
                        Directory.CreateDirectory(pathDestiny);

                    // Cria nome único
                    var fileName = $"{Guid.NewGuid()}_{novoArquivo.FileName}";
                    var pathComplete = Path.Combine(pathDestiny, fileName);

                    // Salva o novo arquivo
                    using (var stream = new FileStream(pathComplete, FileMode.Create))
                    {
                        await novoArquivo.CopyToAsync(stream);
                    }

                    // Atualiza o caminho no banco
                    essay.FilePath = Path.Combine("redacoes", fileName);
                }

                await _context.SaveChangesAsync();

                response.Dados = _mapper.Map<RedacaoDto>(essay);
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

        public async Task<ServiceResponse<string>> UploadFileEssay(IFormFile file)
        {
            var response = new ServiceResponse<string>();

            try
            {
                if (file == null || file.Length == 0)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Arquivo inválido!";
                    return response;
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var pathFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "redacoes");

                if (!Directory.Exists(pathFolder))
                    Directory.CreateDirectory(pathFolder);

                var pathComplete = Path.Combine(pathFolder, fileName);

                using (var stream = new FileStream(pathComplete, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                string pathDatabase = Path.Combine("redacoes", fileName).Replace("\\", "/");
                response.Dados = pathDatabase;
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
                List<Essay> essays = await _context.Essays.ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<RedacaoDto>>(essays);
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
