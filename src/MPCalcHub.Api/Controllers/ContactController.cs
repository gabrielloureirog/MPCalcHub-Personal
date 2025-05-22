using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MPCalcHub.Application.DataTransferObjects;
using MPCalcHub.Application.Interfaces;
using static MPCalcHub.Domain.Constants.AppConstants;

namespace MPCalcHub.Api.Controllers
{
    /// <summary>
    /// Contact controller
    /// </summary>
    [Route("contacts")]
    public class ContactController(ILogger<ContactController> logger, IContactApplicationService contactApplicationService) : BaseController(logger)
    {
        private readonly IContactApplicationService _contactApplicationService = contactApplicationService;

        /// <summary>
        /// Criar um novo contato
        /// </summary>
        /// <param name="model">Objeto com as propriedades para criar um novo contato</param>
        /// <returns>Um objeto do contato criado</returns>
        [HttpPost]
        [Authorize(Policy = Policies.SuperOrModerator)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<object> Create([FromBody] BasicContact model)
        {
            try
            {
                var contact = await _contactApplicationService.Add(model);
                return Ok(contact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Buscar contatos por DDD
        /// </summary>
        /// <param name="ddd">DDD para busca de contatos</param>
        /// <returns>Um objeto do contato criado</returns>
        [HttpGet("find/{ddd}")]
        [Authorize(Policy = Policies.SuperOrGuest)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<object> FindByDDD([FromRoute] int ddd)
        {
            try
            {
                var contacts = await _contactApplicationService.FindByDDD(ddd);
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Editar um contato
        /// </summary>
        /// <param name="model">Objeto com as propriedades para editar um contato</param>
        /// <returns>Um objeto do contato criado</returns>
        [HttpPut]
        [Authorize(Policy = Policies.SuperOrModerator)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<object> Update([FromBody] Contact model)
        {
            try
            {
                var contact = await _contactApplicationService.Update(model);
                return Ok(contact);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
