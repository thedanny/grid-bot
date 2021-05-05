using System;

namespace GridBot.Core.Models
{
	public class CostTrackedPosition :Position
	{
    
		protected override decimal CalculateAvgCostBasis(decimal newUnit, decimal? newCost)
		{
			if(!newCost.HasValue || newCost.Value<=0)
				throw new ArgumentException("cost should be greater than 0");
    				
			return (newUnit * newCost.Value + Amount*AvgCostPerUnit) / newUnit + Amount;
		}
    
		public CostTrackedPosition(Asset asset) : base(asset)
		{
		}
	}
}