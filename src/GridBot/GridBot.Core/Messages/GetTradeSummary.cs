namespace GridBot.Core.Messages
{
	public class GetTradeSummary
	{
		public GetTradeSummary(bool includeDetail=false)
		{
			IncludeDetail = includeDetail;
		}

		public bool IncludeDetail { get; } 
	}
}