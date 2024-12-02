using FluentAssertions;

using Moq;

using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Types.Rebates;

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
                .Returns(new FixedCashAmountRebate());
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
                .Returns(new FixedCashAmountRebate
                {
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
                .Returns(new FixedCashAmountRebate
                {
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

        [Theory]
        [MemberData(nameof(ValidRebateTests))]
        public void Calculate_ValidData_ReturnsSuccess_StoresExpectedRebate(ValidRebateTestCase testCase)
        {
            _mockRebateDataStore.Setup(x => x.GetRebate(SampleRebateId))
                .Returns(testCase.Rebate);
            _mockProductDataStore.Setup(x => x.GetProduct(SampleProductId))
                .Returns(testCase.Product);

            var request = new CalculateRebateRequest
            {
                ProductIdentifier = SampleProductId,
                RebateIdentifier = SampleRebateId,
                Volume = 10
            };
            var result = _rebateService.Calculate(request);

            result.Success.Should().BeTrue();
            _mockRebateDataStore.Verify(
                x => x.StoreCalculationResult(testCase.Rebate, testCase.ExpectedAmount));
        }

        public static TheoryData<ValidRebateTestCase> ValidRebateTests =>
            [
                new()
                {
                    Product = new Product
                    {
                        SupportedIncentives = SupportedIncentiveType.FixedCashAmount,
                    },
                    Rebate = new FixedCashAmountRebate
                    {
                        Amount = 10
                    },
                    ExpectedAmount = 10
                },
                new()
                {
                    Product = new Product
                    {
                        SupportedIncentives = SupportedIncentiveType.FixedRateRebate,
                        Price = 30
                    },
                    Rebate = new FixedRateRebate
                    {
                        Percentage = 0.05M
                    },
                    ExpectedAmount = 15
                },
                new()
                {
                    Product = new Product
                    {
                        SupportedIncentives = SupportedIncentiveType.AmountPerUom,
                        Price = 30
                    },
                    Rebate = new AmountPerUomRebate
                    {
                        Amount = 7
                    },
                    ExpectedAmount = 70
                }
            ];

        public class ValidRebateTestCase
        {
            public Product Product { get; init; }
            public Rebate Rebate { get; init; }
            public decimal ExpectedAmount { get; init; }
        }
    }
}
