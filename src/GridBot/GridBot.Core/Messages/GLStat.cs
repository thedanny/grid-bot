namespace GridBot.Core.Messages
{
	public class GLStat
	{
		public GLStat(decimal unitsSold, decimal unitCost, decimal soldPrice)
		{
			UnitsSold = unitsSold;
			UnitCost = unitCost;
			SoldPrice = soldPrice;
		}

		public decimal UnitsSold { get; }
		public decimal UnitCost { get; }
		public decimal SoldPrice { get; }
		public decimal Total => SoldPrice * UnitsSold;
		public decimal TotalCost => UnitsSold * UnitCost;
	}
}