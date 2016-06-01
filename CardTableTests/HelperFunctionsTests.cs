using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using CardTable;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardTableTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class HelperFunctionsTests
    {
        [TestInitialize]
        public void Initialise()
        {
        }

        [TestMethod]
        public void HelperFunctions_CreateCards_VariousNumberOfDecks_ReturnsCorrectNumberOfCards()
        {
            // act
            Assert.AreEqual(0, HelperFunctions.CreateCards(0).Count, "Unexpected number of cards");
            Assert.AreEqual(52, HelperFunctions.CreateCards(1).Count, "Unexpected number of cards");
            Assert.AreEqual(104, HelperFunctions.CreateCards(2).Count, "Unexpected number of cards");
            Assert.AreEqual(156, HelperFunctions.CreateCards(3).Count, "Unexpected number of cards");
        }

        [TestMethod]
        public void HelperFunctions_CreateCards_MakeSureAllCardsArePresent()
        {
            // arrange
            List<Card> cards = HelperFunctions.CreateCards(1);

            // act
            Assert.IsTrue(TestForPresenceOfAllCards(cards), "Not all cards are present");
        }

        [TestMethod]
        public void HelperFunctions_ShuffleCards_MakeSureAllCardsArePresent()
        {
            // arrange
            List<Card> cards = new List<Card>();
            cards.AddRange(MakeCardDeck(1));
            cards.AddRange(MakeCardDeck(2));
            cards.AddRange(MakeCardDeck(3));
            cards.AddRange(MakeCardDeck(4));
            cards.AddRange(MakeCardDeck(5));

            // act
            List<Card> shuffledCards = cards.Shuffle();

            // assert
            Assert.AreEqual(260, shuffledCards.Count, "Unexpected number of cards");
            Assert.IsTrue(TestForPresenceOfAllCards(shuffledCards), "Not all Cards are present");
        }

        private static bool TestForPresenceOfAllCards(IReadOnlyCollection<Card> cardsToTest)
        {
            int remainder;
            Math.DivRem(cardsToTest.Count, 52, out remainder);
            if (remainder != 0)
            {
                //not enough cards to make whole decks
                return false;
            }

            int decks = cardsToTest.Count / 52;

            List<Card> cardsToFind = new List<Card>(decks * 52);
            for (int deck = 1; deck <= decks; deck++)
            {
                cardsToFind.AddRange(MakeCardDeck(deck));
            }

            if (cardsToTest.Count != cardsToFind.Count)
            {
                // not the same number of cards
                return false;
            }

            foreach (Card cardFound in cardsToTest.Select(cardToTest => cardsToFind.FirstOrDefault(x => x.Deck == cardToTest.Deck && x.Suit == cardToTest.Suit && x.Value == cardToTest.Value)))
            {
                if (cardFound == null)
                {
                    return false;
                }

                cardsToFind.Remove(cardFound);
            }

            return cardsToFind.Count == 0;
        }

        private static IEnumerable<Card> MakeCardDeck(int deckNumber)
        {
            return new List<Card>
                       {
                           new Card(deckNumber, Suit.Spades, Value.Ace),
                           new Card(deckNumber, Suit.Spades, Value.Two),
                           new Card(deckNumber, Suit.Spades, Value.Three),
                           new Card(deckNumber, Suit.Spades, Value.Four),
                           new Card(deckNumber, Suit.Spades, Value.Five),
                           new Card(deckNumber, Suit.Spades, Value.Six),
                           new Card(deckNumber, Suit.Spades, Value.Seven),
                           new Card(deckNumber, Suit.Spades, Value.Eight),
                           new Card(deckNumber, Suit.Spades, Value.Nine),
                           new Card(deckNumber, Suit.Spades, Value.Ten),
                           new Card(deckNumber, Suit.Spades, Value.Jack),
                           new Card(deckNumber, Suit.Spades, Value.Queen),
                           new Card(deckNumber, Suit.Spades, Value.King),
                           new Card(deckNumber, Suit.Diamonds, Value.Ace),
                           new Card(deckNumber, Suit.Diamonds, Value.Two),
                           new Card(deckNumber, Suit.Diamonds, Value.Three),
                           new Card(deckNumber, Suit.Diamonds, Value.Four),
                           new Card(deckNumber, Suit.Diamonds, Value.Five),
                           new Card(deckNumber, Suit.Diamonds, Value.Six),
                           new Card(deckNumber, Suit.Diamonds, Value.Seven),
                           new Card(deckNumber, Suit.Diamonds, Value.Eight),
                           new Card(deckNumber, Suit.Diamonds, Value.Nine),
                           new Card(deckNumber, Suit.Diamonds, Value.Ten),
                           new Card(deckNumber, Suit.Diamonds, Value.Jack),
                           new Card(deckNumber, Suit.Diamonds, Value.Queen),
                           new Card(deckNumber, Suit.Diamonds, Value.King),
                           new Card(deckNumber, Suit.Clubs, Value.Ace),
                           new Card(deckNumber, Suit.Clubs, Value.Two),
                           new Card(deckNumber, Suit.Clubs, Value.Three),
                           new Card(deckNumber, Suit.Clubs, Value.Four),
                           new Card(deckNumber, Suit.Clubs, Value.Five),
                           new Card(deckNumber, Suit.Clubs, Value.Six),
                           new Card(deckNumber, Suit.Clubs, Value.Seven),
                           new Card(deckNumber, Suit.Clubs, Value.Eight),
                           new Card(deckNumber, Suit.Clubs, Value.Nine),
                           new Card(deckNumber, Suit.Clubs, Value.Ten),
                           new Card(deckNumber, Suit.Clubs, Value.Jack),
                           new Card(deckNumber, Suit.Clubs, Value.Queen),
                           new Card(deckNumber, Suit.Clubs, Value.King),
                           new Card(deckNumber, Suit.Hearts, Value.Ace),
                           new Card(deckNumber, Suit.Hearts, Value.Two),
                           new Card(deckNumber, Suit.Hearts, Value.Three),
                           new Card(deckNumber, Suit.Hearts, Value.Four),
                           new Card(deckNumber, Suit.Hearts, Value.Five),
                           new Card(deckNumber, Suit.Hearts, Value.Six),
                           new Card(deckNumber, Suit.Hearts, Value.Seven),
                           new Card(deckNumber, Suit.Hearts, Value.Eight),
                           new Card(deckNumber, Suit.Hearts, Value.Nine),
                           new Card(deckNumber, Suit.Hearts, Value.Ten),
                           new Card(deckNumber, Suit.Hearts, Value.Jack),
                           new Card(deckNumber, Suit.Hearts, Value.Queen),
                           new Card(deckNumber, Suit.Hearts, Value.King)
                       };
        }
    }
}