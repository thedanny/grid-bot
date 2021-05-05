using GridBot.Core.Models;

namespace GridBot.Core.Actors
{
	public class BroadcastPrice
	{
		public BroadcastPrice(Pair pair, decimal lastPrice)
		{
			Pair = pair;
			LastPrice = lastPrice;
		}

		public Pair Pair { get; }
		public decimal LastPrice { get; }
	}
}