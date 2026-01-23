using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Redatech.DataContext;
using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.TurmaService
{
    public class TurmaService : ITurmaInterface
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TurmaService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper; // Agora o AutoMapper pode ser usado no Service
        }

        public async Task<ServiceResponse<List<TurmaDto>>> CreateClass(TurmaDto novaTurmaDto)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = new ServiceResponse<List<TurmaDto>>();

            try
            {
                if (novaTurmaDto == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Informar dados!";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                Class novaTurma = _mapper.Map<Class>(novaTurmaDto);
                novaTurma.Status = true;

                _context.Classes.Add(novaTurma);
                await _context.SaveChangesAsync();

                List<Class> turmas = _context.Classes.ToList();
                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(turmas);

                serviceResponse.Mensagem = "Turma criada com sucesso!";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<TurmaDto>>> DeleteClass(int id)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = new ServiceResponse<List<TurmaDto>>();

            try
            {
                Class turma = _context.Classes.FirstOrDefault(x => x.Id == id);

                if (turma == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Turma não localizado";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                _context.Classes.Remove(turma);
                await _context.SaveChangesAsync();

                List<Class> turmas = await _context.Classes.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(turmas);

            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<TurmaDto>> GetClassById(int id)
        {
            ServiceResponse<TurmaDto> serviceResponse = new ServiceResponse<TurmaDto>();

            try
            {
                Class turma = await _context.Classes.FirstOrDefaultAsync(x => x.Id == id);

                if (turma == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Turma não localizada";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                serviceResponse.Dados = _mapper.Map<TurmaDto>(turma);

                serviceResponse.Mensagem = "Turma encontrada com sucesso";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<TurmaDto>>> GetClasses()
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = new ServiceResponse<List<TurmaDto>>();

            try
            {
                List<Class> turmas = await _context.Classes.ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(turmas);

                serviceResponse.Mensagem = "Lista de turmas obtida com sucesso";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<TurmaDto>>> InactiveClass(int id)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = new ServiceResponse<List<TurmaDto>>();

            try
            {
                Class turma = _context.Classes.FirstOrDefault(x => x.Id == id);

                if (turma == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Turma não localizada";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                turma.Status = !turma.Status;

                _context.Classes.Update(turma);

                await _context.SaveChangesAsync();

                List<Class> turmas = await _context.Classes.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(turmas);
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<TurmaDto>>> UpdateClass(TurmaDto editadoTurmaDto)
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = new ServiceResponse<List<TurmaDto>>();

            try
            {
                Class turmaExistente = await _context.Classes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == editadoTurmaDto.Id);

                if (turmaExistente == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Turma não localizada";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                Class turmaAtualizado = _mapper.Map<Class>(editadoTurmaDto);

                _context.Classes.Update(turmaAtualizado);

                await _context.SaveChangesAsync();

                List<Class> turmas = await _context.Classes.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(turmas);
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Sucesso = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<string>> AddStudentToClass(int turmaId, int alunoId)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var jaExiste = await _context.ClassesStudents
                    .AnyAsync(ta => ta.TurmaId == turmaId && ta.AlunoId == alunoId);

                if (jaExiste)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Aluno já está vinculado a essa turma.";
                    return response;
                }

                var turma = await _context.Classes.FirstOrDefaultAsync(x => x.Id == turmaId);

                if (turma == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Turma não encontrada.";
                    return response;
                }

                if (!turma.Status)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Não é possível adicionar um aluno, turma está inativa.";
                    return response;
                }

                //Se estiver tudo certo, adicione o aluno a turma
                _context.ClassesStudents.Add(new ClassStudent
                {
                    TurmaId = turmaId,
                    AlunoId = alunoId,
                    DataAcao = DateTime.Now
                });

                await _context.SaveChangesAsync();

                response.Sucesso = true;
                response.Mensagem = "Aluno adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
                response.Mensagem = $"Erro: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<string>> RemoveStudentToClass(int turmaId, int alunoId)
        {
            var response = new ServiceResponse<string>();

            try
            {
                var relacao = await _context.ClassesStudents
                    .FirstOrDefaultAsync(ta => ta.TurmaId == turmaId && ta.AlunoId == alunoId);

                if (relacao == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Essa relação não existe.";
                    return response;
                }

                var turma = await _context.Classes.FirstOrDefaultAsync(x => x.Id == turmaId);

                if (turma == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Turma não encontrada.";
                    return response;
                }

                if (!turma.Status)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Não é possível remover um aluno, turma está inativa.";
                    return response;
                }

                _context.ClassesStudents.Remove(relacao);
                await _context.SaveChangesAsync();

                response.Sucesso = true;
                response.Mensagem = "Aluno removido com sucesso da turma.";
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
                response.Mensagem = $"Erro: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> ListStudentsToClass(int turmaId)
        {
            var response = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                var alunosIds = await _context.ClassesStudents
                    .Where(t => t.TurmaId == turmaId)
                    .Select(t => t.AlunoId)
                    .ToListAsync();

                var alunos = await _context.Users
                    .Where(u => alunosIds.Contains(u.Id))
                    .ToListAsync();

                var turma = await _context.Classes.FirstOrDefaultAsync(x => x.Id == turmaId);

                if (turma == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Turma não encontrada.";
                    return response;
                }

                response.Dados = _mapper.Map<List<UsuarioDto>>(alunos);
                response.Sucesso = true;
                response.Mensagem = "Lista de alunos obtida com sucesso.";
            }
            catch (Exception ex)
            {
                response.Sucesso = false;
                response.Mensagem = $"Erro ao buscar alunos da turma: {ex.Message}";
            }

            return response;
        }

        public async Task<ServiceResponse<List<TurmaDto>>> GetClassesByName(string nomeTurmaParcial)
        {
            var serviceResponse = new ServiceResponse<List<TurmaDto>>();

            try
            {
                if (string.IsNullOrEmpty(nomeTurmaParcial))
                {
                    serviceResponse.Sucesso = false;
                    serviceResponse.Mensagem = "Informe um nome para buscar.";
                    return serviceResponse;
                }

                var turmas = await _context.Classes
                    .Where(u => u.Nome.StartsWith(nomeTurmaParcial))
                    .ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(turmas);
                serviceResponse.Sucesso = true;
                serviceResponse.Mensagem = "Turmas encontradas com sucesso!";
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

    }
}
