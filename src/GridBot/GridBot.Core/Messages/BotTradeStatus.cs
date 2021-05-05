using GridBot.Core.Models;

namespace GridBot.Core.Messages
{
	public class BotTradeStatus
	{
		public BotTradeStatus(QuotePosition quote,BasePosition @base, decimal cost, decimal currentValue, decimal realizedPl, decimal unrealizedPl)
		{
			Cost = cost;
			CurrentValue = currentValue;
			RealizedPL = realizedPl;
			UnrealizedPL = unrealizedPl;
			Base = @base;
			Quote = quote;
		}

			
		public BasePosition Base { get;  }
		public QuotePosition Quote { get;  }
		public decimal Cost { get;  }
		public decimal CurrentValue { get;  }
		public decimal RealizedPL { get;  }
		public decimal UnrealizedPL { get;  }
		public decimal TotalPL => RealizedPL + UnrealizedPL;
	}
}