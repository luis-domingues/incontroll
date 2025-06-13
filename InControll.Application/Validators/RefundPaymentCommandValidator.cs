using FluentValidation;

namespace InControll.Application.Validators;

public class RefundPaymentCommandValidator : AbstractValidator<RefundPaymentCommand>
{
    public RefundPaymentCommandValidator()
    {
        RuleFor(x => x.PaymentId).NotEmpty().WithMessage("Payment ID is required");
        When(x => x.Amount.HasValue, () =>
        {
            RuleFor(x => x.Amount).GreaterThanOrEqualTo(0).WithMessage("Refund amount must be greater than 0.");
        });
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Refund reason is required").MaximumLength(500).WithMessage("Reason can't exceed 500 characters.");
    }
}