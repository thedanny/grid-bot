using System.Collections.Generic;
using Akka.Actor;

namespace GridBot.Core.Models
{
	public class GridInstance
	{
		public string Pair { get; set; }
		public decimal Min { get; }
		public decimal Max { get; }
		public List<Bot> Bots { get; set; } 
		public IActorRef TradeManager { get; set; }
		public IActorRef BroadCastGroup { get; set; }
		public decimal Cost { get; set; }
	}
}