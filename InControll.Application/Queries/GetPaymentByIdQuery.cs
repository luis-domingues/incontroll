using InControll.Application.DTOs;
using MediatR;

namespace InControll.Application.Queries;

public class GetPaymentByIdQuery : IRequest<PaymentResponse?>
{
    public Guid PaymentId { get; set; }

    public GetPaymentByIdQuery(Guid paymentId)
    {
        PaymentId = paymentId;
    }
}