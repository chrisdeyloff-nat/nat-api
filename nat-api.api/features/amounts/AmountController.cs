using MediatR;
using Microsoft.AspNetCore.Mvc;
using nat_api.api.features.amounts.command;
using nat_api.api.features.amounts.query;
using System.Threading.Tasks;

namespace serc_api.api.features.amount
{
    [ApiController]
    [Route("amount")]
    public class AmountController : Controller
    {
        private readonly IMediator mediator;
        public AmountController(IMediator mediator) => this.mediator = mediator;

        [HttpPost("create")]
        public async Task<IActionResult> Create(Create.Command input)
        {
            var result = await this.mediator.Send(input);
            return result.ResultOrError(this);
        }

        [HttpPut("edit")]
        public async Task<IActionResult> Update([FromBody] Edit.Command input)
        {
            var result = await this.mediator.Send(input);
            return result.ResultOrError(this);
        }

        [HttpGet("get-by-id")]
        public async Task<IActionResult> GetById([FromQuery] GetById.Query input)
        {
            var result = await mediator.Send(input);
            return result.ResultOrError(this);
        }

        [HttpGet("get-list")]
        public async Task<IActionResult> GetList([FromQuery] GetList.Query input)
        {
            var result = await mediator.Send(input);
            return result.ResultOrError(this);
        }
    }
}