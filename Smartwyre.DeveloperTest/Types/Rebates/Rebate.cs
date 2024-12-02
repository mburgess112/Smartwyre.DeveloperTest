namespace Smartwyre.DeveloperTest.Types.Rebates;

public abstract record Rebate
{
    public string Identifier { get; set; }
    public abstract IncentiveType Incentive { get; }
    
    public bool TryCalculateRebate(Product product, decimal volume, out decimal rebateAmount)
    {
        if (!product.SupportedIncentives.HasFlag(SupportedIncentiveType)
            || !IsValidForRequest(product, volume))
        {
            rebateAmount = 0;
            return false;
        }
        rebateAmount = CalculateRebate(product, volume);
        return true;
    }

    protected abstract bool IsValidForRequest(Product product, decimal volume);
    protected abstract decimal CalculateRebate(Product product, decimal volume);

    private SupportedIncentiveType SupportedIncentiveType
    {
        get
        {
            switch (Incentive)
            {
                case IncentiveType.FixedCashAmount:
                    return SupportedIncentiveType.FixedCashAmount;
                case IncentiveType.FixedRateRebate:
                    return SupportedIncentiveType.FixedRateRebate;
                case IncentiveType.AmountPerUom:
                    return SupportedIncentiveType.AmountPerUom;
                default:
                    return default;
            }
        }
    }
}
