using Microsoft.AspNetCore.Mvc;
using Redatech.Dto;
using Redatech.Models;
using Redatech.Service.CorrecaoService;

namespace Redatech.Controllers
{
    //Criando uma rota base
    [Route("api/[controller]")]
    [ApiController]
    public class CorrecaoController : ControllerBase
    {
        private readonly ICorrecaoInterface _correcaoInterface;

        public CorrecaoController(ICorrecaoInterface correcaoInterface)
        {
            _correcaoInterface = correcaoInterface;
        }

        /// <summary>
        /// Listando todas as correções
        /// </summary>
        /// <param name="">Nenhum parametro é necessário</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<CorrecaoDto>>>> GetCorrections()
        {
            var response = await _correcaoInterface.GetCorrections();
            return Ok(response);
        }

        /// <summary>
        /// Listando a redação pelo Id
        /// </summary>
        /// <param name="id">Passa o id da correção</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<CorrecaoDto>>> GetCorrectionById(int id)
        {
            var response = await _correcaoInterface.GetCorrectionById(id);
            return Ok(response);
        }

        /// <summary>
        /// Criando uma nova correção
        /// </summary>
        /// <param name="novaCorrecao">Passa os dados da nova correção</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<CorrecaoDto>>>> CreateCorrection(CorrecaoDto novaCorrecao)
        {
            return Ok(await _correcaoInterface.CreateCorrection(novaCorrecao));
        }

        /// <summary>
        /// Atualiza uma correção
        /// </summary>
        /// <param name="editadoCorrecao">Passa os dados atualizados da correção escolhida</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<CorrecaoDto>>>> UpdateCorrection(CorrecaoDto editadoCorrecao)
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = await _correcaoInterface.UpdateCorrection(editadoCorrecao);
            return Ok(serviceResponse);
        }

        /// <summary>
        /// Deletando uma correção
        /// </summary>
        /// <param name="id">Passa o id da correção a ser deletada</param>
        /// <returns>Mensagem de sucesso ou erro.</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<CorrecaoDto>>>> DeleteCorrection(int id)
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = await _correcaoInterface.DeleteCorrection(id);
            return Ok(serviceResponse);
        }
    }
}
