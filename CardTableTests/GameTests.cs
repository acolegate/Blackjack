using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using CardTable;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardTableTests
{
    /// <summary>
    /// Summary description for GameTests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class GameTests
    {
        private const decimal TestStake = 5;
        private Game _classUnderTest;

        [TestInitialize]
        public void Initialise()
        {
        }

        [TestMethod]
        public void GameTests_Constructor_PopulateWithPlayersAndCards_ReturnsCorrectValues()
        {
            // Arrange
            List<Player> testPlayers = new List<Player>
                                           {
                                               new Player("One", 1),
                                               new Player("Two", 2),
                                               new Player("Three", 3),
                                               new Player("Four", 4)
                                           };
            Player testBanker = new Player("Banker", 0, true);

            List<Card> testShoe = HelperFunctions.CreateCards(1);

            // Act
            _classUnderTest = new Game(testPlayers, testBanker, TestStake, testShoe);

            // Assert
            CollectionAssert.AreEquivalent(testPlayers, _classUnderTest.Players, "Unexpected players");
            CollectionAssert.AreEquivalent(testShoe, _classUnderTest.Shoe, "Unexpected cards in shoe");
            Assert.AreEqual(RoundState.NotStarted, _classUnderTest.State, "Unexpected game state");
            Assert.AreEqual(TestStake, _classUnderTest.StakePerRound, "Unexpected stake per round");
            Assert.AreEqual(0, _classUnderTest.Rounds, "Unexpected rounds");
        }

        /// <summary>
        /// Mies the test method.
        /// </summary>
        [TestMethod]
        public void GameTests_DealInitialCards_CorrectCardsAreDealtAndRemovedProperlyFromShoe()
        {
            List<Player> testPlayers = new List<Player>
                                           {
                                               new Player("One", 1),
                                               new Player("Two", 2),
                                               new Player("Three", 3),
                                               new Player("Four", 4)
                                           };
            Player testBanker = new Player("Banker", 0, true);

            List<Card> testShoe = HelperFunctions.CreateCards(1);

            _classUnderTest = new Game(testPlayers, testBanker, TestStake, testShoe);

            // Act
            _classUnderTest.DealInitialCards();

            // Assert
            Assert.AreEqual(2, _classUnderTest.Players[0].Hand.Count, "Unexpected number of cards in player 1's hand");
            Assert.AreEqual(2, _classUnderTest.Players[1].Hand.Count, "Unexpected number of cards in player 2's hand");
            Assert.AreEqual(2, _classUnderTest.Players[2].Hand.Count, "Unexpected number of cards in player 3's hand");
            Assert.AreEqual(2, _classUnderTest.Players[3].Hand.Count, "Unexpected number of cards in player 4's hand");

            // cards were not shuffled so we know which cards each player will get
            Assert.IsTrue(_classUnderTest.Players[0].Hand[0].IsSameCard(new Card(1, Suit.Hearts, Value.King)), "Unexpected card 1 in player 1's hand");
            Assert.IsTrue(_classUnderTest.Players[0].Hand[1].IsSameCard(new Card(1, Suit.Hearts, Value.Eight)), "Unexpected card 2 in player 1's hand");

            Assert.IsTrue(_classUnderTest.Players[1].Hand[0].IsSameCard(new Card(1, Suit.Hearts, Value.Queen)), "Unexpected card 1 in player 2's hand");
            Assert.IsTrue(_classUnderTest.Players[1].Hand[1].IsSameCard(new Card(1, Suit.Hearts, Value.Seven)), "Unexpected card 2 in player 2's hand");

            Assert.IsTrue(_classUnderTest.Players[2].Hand[0].IsSameCard(new Card(1, Suit.Hearts, Value.Jack)), "Unexpected card 1 in player 3's hand");
            Assert.IsTrue(_classUnderTest.Players[2].Hand[1].IsSameCard(new Card(1, Suit.Hearts, Value.Six)), "Unexpected card 2 in player 3's hand");

            Assert.IsTrue(_classUnderTest.Players[3].Hand[0].IsSameCard(new Card(1, Suit.Hearts, Value.Ten)), "Unexpected card 1 in player 4's hand");
            Assert.IsTrue(_classUnderTest.Players[3].Hand[1].IsSameCard(new Card(1, Suit.Hearts, Value.Five)), "Unexpected card 2 in player 4's hand");

            Assert.IsTrue(_classUnderTest.Banker.Hand[0].IsSameCard(new Card(1, Suit.Hearts, Value.Nine)), "Unexpeted card 1 in banker's hand");
            Assert.IsTrue(_classUnderTest.Banker.Hand[1].IsSameCard(new Card(1, Suit.Hearts, Value.Four)), "Unexpeted card 1 in banker's hand");

            Assert.IsTrue(_classUnderTest.Shoe[52 - 10 - 1].IsSameCard(new Card(1, Suit.Hearts, Value.Three)), "Unexpected top card in shoe");

            Assert.AreEqual(52 - 10, _classUnderTest.Shoe.Count, "Unexpected number of cards left in shoe");
        }

        [TestMethod]
        public void GameTests_TopCardFromShoe_RemoveAllCardsThenReturnNull()
        {
            // arrange
            List<Card> testShoe = HelperFunctions.CreateCards(1);

            _classUnderTest = new Game(null, null, TestStake, testShoe);

            Card card = null;

            // act - retrieve all cards from the shoe
            for (int i = 0; i < 52; i++)
            {
                card = _classUnderTest.GetTopCardFromShoe();

                // put it on the discard pile
                _classUnderTest.DiscardCard(card);
            }

            // assert - the shoe should be empty and the last card drawn should be Ace of Spades
            Assert.AreEqual(0, _classUnderTest.Shoe.Count, "Unexpected cards left in shoe");
            Assert.IsTrue(card.IsSameCard(new Card(1, Suit.Spades, Value.Ace)), "Unexpected last card drawn");
            Assert.AreEqual(52, _classUnderTest.DiscardPile.Count, "Unexpected number of cards on the discard pile");

            // act 2 - try and take another card when the shoe is empty
            // the discard pile will then be reused and reshuffled
            card = _classUnderTest.GetTopCardFromShoe();
            
            // Assert - it could be any card
            Assert.IsNotNull(card, "Unexpected null card from shoe");
            Assert.AreEqual(0, _classUnderTest.DiscardPile.Count, "Unexpected number of cards on the discard pile");
            Assert.AreEqual(51, _classUnderTest.Shoe.Count, "Unexpected number of cards in shoe");
        }

        [TestMethod]
        public void GameTests_TakeStakes_DebitsAllPlayersWhoHaveMoney()
        {
            // arrange
            List<Player> testPlayers = new List<Player>
                                           {
                                               new Player("One", 2),
                                               new Player("Two", 2),
                                               new Player("Three", 1),
                                               new Player("Four")
                                           };
            Player testBanker = new Player("Banker", 0, true);

            List<Card> cards = new List<Card>();

            _classUnderTest = new Game(testPlayers, testBanker, 1, cards);

            // act 1 - take 1 from everyone
            _classUnderTest.TakeStakes();

            // assert
            Assert.IsTrue(_classUnderTest.Players[0].Balance == 1 && _classUnderTest.Players[0].State == PlayerState.InPlay, "Unexpected balance/state for Player 1");
            Assert.IsTrue(_classUnderTest.Players[1].Balance == 1 && _classUnderTest.Players[1].State == PlayerState.InPlay, "Unexpected balance/state for Player 2");
            Assert.IsTrue(_classUnderTest.Players[2].Balance == 0 && _classUnderTest.Players[2].State == PlayerState.InPlay, "Unexpected balance/state for Player 3");
            Assert.IsTrue(_classUnderTest.Players[3].Balance == 0 && _classUnderTest.Players[3].State == PlayerState.OutOfFunds, "Unexpected balance/state for Player 4");

            // act 2 - take 1 from everyone except #3 who ran out of money after first round
            _classUnderTest.TakeStakes();

            // assert
            Assert.IsTrue(_classUnderTest.Players[0].Balance == 0 && _classUnderTest.Players[0].State == PlayerState.InPlay, "Unexpected balance/state for Player 1");
            Assert.IsTrue(_classUnderTest.Players[1].Balance == 0 && _classUnderTest.Players[1].State == PlayerState.InPlay, "Unexpected balance/state for Player 2");
            Assert.IsTrue(_classUnderTest.Players[2].Balance == 0 && _classUnderTest.Players[2].State == PlayerState.OutOfFunds, "Unexpected balance/state for Player 3");
            Assert.IsTrue(_classUnderTest.Players[3].Balance == 0 && _classUnderTest.Players[3].State == PlayerState.OutOfFunds, "Unexpected balance/state for Player 4");
        }

        [TestMethod]
        public void GameTests_HitPlayer_TakesTopCardAndHandsToPlayer()
        {
            // arrange
            Player testPlayer = new Player("One", 1);
            List<Card> testShoe = HelperFunctions.CreateCards(1);

            Player testBanker = new Player("Banker", 0, true);

            _classUnderTest = new Game(new List<Player>
                                           {
                                               testPlayer
                                           }, testBanker, 1, testShoe);

            _classUnderTest.DealInitialCards();

            // assert
            Assert.AreEqual(52 - 4, _classUnderTest.Shoe.Count, "Unexpected cards in shoe after initial deal");
            Assert.AreEqual(2, _classUnderTest.Players[0].Hand.Count, "Unexpected number of cards in player's hand after initial deal");

            Card cardThatWillBeDealt = _classUnderTest.Shoe[_classUnderTest.Shoe.Count - 1];

            // act
            _classUnderTest.HitPlayer(testPlayer);

            // assert
            Assert.AreEqual(52 - 5, _classUnderTest.Shoe.Count, "Unexpected cards in shoe after hit");
            Assert.AreEqual(3, _classUnderTest.Players[0].Hand.Count, "Unexpected number of cards in player's hand after hit");
            Assert.IsTrue(_classUnderTest.Players[0].Hand[2].IsSameCard(cardThatWillBeDealt), "Unexpected card was dealt to player");
        }

        [TestMethod]
        public void GameTests_GoRoundTable_AllPlayersStand_NoHitsForPlayers_OneHitForBanker()
        {
            // arrange
            List<Player> testPlayers = new List<Player>
                                           {
                                               new Player("One", 1),
                                               new Player("Two", 1),
                                               new Player("Three"), // out of funds
                                               new Player("Four", 1)
                                           };
            Player testBanker = new Player("Banker", 0, true);

            List<Card> testShoe = HelperFunctions.CreateCards(1);
            _classUnderTest = new Game(testPlayers, testBanker, 1, testShoe);

            Assert.AreEqual(0, _classUnderTest.Rounds, "Unexpected number of rounds before gameplay starts");
            Assert.AreEqual(RoundState.NotStarted, _classUnderTest.State, "Unexpected state before gameplay starts");

            Assert.AreEqual(52, _classUnderTest.Shoe.Count, "Unexpected number of card in shoe before initial dealing");

            // act
            _classUnderTest.PlayRound();

            // Assert
            Assert.AreEqual(2, _classUnderTest.Players[0].Hand.Count, "Unexpected number of cards for player 1");
            Assert.AreEqual(2, _classUnderTest.Players[1].Hand.Count, "Unexpected number of cards for player 2");
            Assert.AreEqual(0, _classUnderTest.Players[2].Hand.Count, "Unexpected number of cards for player 3");
            Assert.AreEqual(2, _classUnderTest.Players[3].Hand.Count, "Unexpected number of cards for player 4");
            Assert.AreEqual(3, _classUnderTest.Banker.Hand.Count, "Unexpected number of cards for banker");

            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Players[0].State, "Unexpected state for player 1");
            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Players[1].State, "Unexpected state for player 2");
            Assert.AreEqual(PlayerState.OutOfFunds, _classUnderTest.Players[2].State, "Unexpected state for player 3");
            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Players[3].State, "Unexpected state for player 4");
            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Banker.State, "Unexpected state for Banker");

            Assert.AreEqual(52 - 9, _classUnderTest.Shoe.Count, "Unexpected number of card in shoe after first round - Only 1 additional card should have been dealt to the Banker");

            Assert.AreEqual(1, _classUnderTest.Rounds, "Unexpected number of rounds after first round");
            Assert.AreEqual(RoundState.Finished, _classUnderTest.State, "Unexpected state after first round");
        }

        [TestMethod]
        public void GameTests_GoRoundTable_AllPlayersStand_NoHitsForPlayers_BankerGoesBust()
        {
            // arrange
            List<Player> testPlayers = new List<Player>
                                           {
                                               new Player("One", 1),
                                               new Player("Two", 1),
                                               new Player("Three"), // out of funds
                                               new Player("Four", 1)
                                           };
            Player testBanker = new Player("Banker", 0, true);

            List<Card> testShoe = new List<Card>

            {

                // b - hit - goes bust
                new Card(1, Suit.Hearts, Value.Ten),

                // b
                new Card(1, Suit.Hearts, Value.Six),
                
                //p4
                new Card(1, Suit.Hearts, Value.Seven),

                // p2
                new Card(1, Suit.Hearts, Value.Eight),

                // p1
                new Card(1, Suit.Hearts, Value.Nine),

                // b
                new Card(1, Suit.Hearts, Value.Ten),

                // p4
                new Card(1, Suit.Hearts, Value.Jack),

                // p2
                new Card(1, Suit.Hearts, Value.Queen),

                // p1
                new Card(1, Suit.Hearts, Value.King) 
                                      };
                                

            _classUnderTest = new Game(testPlayers, testBanker, 1, testShoe);

            Assert.AreEqual(0, _classUnderTest.Rounds, "Unexpected number of rounds before gameplay starts");
            Assert.AreEqual(RoundState.NotStarted, _classUnderTest.State, "Unexpected state before gameplay starts");

            Assert.AreEqual(9, _classUnderTest.Shoe.Count, "Unexpected number of card in shoe before initial dealing");
            
            // act
            _classUnderTest.PlayRound();

            // Assert
            Assert.AreEqual(2, _classUnderTest.Players[0].Hand.Count, "Unexpected number of cards for player 1");
            Assert.AreEqual(2, _classUnderTest.Players[1].Hand.Count, "Unexpected number of cards for player 2");
            Assert.AreEqual(0, _classUnderTest.Players[2].Hand.Count, "Unexpected number of cards for player 3");
            Assert.AreEqual(2, _classUnderTest.Players[3].Hand.Count, "Unexpected number of cards for player 4");
            Assert.AreEqual(3, _classUnderTest.Banker.Hand.Count, "Unexpected number of cards for banker");

            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Players[0].State, "Unexpected state for player 1");
            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Players[1].State, "Unexpected state for player 2");
            Assert.AreEqual(PlayerState.OutOfFunds, _classUnderTest.Players[2].State, "Unexpected state for player 3");
            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Players[3].State, "Unexpected state for player 4");
            Assert.AreEqual(PlayerState.Bust, _classUnderTest.Banker.State, "Unexpected state for Banker");

            Assert.AreEqual(0, _classUnderTest.Shoe.Count, "Unexpected number of card in shoe after first round - Only 1 additional card should have been dealt to the Banker");

            Assert.AreEqual(1, _classUnderTest.Rounds, "Unexpected number of rounds after first round");
            Assert.AreEqual(RoundState.Finished, _classUnderTest.State, "Unexpected state after first round");
        }

        [TestMethod]
        public void GameTests_GoRoundTable_AllPlayersHitThenGoBust()
        {
            // arrange
            List<Player> testPlayers = new List<Player>
                                           {
                                               new Player("One", 1),
                                               new Player("Two", 1),
                                               new Player("Three", 1),
                                               new Player("Four", 1)
                                           };
            Player testBanker = new Player("Banker", 0, true);

            // cards are dealt from the top of the stack i.e. from the end
            List<Card> testShoe = new List<Card>
                                      {
                                          // third round - all go bust
                                          new Card(1, Suit.Spades, Value.Six),
                                          new Card(1, Suit.Clubs, Value.Six),
                                          new Card(1, Suit.Hearts, Value.Six),
                                          new Card(1, Suit.Diamonds, Value.Six),

                                          // second round - everyone at 16
                                          new Card(1, Suit.Spades, Value.Ten),
                                          new Card(1, Suit.Clubs, Value.Nine),
                                          new Card(1, Suit.Hearts, Value.Eight),
                                          new Card(1, Suit.Diamonds, Value.Seven),
                                          new Card(1, Suit.Spades, Value.Six),

                                          // first round
                                          new Card(1, Suit.Clubs, Value.Six),
                                          new Card(1, Suit.Hearts, Value.Seven),
                                          new Card(1, Suit.Diamonds, Value.Eight),
                                          new Card(1, Suit.Spades, Value.Nine),
                                          new Card(1, Suit.Clubs, Value.Ten),
                                      };

            _classUnderTest = new Game(testPlayers, testBanker, 1, testShoe);

            Assert.AreEqual(14, _classUnderTest.Shoe.Count, "Unexpected number of cards in shoe before initial dealing");
            Assert.AreEqual(0, _classUnderTest.Rounds, "Unexpected number of rounds before gameplay starts");
            Assert.AreEqual(RoundState.NotStarted, _classUnderTest.State, "Unexpected state before gameplay starts");

            // act
            _classUnderTest.PlayRound();

            // Assert
            Assert.AreEqual(3, _classUnderTest.Players[0].Hand.Count, "Unexpected number of cards for player 1");
            Assert.AreEqual(3, _classUnderTest.Players[1].Hand.Count, "Unexpected number of cards for player 2");
            Assert.AreEqual(3, _classUnderTest.Players[2].Hand.Count, "Unexpected number of cards for player 3");
            Assert.AreEqual(3, _classUnderTest.Players[3].Hand.Count, "Unexpected number of cards for player 4");

            Assert.AreEqual(PlayerState.Bust, _classUnderTest.Players[0].State, "Unexpected state for player 1");
            Assert.AreEqual(PlayerState.Bust, _classUnderTest.Players[1].State, "Unexpected state for player 2");
            Assert.AreEqual(PlayerState.Bust, _classUnderTest.Players[2].State, "Unexpected state for player 3");
            Assert.AreEqual(PlayerState.Bust, _classUnderTest.Players[3].State, "Unexpected state for player 4");

            Assert.AreEqual(0, _classUnderTest.Shoe.Count, "Unexpected number of card in show after first round - all cards should have been dealt");

            Assert.AreEqual(1, _classUnderTest.Rounds, "Unexpected number of rounds after first round");
            Assert.AreEqual(RoundState.Finished, _classUnderTest.State, "Unexpected state after first round");
        }

        [TestMethod]
        public void GameTests_GoRoundTable_AllPlayersHitTakeLotsOfCardsThenStandAt21()
        {
            // arrange
            List<Player> testPlayers = new List<Player>
                                           {
                                               new Player("One", 1),
                                               new Player("Two", 1),
                                               new Player("Three", 1),
                                               new Player("Four", 1)
                                           };
            Player testBanker = new Player("Banker", 0, true);

            // cards are dealt from the top of the stack i.e. from the end
            List<Card> testShoe = new List<Card>
                                      {
                                          // 3 hits for player 4
                                          new Card(1, Suit.Hearts, Value.Seven),
                                          new Card(1, Suit.Hearts, Value.Five),
                                          new Card(1, Suit.Hearts, Value.Four),

                                          // 3 hits for player 3
                                          new Card(1, Suit.Diamonds, Value.Seven),
                                          new Card(1, Suit.Diamonds, Value.Five),
                                          new Card(1, Suit.Diamonds, Value.Four),

                                          // 3 hits for player 2
                                          new Card(1, Suit.Spades, Value.Seven),
                                          new Card(1, Suit.Spades, Value.Five),
                                          new Card(1, Suit.Spades, Value.Four),

                                          // 3 hits for player 1
                                          new Card(1, Suit.Clubs, Value.Seven),
                                          new Card(1, Suit.Clubs, Value.Five),
                                          new Card(1, Suit.Clubs, Value.Four),

                                          // second card
                                          new Card(1, Suit.Hearts, Value.Ace),
                                          new Card(1, Suit.Hearts, Value.Three),
                                          new Card(1, Suit.Diamonds, Value.Three),
                                          new Card(1, Suit.Spades, Value.Three),
                                          new Card(1, Suit.Clubs, Value.Three),

                                          // first card
                                          new Card(1, Suit.Hearts, Value.King),
                                          new Card(1, Suit.Hearts, Value.Two),
                                          new Card(1, Suit.Diamonds, Value.Two),
                                          new Card(1, Suit.Spades, Value.Two),
                                          new Card(1, Suit.Clubs, Value.Two)
                                      };

            _classUnderTest = new Game(testPlayers, testBanker, 1, testShoe);

            Assert.AreEqual(22, _classUnderTest.Shoe.Count, "Unexpected number of cards in shoe before initial dealing");
            Assert.AreEqual(0, _classUnderTest.Rounds, "Unexpected number of rounds before gameplay starts");
            Assert.AreEqual(RoundState.NotStarted, _classUnderTest.State, "Unexpected state before gameplay starts");

            // act
            _classUnderTest.PlayRound();

            // Assert
            Assert.AreEqual(5, _classUnderTest.Players[0].Hand.Count, "Unexpected number of cards for player 1");
            Assert.AreEqual(5, _classUnderTest.Players[1].Hand.Count, "Unexpected number of cards for player 2");
            Assert.AreEqual(5, _classUnderTest.Players[2].Hand.Count, "Unexpected number of cards for player 3");
            Assert.AreEqual(5, _classUnderTest.Players[3].Hand.Count, "Unexpected number of cards for player 4");

            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Players[0].State, "Unexpected state for player 1");
            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Players[1].State, "Unexpected state for player 2");
            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Players[2].State, "Unexpected state for player 3");
            Assert.AreEqual(PlayerState.Standing, _classUnderTest.Players[3].State, "Unexpected state for player 4");

            Assert.AreEqual(0, _classUnderTest.Shoe.Count, "Unexpected number of card in show after first round - all cards should have been dealt");

            Assert.AreEqual(1, _classUnderTest.Rounds, "Unexpected number of rounds after first round");
            Assert.AreEqual(RoundState.Finished, _classUnderTest.State, "Unexpected state after first round");
        }

        [TestMethod]
        public void GameTests_PayPlayer_BankerIsDebitedAndPlayerIsCredited()
        {
            // arrange
            Player testPlayer = new Player("One");

            List<Player> testPlayers = new List<Player>
                                           {
                                               testPlayer
                                           };
            Player testBanker = new Player("Banker", isBanker: true);

            List<Card> testShoe = HelperFunctions.CreateCards(1);

            _classUnderTest = new Game(testPlayers, testBanker, TestStake, testShoe);

            // assert 1
            Assert.AreEqual(0, _classUnderTest.Banker.Balance, "Unexpected initial banker balance");
            Assert.AreEqual(0, _classUnderTest.Players[0].Balance, "Unexpected initial player balance");

            // act
            _classUnderTest.PayPlayer(testPlayer, 1);

            // assert 2
            Assert.AreEqual(-1, _classUnderTest.Banker.Balance, "Unexpected banker balance after payment");
            Assert.AreEqual(1, _classUnderTest.Players[0].Balance, "Unexpected player balance after payment");
        }

        [TestMethod]
        public void GameTests_PayEqualOrGreaterThanBanker_Various()
        {
            // arrange

            List<Player> testPlayers = new List<Player>
                                           {
                                               new Player("One", 1)
                                                   {
                                                       State = PlayerState.Bust,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.Ten),
                                                                      new Card(1, Suit.Clubs, Value.Six),
                                                                      new Card(1, Suit.Clubs, Value.Seven)
                                                                  }
                                                   },
                                               new Player("Two", 1)
                                                   {
                                                       State = PlayerState.Standing,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.Ace),
                                                                      new Card(1, Suit.Clubs, Value.Nine)
                                                                  }
                                                   },
                                               new Player("Three", 1)
                                                   {
                                                       State = PlayerState.Standing,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.Nine),
                                                                      new Card(1, Suit.Clubs, Value.Ten)
                                                                  }
                                                   },
                                               new Player("Four", 1)
                                                   {
                                                       State = PlayerState.Standing,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.Ten),
                                                                      new Card(1, Suit.Clubs, Value.Seven)
                                                                  }
                                                   },
                                           };

            Player testBanker = new Player("Banker", 0, true)
                                    {
                                        Hand = new List<Card>
                                                   {
                                                       new Card(1, Suit.Clubs, Value.Ten),
                                                       new Card(1, Suit.Clubs, Value.Nine)
                                                   }
                                    };

            // cards are dealt from the top of the stack i.e. from the end
            List<Card> testShoe = HelperFunctions.CreateCards(1);

            _classUnderTest = new Game(testPlayers, testBanker, 1, testShoe);

            // act
            _classUnderTest.PayEqualOrGreaterThanBanker();

            // assert
            Assert.AreEqual(1, _classUnderTest.Players[0].Balance, "Unexpected player 1 balance. Player went bust. There should be no change");
            Assert.AreEqual(3, _classUnderTest.Players[1].Balance, "Unexpected player 2 balance. Player should be paid 2 x stake");
            Assert.AreEqual(2, _classUnderTest.Players[2].Balance, "Unexpected player 3 balance. Player should have received their stake back");
            Assert.AreEqual(1, _classUnderTest.Players[3].Balance, "Unexpected player 4 balance. Player lost against banker. There should be no change");
        }


        [TestMethod]
        public void GameTests_PayAllOtherBlackjacks_Various()
        {
            // arrange

            List<Player> testPlayers = new List<Player>
                                           {
                                               new Player("One", 1)
                                                   {
                                                       State = PlayerState.Bust,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.Ten),
                                                                      new Card(1, Suit.Clubs, Value.Six),
                                                                      new Card(1, Suit.Clubs, Value.Seven)
                                                                  }
                                                   },
                                               new Player("Two", 1)
                                                   {
                                                       State = PlayerState.Standing,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.Ace),
                                                                      new Card(1, Suit.Clubs, Value.Nine)
                                                                  }
                                                   },
                                               new Player("Three", 1)
                                                   {
                                                       State = PlayerState.Standing,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.King),
                                                                      new Card(1, Suit.Clubs, Value.Ace)
                                                                  }
                                                   },
                                               new Player("Four", 1)
                                                   {
                                                       State = PlayerState.Standing,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.Ten),
                                                                      new Card(1, Suit.Clubs, Value.Seven)
                                                                  }
                                                   },
                                           };

            Player testBanker = new Player("Banker", 0, true)
                                    {
                                        Hand = new List<Card>
                                                   {
                                                       new Card(1, Suit.Clubs, Value.Queen),
                                                       new Card(1, Suit.Clubs, Value.Ace)
                                                   }
                                    };

            // cards are dealt from the top of the stack i.e. from the end
            List<Card> testShoe = HelperFunctions.CreateCards(1);

            _classUnderTest = new Game(testPlayers, testBanker, 1, testShoe);

            // act
            _classUnderTest.PayAllOtherBlackjacks();

            Assert.AreEqual(1, _classUnderTest.Players[0].Balance, "Unexpected player 1 balance. Player went bust. There should be no change");
            Assert.AreEqual(1, _classUnderTest.Players[1].Balance, "Unexpected player 2 balance. Player didn't have blackjack. Should be no change");
            Assert.AreEqual(2, _classUnderTest.Players[2].Balance, "Unexpected player 3 balance. Player should have received their stake back");
            Assert.AreEqual(1, _classUnderTest.Players[3].Balance, "Unexpected player 4 balance. Player didn't have blackjack. Should be no change");
        }

        [TestMethod]
        public void GameTests_PayEveryoneStillStanding2To1_Various()
        {
            // arrange

            List<Player> testPlayers = new List<Player>
                                           {
                                               new Player("One", 1)
                                                   {
                                                       State = PlayerState.Bust,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.Ten),
                                                                      new Card(1, Suit.Clubs, Value.Six),
                                                                      new Card(1, Suit.Clubs, Value.Seven)
                                                                  }
                                                   },
                                               new Player("Two", 1)
                                                   {
                                                       State = PlayerState.Standing,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.Ace),
                                                                      new Card(1, Suit.Clubs, Value.Nine)
                                                                  }
                                                   },
                                               new Player("Three", 1)
                                                   {
                                                       State = PlayerState.Standing,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.King),
                                                                      new Card(1, Suit.Clubs, Value.Ace)
                                                                  }
                                                   },
                                               new Player("Four", 1)
                                                   {
                                                       State = PlayerState.Standing,
                                                       Hand = new List<Card>
                                                                  {
                                                                      new Card(1, Suit.Clubs, Value.Ten),
                                                                      new Card(1, Suit.Clubs, Value.Seven)
                                                                  }
                                                   },
                                           };

            Player testBanker = new Player("Banker", 0, true)
                                    {
                                        Hand = new List<Card>
                                                   {
                                                       new Card(1, Suit.Clubs, Value.Queen),
                                                       new Card(1, Suit.Clubs, Value.Six),
                                                       new Card(1, Suit.Clubs, Value.Six)
                                                   },
                                        State = PlayerState.Bust
                                    };

            // cards are dealt from the top of the stack i.e. from the end
            List<Card> testShoe = HelperFunctions.CreateCards(1);

            _classUnderTest = new Game(testPlayers, testBanker, 1, testShoe);

            // act
            _classUnderTest.PayEveryoneStillStanding2To1();

            Assert.AreEqual(1, _classUnderTest.Players[0].Balance, "Unexpected player 1 balance. Player went bust. There should be no change");
            Assert.AreEqual(3, _classUnderTest.Players[1].Balance, "Unexpected player 2 balance. Player should have received twice their stake");
            Assert.AreEqual(3, _classUnderTest.Players[2].Balance, "Unexpected player 3 balance. Player should have received twice their stake");
            Assert.AreEqual(3, _classUnderTest.Players[3].Balance, "Unexpected player 4 balance. Player should have received twice their stake");
        }
    }

    [ExcludeFromCodeCoverage]
    public static class ExtensionMethods
    {
        public static bool IsSameCard(this Card a, Card b)
        {
            return a.Deck == b.Deck && a.Suit == b.Suit && a.Value == b.Value;
        }
    }
}
