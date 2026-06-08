namespace TechMoves_Logistics.Services
{
    public interface ICurrencyApiClient
    {
        // GET /api/currency/usd-zar
        Task<decimal> GetUsdToZarRateAsync();

        decimal ConvertUsdToZar(decimal usdAmount, decimal rate);
    }
}
