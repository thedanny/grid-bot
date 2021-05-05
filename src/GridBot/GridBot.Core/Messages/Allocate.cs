using System.Linq;
using GridBot.Core.Models;

namespace GridBot.Core.Messages
{
	public class Allocate
	{
		public Allocate(Asset asset, decimal amount, decimal? costBasisPerUnit=null)
		{
			Asset = asset;
			Amount = amount;
			CostBasisPerUnit = costBasisPerUnit;
		}

		public Asset Asset { get; }
		public decimal Amount { get; }
		public decimal? CostBasisPerUnit { get; }
	}
}