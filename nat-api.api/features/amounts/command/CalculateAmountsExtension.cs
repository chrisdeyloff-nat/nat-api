using System;
using nat_api.data.models.amount;

namespace nat_api.api.features.amounts.command
{
    public static class CalculateAmountsExtension
    {
        public static void CalculateAmounts(this Amount self)
        {
            if (self == null) return;

            var tempAmount = self.Value;

            if (tempAmount > 1)
            {
                self.SilverDollarValue = Decimal.ToInt32(Math.Truncate(tempAmount));
                tempAmount = tempAmount - self.SilverDollarValue;
            }

            if (tempAmount > 0)
            {
                var tempCents = Decimal.ToInt32(tempAmount * 100);
                
                self.HalfDollarValue = tempCents / 50;
                tempCents = tempCents % 50;

                self.QuarterValue = tempCents / 25;
                tempCents = tempCents % 25;

                self.DimeValue = tempCents / 10;
                tempCents = tempCents % 10;

                self.NickelValue = tempCents / 5;
                tempCents = tempCents % 5;

                self.PennyValue = tempCents;
            }
        }

    }
}