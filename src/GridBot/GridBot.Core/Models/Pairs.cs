namespace GridBot.Core.Models
{
	public static class Pairs
	{
		public static Pair DOGEUSDT { get; }=new Pair(Assets.DOGE,Assets.USDT);
		public static Pair BNBUSDT { get; }=new Pair(Assets.BNB,Assets.USDT);
		public static Pair BNBUSD { get; }=new Pair(Assets.BNB,Assets.USD);
		public static Pair BNBBUSD { get; }=new Pair(Assets.BNB,Assets.BUSD);
		
		
	}
}