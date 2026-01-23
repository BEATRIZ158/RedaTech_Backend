using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Redatech.DataContext;
using Redatech.Dto;
using Redatech.Models;

namespace Redatech.Service.TurmaService
{
    public class ClassService : IClassService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ClassService(ApplicationDbContext context, IMapper mapper)
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

                Class newClass = _mapper.Map<Class>(novaTurmaDto);
                newClass.IsActive = true;

                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();

                List<Class> classes = _context.Classes.ToList();
                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(classes);

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
                Class classFound = _context.Classes.FirstOrDefault(c => c.Id == id);

                if (classFound == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Turma não localizado";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                _context.Classes.Remove(classFound);
                await _context.SaveChangesAsync();

                List<Class> classes = await _context.Classes.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(classes);
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
                Class classFound = await _context.Classes.FirstOrDefaultAsync(c => c.Id == id);

                if (classFound == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Turma não localizada";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                serviceResponse.Dados = _mapper.Map<TurmaDto>(classFound);

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
                List<Class> classes = await _context.Classes.ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(classes);

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
                Class classFound = _context.Classes.FirstOrDefault(x => x.Id == id);

                if (classFound == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Turma não localizada";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                classFound.IsActive = !classFound.IsActive;

                _context.Classes.Update(classFound);

                await _context.SaveChangesAsync();

                List<Class> classes = await _context.Classes.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(classes);
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
                Class classFound = await _context.Classes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == editadoTurmaDto.Id);

                if (classFound == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Turma não localizada";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                Class classUpdated = _mapper.Map<Class>(editadoTurmaDto);

                _context.Classes.Update(classUpdated);

                await _context.SaveChangesAsync();

                List<Class> classes = await _context.Classes.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(classes);
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
                var linkExists = await _context.ClassesStudents
                    .AnyAsync(link => link.ClassId == turmaId && link.StudentId == alunoId);

                if (linkExists)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Aluno já está vinculado a essa turma.";
                    return response;
                }

                var classFound = await _context.Classes.FirstOrDefaultAsync(x => x.Id == turmaId);

                if (classFound == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Turma não encontrada.";
                    return response;
                }

                if (!classFound.IsActive)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Não é possível adicionar um aluno, turma está inativa.";
                    return response;
                }

                //Se estiver tudo certo, adicione o aluno a turma
                _context.ClassesStudents.Add(new ClassStudent
                {
                    ClassId = turmaId,
                    StudentId = alunoId,
                    JoinedAt = DateTime.Now
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
                var linkClassStudent = await _context.ClassesStudents
                    .FirstOrDefaultAsync(link => link.ClassId == turmaId && link.StudentId == alunoId);

                if (linkClassStudent == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Essa relação não existe.";
                    return response;
                }

                var classFound = await _context.Classes.FirstOrDefaultAsync(c => c.Id == turmaId);

                if (classFound == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Turma não encontrada.";
                    return response;
                }

                if (!classFound.IsActive)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Não é possível remover um aluno, turma está inativa.";
                    return response;
                }

                _context.ClassesStudents.Remove(linkClassStudent);
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

        public async Task<ServiceResponse<List<UsuarioDto>>> ListStudentsToClass(int classId)
        {
            var response = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                var studentsIds = await _context.ClassesStudents
                    .Where(c => c.ClassId == classId)
                    .Select(c => c.StudentId)
                    .ToListAsync();

                var students = await _context.Users
                    .Where(user => studentsIds.Contains(user.Id))
                    .ToListAsync();

                var classFound = await _context.Classes.FirstOrDefaultAsync(c => c.Id == classId);

                if (classFound == null)
                {
                    response.Sucesso = false;
                    response.Mensagem = "Turma não encontrada.";
                    return response;
                }

                response.Dados = _mapper.Map<List<UsuarioDto>>(students);
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

                var classes = await _context.Classes
                    .Where(c => c.Name.StartsWith(nomeTurmaParcial))
                    .ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<TurmaDto>>(classes);
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
