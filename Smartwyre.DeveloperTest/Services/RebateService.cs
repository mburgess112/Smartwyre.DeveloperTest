using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Types;
using Smartwyre.DeveloperTest.Types.Rebates;

namespace Smartwyre.DeveloperTest.Services;

public class RebateService : IRebateService
{
    private readonly IProductDataStore _productDataStore;
    private readonly IRebateDataStore _rebateDataStore;

    public RebateService(
        IProductDataStore productDataStore,
        IRebateDataStore rebateDataStore) 
    {
        _productDataStore = productDataStore;
        _rebateDataStore = rebateDataStore;
    }

    public CalculateRebateResult Calculate(CalculateRebateRequest request)
    {
        Rebate rebate = _rebateDataStore.GetRebate(request.RebateIdentifier);
        Product product = _productDataStore.GetProduct(request.ProductIdentifier);

        var result = new CalculateRebateResult();
        if (rebate == null || product == null)
        {
            // The product null-check wasn't applied to the fixed-cash case, but the code would fail without it
            return result;
        }

        // TODO: next step would be to break out separate set of tests for each of the implementation classes
        // The machinery code in this class could be covered with one simple case, or even just integration testing
        if (rebate.TryCalculateRebate(product, request.Volume, out var rebateAmount)) 
        {
            result.Success = true;
            // This was originally a separately-created object from the rebate store used for reading
            // There doesn't seem to be a big reason for doing this, but if we wanted it we could
            // segregate the interface.
            _rebateDataStore.StoreCalculationResult(rebate, rebateAmount);
        }

        return result;
    }
}
