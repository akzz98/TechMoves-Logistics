using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using TechMoveLogistics.Repositories.Interfaces;
using TechMoves_Logistics.Models;
using TechMoves_Logistics.Models.Enums;
using TechMoves_Logistics.Repositories.Interfaces;
using TechMoves_Logistics.Services;

namespace TechMovesLogistics.Tests
{
    public class ServiceRequestServiceTests
    {
        private readonly Mock<IServiceRequestRepository> _mockRepo;
        private readonly Mock<IContractRepository> _mockContractRepo;
        private readonly ServiceRequestService _service;

        public ServiceRequestServiceTests()
        {
            // Create mock repositories to isolate the service layer.
            _mockRepo = new Mock<IServiceRequestRepository>();
            _mockContractRepo = new Mock<IContractRepository>();
            // Inject mocked dependencies into the service.
            _service = new ServiceRequestService(_mockRepo.Object, _mockContractRepo.Object);
        }
        [Fact]
        public async Task CreateRequest_ForExpiredContract_ShouldThrowException()
        {
            // Arrange: Create an expired contract
            var contract = new Contract
            {
                Id = 1,
                Status = ContractStatus.Expired
            };
            _mockContractRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(contract);

            var request = new ServiceRequest { ContractId = 1 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.CreateServiceRequestAsync(request));

            Assert.Equal("Cannot create a service request for an Expired or OnHold contract.", exception.Message);
        }
    }
}
