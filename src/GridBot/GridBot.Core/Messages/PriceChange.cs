using System;
using GridBot.Core.Models;

namespace GridBot.Core.Messages
{
	public class PriceChange
	{
		public PriceChange(Pair pair, decimal price, DateTime time)
		{
			Pair = pair;
			Price = price;
			Time = time;
		}

		public Pair Pair { get; }
		public decimal Price { get; }
		public DateTime Time { get; }
	}
}