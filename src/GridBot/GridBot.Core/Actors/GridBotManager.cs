using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Akka.IO;
using Akka.Routing;
using GridBot.Core.Messages;
using GridBot.Core.Models;

namespace GridBot.Core.Actors
{
	public class GridBotManager : ReceiveActor
	{
		private readonly IActorRef _trader;
		private GridInstance _gridInstance;
		private readonly IActorRef _priceFeed;
		private readonly ILoggingAdapter _log;
		private IActorRef _allBots;
		private int _remainingStatusCount;
		private IActorRef _botStatusRequester;
		private List<BotTradeStatus> _gridStat;
		private List<GLStat> _glStat=new List<GLStat>();
		private GLTracking _glTracking= new GLTracking();
		
		public GridBotManager(IActorRef priceFeed,IActorRef trader)
		{
			_trader = trader;
			_log = Context.GetLogger();
			_priceFeed =priceFeed?? Context.ActorOf(Props.Create(() => new PriceFeed()), "PriceFeed");
			
			Become(Ready);
		}

		private void Ready()
		{
			Receive<GetTradeStatus>(_ =>
			{
				
				Become(CollectingStats);
				_remainingStatusCount = _gridInstance.Bots.Count;
				_botStatusRequester = Sender;
				_gridStat=new List<BotTradeStatus>();
				_allBots.Tell(new GetTradeStatus());

			});

			Receive<GLStat>(s =>
			{
				_glStat.Add(s);
			
				_glTracking.Cost += s.TotalCost;
				_glTracking.GL += (s.Total-s.TotalCost);

			});

			Receive<GetTradeSummary>(m =>
			{

				Sender.Tell(new TradeSummary(_gridInstance.Cost,_gridInstance.Cost+ _glTracking.GL));
			});
			
			Receive<LaunchGridBot>(m =>
			{
				var priceIncrement = (m.UpperBound - m.LowerBound) / m.GridCount;
				
				var allocatedFundPerGrid = m.AllocatedFund / m.GridCount;

				
				_log.Info($"Price Inc : {priceIncrement:n3} , Fund per Grid :{allocatedFundPerGrid:c3}");

				var bots=new List<Bot>();
				var priceSubscriptions=new List<string>();
				
				foreach (var g in Enumerable.Range(0, m.GridCount))
				{
					var buyPrice = m.LowerBound + g * priceIncrement;
					var sellPrice = buyPrice + priceIncrement;

					var spec = new GridSpec(m.Pair,buyPrice,sellPrice);
					var bot = new Bot {
                              			No = g,
                              			Actor =Context.ActorOf(Props.Create(() => new GridBot(spec, _trader)),$"{m.Pair}-{g}"),
                              			Spec = spec,
									  };
					
					_log.Info($"Bot #{g} : SellAt: {spec.SellAt:c2} , BuyAt:{spec.BuyAt:c3} ");
					bot.Actor.Tell(new Allocate(m.Pair.Quote, allocatedFundPerGrid));
					
					bots.Add(bot);
					
					priceSubscriptions.Add(bot.Actor.Path.ToString());
					
				}

				_allBots = Context.ActorOf(Props.Empty.WithRouter(new BroadcastGroup(priceSubscriptions)));
				
				priceSubscriptions.Add(_trader.Path.ToString());

				_gridInstance = new GridInstance
				{
					Bots = bots,
					Cost=m.AllocatedFund,
					BroadCastGroup = Context.ActorOf(Props.Empty.WithRouter(new BroadcastGroup(priceSubscriptions)))
				};
				
				
				
				_priceFeed.Tell(new Subscribe(m.Pair,_gridInstance.BroadCastGroup));
				
			});
		}

		private void CollectingStats()
		{
			
			Receive<BotTradeStatus>(s =>
			{
				_remainingStatusCount--;
				
				_gridStat.Add(s);

				if (_remainingStatusCount <= 0)
				{
					_botStatusRequester.Tell(_gridStat.ToArray());
					Become(Ready);
				}
				
			});
		}
	}
}