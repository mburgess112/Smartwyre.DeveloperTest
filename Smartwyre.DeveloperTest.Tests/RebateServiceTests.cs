using FluentAssertions;

using Moq;

using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

using Xunit;

namespace Smartwyre.DeveloperTest.Tests
{
    public class RebateServiceTests
    {
        private readonly RebateService _rebateService;
        private readonly Mock<IProductDataStore> _mockProductDataStore;
        private readonly Mock<IRebateDataStore> _mockRebateDataStore;

        public RebateServiceTests()
        {
            _mockProductDataStore = new Mock<IProductDataStore>();
            _mockRebateDataStore = new Mock<IRebateDataStore>();
            _rebateService = new RebateService(
                _mockProductDataStore.Object,
                _mockRebateDataStore.Object);
        }

        [Fact]
        public void Calculate_HasNullRebate_ReturnsNoSuccess_DoesNotStoreData()
        {
            _mockRebateDataStore.Setup(x => x.GetRebate("rebate"))
                .Returns((Rebate)null); 
            _mockProductDataStore.Setup(x => x.GetProduct("product"))
                .Returns(new Product());

            var request = new CalculateRebateRequest
            {
                ProductIdentifier = "product",
                RebateIdentifier = "rebate"
            };
            var result = _rebateService.Calculate(request);

            result.Success.Should().BeFalse();
            _mockRebateDataStore
                .Verify(x => x.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()), 
                Times.Never);
        }

        [Fact]
        public void Calculate_HasNullProduct_ReturnsNoSuccess_DoesNotStoreData()
        {
            _mockRebateDataStore.Setup(x => x.GetRebate("rebate"))
                .Returns(new Rebate());
            _mockProductDataStore.Setup(x => x.GetProduct("product"))
                .Returns((Product)null);

            var request = new CalculateRebateRequest
            {
                ProductIdentifier = "product",
                RebateIdentifier = "rebate"
            };
            var result = _rebateService.Calculate(request);

            result.Success.Should().BeFalse();
            _mockRebateDataStore
                .Verify(x => x.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()),
                Times.Never);
        }
    }
}
