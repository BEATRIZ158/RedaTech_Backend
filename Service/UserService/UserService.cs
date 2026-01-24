using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Redatech.DataContext;
using Redatech.Dto;
using Redatech.Estaticos.Login;
using Redatech.Models;

namespace Redatech.Service.UsuarioService
{
    public class UserService : IUserService
    {

        //Variável que armazenará a instância do ApplicationDbContext
        //readonly significa que o valor dessa variável só pode ser atribuído no construtor e não pode ser alterado depois
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        //Construtor da classe
        //Quando UsuarioService for criado, o contexto do banco será passado via injeção de dependência.
        //_context = context; armazena essa instância para ser usada nos métodos do serviço.
        public UserService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper; // Agora o AutoMapper pode ser usado no Service
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> CreateUser(UsuarioDto novoUsuarioDto)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                if (novoUsuarioDto == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Informar dados!";
                    serviceResponse.Success = false;
                    return serviceResponse;
                }

                // Mapeia o DTO para a entidade que será salva no banco
                User newUser = _mapper.Map<User>(novoUsuarioDto);
                newUser.IsActive = true;

                newUser.PasswordHash = CriptografiaHash.GerarHash(novoUsuarioDto.SenhaHash);

                // Adiciona ao banco de dados
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Recupera todos os usuários já convertidos para DTO
                List<User> usuarios = _context.Users.ToList();
                serviceResponse.Data = _mapper.Map<List<UsuarioDto>>(usuarios);

                serviceResponse.Message = "Usuário criado com sucesso!";
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> DeleteUser(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                User user = _context.Users.FirstOrDefault(user => user.Id == id);

                if (user == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Usuário não localizado";
                    serviceResponse.Success = false;

                    return serviceResponse;
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                List<User> usuarios = await _context.Users.ToListAsync();
                serviceResponse.Data = _mapper.Map<List<UsuarioDto>>(usuarios);

            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UsuarioDto>> GetUserById(int id)
        {
            ServiceResponse<UsuarioDto> serviceResponse = new ServiceResponse<UsuarioDto>();

            try
            {
                // Realiza a busca do usuário no banco
                User user = await _context.Users.FirstOrDefaultAsync(user => user.Id == id);

                // Caso não encontre o usuário, atribui a mensagem de erro
                if (user == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Usuário não localizado";
                    serviceResponse.Success = false;
                    return serviceResponse; // Retorna a resposta com erro imediatamente
                }

                // Se o usuário for encontrado, mapeia para o DTO
                serviceResponse.Data = _mapper.Map<UsuarioDto>(user);

                // Sucesso
                serviceResponse.Message = "Usuário encontrado com sucesso";
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                // Em caso de erro no processo
                serviceResponse.Success = false;
                serviceResponse.Message = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> GetUsers()
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                // Obtém todos os usuários do banco
                List<User> users = await _context.Users.ToListAsync();

                // Converte a lista de UsuarioModel para UsuarioDto usando AutoMapper
                serviceResponse.Data = _mapper.Map<List<UsuarioDto>>(users);

                // Define a mensagem de sucesso
                serviceResponse.Message = "Lista de usuários obtida com sucesso";
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna a mensagem de exceção
                serviceResponse.Success = false;
                serviceResponse.Message = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        //O que tem dentro do ServiceResponse<> é o tipo que será retornado no return
        public async Task<ServiceResponse<List<UsuarioDto>>> InactiveUser(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                User user = _context.Users.FirstOrDefault(user => user.Id == id);

                if (user == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Usuário não localizado";
                    serviceResponse.Success = false;

                    return serviceResponse; // Retorna a resposta com erro imediatamente
                }

                // Alterna o status (ativo/inativo) de forma mais eficiente
                user.IsActive = !user.IsActive;

                /*Dentro da tabela de usuario, fazer um update no usuario definido */
                _context.Users.Update(user);

                /* Salvar a operação realizada */
                await _context.SaveChangesAsync();

                List<User> users = await _context.Users.ToListAsync();
                serviceResponse.Data = _mapper.Map<List<UsuarioDto>>(users);
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> UpdateUser(UsuarioDto editadoUsuarioDto)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                // Busca o usuário no banco, mas sem rastreamento para evitar conflitos
                User userFound = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(user => user.Id == editadoUsuarioDto.Id);

                if (userFound == null)
                {
                    serviceResponse.Data = null;
                    serviceResponse.Message = "Usuário não localizado";
                    serviceResponse.Success = false;
                    return serviceResponse;
                }

                // Mapeia os dados editados do DTO para um objeto do tipo UsuarioModel
                User userUpdated = _mapper.Map<User>(editadoUsuarioDto);

                userUpdated.PasswordHash = CriptografiaHash.GerarHash(editadoUsuarioDto.SenhaHash);

                // Atualiza o objeto no contexto
                _context.Users.Update(userUpdated);

                // Salva as mudanças no banco
                await _context.SaveChangesAsync();

                // Retorna a lista atualizada de usuários
                List<User> usuarios = await _context.Users.ToListAsync();
                serviceResponse.Data = _mapper.Map<List<UsuarioDto>>(usuarios);
                serviceResponse.Success = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> GetUserByName(string nomeParcial)
        {
            var serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                if (string.IsNullOrEmpty(nomeParcial))
                {
                    serviceResponse.Success = false;
                    serviceResponse.Message = "Informe um nome para buscar.";
                    return serviceResponse;
                }

                //Já o EndsWith() seria LIKE '%ami'.
                //Contains(nomeParcial) é o equivalente a LIKE '%Nome%'
                //StartsWith e´o equivalente a LIKE 'Nome%'
                var users = await _context.Users
                    .Where(user => user.Name.StartsWith(nomeParcial))
                    .ToListAsync();

                serviceResponse.Data = _mapper.Map<List<UsuarioDto>>(users);
                serviceResponse.Success = true;
                serviceResponse.Message = "Usuários encontrados com sucesso!";
            }
            catch (Exception ex)
            {
                serviceResponse.Success = false;
                serviceResponse.Message = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UsuarioLogadoDto>> Login(LoginDto loginDto)
        {
            var response = new ServiceResponse<UsuarioLogadoDto>();

            var usuario = await _context.Users
                .FirstOrDefaultAsync(user => user.Email == loginDto.Email);

            if (usuario == null || !CriptografiaHash.VerificarSenha(loginDto.Senha, usuario.PasswordHash))
            {
                response.Success = false;
                response.Message = "Usuário ou senha inválidos!";
                return response;
            }

            var usuarioLogado = new UsuarioLogadoDto
            {
                Id = usuario.Id,
                Nome = usuario.Name,
                Email = usuario.Email
            };

            response.Success = true;
            response.Message = "Login realizado com sucesso!";
            response.Data = usuarioLogado;

            return response;
        }
    }
}
