using InControll.Application.DTOs;
using InControll.Domain;
using InControll.Domain.Entities;
using MediatR;

namespace InControll.Application.Handlers;

public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITransactionLogRepository _transactionLogRepository;
    /*IPaymentGatewayService _paymentGatewayService*/

    //payment gateway service (simulated for now)
    public ProcessPaymentCommandHandler(IPaymentRepository paymentRepository, ITransactionLogRepository transactionLogRepository /*IPaymentGatewayService paymentGatewayService*/)
    {
        _paymentRepository = paymentRepository;
        _transactionLogRepository = transactionLogRepository;
        // _paymentGatewayService = paymentGatewayService;
    }

    public async Task<PaymentResponse> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = new Payment(
            request.Amount,
            request.Currency,
            request.PaymentMethod,
            request.CustomerId,
            request.OrderId
        );
        
        // simular chamada a um gateway de pagamento externo
        string simulateGatewayTransactionId = $"gtw_txn_{Guid.NewGuid().ToString().Substring(0, 8)}";
        string simulatedGatewayResponse = "{ \"status\": \"success\", \"message\": \"Payment processed successfully by gateway\" }";
        TransactionStatus transactionStatus = TransactionStatus.Success;
        PaymentStatus newPaymentStatus = PaymentStatus.Approved;

        var transaction = new Transaction(
            payment.Id,
            request.Amount,
            TransactionType.Authorization,
            transactionStatus,
            simulateGatewayTransactionId,
            simulatedGatewayResponse,
            "Initial payment authorization"
        );
        
        //adicionar transação ao pagamento (lógica do domínio que atualiza o status do Payment)
        payment.AddTransaction(transaction);
        payment.UpdateStatus(newPaymentStatus); //força o status baseado na simulação do gateway
        
        await _paymentRepository.AddAsync(payment);
        await _transactionLogRepository.AddAsync(transaction);

        return new PaymentResponse
        {
            PaymentId = payment.Id,
            Amount = payment.Amount,
            Status = payment.Status,
            CreatedAt = payment.CreatedAt,
            Message = "Payment processing initiated successfully."
        };
    }
}