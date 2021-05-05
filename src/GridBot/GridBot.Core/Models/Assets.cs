namespace GridBot.Core.Models
{
	public class Assets
	{

		public static readonly Asset BNB = new Asset("BNB",AssetType.BlockChainToken);
		public static readonly Asset USD = new Asset("USD",AssetType.FiatCurrency);
		public static readonly Asset USDT = new Asset("USDT",AssetType.BlockChainToken);
		public static readonly Asset BUSD = new Asset("BUSD",AssetType.BlockChainToken);
		public static readonly Asset DOGE = new Asset("DOGE",AssetType.BlockChainToken);

	}
}