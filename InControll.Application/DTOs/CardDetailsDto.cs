namespace InControll.Application.DTOs;

public class CardDetailsDto
{
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string ExpirationMonth { get; set; } = string.Empty;
    public string ExpirationYear { get; set; } = string.Empty;
    public string SecurityCode { get; set; } = string.Empty;
}