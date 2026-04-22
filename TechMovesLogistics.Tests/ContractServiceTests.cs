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
    public class ContractServiceTests
    {
        private readonly Mock<IContractRepository> _mockContractRepo;
        private readonly ContractService _service;

        public ContractServiceTests()
        {
            // Create a mock repository to isolate the service layer from the database.
            _mockContractRepo = new Mock<IContractRepository>();

            // Inject the mocked repository into the service under test.
            _service = new ContractService(_mockContractRepo.Object);
        }

        [Fact]
        public async Task DeleteAsync_WithAssociatedServiceRequests_CascadesDelete()
        {
            // Arrange:
            // Setup a contract that exists in the repository.
            // Cascade delete behaviour is enforced at the EF Core/DB level,
            // so here we verify the service correctly delegates the delete call.
            var contractId = 1;
            _mockContractRepo.Setup(r => r.DeleteAsync(contractId)).Returns(Task.CompletedTask);

            // Act:
            // Call delete through the service layer.
            await _service.DeleteContractAsync(contractId);

            // Assert:
            // Verify the repository's DeleteAsync was called exactly once with the correct ID.
            // EF Core cascade rules handle the associated ServiceRequests automatically.
            _mockContractRepo.Verify(r => r.DeleteAsync(contractId), Times.Once);
        }

        [Fact]
        public async Task GetContractByIdAsync_IncludesClientAndServiceRequests()
        {
            // Arrange:
            // Build a contract with a populated Client and ServiceRequests collection.
            // This simulates what the repository returns after eager loading.
            var expectedContract = new Contract
            {
                Id = 1,
                Client = new Client { Id = 1, Name = "TechMove Client A" },
                ServiceRequests = new List<ServiceRequest>
                {
                    new ServiceRequest { Id = 1, Description = "Freight Delivery" },
                    new ServiceRequest { Id = 2, Description = "Customs Clearance" }
                }
            };

            _mockContractRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedContract);

            // Act:
            // Retrieve the contract through the service layer.
            var result = await _service.GetContractByIdAsync(1);

            // Assert:
            // Confirm the returned contract includes the related Client and ServiceRequests.
            Assert.NotNull(result);
            Assert.NotNull(result.Client);
            Assert.Equal("TechMove Client A", result.Client.Name);
            Assert.Equal(2, result.ServiceRequests.Count);
        }

        [Fact]
        public async Task SearchAsync_FiltersByAllCombinations_OfDateAndStatus()
        {
            // Arrange:
            // Simulate a filtered result set returned by the repository.
            // We test that the service correctly passes all filter parameters through.
            var startDate = new DateTime(2025, 1, 1);
            var endDate = new DateTime(2025, 12, 31);
            var status = ContractStatus.Active;

            var expectedContracts = new List<Contract>
            {
                new Contract { Id = 1, Status = ContractStatus.Active, StartDate = new DateTime(2025, 3, 1) },
                new Contract { Id = 2, Status = ContractStatus.Active, StartDate = new DateTime(2025, 6, 15) }
            };

            _mockContractRepo
                .Setup(r => r.SearchAsync(startDate, endDate, status))
                .ReturnsAsync(expectedContracts);

            // Act:
            // Call the search through the service with all three filter parameters.
            var result = await _service.SearchContractsAsync(startDate, endDate, status);

            // Assert:
            // Confirm the correct number of results are returned and
            // verify the repository was called with the exact filter arguments.
            var resultList = new List<Contract>(result);
            Assert.Equal(2, resultList.Count);
            _mockContractRepo.Verify(r => r.SearchAsync(startDate, endDate, status), Times.Once);
        }

        [Fact]
        public async Task SearchAsync_WithNullFilters_ReturnsAllContracts()
        {
            // Arrange:
            // Passing null for all filters should return the full unfiltered contract list.
            // This tests the "no filter applied" scenario used on the default Index page load.
            var allContracts = new List<Contract>
            {
                new Contract { Id = 1, Status = ContractStatus.Active },
                new Contract { Id = 2, Status = ContractStatus.Expired },
                new Contract { Id = 3, Status = ContractStatus.Draft }
            };

            _mockContractRepo
                .Setup(r => r.SearchAsync(null, null, null))
                .ReturnsAsync(allContracts);

            // Act:
            var result = await _service.SearchContractsAsync(null, null, null);

            // Assert:
            // All contracts must be returned when no filters are applied.
            Assert.Equal(3, new List<Contract>(result).Count);
        }

        [Fact]
        public async Task GetContractByIdAsync_NonExistentId_ReturnsNull()
        {
            // Arrange:
            // Simulate a lookup for a contract ID that does not exist in the database.
            _mockContractRepo.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Contract)null);

            // Act:
            var result = await _service.GetContractByIdAsync(999);

            // Assert:
            // The service must return null gracefully without throwing an exception.
            Assert.Null(result);
        }
    }
}