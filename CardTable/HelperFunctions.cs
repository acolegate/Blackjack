using System;
using System.Collections.Generic;
using System.Linq;

namespace CardTable
{
	public static class HelperFunctions
	{
		private static readonly Random GlobalRandomNumberGenerator = new Random();

		[ThreadStatic]
		private static Random _random;

		private static Random RandomNumberGenerator
		{
			get
			{
				if (_random == null)
				{
					int seed;
					lock (GlobalRandomNumberGenerator)
					{
						seed = GlobalRandomNumberGenerator.Next();
					}
					_random = new Random(seed);
				}
				return _random;
			}
		}

		public static List<T> Shuffle<T>(this IEnumerable<T> items)
		{
			return new List<T>(items.OrderBy(i => RandomNumberGenerator.Next()));
		}

		public static List<Card> CreateCards(int decks)
		{
			List<Card> cards = new List<Card>(52 * decks);

			for (int deckNumber = 1; deckNumber <= decks; deckNumber++)
			{
				cards.AddRange(new List<Card> { new Card(1, Suit.Spades, Value.Ace),
												new Card(1, Suit.Spades, Value.Two),
												new Card(1, Suit.Spades, Value.Three),
												new Card(1, Suit.Spades, Value.Four),
												new Card(1, Suit.Spades, Value.Five),
												new Card(1, Suit.Spades, Value.Six),
												new Card(1, Suit.Spades, Value.Seven),
												new Card(1, Suit.Spades, Value.Eight),
												new Card(1, Suit.Spades, Value.Nine),
												new Card(1, Suit.Spades, Value.Ten),
												new Card(1, Suit.Spades, Value.Jack),
												new Card(1, Suit.Spades, Value.Queen),
												new Card(1, Suit.Spades, Value.King),
												new Card(1, Suit.Diamonds, Value.Ace),
												new Card(1, Suit.Diamonds, Value.Two),
												new Card(1, Suit.Diamonds, Value.Three),
												new Card(1, Suit.Diamonds, Value.Four),
												new Card(1, Suit.Diamonds, Value.Five),
												new Card(1, Suit.Diamonds, Value.Six),
												new Card(1, Suit.Diamonds, Value.Seven),
												new Card(1, Suit.Diamonds, Value.Eight),
												new Card(1, Suit.Diamonds, Value.Nine),
												new Card(1, Suit.Diamonds, Value.Ten),
												new Card(1, Suit.Diamonds, Value.Jack),
												new Card(1, Suit.Diamonds, Value.Queen),
												new Card(1, Suit.Diamonds, Value.King),
												new Card(1, Suit.Clubs, Value.Ace),
												new Card(1, Suit.Clubs, Value.Two),
												new Card(1, Suit.Clubs, Value.Three),
												new Card(1, Suit.Clubs, Value.Four),
												new Card(1, Suit.Clubs, Value.Five),
												new Card(1, Suit.Clubs, Value.Six),
												new Card(1, Suit.Clubs, Value.Seven),
												new Card(1, Suit.Clubs, Value.Eight),
												new Card(1, Suit.Clubs, Value.Nine),
												new Card(1, Suit.Clubs, Value.Ten),
												new Card(1, Suit.Clubs, Value.Jack),
												new Card(1, Suit.Clubs, Value.Queen),
												new Card(1, Suit.Clubs, Value.King),
												new Card(1, Suit.Hearts, Value.Ace),
												new Card(1, Suit.Hearts, Value.Two),
												new Card(1, Suit.Hearts, Value.Three),
												new Card(1, Suit.Hearts, Value.Four),
												new Card(1, Suit.Hearts, Value.Five),
												new Card(1, Suit.Hearts, Value.Six),
												new Card(1, Suit.Hearts, Value.Seven),
												new Card(1, Suit.Hearts, Value.Eight),
												new Card(1, Suit.Hearts, Value.Nine),
												new Card(1, Suit.Hearts, Value.Ten),
												new Card(1, Suit.Hearts, Value.Jack),
												new Card(1, Suit.Hearts, Value.Queen),
												new Card(1, Suit.Hearts, Value.King)

											}
								);
			}

			return cards;
		}
	}
}