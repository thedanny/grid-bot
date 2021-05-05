using GridBot.Core.Models;

namespace GridBot.Core.Messages
{
	public class LaunchGridBot
	{
		public LaunchGridBot(Pair pair, int gridCount, decimal lowerBound,  decimal upperBound,decimal allocatedFund)
		{
			Pair = pair;
			GridCount = gridCount;
			LowerBound = lowerBound;
			AllocatedFund = allocatedFund;
			UpperBound = upperBound;
		}

		public Pair Pair { get; }
		public int GridCount { get; }
		public decimal LowerBound { get; }
		public decimal UpperBound { get;  }
		public decimal AllocatedFund { get; set; }
	}
}