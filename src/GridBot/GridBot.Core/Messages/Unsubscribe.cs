using Akka.Actor;

namespace GridBot.Core.Messages
{
	public class Unsubscribe
	{
		public Unsubscribe(string pair, IActorRef subscriber)
		{
			Pair = pair;
			Subscriber = subscriber;
		}

		public string Pair { get; }
		public IActorRef Subscriber { get; }
	}
}