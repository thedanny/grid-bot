namespace GridBot.Core.Models
{
	public class Pair
	{
		public Pair(Asset @base, Asset quote, decimal minSell=0m, decimal minBuy=0m)
		{
			Base = @base;
			Quote = quote;
			MinSell = minSell;
			MinBuy = minBuy;
			Symbol = $"{Base.ID}{Quote.ID}";
		}

		public string Symbol { get; }
		public Asset Base { get; }
		public Asset Quote { get; }
		
		public decimal MinSell { get;  }
		public decimal MinBuy { get;  }

		public override string ToString()
		{
			return Symbol;
		}
	}
}