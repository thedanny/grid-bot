using System;

namespace GridBot.Core.Models
{
	public class Position
	{
		public Position(Asset asset)
		{
			Asset = asset;
		}
    
		public Asset Asset { get; }
    			
		public decimal Initial { get; private set; }
		//public decimal TotalCost => Initial * AvgCostPerUnit;
		public decimal Amount { get; private set; }
    			
		public decimal AvgCostPerUnit { get; private set; }
    
		protected virtual decimal CalculateAvgCostBasis(decimal newUnit, decimal? newCost)
		{
			if(newCost.HasValue)
				throw new ArgumentException("Unit Cannot be set for this asset");
    				
			return 1m;
		}
		public  Position Add(decimal unitsToAdd, decimal? unitCost)
		{
			AvgCostPerUnit = CalculateAvgCostBasis(unitsToAdd,unitCost);
			Amount += unitsToAdd;
			return this;
		}
    			
		public  Position DepositFund(decimal unitsToAdd, decimal? unitCost)
		{
			Initial += unitsToAdd;
			Amount += unitsToAdd;
			AvgCostPerUnit = CalculateAvgCostBasis(unitsToAdd,unitCost);
			return this;
		}
    
		public  Position Remove(decimal unitsToRemove)
		{
			Amount -= unitsToRemove;
			return this;
		}
    			
		public  Position RemoveAll()
		{
			Amount = 0;
			return this;
		}
    			
    			
    			
	}
}