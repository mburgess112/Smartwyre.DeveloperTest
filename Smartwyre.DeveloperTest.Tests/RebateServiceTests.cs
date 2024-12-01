using FluentAssertions;

using Moq;

using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

using System.Collections.Generic;

using Xunit;

namespace Smartwyre.DeveloperTest.Tests
{
    public class RebateServiceTests
    {
        private const string SampleRebateId = "rebate";
        private const string SampleProductId = "product";

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
            _mockRebateDataStore.Setup(x => x.GetRebate(SampleRebateId))
                .Returns((Rebate)null); 
            _mockProductDataStore.Setup(x => x.GetProduct(SampleProductId))
                .Returns(new Product());

            var request = new CalculateRebateRequest
            {
                ProductIdentifier = SampleProductId,
                RebateIdentifier = SampleRebateId
            };
            var result = _rebateService.Calculate(request);

            result.Success.Should().BeFalse();
            _mockRebateDataStore.Verify(
                x => x.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()),
                Times.Never);
        }

        [Fact]
        public void Calculate_HasNullProduct_ReturnsNoSuccess_DoesNotStoreData()
        {
            _mockRebateDataStore.Setup(x => x.GetRebate(SampleRebateId))
                .Returns(new Rebate());
            _mockProductDataStore.Setup(x => x.GetProduct(SampleProductId))
                .Returns((Product)null);

            var request = new CalculateRebateRequest
            {
                ProductIdentifier = SampleProductId,
                RebateIdentifier = SampleRebateId
            };
            var result = _rebateService.Calculate(request);

            result.Success.Should().BeFalse();
            _mockRebateDataStore.Verify(
                x => x.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()),
                Times.Never);
        }

        [Fact]
        public void Calculate_FixedCash_NotSupported_ReturnsNoSuccess_DoesNotStoreData()
        {
            _mockRebateDataStore.Setup(x => x.GetRebate(SampleRebateId))
                .Returns(new Rebate
                {
                    Incentive = IncentiveType.FixedCashAmount,
                    Amount = 10
                });
            _mockProductDataStore.Setup(x => x.GetProduct(SampleProductId))
                .Returns(new Product());

            var request = new CalculateRebateRequest
            {
                ProductIdentifier = SampleProductId,
                RebateIdentifier = SampleRebateId
            };
            var result = _rebateService.Calculate(request);

            result.Success.Should().BeFalse();
            _mockRebateDataStore.Verify(
                x => x.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()),
                Times.Never);
        }

        [Fact]
        public void Calculate_FixedCash_HasZeroAmount_ReturnsNoSuccess_DoesNotStoreData()
        {
            _mockRebateDataStore.Setup(x => x.GetRebate(SampleRebateId))
                .Returns(new Rebate
                {
                    Incentive = IncentiveType.FixedCashAmount,
                    Amount = 0
                });
            _mockProductDataStore.Setup(x => x.GetProduct(SampleProductId))
                .Returns(new Product
                {
                    SupportedIncentives = SupportedIncentiveType.FixedCashAmount,
                });

            var request = new CalculateRebateRequest
            {
                ProductIdentifier = SampleProductId,
                RebateIdentifier = SampleRebateId
            };
            var result = _rebateService.Calculate(request);

            result.Success.Should().BeFalse();
            _mockRebateDataStore.Verify(
                x => x.StoreCalculationResult(It.IsAny<Rebate>(), It.IsAny<decimal>()),
                Times.Never);
        }

        [Fact]
        public void Calculate_FixedCash_ValidData_ReturnsSuccess_StoresExpectedRebate()
        {
            _mockRebateDataStore.Setup(x => x.GetRebate(SampleRebateId))
                .Returns(new Rebate
                {
                    Incentive = IncentiveType.FixedCashAmount,
                    Amount = 10
                });
            _mockProductDataStore.Setup(x => x.GetProduct(SampleProductId))
                .Returns(new Product
                {
                    SupportedIncentives = SupportedIncentiveType.FixedCashAmount,
                });

            var request = new CalculateRebateRequest
            {
                ProductIdentifier = SampleProductId,
                RebateIdentifier = SampleRebateId
            };
            var result = _rebateService.Calculate(request);

            result.Success.Should().BeTrue();
            _mockRebateDataStore.Verify(
                x => x.StoreCalculationResult(It.IsAny<Rebate>(), 10M));
        }
    }
}
