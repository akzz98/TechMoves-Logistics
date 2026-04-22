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
        private readonly string _testRoot;

        public FileServiceTests()
        {
            // Mock the web hosting environment so that file paths resolve correctly during testing
            _mockEnvironment = new Mock<IWebHostEnvironment>();
            _testRoot = Path.Combine(Directory.GetCurrentDirectory(), "TestUploads");

            // Simulate application root paths used by FileService
            _mockEnvironment.Setup(e => e.WebRootPath).Returns(_testRoot);
            _mockEnvironment.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());

            // Inject mocked environment into the service
            _service = new FileService(_mockEnvironment.Object);
        }

        // Helper method to reduce repetition when creating mock file uploads
        private IFormFile CreateMockFile(string fileName, string contentType, int size = 10)
        {
            var content = new MemoryStream(Encoding.UTF8.GetBytes(new string('A', size)));

            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.ContentType).Returns(contentType);
            fileMock.Setup(f => f.Length).Returns(content.Length);
            fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                    .Returns((Stream target, CancellationToken token) => content.CopyToAsync(target));

            return fileMock.Object;
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

        [Fact]
        public async Task SavePdf_ValidPdf_ShouldSucceed()
        {
            // Arrange:
            // Simulate a valid PDF file upload with the correct extension and MIME type
            var file = CreateMockFile("contract.pdf", "application/pdf");

            // Act:
            // Attempt to save the valid PDF through the service
            var path = await _service.SavePdfAsync(file);

            // Assert:
            // Confirm the returned path is not null and follows the expected format
            Assert.NotNull(path);
            Assert.StartsWith("/uploads/", path);
        }

        [Fact]
        public void IsValidPdf_ValidPdf_ShouldReturnTrue()
        {
            // Arrange:
            // Create a mock file that meets all PDF validation requirements
            var file = CreateMockFile("contract.pdf", "application/pdf");

            // Act:
            // Run the file through the validation method
            var result = _service.IsValidPdf(file);

            // Assert:
            // A valid PDF with correct extension and MIME type must return true
            Assert.True(result);
        }

        [Fact]
        public void IsValidPdf_WrongMimeType_ShouldReturnFalse()
        {
            // Arrange:
            // Simulate a file that has a .pdf extension but an incorrect MIME type
            // This tests that the service validates MIME type independently of the extension
            var file = CreateMockFile("contract.pdf", "text/plain");

            // Act:
            // Run the file through the validation method
            var result = _service.IsValidPdf(file);

            // Assert:
            // A mismatched MIME type must cause validation to fail even with a valid extension
            Assert.False(result);
        }

        [Fact]
        public void IsValidPdf_NullOrEmptyFile_ShouldReturnFalse()
        {
            // Arrange:
            // Simulate a file upload where no file content was provided (0 bytes)
            // This tests the null/empty guard at the top of IsValidPdf
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.Length).Returns(0);

            // Act:
            // Run the empty file through the validation method
            var result = _service.IsValidPdf(fileMock.Object);

            // Assert:
            // An empty file must be rejected without throwing an exception
            Assert.False(result);
        }

        [Fact]
        public async Task SavePdfAsync_GeneratesUniqueFilename_WithGuid()
        {
            // Arrange:
            // Use the same source file for two separate upload calls
            // This verifies that each save operation generates a unique GUID-based filename
            var file = CreateMockFile("contract.pdf", "application/pdf");

            // Act:
            // Save the same file twice and compare the resulting paths
            var path1 = await _service.SavePdfAsync(file);
            var path2 = await _service.SavePdfAsync(file);

            // Assert:
            // Both paths must be different, confirming GUID uniqueness prevents overwrites
            Assert.NotEqual(path1, path2);
        }

        [Fact]
        public async Task SavePdfAsync_ReturnsRelativePathForDatabase()
        {
            // Arrange:
            // Simulate a valid PDF upload to verify the format of the returned path
            var file = CreateMockFile("contract.pdf", "application/pdf");

            // Act:
            // Save the file and capture the returned relative path
            var path = await _service.SavePdfAsync(file);

            // Assert:
            // The returned path must be relative (not absolute) and end with .pdf
            // This ensures the path is safe to store in the database
            Assert.StartsWith("/uploads/", path);
            Assert.EndsWith(".pdf", path);
        }

        [Fact]
        public async Task DeleteFile_WithValidPath_RemovesFile()
        {
            // Arrange:
            // First save a real file to disk so we have something to delete
            var file = CreateMockFile("contract.pdf", "application/pdf");
            var path = await _service.SavePdfAsync(file);
            var fullPath = Path.Combine(_testRoot, path.TrimStart('/'));

            // Confirm the file was actually created before attempting deletion
            Assert.True(File.Exists(fullPath));

            // Act:
            // Delete the file using the relative path returned by SavePdfAsync
            _service.DeleteFile(path);

            // Assert:
            // The file must no longer exist on disk after deletion
            Assert.False(File.Exists(fullPath));
        }

        [Fact]
        public void DeleteFile_WithNullOrInvalidPath_HandlesGracefully()
        {
            // Arrange:
            // Simulate a scenario where a null path is passed to DeleteFile
            // This tests the null guard at the top of the method

            // Act & Assert:
            // DeleteFile must handle null input silently without throwing any exception
            var exception = Record.Exception(() => _service.DeleteFile(null));
            Assert.Null(exception);
        }
    }
}