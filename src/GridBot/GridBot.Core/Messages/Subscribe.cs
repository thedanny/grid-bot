using Akka.Actor;
using GridBot.Core.Models;

namespace GridBot.Core.Messages
{
	public class Subscribe
	{
		public Subscribe(Pair pair, IActorRef subscriber)
		{
			Pair = pair;
			Subscriber = subscriber;
		}

		public Pair Pair { get; }
		public IActorRef Subscriber { get; }
	}
}