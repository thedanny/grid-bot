using System;

namespace GridBot.Core.Messages
{
	public class Bought
	{
		public Bought(decimal unitsBought, decimal totalCost, decimal filledPrice, DateTime time)
		{
			UnitsBought = unitsBought;
			FilledPrice = filledPrice;
			Time = time;
			TotalCost = totalCost;
		}

		public decimal UnitsBought { get; }
		public decimal FilledPrice { get; }
		
		public decimal TotalCost { get; }
		public DateTime Time { get; set; }
	}
}