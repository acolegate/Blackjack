using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using CardTable;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardTableTests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class HandExtensionMethodsTests
    {
        [TestMethod]
        public void HandExtensionMethodsTests_FinalValue_Various()
        {
            List<Card> noCards = new List<Card>();

            List<Card> aceAnd10 = new List<Card>
                                      {
                                          new Card(1, Suit.Clubs, Value.Ace),
                                          new Card(1, Suit.Clubs, Value.Ten)
                                      };

            List<Card> aceFiveAndTen = new List<Card>
                                           {
                                               new Card(1, Suit.Clubs, Value.Ace),
                                               new Card(1, Suit.Clubs, Value.Five),
                                               new Card(1, Suit.Clubs, Value.Ten)
                                           };

            List<Card> aceNineNine = new List<Card>
                                         {
                                             new Card(1, Suit.Clubs, Value.Ace),
                                             new Card(1, Suit.Clubs, Value.Ace),
                                             new Card(1, Suit.Clubs, Value.Nine)
                                         };

            List<Card> acesTenAndSeven = new List<Card>
                                             {
                                                 new Card(1, Suit.Clubs, Value.Ace),
                                                 new Card(1, Suit.Clubs, Value.Ace),
                                                 new Card(1, Suit.Clubs, Value.Ace),
                                                 new Card(1, Suit.Clubs, Value.Ace),
                                                 new Card(1, Suit.Clubs, Value.Ten),
                                                 new Card(1, Suit.Clubs, Value.Seven)
                                             };

            List<Card> acesAndSeven = new List<Card>
                                          {
                                              new Card(1, Suit.Clubs, Value.Ace),
                                              new Card(1, Suit.Clubs, Value.Ace),
                                              new Card(1, Suit.Clubs, Value.Ace),
                                              new Card(1, Suit.Clubs, Value.Ace),
                                              new Card(1, Suit.Clubs, Value.Seven)
                                          };

            // act/assert
            Assert.AreEqual(0, noCards.FinalValue(), "Unexpected value from empty hand");
            Assert.AreEqual(21, aceAnd10.FinalValue(), "Unexpected value from Ace and Ten");
            Assert.AreEqual(16, aceFiveAndTen.FinalValue(), "Unexpected value from Ace, Five and Ten");
            Assert.AreEqual(21, aceNineNine.FinalValue(), "Unexpected value from Ace, Nine and Nine");
            Assert.AreEqual(21, acesTenAndSeven.FinalValue(), "Unexpected value from 3 x Aces, Ten and Seven");
            Assert.AreEqual(21, acesAndSeven.FinalValue(), "Unexpected value from all 4 x Aces and Seven");
        }

        [TestMethod]
        public void HandExtensionMethodsTests_ContainsAnyCourtCard_Variations()
        {
            // act
            List<Card> cardsWithAJack = new List<Card>
                                            {
                                                new Card(1, Suit.Clubs, Value.Two),
                                                new Card(1, Suit.Clubs, Value.Jack),
                                                new Card(1, Suit.Clubs, Value.Three)
                                            };
            List<Card> cardsWithAQueen = new List<Card>
                                             {
                                                 new Card(1, Suit.Clubs, Value.Two),
                                                 new Card(1, Suit.Clubs, Value.Queen),
                                                 new Card(1, Suit.Clubs, Value.Three)
                                             };
            List<Card> cardsWithAKing = new List<Card>
                                            {
                                                new Card(1, Suit.Clubs, Value.Two),
                                                new Card(1, Suit.Clubs, Value.King),
                                                new Card(1, Suit.Clubs, Value.Three)
                                            };

            List<Card> cardsWithNoCourtCards = new List<Card>
                                                   {
                                                       new Card(1, Suit.Clubs, Value.Two),
                                                       new Card(1, Suit.Clubs, Value.Three),
                                                       new Card(1, Suit.Clubs, Value.Four)
                                                   };

            List<Card> cardsWithAllCourtCards = new List<Card>
                                                    {
                                                        new Card(1, Suit.Clubs, Value.Jack),
                                                        new Card(1, Suit.Clubs, Value.Queen),
                                                        new Card(1, Suit.Clubs, Value.King)
                                                    };

            // act/assert
            Assert.IsTrue(cardsWithAJack.ContainsAnyCourtCard(), "Unexpected card type");
            Assert.IsTrue(cardsWithAQueen.ContainsAnyCourtCard(), "Unexpected card type");
            Assert.IsTrue(cardsWithAKing.ContainsAnyCourtCard(), "Unexpected card type");
            Assert.IsTrue(cardsWithAllCourtCards.ContainsAnyCourtCard(), "Unexpected card type");
            Assert.IsFalse(cardsWithNoCourtCards.ContainsAnyCourtCard(), "Unexpected card type");
            Assert.IsFalse(new List<Card>().ContainsAnyCourtCard(), "Unexpected card type");
        }

        [TestMethod]
        public void HandExtensionMethodsTests_ContainsAnyAce_Variations()
        {
            List<Card> cardsWithNoAces = new List<Card>
                                             {
                                                 new Card(1, Suit.Clubs, Value.Two),
                                                 new Card(1, Suit.Clubs, Value.Three),
                                                 new Card(1, Suit.Clubs, Value.Four)
                                             };

            List<Card> cardsWithOneAce = new List<Card>
                                             {
                                                 new Card(1, Suit.Clubs, Value.Two),
                                                 new Card(1, Suit.Clubs, Value.Ace),
                                                 new Card(1, Suit.Clubs, Value.Three)
                                             };

            List<Card> cardsWithAllAces = new List<Card>
                                              {
                                                  new Card(1, Suit.Clubs, Value.Ace),
                                                  new Card(1, Suit.Diamonds, Value.Ace),
                                                  new Card(1, Suit.Hearts, Value.Ace),
                                                  new Card(1, Suit.Spades, Value.Ace)
                                              };

            // act/assert
            Assert.IsFalse(cardsWithNoAces.ContainsAnyAce(), "Unexpected card type");
            Assert.IsFalse(new List<Card>().ContainsAnyAce(), "Unexpected card type");
            Assert.IsTrue(cardsWithOneAce.ContainsAnyAce(), "Unexpected card type");
            Assert.IsTrue(cardsWithAllAces.ContainsAnyAce(), "Unexpected card type");
        }
    }
}