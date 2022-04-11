using nat_api.api.responses;

namespace nat_api.api.features.amounts
{
    public class AmountResponse : BaseResponse
    {
        public long Id { get; set; }
        public decimal Value { get; set; }
        public int SilverDollarValue { get; set; }
        public int HalfDollarValue { get; set; }
        public int QuarterValue { get; set; }
        public int DimeValue { get; set; }
        public int NickelValue { get; set; }
        public int PennyValue { get; set; }
    }
}
