namespace nat_api.data.models.amount
{
    public class Amount : BaseModel
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
