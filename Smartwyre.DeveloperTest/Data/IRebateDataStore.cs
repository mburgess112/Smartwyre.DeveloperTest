using Smartwyre.DeveloperTest.Types.Rebates;

namespace Smartwyre.DeveloperTest.Data
{
    public interface IRebateDataStore
    {
        Rebate GetRebate(string rebateIdentifier);
        void StoreCalculationResult(Rebate account, decimal rebateAmount);
    }
}
