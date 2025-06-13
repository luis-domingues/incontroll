using InControll.Application.DTOs;
using MediatR;

namespace InControll.Application;

public class ProcessPaymentCommand : IRequest<PaymentResponse>
{
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string OrderId { get; set; } = string.Empty;
    public CardDetailsDto? CardDetails { get; set; }
}
