using System;
using System.Collections.Generic;
using System.Text;
using TechMoves_Logistics.Services;

namespace TechMovesLogistics.Tests
{
    public class CurrencyServiceTests
    {
        private readonly CurrencyService _service;
        public CurrencyServiceTests()
        {
            // Use null for HttpClient because we are not testing the network
            _service = new CurrencyService(null, null);
        }
        [Fact]
        public void ConvertUsdToZar_ShouldReturnCorrectCalculation()
        {
            //Arrange:
            //Provide a known USD amount and exchange rate`
            decimal usd = 100m;
            decimal rate = 18.50m;
            decimal expected = 1850.00m;

            //Act:
            //Perform the conversion using the service method
            var result = _service.ConvertUsdToZar(usd, rate);

            //Assert:
            //Verify that the result matches the expected value
            Assert.Equal(expected, result);
        }
        [Fact]
        public void ConvertUsdToZar_NegativeAmount_ShouldThrowException()
        {
            //Arrange:
            //Provide a negative USD amount and a valid exchange rate
            decimal usd = -10m;
            decimal rate = 18.50m;

            //Act & Assert:
            //Verify that calling the conversion method with a negative amount throws an ArgumentException
            Assert.Throws<ArgumentException>(() => _service.ConvertUsdToZar(usd, rate));
        }
    }
}
