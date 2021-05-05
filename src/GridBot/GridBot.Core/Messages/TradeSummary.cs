namespace GridBot.Core.Messages
{
	public class TradeSummary
	{
		public TradeSummary(decimal cost, decimal proceeds)
		{
			Cost = cost;
			Proceeds = proceeds;
		}

		public decimal Cost { get; }
		public decimal Proceeds { get; }
		public decimal GLAmount => Proceeds - Cost;
		public decimal GLPercentage => Cost>0? GLAmount / Cost:0m;
	}
}