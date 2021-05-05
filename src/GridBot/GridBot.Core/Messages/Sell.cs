using GridBot.Core.Models;

namespace GridBot.Core.Messages
{
	public class Sell
	{
		public Pair Pair { get; }
		public decimal LimitPrice { get; }
		public decimal Amount { get; }

		public Sell(Pair pair, in decimal limitPrice, in decimal amount)
		{
			Pair = pair;
			LimitPrice = limitPrice;
			Amount = amount;
		}
	}
}