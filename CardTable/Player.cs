using System.Collections.Generic;

namespace CardTable
{
    public enum PlayerState
    {
        InPlay = 1,
        Standing = 2,
        Bust = 3,
        OutOfFunds = 4,
        IsBanker = 5
    }

    public enum PlayerAction
    {
        Hit = 1,
        Stand = 2,
        //Surrender = 3,
        //Insure = 4,
        //Split = 5,
        //DoubleDown = 6,
        Bust = 7
    }

    public class Player
    {
        public Player(string name, decimal balance = 0, bool isBanker = false)
        {
            Name = name;
            Balance = balance;

            if (isBanker)
            {
                State = PlayerState.IsBanker;
                Balance = 0;
            }
            else
            {
                State = balance > 0 ? PlayerState.InPlay : PlayerState.OutOfFunds;
            }

            Hand = new List<Card>();
        }

        public string Name { get; }
        public List<Card> Hand { get; set; }
        public PlayerState State { get; set; }
        public decimal Balance { get; private set; }

        public PlayerAction Decide
        {
            get
            {
                if (Hand.FinalValue() > 21)
                {
                    return PlayerAction.Bust;
                }
                if (Hand.IsBlackjack())
                {
                    return PlayerAction.Stand;
                }
                // ReSharper disable once ConvertIfStatementToReturnStatement
                if (Hand.FinalValue() >= 17)
                {
                    return PlayerAction.Stand;
                }

                return PlayerAction.Hit;
            }
        }

        public void DebitBalance(decimal amount)
        {
            Balance -= amount;
        }

        public void CreditBalance(decimal amount)
        {
            Balance += amount;
        }
    }
}