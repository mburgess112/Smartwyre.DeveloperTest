using Smartwyre.DeveloperTest.Data;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;

using System;

namespace Smartwyre.DeveloperTest.Runner;

class Program
{
    static void Main(string[] args)
    {
        // need to validate/document args: can use CommandLineRunner to specify
        var request = new CalculateRebateRequest
        {
            ProductIdentifier = args[0],
            RebateIdentifier = args[1],
            Volume = decimal.Parse(args[2])
        };
        Console.WriteLine($"Processing request: {request}");

        // should ideally have some base logic in the real data stores for integration testing
        var service = new RebateService(new ProductDataStore(), new RebateDataStore());

        var result = service.Calculate(request);
        Console.WriteLine($"Calculated successfully: {result.Success}");
    }
}
