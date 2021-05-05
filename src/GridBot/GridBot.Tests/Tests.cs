using System;
using System.IO;
using System.Linq;
using Akka.TestKit.NUnit3;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace GridBot.Tests
{
	public class Tests :TestKit
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void ActorSystemWorks()
		{

			var x = JsonConvert.DeserializeObject<decimal[][]>(File.ReadAllText(
				@"C:\Data\Experiments\grid-bot\src\GridBot\GridBot.Tests\ChartData\bnbusdt-5m-klines.json"))
				.Select(a=>a[1]).ToArray()
				;


			Console.WriteLine( string.Join(",", x ));
			
		
			Assert.Pass();

		}
	}
}