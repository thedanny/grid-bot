using System;
using GridBot.Core.Actors;
using GridBot.Core.Models;

namespace GridBot.Core.Messages
{
	public class Buy
	{
		public Pair Pair { get; }
		public decimal LimitPrice { get; }
		public decimal Amount { get; }

		public Buy(Pair pair, in decimal limitPrice, in decimal amount)
		{
			Pair = pair;
			LimitPrice = limitPrice;
			Amount = amount;
		}
	}
}