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
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Informar dados!";
                    serviceResponse.Success = false;
                    return serviceResponse;
                }

                Class newClass = _mapper.Map<Class>(novaTurmaDto);
                newClass.IsActive = true;

                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();

                List<Class> classes = _context.Classes.ToList();
                serviceResponse.Data = _mapper.Map<List<TurmaDto>>(classes);

                serviceResponse.Message = "Turma criada com sucesso!";
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Message = ex.Message;
                serviceResponse.Success = false;
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
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Turma não localizado";
                    serviceResponse.Success = false;

                    return serviceResponse;
                }

                _context.Classes.Remove(classFound);
                await _context.SaveChangesAsync();

                List<Class> classes = await _context.Classes.ToListAsync();
                serviceResponse.Data = _mapper.Map<List<TurmaDto>>(classes);
            }
            catch (Exception ex)
            {
                serviceResponse.Message = ex.Message;
                serviceResponse.Success = false;
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
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Turma não localizada";
                    serviceResponse.Success = false;
                    return serviceResponse;
                }

                serviceResponse.Data = _mapper.Map<TurmaDto>(classFound);

                serviceResponse.Message = "Turma encontrada com sucesso";
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Message = ex.Message;
                serviceResponse.Success = false;
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<TurmaDto>>> GetClasses()
        {
            ServiceResponse<List<TurmaDto>> serviceResponse = new ServiceResponse<List<TurmaDto>>();

            try
            {
                List<Class> classes = await _context.Classes.ToListAsync();

                serviceResponse.Data = _mapper.Map<List<TurmaDto>>(classes);

                serviceResponse.Message = "Lista de turmas obtida com sucesso";
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Message = ex.Message;
                serviceResponse.Success = false;
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
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Turma não localizada";
                    serviceResponse.Success = false;

                    return serviceResponse;
                }

                classFound.IsActive = !classFound.IsActive;

                _context.Classes.Update(classFound);

                await _context.SaveChangesAsync();

                List<Class> classes = await _context.Classes.ToListAsync();
                serviceResponse.Data = _mapper.Map<List<TurmaDto>>(classes);
            }
            catch (Exception ex)
            {
                serviceResponse.Message = ex.Message;
                serviceResponse.Success = false;
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
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Turma não localizada";
                    serviceResponse.Success = false;
                    return serviceResponse;
                }

                Class classUpdated = _mapper.Map<Class>(editadoTurmaDto);

                _context.Classes.Update(classUpdated);

                await _context.SaveChangesAsync();

                List<Class> classes = await _context.Classes.ToListAsync();
                serviceResponse.Data = _mapper.Map<List<TurmaDto>>(classes);
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Message = ex.Message;
                serviceResponse.Success = false;
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
                    response.Success = false;
                    response.Message = "Aluno já está vinculado a essa turma.";
                    return response;
                }

                var classFound = await _context.Classes.FirstOrDefaultAsync(x => x.Id == turmaId);

                if (classFound == null)
                {
                    response.Success = false;
                    response.Message = "Turma não encontrada.";
                    return response;
                }

                if (!classFound.IsActive)
                {
                    response.Success = false;
                    response.Message = "Não é possível adicionar um aluno, turma está inativa.";
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

                response.Success = true;
                response.Message = "Aluno adicionado com sucesso!";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro: {ex.Message}";
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
                    response.Success = false;
                    response.Message = "Essa relação não existe.";
                    return response;
                }

                var classFound = await _context.Classes.FirstOrDefaultAsync(c => c.Id == turmaId);

                if (classFound == null)
                {
                    response.Success = false;
                    response.Message = "Turma não encontrada.";
                    return response;
                }

                if (!classFound.IsActive)
                {
                    response.Success = false;
                    response.Message = "Não é possível remover um aluno, turma está inativa.";
                    return response;
                }

                _context.ClassesStudents.Remove(linkClassStudent);
                await _context.SaveChangesAsync();

                response.Success = true;
                response.Message = "Aluno removido com sucesso da turma.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro: {ex.Message}";
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
                    response.Success = false;
                    response.Message = "Turma não encontrada.";
                    return response;
                }

                response.Data = _mapper.Map<List<UsuarioDto>>(students);
                response.Success = true;
                response.Message = "Lista de alunos obtida com sucesso.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Erro ao buscar alunos da turma: {ex.Message}";
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
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Informe um nome para buscar.";
                    return serviceResponse;
                }

                var classes = await _context.Classes
                    .Where(c => c.Name.StartsWith(nomeTurmaParcial))
                    .ToListAsync();

                serviceResponse.Data = _mapper.Map<List<TurmaDto>>(classes);
                serviceResponse.Success = true;
                serviceResponse.Message = "Turmas encontradas com sucesso!";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }
    }
}
