using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using GridBot.Core.Messages;

namespace GridBot.Core.Actors
{
	public class PriceFeed : ReceiveActor
	{
		private readonly Dictionary<string,List<Subscription>> _subscriptions=new Dictionary<string, List<Subscription>>();
		
		public PriceFeed()
		{

			Receive<BroadcastPrice>(p =>
			{
				if(!_subscriptions.TryGetValue(p.Pair.ToString(), out var list)) return;

				foreach (var subscription in list)
				{
					subscription.Subscriber.Tell(new PriceChange(p.Pair,p.LastPrice,DateTime.UtcNow));
				}

			});
			
			
			
			Receive<Subscribe>(s =>
			{
				if(!_subscriptions.TryGetValue(s.Pair.Symbol, out var subscriptions))
					_subscriptions.Add(s.Pair.Symbol,subscriptions=new List<Subscription>());
				
				subscriptions.Add(new Subscription(s.Pair.Symbol,s.Subscriber));
	
			});

			Receive<Unsubscribe>(s =>
			{
				if (!_subscriptions.TryGetValue(s.Pair, out var subscriptions)) return;
				
				var sub = subscriptions.FirstOrDefault(a => a.Subscriber.Equals(s.Subscriber));

				if (sub != null)
					subscriptions.Remove(sub);
			});

			
			

		}
		
		
		public class Subscription
		{
			public Subscription(string pair, IActorRef subscriber)
			{
				Pair = pair;
				Subscriber = subscriber;
			}

			public string Pair { get;  }
			public IActorRef Subscriber { get; }
			
		}
		
		
		
	}
}