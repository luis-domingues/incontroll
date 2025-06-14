using InControll.Application;
using InControll.Application.DTOs;
using InControll.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InControll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(PaymentResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentCommand command)
    {
        var response = await _mediator.Send(command);
        return CreatedAtAction(nameof(ProcessPayment), new {id = response.PaymentId}, response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(PaymentResponse), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> GetPaymentById(Guid paymentId)
    {
        var query = new GetPaymentByIdQuery(paymentId);
        var response = await _mediator.Send(query);
        if (response == null) return NotFound();
        return Ok(response);
    }

    [HttpPost("{paymentId:guid}/refund")]
    [ProducesResponseType(typeof(PaymentResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    [ProducesResponseType(409)]
    [ProducesResponseType(500)]
    public async Task<IActionResult> RefundPayment(Guid paymentId, [FromBody] RefundPaymentCommand command)
    {
        command.PaymentId = paymentId;
        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (ApplicationException ex)
        {
            if(ex.Message.Contains("not found")) return NotFound(new {message = ex.Message});
            return BadRequest(new {message = ex.Message});
        }
    }
}