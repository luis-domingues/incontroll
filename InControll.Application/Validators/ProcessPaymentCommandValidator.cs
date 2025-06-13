using FluentValidation;

namespace InControll.Application.Validators;

public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
{
    public ProcessPaymentCommandValidator()
    {
        RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than 0.");
        RuleFor(x => x.Currency).NotEmpty().WithMessage("Currency is required.").Length(3).WithMessage("Currency must be 3 characters (e.g., 'BRL').");
        RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Payment method is required.");
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("CustomerId is required.");
        RuleFor(x => x.OrderId).NotEmpty().WithMessage("OrderId is required.");

        When(x => x.PaymentMethod.Equals("CreditCard", StringComparison.OrdinalIgnoreCase), () =>
        {
            RuleFor(x => x.CardDetails).NotNull().WithMessage("Card details are required for credit card payments.");
            RuleFor(x => x.CardDetails!.CardNumber).NotEmpty().WithMessage("Card number is required.").CreditCard().WithMessage("Invalid credit card number.");
            RuleFor(x => x.CardDetails!.CardHolderName).NotEmpty().WithMessage("Card holder name is required.");
            RuleFor(x => x.CardDetails!.ExpirationMonth).NotEmpty().WithMessage("Expiration month is required.").Length(2).WithMessage("Expiration month must be 2 characters.").Must(BeAValidMonth).WithMessage("Invalid expiration month.");
            RuleFor(x => x.CardDetails!.ExpirationYear).NotEmpty().WithMessage("Expiration year is required.").Length(4).WithMessage("Expiration year must be 4 characters.").Must(BeAValidYear).WithMessage("Invalid expiration year.");
            RuleFor(x => x.CardDetails!.SecurityCode).NotEmpty().WithMessage("Security code is required.").Length(3,4).WithMessage("Security code must be 3 or 4 characters.");
        });
    }

    private bool BeAValidMonth(string month)
    {
        if(int.TryParse(month, out int m)) return m >= 1 && m <= 12;
        return false;
    }

    private bool BeAValidYear(string year)
    {
        if(int.TryParse(year, out int y)) return y >= DateTime.UtcNow.Year && y <= DateTime.Now.Year + 10;
        return false;
    }
}