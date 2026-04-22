using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TechMoveLogistics.Repositories.Interfaces;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;
using TechMoves_Logistics.Repositories.Interfaces;
using TechMoves_Logistics.Services;
using Xunit;

namespace TechMovesLogistics.Tests
{
    public class ServiceRequestServiceTests
    {
        private readonly Mock<IServiceRequestRepository> _mockRepo;
        private readonly Mock<IContractRepository> _mockContractRepo;
        private readonly ServiceRequestService _service;

        public ServiceRequestServiceTests()
        {
            // Create mock repositories to isolate the service layer from the database.
            _mockRepo = new Mock<IServiceRequestRepository>();
            _mockContractRepo = new Mock<IContractRepository>();

            // Inject mocked dependencies into the service.
            _service = new ServiceRequestService(_mockRepo.Object, _mockContractRepo.Object);
        }

        [Fact]
        public async Task CreateRequest_ForExpiredContract_ShouldThrowException()
        {
            // Arrange: 
            // Setup a mock contract that is marked as Expired.
            var contract = new Contract { Id = 1, Status = ContractStatus.Expired };
            _mockContractRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);

            var request = new ServiceRequest { ContractId = 1 };

            // Act & Assert:
            // Ensure the service blocks creation and provides the correct error message.
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateServiceRequestAsync(request));

            Assert.Equal("Cannot create a Service Request for an Expired contract.", exception.Message);
        }

        [Fact]
        public async Task CreateRequest_ForOnHoldContract_ShouldThrowException()
        {
            // Arrange: 
            // Simulate a contract that is currently On Hold.
            var contract = new Contract { Id = 2, Status = ContractStatus.OnHold };
            _mockContractRepo.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(contract);

            var request = new ServiceRequest { ContractId = 2 };

            // Act & Assert:
            // Verify that On Hold status specifically blocks the workflow.
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateServiceRequestAsync(request));

            Assert.Equal("Cannot create a Service Request for a contract that is On Hold.", exception.Message);
        }

        [Fact]
        public async Task CreateRequest_ForActiveContract_ShouldSucceed()
        {
            // Arrange: 
            // Setup an Active contract which is the "Happy Path" for this workflow.
            var contract = new Contract { Id = 3, Status = ContractStatus.Active };
            _mockContractRepo.Setup(r => r.GetByIdAsync(3)).ReturnsAsync(contract);

            var request = new ServiceRequest { ContractId = 3, Description = "Standard Delivery" };

            // Act:
            // Attempt to create the request.
            await _service.CreateServiceRequestAsync(request);

            // Assert:
            // Verify that the repository's AddAsync method was called exactly once.
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<ServiceRequest>()), Times.Once);
        }

        [Fact]
        public async Task CreateRequest_ForDraftContract_ShouldSucceed()
        {
            // Arrange: 
            // Verify that a Draft contract still allows requests to be raised.
            var contract = new Contract { Id = 4, Status = ContractStatus.Draft };
            _mockContractRepo.Setup(r => r.GetByIdAsync(4)).ReturnsAsync(contract);

            var request = new ServiceRequest { ContractId = 4 };

            // Act:
            await _service.CreateServiceRequestAsync(request);

            // Assert:
            // Confirm the request is persisted successfully.
            _mockRepo.Verify(r => r.AddAsync(It.IsAny<ServiceRequest>()), Times.Once);
        }

        [Fact]
        public async Task CreateRequest_WithNonExistentContract_ShouldThrowException()
        {
            // Arrange: 
            // Setup the mock repository to return null, simulating a missing contract record.
            _mockContractRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Contract)null);

            var request = new ServiceRequest { ContractId = 99 };

            // Act & Assert:
            // Verify the system catches invalid IDs before processing the request.
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateServiceRequestAsync(request));

            Assert.Equal("Contract not found.", exception.Message);
        }

        [Fact]
        public async Task GetByIdAsync_VerifyRepositoryCall()
        {
            // Arrange:
            // Setup a mock request to be returned by the repository.
            var expectedRequest = new ServiceRequest { Id = 10, Description = "Urgent Shipment" };
            _mockRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(expectedRequest);

            // Act:
            var result = await _service.GetByIdAsync(10);

            // Assert:
            // Confirm the service returns the data correctly from the repository layer.
            Assert.NotNull(result);
            Assert.Equal("Urgent Shipment", result.Description);
            _mockRepo.Verify(r => r.GetByIdAsync(10), Times.Once);
        }
    }
}