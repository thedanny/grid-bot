using Akka.Actor;
using GridBot.Core.Messages;
using GridBot.Core.Models;

namespace GridBot.Core.Actors
{
	public class GridBot : ReceiveActor
	{
		private readonly GridSpec _spec;
		private readonly IActorRef _trader;
		
		private readonly Position _quotePosition;
		private readonly Position _basePosition;
		private readonly Position _assetOnSellOrder;
		private readonly Position _assetOnBuyOrder;

		private readonly Pair _tradingPair;
		private PriceChange _lastPrice;

		private BotTradeStatus _tradeStatus;
		private bool _isTradeStatusStale = true;


		public GridBot(GridSpec spec ,IActorRef trader)
		{
			_spec = spec;
			_tradingPair = spec.Pair;
			_trader = trader;
			
			_basePosition =new CostTrackedPosition(_tradingPair.Base);
			_quotePosition =new Position(_tradingPair.Quote);
			
			_assetOnSellOrder =new Position(_tradingPair.Base);
			_assetOnBuyOrder =new Position(_tradingPair.Quote);

			Become(Ready);
		}

		private void Ready()
		{
			Receive<Allocate>(m=>m.Asset==_basePosition.Asset,
				m =>
			{
				_basePosition.DepositFund(m.Amount,m.CostBasisPerUnit);
				
				UpdateTradeStatus();
				
				Self.Tell(InitiateSell.Instance);
				
			});
			
			Receive<Allocate>(m=>m.Asset==_quotePosition.Asset,
				m =>
			{
				_quotePosition.DepositFund(m.Amount,m.CostBasisPerUnit);
				UpdateTradeStatus();
				//delayed place order
				Self.Tell(InitiateBuy.Instance);
			});

			Receive<GetTradeStatus>(m =>
			{
				if (_isTradeStatusStale)
				{
					UpdateTradeStatus();
				}
				
				Sender.Tell(_tradeStatus);

			});
			
			Receive<PriceChange>(m =>
			{
				_lastPrice = m;
			});
			
			Receive<Sold>(m =>
			{
				_assetOnSellOrder.Remove(m.UnitsSold);
				_quotePosition.Add(m.TotalProceed, null);
				UpdateTradeStatus();
				Context.Parent.Tell(new GLStat(m.UnitsSold,_basePosition.AvgCostPerUnit,m.FilledPrice));
				
				Self.Tell(InitiateBuy.Instance);
			});
			
			Receive<Bought>(m =>
			{
				_assetOnBuyOrder.Remove(m.TotalCost);
				_basePosition.Add(m.UnitsBought, m.FilledPrice);
				UpdateTradeStatus();
				Self.Tell(InitiateSell.Instance);
			});

			Receive<InitiateBuy>(_ =>
			{
				if (!HasFundToBuy()) return;
			
				var availableFundToBuyWith = _quotePosition.Amount;
			
				_quotePosition.Remove(availableFundToBuyWith);
				_assetOnBuyOrder.Add(availableFundToBuyWith, null);
			
				_trader.Tell(new Buy(_spec.Pair,_spec.BuyAt,availableFundToBuyWith));
			});
			
			
			Receive<InitiateSell>(_ =>
			{
				if (!HasAssetToSell()) return;
			
				var unitsToSell = _basePosition.Amount;
			
				_assetOnSellOrder.Add(unitsToSell, null);
				_basePosition.Remove(unitsToSell);
			
				_trader.Tell(new Sell(_tradingPair,_spec.SellAt,unitsToSell));
			});

		}

		private void UpdateTradeStatus()
		{
			var currentPrice = _lastPrice?.Price ?? _basePosition.AvgCostPerUnit;

			var totalBaseAsset = _assetOnSellOrder.Amount + _basePosition.Amount;
			var totalQuoteAsset = _quotePosition.Amount + _assetOnBuyOrder.Amount;

			var totalCost = _quotePosition.Initial + _basePosition.Initial * _basePosition.AvgCostPerUnit;

			var currentValue = totalQuoteAsset + totalBaseAsset * currentPrice;

			var pl = currentValue - totalCost;

			var unrealizedPl = totalBaseAsset * currentPrice - totalBaseAsset * _basePosition.AvgCostPerUnit;

			_tradeStatus = new BotTradeStatus(
				new QuotePosition
				{
					Asset = _tradingPair.Quote,
					Total = totalQuoteAsset,
				},
				new BasePosition
				{
					Asset = _basePosition.Asset,
					Units = totalBaseAsset,
					UnitCost = _basePosition.AvgCostPerUnit,
					UnitPrice = currentPrice,
				}
				, totalCost
				, currentValue
				, pl - unrealizedPl
				, unrealizedPl
			);

			_isTradeStatusStale = false;
		}

		private bool HasAssetToSell() => _basePosition.Amount > _spec.Pair.MinSell;
		private bool HasFundToBuy()=>  _quotePosition.Amount > _spec.Pair.MinBuy;
		

	

		private class InitiateBuy
		{
			public static readonly InitiateBuy Instance =new InitiateBuy();
		}
		
		private class InitiateSell
		{
			public static readonly InitiateSell Instance = new InitiateSell();
		}

	}
}