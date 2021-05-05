using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using GridBot.Core.Messages;
using GridBot.Core.Models;

namespace GridBot.Core.Actors
{
	public class TradeManager : ReceiveActor
	{
		
		private readonly List<Order> _orders=new List<Order>();
		private readonly List<Order> _filledOrders=new List<Order>();
		private PriceChange _lastPrice;
		private int _orderNo;

		public TradeManager()
		{
			_orderNo = 0;
			// Context.System.Scheduler
			// 	.ScheduleTellRepeatedly(TimeSpan.FromSeconds(2),TimeSpan.FromSeconds(2),Self,CheckOrders.Instance,Self);
			
			Receive<CheckOrders>(_ =>
			{

			});
			
			Receive<GetOrders>(a =>
			{
				var active = a.Filter == GetOrders.FilterType.NonFilled
				             || a.Filter == GetOrders.FilterType.All
					? from o in _orders select new OrderInfo(o.Pair, o.Type, o.FilledPrice, o.Amount,o.LimitPrice,o.Sender.Path.Name,o.OrderNo)
					: Enumerable.Empty<OrderInfo>();
				
				var filled=a.Filter == GetOrders.FilterType.Filled
				           || a.Filter == GetOrders.FilterType.All
					? from o in _filledOrders select new OrderInfo(o.Pair, o.Type, o.FilledPrice, o.Amount,o.LimitPrice,o.Sender.Path.Name,o.OrderNo)
					: Enumerable.Empty<OrderInfo>();
				
				Sender.Tell(active.Union(filled).OrderBy(a=>a.OrderNo).ToArray());
				
			});
			
			Receive<PriceChange>(p =>
			{
				_lastPrice = p;
				ExecuteTrade(p);
			});
			
			Receive<Buy>(b =>
			{
				
				_orders.Add(new Order(Sender,b.Pair,b.LimitPrice,b.Amount,OrderType.Buy,++_orderNo));
				
				if(_lastPrice!=null) ExecuteTrade(_lastPrice);

			});
			
			Receive<Sell>(s =>
			{
				_orders.Add(new Order(Sender,s.Pair,s.LimitPrice,s.Amount,OrderType.Sell,++_orderNo));
				if(_lastPrice!=null) ExecuteTrade(_lastPrice);

			});
			
			
		}

		private void ExecuteTrade(PriceChange p)
		{
			foreach (var order in _orders.Where(a => a.Pair == p.Pair && !a.IsFilled).ToArray())
			{
				switch (order.Type)
				{
					case OrderType.Buy:
						if (p.Price >= order.LimitPrice) continue;

						_orders.Remove(order);
						var price = p.Price;
						var unitsBought = order.Amount / p.Price;
						_filledOrders.Add(order.MarkFilled(p.Price,unitsBought,-order.Amount));
						order.Sender.Tell(new Bought(order.UnitsFilled,price*unitsBought  ,p.Price, DateTime.UtcNow));

						break;
					case OrderType.Sell:
						if (p.Price <= order.LimitPrice) continue;
						_orders.Remove(order);
						var unitsSold = order.Amount;
						_filledOrders.Add(order.MarkFilled(p.Price,unitsSold,unitsSold*p.Price));
						order.Sender.Tell(new Sold(order.Amount, order.Proceeds, p.Price, DateTime.UtcNow));


						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}

		private class CheckOrders
		{
			//public static CheckOrders Instance { get; }=new CheckOrders();
			
		}
		
		public class Order
		{
			public Order(IActorRef sender, Pair pair, decimal limitPrice, decimal amount, OrderType type, int orderNo)
			{
				Sender = sender;
				Pair = pair;
				LimitPrice = limitPrice;
				Amount = amount;
				Type = type;
				OrderNo = orderNo;
			}

			public int OrderNo { get; }
			public IActorRef Sender { get; }
			public Pair Pair { get;  }
			public decimal LimitPrice { get; }
			public decimal? FilledPrice { get; private set; }

			public decimal UnitsFilled { get; private set; }
			public decimal Proceeds { get; private set; }
			public decimal Amount { get; }
			
			
			public OrderType Type { get; }
			
			public bool IsFilled { get; set; }

			public Order MarkFilled(decimal filledPrice,decimal unitsFilled,decimal proceeds)
			{
				IsFilled = true;
				FilledPrice = filledPrice;
				UnitsFilled = unitsFilled;
				Proceeds = proceeds;
					
				return this;
			}
			
		}

		
	}
}