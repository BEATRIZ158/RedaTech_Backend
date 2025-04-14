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

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<CorrecaoDto>>>> GetCorrecoes()
        {
            var response = await _correcaoInterface.GetCorrecoes();
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceResponse<CorrecaoDto>>> GetCorrecaoById(int id)
        {
            var response = await _correcaoInterface.GetCorrecaoById(id);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<CorrecaoDto>>>> CreateCorrecao(CorrecaoDto novaCorrecao)
        {
            return Ok(await _correcaoInterface.CreateCorrecao(novaCorrecao));
        }

        [HttpPut]
        public async Task<ActionResult<ServiceResponse<List<CorrecaoDto>>>> UpdateCorrecao(CorrecaoDto editadoCorrecao)
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = await _correcaoInterface.UpdateCorrecao(editadoCorrecao);
            return Ok(serviceResponse);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ServiceResponse<List<CorrecaoDto>>>> DeleteCorrecao(int id)
        {
            ServiceResponse<List<CorrecaoDto>> serviceResponse = await _correcaoInterface.DeleteCorrecao(id);
            return Ok(serviceResponse);
        }
    }
}
