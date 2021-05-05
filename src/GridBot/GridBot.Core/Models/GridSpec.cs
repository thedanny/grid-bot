namespace GridBot.Core.Models
{
	public class GridSpec
	{
		public GridSpec(Pair pair, decimal buyAt, decimal sellAt)
		{
			Pair = pair;
			BuyAt = buyAt;
			SellAt = sellAt;
		}

		public Pair Pair { get;  }
		public decimal BuyAt { get;  }
		public decimal SellAt { get;  }
		
		
	}
}