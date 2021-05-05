using System;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using Akka.TestKit.NUnit3;
using GridBot.Core.Actors;
using GridBot.Core.Messages;
using GridBot.Core.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridBot.Tests
{
	public class GridBotTests :TestKit
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void GridBotPlacesOrder()
		{
			var tradingPair = Pairs.BNBUSDT;
			var trader = TestActor;
			var spec = new GridSpec(tradingPair, 5.55m, 6.00m);
			var gridBot = ActorOf(Props.Create(() => new Core.Actors.GridBot(spec, trader)));

			gridBot.Tell(new Allocate(Assets.USDT, 100));
			
			gridBot.Tell(new PriceChange(Pairs.BNBUSDT,4.0m,DateTime.UtcNow));

			var buy = ExpectMsg<Buy>();
			
			Assert.AreEqual(100,buy.Amount);
			
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
		[Test]
		public void GridTrades()
		{
			var pair = Pairs.BNBUSDT;
			var trader = ActorOf<TradeManager>();
			
			var gridBot = ActorOf(Props.Create<Core.Actors.GridBot>(new GridSpec(pair, 2m, 6m), trader));

			var router = new BroadcastGroup(trader.Path.ToString(),gridBot.Path.ToString());
			var broadcast = ActorOf(Props.Empty.WithRouter(router));
			
			gridBot.Tell(new Allocate(Assets.USDT, 50));


			broadcast.Tell(new PriceChange(pair, 1, DateTime.UtcNow));
			Task.Delay(200).Wait();
			broadcast.Tell(new PriceChange(pair, 2, DateTime.UtcNow));
			Task.Delay(400).Wait();
			broadcast.Tell(new PriceChange(pair, 6, DateTime.UtcNow));
			Task.Delay(400).Wait();
			broadcast.Tell(new PriceChange(pair, 7, DateTime.UtcNow));
			Task.Delay(100).Wait();
			broadcast.Tell(new PriceChange(pair, 2, DateTime.UtcNow));
			Task.Delay(100).Wait();
			var p = gridBot.Ask<BotTradeStatus>(new GetTradeStatus()).Result;

			var b = p.Base;
			var q = p.Quote;
			Console.WriteLine($"{b.Asset.ID,4}  units: {b.Units,8:n4}  cost:{b.UnitCost,8:c4}  Price:{b.UnitPrice,8:c4}");
			Console.WriteLine($"{q.Asset.ID,4}  units: {q.Total,8:n4} ");
			Console.WriteLine($"\tTotal Cost:{p.Cost:c4} ,Total:{p.CurrentValue:c4} PL:{p.TotalPL:c4}");
			
			
			Task.Delay(100).Wait();
			var trades = trader.Ask<OrderInfo[]>(new GetOrders()).Result;

			foreach (var t in trades)
			{
				Console.WriteLine($"{t.Type:G} {t.Amount:n2} @ {t.LimitPrice} @ {t.FilledPrice:c2} ");
			}



		}
		
		
	}
}