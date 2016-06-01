namespace CardTable
{
    public enum Value
    {
        None = 0,
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public enum Suit
    {
        None = 0,
        Spades = 1,
        Diamonds = 2,
        Clubs = 3,
        Hearts = 4
    }

    public class Card // : IComparable<Card>, IEquatable<Card>, IComparer<Card>, IEqualityComparer<Card>
    {
        public Card(int deck, Suit suit, Value value)
        {
            Deck = deck;
            Suit = suit;
            Value = value;
        }

        public Value Value { get; }
        public Suit Suit { get; }
        public int Deck { get; }

        public string ToCode()
        {
            string code = string.Empty;

            switch (Value)
            {
                case Value.Ace:
                    {
                        code += "A";
                        break;
                    }
                case Value.Jack:
                    {
                        code += "J";
                        break;
                    }
                case Value.Queen:
                    {
                        code += "Q";
                        break;
                    }
                case Value.King:
                    {
                        code += "K";
                        break;
                    }
                default:
                    {
                        code += ((int)Value).ToString();
                        break;
                    }
            }

            switch (Suit)
            {
                case Suit.Clubs:
                    {
                        code += "♣";
                        break;
                    }
                case Suit.Diamonds:
                    {
                        code += "♦";
                        break;
                    }
                case Suit.Hearts:
                    {
                        code += "♥";
                        break;
                    }
                case Suit.Spades:
                    {
                        code += "♠";
                        break;
                    }
            }

            return code;
        }
    }
}