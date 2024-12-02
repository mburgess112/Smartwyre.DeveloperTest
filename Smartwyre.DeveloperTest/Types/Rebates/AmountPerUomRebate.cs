using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Smartwyre.DeveloperTest.Types.Rebates
{
    public record AmountPerUomRebate : Rebate
    {
        public decimal Amount { get; set; }

        public override IncentiveType Incentive => IncentiveType.AmountPerUom;

        protected override decimal CalculateRebate(Product product, decimal volume)
        {
            return Amount * volume;
        }

        protected override bool IsValidForRequest(Product product, decimal volume)
        {
            return Amount > 0 && volume > 0;
        }
    }
}
