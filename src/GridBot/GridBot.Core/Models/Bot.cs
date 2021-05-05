using Akka.Actor;

namespace GridBot.Core.Models
{
	public class Bot
	{
		public IActorRef Actor { get; set; }
		public GridSpec Spec { get; set; }
		
		public int No { get; set; }
	}
}