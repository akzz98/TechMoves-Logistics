using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TechMoves_Logistics.Services;

namespace TechMovesLogistics.Tests
{
    public class FileServiceTests
    {
        private readonly FileService _service;
        private readonly Mock<IWebHostEnvironment> _mockEnvironment;

        public FileServiceTests()
        {
            // Mock the web hosting environment so that file paths
            _mockEnvironment = new Mock<IWebHostEnvironment>();

            // Simulate application root paths used by FileService
            _mockEnvironment.Setup(e => e.WebRootPath).Returns("wwwroot");
            _mockEnvironment.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            // Inject mocked environment into the service
            _service = new FileService(_mockEnvironment.Object);
        }

        [Fact]
        public async Task SavePdf_InvalidExtension_ShouldThrowException()
        {
            // Arrange:
            // Simulate an invalid file upload scenario where a non-PDF file is provided.
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("host.exe");
            fileMock.Setup(f => f.Length).Returns(100);

            // Act & Assert:
            // Ensure that invalid file types are rejected by the service
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.SavePdfAsync(fileMock.Object));

            // Validate that the error message is explicit and user-friendly
            Assert.Equal("Only PDF files are allowed.", exception.Message);
        }
    }
}
