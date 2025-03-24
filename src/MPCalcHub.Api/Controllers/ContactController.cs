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
        /// Excluir um contato pelo Id
        /// </summary>
        /// <param name="id">Id do contato</param>
        /// <returns>Contato excluído com sucesso</returns>
        [HttpDelete, Route("{id}")]
        [Authorize(Policy = Policies.SuperOrModerator)]
        [Produces("application/json")]
        [ProducesResponseType(typeof(Contact), StatusCodes.Status200OK)]
        public async Task<object> Delete([FromRoute] Guid id)
        {
            try
            {
                await _contactApplicationService.Remove(id);
                return Accepted();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
