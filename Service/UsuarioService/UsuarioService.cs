using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Redatech.DataContext;
using Redatech.Dto;
using Redatech.Estaticos.Login;
using Redatech.Models;

namespace Redatech.Service.UsuarioService
{
    public class UsuarioService : IUsuarioInterface
    {

        //Variável que armazenará a instância do ApplicationDbContext
        //readonly significa que o valor dessa variável só pode ser atribuído no construtor e não pode ser alterado depois
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        //Construtor da classe
        //Quando UsuarioService for criado, o contexto do banco será passado via injeção de dependência.
        //_context = context; armazena essa instância para ser usada nos métodos do serviço.
        public UsuarioService(ApplicationDbContext context, IMapper mapper)
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
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Informar dados!";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                // Mapeia o DTO para a entidade que será salva no banco
                User novoUsuario = _mapper.Map<User>(novoUsuarioDto);
                novoUsuario.Status = true;

                novoUsuario.SenhaHash = CriptografiaHash.GerarHash(novoUsuarioDto.SenhaHash);

                // Adiciona ao banco de dados
                _context.Users.Add(novoUsuario);
                await _context.SaveChangesAsync();

                // Recupera todos os usuários já convertidos para DTO
                List<User> usuarios = _context.Users.ToList();
                serviceResponse.Dados = _mapper.Map<List<UsuarioDto>>(usuarios);

                serviceResponse.Mensagem = "Usuário criado com sucesso!";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> DeleteUser(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                User usuario = _context.Users.FirstOrDefault(x => x.Id == id);

                if (usuario == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Usuário não localizado";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                _context.Users.Remove(usuario);
                await _context.SaveChangesAsync();

                List<User> usuarios = await _context.Users.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<UsuarioDto>>(usuarios);

            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UsuarioDto>> GetUserById(int id)
        {
            ServiceResponse<UsuarioDto> serviceResponse = new ServiceResponse<UsuarioDto>();

            try
            {
                // Realiza a busca do usuário no banco
                User usuario = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);

                // Caso não encontre o usuário, atribui a mensagem de erro
                if (usuario == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Usuário não localizado";
                    serviceResponse.Sucesso = false;
                    return serviceResponse; // Retorna a resposta com erro imediatamente
                }

                // Se o usuário for encontrado, mapeia para o DTO
                serviceResponse.Dados = _mapper.Map<UsuarioDto>(usuario);

                // Sucesso
                serviceResponse.Mensagem = "Usuário encontrado com sucesso";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                // Em caso de erro no processo
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> GetUsers()
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                // Obtém todos os usuários do banco
                List<User> usuarios = await _context.Users.ToListAsync();

                // Converte a lista de UsuarioModel para UsuarioDto usando AutoMapper
                serviceResponse.Dados = _mapper.Map<List<UsuarioDto>>(usuarios);

                // Define a mensagem de sucesso
                serviceResponse.Mensagem = "Lista de usuários obtida com sucesso";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                // Em caso de erro, retorna a mensagem de exceção
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        //O que tem dentro do ServiceResponse<> é o tipo que será retornado no return
        public async Task<ServiceResponse<List<UsuarioDto>>> InactiveUser(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                User usuario = _context.Users.FirstOrDefault(x => x.Id == id);

                if (usuario == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Usuário não localizado";
                    serviceResponse.Sucesso = false;

                    return serviceResponse; // Retorna a resposta com erro imediatamente
                }

                // Alterna o status (ativo/inativo) de forma mais eficiente
                usuario.Status = !usuario.Status;

                /*Dentro da tabela de usuario, fazer um update no usuario definido */
                _context.Users.Update(usuario);

                /* Salvar a operação realizada */
                await _context.SaveChangesAsync();

                List<User> usuarios = await _context.Users.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<UsuarioDto>>(usuarios);
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> UpdateUser(UsuarioDto editadoUsuarioDto)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                // Busca o usuário no banco, mas sem rastreamento para evitar conflitos
                User usuarioExistente = await _context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == editadoUsuarioDto.Id);

                if (usuarioExistente == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Usuário não localizado";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                // Mapeia os dados editados do DTO para um objeto do tipo UsuarioModel
                User usuarioAtualizado = _mapper.Map<User>(editadoUsuarioDto);

                usuarioAtualizado.SenhaHash = CriptografiaHash.GerarHash(editadoUsuarioDto.SenhaHash);

                // Atualiza o objeto no contexto
                _context.Users.Update(usuarioAtualizado);

                // Salva as mudanças no banco
                await _context.SaveChangesAsync();

                // Retorna a lista atualizada de usuários
                List<User> usuarios = await _context.Users.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<UsuarioDto>>(usuarios);
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
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
                    serviceResponse.Sucesso = false;
                    serviceResponse.Mensagem = "Informe um nome para buscar.";
                    return serviceResponse;
                }

                //Já o EndsWith() seria LIKE '%ami'.
                //Contains(nomeParcial) é o equivalente a LIKE '%Nome%'
                //StartsWith e´o equivalente a LIKE 'Nome%'
                var usuarios = await _context.Users
                    .Where(u => u.Nome.StartsWith(nomeParcial))
                    .ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<UsuarioDto>>(usuarios);
                serviceResponse.Sucesso = true;
                serviceResponse.Mensagem = "Usuários encontrados com sucesso!";
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UsuarioLogadoDto>> Login(LoginDto loginDto)
        {
            var response = new ServiceResponse<UsuarioLogadoDto>();

            var usuario = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

            if (usuario == null || !CriptografiaHash.VerificarSenha(loginDto.Senha, usuario.SenhaHash))
            {
                response.Sucesso = false;
                response.Mensagem = "Usuário ou senha inválidos!";
                return response;
            }

            var usuarioLogado = new UsuarioLogadoDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                TipoUsuario = usuario.TipoUsuario
            };

            response.Sucesso = true;
            response.Mensagem = "Login realizado com sucesso!";
            response.Dados = usuarioLogado;

            return response;
        }
    }
}
