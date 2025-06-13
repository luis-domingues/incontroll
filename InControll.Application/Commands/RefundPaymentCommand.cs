using InControll.Application.DTOs;
using MediatR;

namespace InControll.Application;

public class RefundPaymentCommand : IRequest<PaymentResponse>
{
    public Guid PaymentId { get; set; }
    public decimal? Amount { get; set; }
    public string Reason { get; set; } = string.Empty;
}