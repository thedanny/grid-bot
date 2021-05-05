using System;

namespace GridBot.Core.Messages
{
	public class Sold
	{
		
		public Sold(decimal unitsSold, decimal totalProceed , decimal filledPrice, DateTime time)
		{
			UnitsSold = unitsSold;
			FilledPrice = filledPrice;
			TotalProceed = totalProceed;
			Time = time;
		}

		public decimal UnitsSold { get; }
		public decimal FilledPrice { get; }
		public decimal TotalProceed { get;  }
		public DateTime Time { get; set; }
	}
}