namespace GridBot.Core.Messages
{
	public class GetOrders
	{
		public FilterType Filter { get; }

		public GetOrders(FilterType filter=FilterType.All)
		{
			Filter = filter;
		}
		
		public enum FilterType
		{
			All,
			Filled,
			NonFilled
		}
	}
}