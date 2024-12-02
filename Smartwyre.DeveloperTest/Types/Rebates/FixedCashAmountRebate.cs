namespace Smartwyre.DeveloperTest.Types.Rebates
{
    public record FixedCashAmountRebate : Rebate
    {
        public decimal Amount { get; set; }

        public override IncentiveType Incentive => IncentiveType.FixedCashAmount;

        protected override decimal CalculateRebate(Product product, decimal volume)
        {
            return Amount;
        }

        protected override bool IsValidForRequest(Product product, decimal volume)
        {
            return Amount > 0;
        }
    }
}
