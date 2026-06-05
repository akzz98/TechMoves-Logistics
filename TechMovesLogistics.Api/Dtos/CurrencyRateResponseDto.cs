namespace TechMovesLogistics.Api.Dtos
{
    public class CurrencyRateResponseDto
    {
        public string FromCurrency { get; set; } = "USD";
        public string ToCurrency { get; set; } = "ZAR";
        public decimal Rate { get; set; }
    }
}
