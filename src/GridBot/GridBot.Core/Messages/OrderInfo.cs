using GridBot.Core.Models;

namespace GridBot.Core.Messages
{
	public class OrderInfo
	{
		public OrderInfo(Pair pair, OrderType type, decimal? filledPrice, decimal amount, decimal limitPrice, string submittedBy, int orderNo)
		{
			
			Pair = pair;
			Type = type;
			FilledPrice = filledPrice;
			Amount = amount;
			LimitPrice = limitPrice;
			SubmittedBy = submittedBy;
			OrderNo = orderNo;
		}

		public int OrderNo { get; }
		public string SubmittedBy { get; }
		public Pair Pair { get; }
		public decimal LimitPrice { get;}
		public decimal? FilledPrice { get;}
		public decimal Amount { get;  }
		public bool IsFilled => FilledPrice.HasValue;
		public OrderType Type { get; }
	}
}