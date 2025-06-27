using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Redatech.DataContext;
using Redatech.Dto;
using Redatech.Enums;
using Redatech.Estaticos.Login;
using Redatech.Models;
using System.Security.Claims;

namespace Redatech.Service.UsuarioService
{
    public class UsuarioService : IUsuarioInterface
    {

        //Variável que armazenará a instância do ApplicationDbContext
        //readonly significa que o valor dessa variável só pode ser atribuído no construtor e não pode ser alterado depois
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //Construtor da classe
        //Quando UsuarioService for criado, o contexto do banco será passado via injeção de dependência.
        //_context = context; armazena essa instância para ser usada nos métodos do serviço.
        public UsuarioService(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper; // Agora o AutoMapper pode ser usado no Service
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> CreateUsuario(UsuarioDto novoUsuarioDto)
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
                UsuarioModel novoUsuario = _mapper.Map<UsuarioModel>(novoUsuarioDto);

                if (ValidaCpf(novoUsuario) != false)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "CPF já cadastrado.";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                if (ValidaEmail(novoUsuario) != false)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "E-mail já cadastrado.";
                    serviceResponse.Sucesso = false;
                    return serviceResponse;
                }

                novoUsuario.Status = true;

                novoUsuario.SenhaHash = CriptografiaHash.GerarHash(novoUsuarioDto.SenhaHash);

                // Adiciona ao banco de dados
                _context.Usuarios.Add(novoUsuario);
                await _context.SaveChangesAsync();

                // Recupera todos os usuários já convertidos para DTO
                List<UsuarioModel> usuarios = _context.Usuarios.ToList();
                serviceResponse.Dados = _mapper.Map<List<UsuarioDto>>(usuarios);

                serviceResponse.Mensagem = "Usuário criado com sucesso!";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                var mensagemErro = ex.InnerException?.Message ?? ex.Message;
                serviceResponse.Mensagem = $"Erro ao salvar no banco: {mensagemErro}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> DeleteUsuario(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                UsuarioModel usuario = _context.Usuarios.FirstOrDefault(x => x.Id == id);

                if (usuario == null)
                {
                    serviceResponse.Dados = null;
                    serviceResponse.Mensagem = "Usuário não localizado";
                    serviceResponse.Sucesso = false;

                    return serviceResponse;
                }

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                List<UsuarioModel> usuarios = await _context.Usuarios.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<UsuarioDto>>(usuarios);

            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UsuarioDto>> GetUsuarioById(int id)
        {
            ServiceResponse<UsuarioDto> serviceResponse = new ServiceResponse<UsuarioDto>();

            try
            {
                // Realiza a busca do usuário no banco
                UsuarioModel usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);

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

        public async Task<ServiceResponse<List<UsuarioDto>>> GetUsuarios()
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                // Obtém todos os usuários do banco
                List<UsuarioModel> usuarios = await _context.Usuarios.ToListAsync();

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
        public async Task<ServiceResponse<List<UsuarioDto>>> InativaUsuario(int id)
        {
            ServiceResponse<List<UsuarioDto>> serviceResponse = new ServiceResponse<List<UsuarioDto>>();

            try
            {
                UsuarioModel usuario = _context.Usuarios.FirstOrDefault(x => x.Id == id);

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
                _context.Usuarios.Update(usuario);

                /* Salvar a operação realizada */
                await _context.SaveChangesAsync();

                List<UsuarioModel> usuarios = await _context.Usuarios.ToListAsync();
                serviceResponse.Dados = _mapper.Map<List<UsuarioDto>>(usuarios);
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UsuarioDto>> UpdateUsuario(UsuarioDto editadoUsuarioDto)
        {
            ServiceResponse<UsuarioDto> serviceResponse = new ServiceResponse<UsuarioDto>();

            try
            {
                // Busca o usuário no banco, mas sem rastreamento para evitar conflitos
                UsuarioModel usuarioExistente = await _context.Usuarios
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
                UsuarioModel usuarioAtualizado = _mapper.Map<UsuarioModel>(editadoUsuarioDto);

                usuarioAtualizado.SenhaHash = CriptografiaHash.GerarHash(editadoUsuarioDto.SenhaHash);

                // Atualiza o objeto no contexto
                _context.Usuarios.Update(usuarioAtualizado);

                // Salva as mudanças no banco
                await _context.SaveChangesAsync();

                serviceResponse.Dados = _mapper.Map<UsuarioDto>(usuarioAtualizado);
                serviceResponse.Mensagem = "Usuário atualizado com sucesso!";
                serviceResponse.Sucesso = true;
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<List<UsuarioDto>>> GetUsuariosByName(string nomeParcial)
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
                var usuarios = await _context.Usuarios
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

        public async Task<ServiceResponse<List<UsuarioDto>>> GetAlunosByName(string nomeParcial)
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

                var usuarios = await _context.Usuarios
                    .Where(u => u.Nome.StartsWith(nomeParcial) && u.TipoUsuario == TipoUsuario.Aluno)
                    .ToListAsync();

                serviceResponse.Dados = _mapper.Map<List<UsuarioDto>>(usuarios);
                serviceResponse.Sucesso = true;
                serviceResponse.Mensagem = "Usuários do tipo Aluno encontrados com sucesso!";
            }
            catch (Exception ex)
            {
                serviceResponse.Sucesso = false;
                serviceResponse.Mensagem = $"Erro: {ex.Message}";
            }

            return serviceResponse;
        }

        public async Task<ServiceResponse<UsuarioDto>> InativarAluno(int id)
        {
            ServiceResponse<UsuarioDto> serviceResponse = new ServiceResponse<UsuarioDto>();

            UsuarioModel usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);
            
            if (usuario == null)
            {
                serviceResponse.Dados = null;
                serviceResponse.Mensagem = "Usuário não encontrado.";
                serviceResponse.Sucesso = false;
                return serviceResponse;
            }
            usuario.Status = false; // Desativa o usuário

            _context.Usuarios.Update(usuario); // Atualiza o usuário no contexto
            await _context.SaveChangesAsync();

            return serviceResponse;
        }

        public async Task<ServiceResponse<UsuarioDto>> AtivarAluno(int id)
        {
            ServiceResponse<UsuarioDto> serviceResponse = new ServiceResponse<UsuarioDto>();

            UsuarioModel usuario = await _context.Usuarios.FirstOrDefaultAsync(x => x.Id == id);

            if (usuario == null)
            {
                serviceResponse.Dados = null;
                serviceResponse.Mensagem = "Usuário não encontrado.";
                serviceResponse.Sucesso = false;
                return serviceResponse;
            }

            usuario.Status = true;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            serviceResponse.Dados = _mapper.Map<UsuarioDto>(usuario);
            serviceResponse.Sucesso = true;
            serviceResponse.Mensagem = "Status atualizado com sucesso!";

            return serviceResponse;
        }

        private bool ValidaCpf(UsuarioModel novoUsuarioModel)
        {
            // Implementar a lógica de validação de CPF aqui, se necessário.
            // Por enquanto, não há implementação específica para validação de CPF.

            var usuarioEncontradoNoBanco = _context.Usuarios.FirstOrDefault(x => x.Cpf == novoUsuarioModel.Cpf);

            if(usuarioEncontradoNoBanco != null)
            {
                return true;
            }

            return false;
        }

        private bool ValidaEmail(UsuarioModel novoUsuarioModel)
        {
            var usuarioEncontradoNoBanco = _context.Usuarios.FirstOrDefault(x => x.Email == novoUsuarioModel.Email);
            if (usuarioEncontradoNoBanco != null)
            {
                return true;
            }
            return false;
        }

        public async Task<ServiceResponse<int>> ObterIdDoUsuarioLogado()
        {
            var response = new ServiceResponse<int>();

            try
            {
                var userId = int.Parse(_httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
                response.Dados = userId;
            }
            catch (Exception)
            {
                response.Sucesso = false;
                response.Mensagem = "Não foi possível obter o ID do usuário logado.";
            }

            return response;
        }
    }
}
