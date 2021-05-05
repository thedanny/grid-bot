using System;
using System.Linq;
using Akka.Actor;
using Akka.TestKit.NUnit3;
using GridBot.Core.Actors;
using GridBot.Core.Messages;
using GridBot.Core.Models;
using Newtonsoft.Json;
using NUnit.Framework;

namespace GridBot.Tests
{
	public class TradeManagerTests : TestKit
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test(Description= "Should return empty array")]
		public void TraderReturnsEmpty()
		{
			var tradeManager = ActorOf(Props.Create<TradeManager>());
			
			tradeManager.Tell(new GetOrders());

			var orders= ExpectMsg<OrderInfo[]>();
			
			Assert.IsEmpty(orders);
		}
		
		[Test(Description= "Should return active orders")]
		public void TraderReturnsActiveOrders()
		{
			var tradeManager = ActorOf(Props.Create<TradeManager>());
			
			tradeManager.Tell(new Buy(Pairs.BNBUSDT, 456,1.5m));
			
			tradeManager.Tell(new GetOrders());

			var orders= ExpectMsg<OrderInfo[]>();
			
			Assert.IsNotEmpty(orders);
			
			Assert.IsFalse(orders[0].IsFilled);
		}	
		
		
		[Test(Description= "Should execute order sell when price is above limit")]
		public void TraderShouldSell()
		{
			var tradeManager = ActorOf(Props.Create<TradeManager>());
			
			tradeManager.Tell(new Sell(Pairs.BNBUSDT, 456,1.5m));
			
			tradeManager.Tell(new PriceChange(Pairs.BNBUSDT, 460,DateTime.UtcNow));
			
			var sold= ExpectMsg<Sold>();

			Console.WriteLine(JsonConvert.SerializeObject(sold));
			tradeManager.Tell(new GetOrders(GetOrders.FilterType.Filled));

			
			var order= ExpectMsg<OrderInfo[]>().FirstOrDefault();
			Console.WriteLine(JsonConvert.SerializeObject(order));
			
			Assert.IsNotNull(order);
			
			Assert.IsTrue(order.IsFilled);
			
			Assert.AreEqual(Pairs.BNBUSDT, order.Pair);
			Assert.AreEqual(1.5m, order.Amount);
			Assert.AreEqual(460m, order.FilledPrice);
		}
		
		
		[Test(Description= "Should execute buy order when price is below limit")]
		public void TraderShouldBuy()
		{
			var tradeManager = ActorOf(Props.Create<TradeManager>());
			
			tradeManager.Tell(new Buy(Pairs.BNBUSDT, 450,2m));
			
			tradeManager.Tell(new PriceChange(Pairs.BNBUSDT, 445,DateTime.UtcNow));
			
			var bought= ExpectMsg<Bought>();

			Console.WriteLine("bought:"+ JsonConvert.SerializeObject(bought));
			
			tradeManager.Tell(new GetOrders(GetOrders.FilterType.Filled));

			
			var order= ExpectMsg<OrderInfo[]>().FirstOrDefault();
			Console.WriteLine("Order : "+ JsonConvert.SerializeObject(order));
			
			Assert.IsNotNull(order);
			
			Assert.IsTrue(order.IsFilled);
			
			Assert.AreEqual(Pairs.BNBUSDT, order.Pair);
			Assert.AreEqual(2m, order.Amount);
			Assert.AreEqual(445m, order.FilledPrice);
		}	
		
		
		[Test(Description= "Should not execute sell  order when price is below limit")]
		public void TraderShouldNotSell()
		{
			var tradeManager = ActorOf(Props.Create<TradeManager>());
			
			tradeManager.Tell(new Sell(Pairs.BNBUSDT, 456,1.5m));
			
			tradeManager.Tell(new PriceChange(Pairs.BNBUSDT, 440,DateTime.UtcNow));
			
			ExpectNoMsg(TimeSpan.FromSeconds(2));

			
			tradeManager.Tell(new GetOrders());

			
			var order= ExpectMsg<OrderInfo[]>().FirstOrDefault();
			Console.WriteLine("Order :"+JsonConvert.SerializeObject(order));
			
			Assert.IsNotNull(order);
			
			Assert.IsFalse(order.IsFilled);
			
			Assert.AreEqual(Pairs.BNBUSDT, order.Pair);
			Assert.AreEqual(1.5m, order.Amount);
			Assert.IsNull(order.FilledPrice);
		}
		
		
		[Test(Description= "Should execute buy order when price is below limit")]
		public void TraderShouldNotBuy()
		{
			var tradeManager = ActorOf(Props.Create<TradeManager>());
			
			tradeManager.Tell(new Buy(Pairs.BNBUSDT, 450,2m));
			
			tradeManager.Tell(new PriceChange(Pairs.BNBUSDT, 455,DateTime.UtcNow));
			
			 ExpectNoMsg(TimeSpan.FromSeconds(2));

			 tradeManager.Tell(new GetOrders());

			
			var order= ExpectMsg<OrderInfo[]>().FirstOrDefault();
			Console.WriteLine("Order : "+ JsonConvert.SerializeObject(order));
			
			Assert.IsNotNull(order);
			
			Assert.IsFalse(order.IsFilled);
			
			Assert.AreEqual(Pairs.BNBUSDT, order.Pair);
			Assert.AreEqual(2m, order.Amount);
			Assert.IsNull(order.FilledPrice);
		}
		
		
	}
}