using System;

namespace Smartwyre.DeveloperTest.Types.Rebates
{
    public record FixedRateRebate : Rebate
    {
        public decimal Percentage { get; set; }

        public override IncentiveType Incentive => IncentiveType.FixedRateRebate;

        protected override decimal CalculateRebate(Product product, decimal volume)
        {
            return product.Price * Percentage * volume;
        }

        protected override bool IsValidForRequest(Product product, decimal volume)
        {
            return Percentage > 0
                && volume > 0
                && product.Price > 0;
        }
    }
}
