using System.Collections.Generic;
using System.Linq;

namespace CardTable
{
    public static class HandExtensionMethods
    {
        public static int FinalValue(this List<Card> cards)
        {
            if (cards.Any())
            {
                int handValue = 0;
                int aceCount = 0;

                foreach (Card card in cards)
                {
                    // treat all aces as 11
                    switch (card.Value)
                    {
                        case Value.Ace:
                            {
                                aceCount++;
                                handValue += 11;
                                break;
                            }
                        case Value.Jack:
                        case Value.Queen:
                        case Value.King:
                            {
                                handValue += 10;
                                break;
                            }
                        default:
                            {
                                handValue += (int)card.Value;
                                break;
                            }
                    }
                }
                
                // check if counting all aces as 11 will cause the player to go bust...
                if (aceCount >= 1 && handValue > 21)
                {
                    // switch each Ace down to 1 point by subtracting 10 for each until the hand value is <= 21
                    for (int i = 1; i <= aceCount; i++)
                    {
                        handValue -= 10;
                        if (handValue <= 21)
                        {
                            break;
                        }
                    }
                }

                return handValue;
            }
            return 0;
        }

        public static bool IsBlackjack(this List<Card> cards)
        {
            return cards.Count == 2 && cards.ContainsAnyCourtCard() && cards.ContainsAnyAce();
        }

        public static bool ContainsAnyCourtCard(this IEnumerable<Card> cards)
        {
            return cards.Any(card => card.Value == Value.Jack || card.Value == Value.Queen || card.Value == Value.King);
        }

        public static bool ContainsAnyAce(this IEnumerable<Card> cards)
        {
            return cards.Any(card => card.Value == Value.Ace);
        }
    }
}