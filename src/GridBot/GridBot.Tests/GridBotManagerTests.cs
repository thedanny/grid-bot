using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using GridBot.Core.Actors;
using GridBot.Core.Messages;
using GridBot.Core.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridBot.Tests
{
	public class GridBotManagerTests :TestKit
	{
		[SetUp]
		public void Setup()
		{
			
		}

		[Test]
		public void GridBotPlacesOrder()
		{
			var tradingPair = Pairs.DOGEUSDT;
			var priceFeed = ActorOf<PriceFeed>();
			var tradBoatManager = ActorOf(Props.Create<GridBotManager>(priceFeed,ActorOf<TradeManager>()));
			
			tradBoatManager.Tell(new LaunchGridBot(tradingPair,20,0.41m,0.51m,100));
			
			var prices = LoadSamplePrices();
			
			foreach (var i in prices)
			{
				Task.Delay(100).Wait();
				priceFeed.Tell(new BroadcastPrice(tradingPair,i));
				
			}
			Task.Delay(1000).Wait();
			var s= tradBoatManager.Ask<TradeSummary>(new GetTradeSummary()).Result;

			Console.WriteLine($"Total Cost:{s.Cost:c4} ,Total:{s.Proceeds:c4} PL:{s.GLAmount:c4}  PL:{s.GLPercentage:p2}  ");
			return;
			
			
			// var totalCost = 0m;
			// var totalValue = 0m;
			// var totalPl = 0m;
			// foreach (var p in stats)
			// {
			// 	var b = p.Base;
			// 	var q = p.Quote;
			// 	//Console.WriteLine($"{b.Asset.ID,4}  units: {b.Units,8:n4} / {q.Asset.ID,4}  units: {q.Total,8:n4} | Total Cost:{p.Cost:c4} ,Total:{p.CurrentValue:c4} PL:{p.TotalPL:c4}");
			//
			// 	totalCost += p.Cost;
			// 	totalValue += p.CurrentValue;
			// 	totalPl += p.TotalPL;
			// }
			//
			// Console.WriteLine($"\tSUMMARY Total Cost:{totalCost:c4} ,Total:{totalValue:c4} PL:{totalPl:c4}");

			
			//Assert.AreEqual(100,buy.Amount);

		}

		private static decimal[] LoadSamplePrices()
		{
			var sample = JsonConvert.DeserializeObject<decimal[][]>(File.ReadAllText(
					//@"./ChartData/bnbusdt-1m-klines.json"
					@"./ChartData/dogeusdt-5m-klines-small.json"
				))
				.Select(a => a[1]).ToArray();
			return sample;
		}

		[Test]
		public void GridBotShouldReturnCurrentStat()
		{
			var tradingPair = Pairs.BNBUSDT;
			var trader = ActorOf<TradeManager>();
			var spec = new GridSpec(tradingPair, 5.55m, 6.00m);
			var gridBot = ActorOf(Props.Create(() => new Core.Actors.GridBot(spec, trader)));

			gridBot.Tell(new Allocate(Assets.USDT, 100));
			
			gridBot.Tell(new PriceChange(Pairs.BNBUSDT,4.0m,DateTime.UtcNow));
			trader.Tell(new PriceChange(Pairs.BNBUSDT,4.0m,DateTime.UtcNow));
			
			gridBot.Tell(new GetTradeStatus());

			var p = ExpectMsg<BotTradeStatus>();

			Console.WriteLine("Status "+JsonConvert.SerializeObject(p));



		}
		
		
	}
}