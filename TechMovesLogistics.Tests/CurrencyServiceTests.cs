using Microsoft.Extensions.Configuration;
using Moq;
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
        [Fact]
        public async Task GetUsdToZarRate_WhenApiIsDown_ShouldReturnFallbackRate()
        {
            // Arrange:
            // Simulate a failing external API using a custom HttpMessageHandler.
            var handler = new HttpMessageHandler_AlwaysFail();
            var httpClient = new HttpClient(handler);

            // Mock configuration values used by the service.
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c[It.Is<string>(s => s == "CurrencyApi:BaseUrl")])
                      .Returns("https://invalid-url-that-will-fail.com");

            // Inject mocked dependencies into service.
            var service = new CurrencyService(httpClient, mockConfig.Object);

            // Act:
            // Attempt to retrieve exchange rate while API is unavailable.
            var rate = await service.GetUsdToZarRateAsync();

            // Assert:
            // Ensure system gracefully falls back to a default safe rate.
            Assert.Equal(18.50m, rate);
        }
        [Fact]
        public void ConvertUsdToZar_WithZeroRate_ShouldThrowException()
        {
            // Arrange:
            // Exchange rates must be positive values; zero is invalid in financial conversion logic.
            decimal usd = 100m;
            decimal rate = 0m;

            // Act & Assert:
            // Ensure the service enforces business rules and rejects invalid exchange rates.
            Assert.Throws<ArgumentException>(() =>
                _service.ConvertUsdToZar(usd, rate));
        }
        [Fact]
        public void ConvertUsdToZar_WithNegativeRate_ShouldThrowException()
        {
            // Arrange:
            // Negative exchange rates are logically invalid and should be rejected.
            decimal usd = 100m;
            decimal rate = -5m;

            // Act & Assert:
            // Validate that invalid financial input is not processed silently.
            Assert.Throws<ArgumentException>(() =>
                _service.ConvertUsdToZar(usd, rate));
        }
        [Fact]
        public void ConvertUsdToZar_RoundingDecimalsCorrectly_ShouldReturnTwoDecimalPlaces()
        {
            // Arrange:
            // Test ensures financial rounding rules are correctly applied.
            decimal usd = 1.234m;
            decimal rate = 18.50m;
            decimal expected = 22.83m; // expected rounded result (not truncated)

            // Act:
            var result = _service.ConvertUsdToZar(usd, rate);

            // Assert:
            // Ensure correct financial rounding to 2 decimal places.
            Assert.Equal(expected, result);
        }
        [Fact]
        public async Task GetUsdToZarRateAsync_WhenApiReturnsValidResponse_ShouldReturnParsedRate()
        {
            // Arrange:
            // Simulate a successful external API response with valid JSON payload.
            var fakeJson = """
        {
            "result": "success",
            "conversion_rates": {
                "ZAR": 18.50
            }
        }
        """;

            var handler = new HttpMessageHandler_Success(fakeJson);
            var httpClient = new HttpClient(handler);

            // Mock configuration for API base URL (used inside service)
            var mockConfig = new Mock<IConfiguration>();
            mockConfig.Setup(c => c["CurrencyApi:BaseUrl"])
                      .Returns("https://fake-api.com/latest/USD");

            var service = new CurrencyService(httpClient, mockConfig.Object);

            // Act:
            // Call external API method (mocked) to retrieve exchange rate.
            var rate = await service.GetUsdToZarRateAsync();

            // Assert:
            // Validate correct parsing of API response.
            Assert.Equal(18.50m, rate);
        }

        // Helper: simulates a network failure
        public class HttpMessageHandler_AlwaysFail : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                throw new HttpRequestException("Simulated network failure.");
            }
        }
        // Helper: simulates a successful HTTP response
        public class HttpMessageHandler_Success : HttpMessageHandler
        {
            private readonly string _responseJson;

            public HttpMessageHandler_Success(string responseJson)
            {
                _responseJson = responseJson;
            }

            protected override Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
                {
                    Content = new StringContent(_responseJson, System.Text.Encoding.UTF8, "application/json")
                };
                return Task.FromResult(response);
            }

        }
    }
}

