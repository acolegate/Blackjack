using System.Diagnostics.CodeAnalysis;
using System.Linq;

using CardTable;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CardTableTests
{
    /// <summary>
    /// Summary description for GameTests
    /// </summary>
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PlayerTests
    {
        private Player _classUnderTest;

        [TestInitialize]
        public void Initialise()
        {
        }

        [TestMethod]
        public void PlayerTests_Constructor_Populate_ReturnsCorrectValues()
        {
            // Act/Arrange
            _classUnderTest = new Player("One", 1);

            // Assert
            Assert.AreEqual(1, _classUnderTest.Balance, "Unexpected Balance");

            Assert.IsNotNull(_classUnderTest.Hand, "Unexpected Hand");
            Assert.IsFalse(_classUnderTest.Hand.Any(), "Unexpected hand content");

            Assert.AreEqual("One", _classUnderTest.Name, "Unexpected name");
            Assert.AreEqual(PlayerState.InPlay, _classUnderTest.State, "Unexpected player state");
        }

        [TestMethod]
        public void PlayerTests_Decide_PlayerHasTwoAces_Hit()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);
            _classUnderTest.Hand.Add(new Card(1, Suit.Clubs, Value.Ace));
            _classUnderTest.Hand.Add(new Card(1, Suit.Diamonds, Value.Ace));

            // act
            PlayerAction action = _classUnderTest.Decide;

            // Assert
            Assert.AreEqual(PlayerAction.Hit, action, "Unexpected action");
        }

        [TestMethod]
        public void PlayerTests_Decide_PlayerHasTwoTwos_Hit()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);
            _classUnderTest.Hand.Add(new Card(1, Suit.Clubs, Value.Two));
            _classUnderTest.Hand.Add(new Card(1, Suit.Diamonds, Value.Two));

            // act
            PlayerAction action = _classUnderTest.Decide;

            // Assert
            Assert.AreEqual(PlayerAction.Hit, action, "Unexpected action");
        }

        [TestMethod]
        public void PlayerTests_Decide_PlayerHasTwoCourtCards_Stand()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);
            _classUnderTest.Hand.Add(new Card(1, Suit.Clubs, Value.King));
            _classUnderTest.Hand.Add(new Card(1, Suit.Diamonds, Value.Queen));

            // act
            PlayerAction action = _classUnderTest.Decide;

            // Assert
            Assert.AreEqual(PlayerAction.Stand, action, "Unexpected action");
        }

        [TestMethod]
        public void PlayerTests_Decide_PlayerHasGoneBust_GoneBust()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);
            _classUnderTest.Hand.Add(new Card(1, Suit.Clubs, Value.Nine));
            _classUnderTest.Hand.Add(new Card(1, Suit.Diamonds, Value.Seven));
            _classUnderTest.Hand.Add(new Card(1, Suit.Spades, Value.Six));

            // act
            PlayerAction action = _classUnderTest.Decide;

            // Assert
            Assert.AreEqual(PlayerAction.Bust, action, "Unexpected action");
        }

        [TestMethod]
        public void PlayerTests_Decide_PlayerHasBlackjack_Stand()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);
            _classUnderTest.Hand.Add(new Card(1, Suit.Clubs, Value.Ace));
            _classUnderTest.Hand.Add(new Card(1, Suit.Diamonds, Value.King));

            // act
            PlayerAction action = _classUnderTest.Decide;

            // Assert
            Assert.AreEqual(PlayerAction.Stand, action, "Unexpected action");
        }

        [TestMethod]
        public void PlayerTests_DebitBalanceWithPositive_BalanceIsDebited()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);

            // Act
            _classUnderTest.DebitBalance(1);

            // Assert
            Assert.AreEqual(0, _classUnderTest.Balance, "Unexpected balance");
        }

        [TestMethod]
        public void PlayerTests_DebitBalanceWithNegative_BalanceIsCredited()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);

            // Act
            _classUnderTest.DebitBalance(-1);

            // Assert
            Assert.AreEqual(2, _classUnderTest.Balance, "Unexpected balance");
        }

        [TestMethod]
        public void PlayerTests_DebitBalanceWithZero_BalanceIsUnchanged()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);

            // Act
            _classUnderTest.DebitBalance(0);

            // Assert
            Assert.AreEqual(1, _classUnderTest.Balance, "Unexpected balance");
        }

        [TestMethod]
        public void PlayerTests_CreditBalanceWithPositive_BalanceIsCredited()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);

            // Act
            _classUnderTest.CreditBalance(1);

            // Assert
            Assert.AreEqual(2, _classUnderTest.Balance, "Unexpected balance");
        }

        [TestMethod]
        public void PlayerTests_CreditBalanceWithNegative_BalanceIsDebited()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);

            // Act
            _classUnderTest.CreditBalance(-1);

            // Assert
            Assert.AreEqual(0, _classUnderTest.Balance, "Unexpected balance");
        }

        [TestMethod]
        public void PlayerTests_CreditBalanceWithZero_BalanceIsUnchanged()
        {
            // Arrange
            _classUnderTest = new Player("One", 1);

            // Act
            _classUnderTest.CreditBalance(0);

            // Assert
            Assert.AreEqual(1, _classUnderTest.Balance, "Unexpected balance");
        }
    }
}