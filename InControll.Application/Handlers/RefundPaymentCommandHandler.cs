using InControll.Application.DTOs;
using InControll.Domain;
using InControll.Domain.Entities;
using MediatR;

namespace InControll.Application.Handlers;

public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, PaymentResponse>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly ITransactionLogRepository _transactionLogRepository;
    //private readonly IPaymentGatewayService _paymentGatewayService;

    public RefundPaymentCommandHandler(IPaymentRepository paymentRepository, ITransactionLogRepository transactionLogRepository /*IPaymentGatewayService paymentGatewayService*/)
    {
        _paymentRepository = paymentRepository;
        _transactionLogRepository = transactionLogRepository;
        // _paymentGatewayService = paymentGatewayService;
    }
    
    public async Task<PaymentResponse> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await _paymentRepository.GetByIdAsync(request.PaymentId);
        if(payment == null) throw new ApplicationException($"Payment with ID {request.PaymentId} not found.");
        
        if(payment.Status == PaymentStatus.Refunded || payment.Status == PaymentStatus.Cancelled || payment.Status == PaymentStatus.Denied) 
            throw new ApplicationException($"Payment with ID {request.PaymentId} cannot be refunded in status {payment.Status}.");
        
        decimal refundAmount = request.Amount ?? payment.Amount;
        var totalCaptured = payment.Transactions.Where(t => t.Type == TransactionType.Capture || t.Status == TransactionStatus.Success).Sum(t => t.Amount);
        var totalRefunded = payment.Transactions.Where(t => t.Type == TransactionType.Refund && t.Status == TransactionStatus.Success).Sum(t => t.Amount);
        
        if(refundAmount <=0) throw new ArgumentException("Refund amount must be greater than or equal to zero.");
        if(refundAmount > (totalCaptured - totalRefunded)) throw new ArgumentException($"Refund amount {refundAmount} exceeds available amount for refund. Remaining: {(totalCaptured - totalRefunded)}");
        
        //simular chamado ao gateway de pagamento para estorno
        string simulatedGetwayTransactionId = $"gtw_ref_{Guid.NewGuid().ToString().Substring(0, 8)}";
        string simulatedGatewayResponse = "{ \"status\": \"refund_success\", \"message\": \"Refund processed by gateway\" }";
        TransactionStatus transactionStatus = TransactionStatus.Success;

        var refundTransaction = new Transaction(
            payment.Id,
            refundAmount,
            TransactionType.Refund,
            transactionStatus,
            simulatedGetwayTransactionId,
            simulatedGatewayResponse,
            request.Reason
        );
        
        payment.AddTransaction(refundTransaction);
        await _paymentRepository.UpdateAsync(payment);
        await _transactionLogRepository.AddAsync(refundTransaction);

        return new PaymentResponse
        {
            PaymentId = payment.Id,
            Amount = payment.Amount,
            Status = payment.Status,
            CreatedAt = payment.CreatedAt,
            Message = $"Payment refunded successfully. New status: {payment.Status}"
        };
    }
}