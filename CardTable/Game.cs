using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace CardTable
{
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum RoundState
    {
        NotStarted = 1,
        InPlay = 2,
        //WaitingForPlayer = 3,
        Finished = 4
    }

    public delegate void MessageEventHandler(object sender, MessageEventArgs e);

    public sealed class Game : IDisposable
    {
        private readonly int _totalCards;

        public Game(List<Player> players, Player banker, decimal stakePerRound, List<Card> shoe)
        {
            Players = players;
            StakePerRound = stakePerRound;
            Shoe = shoe;
            State = RoundState.NotStarted;
            Rounds = 0;
            Banker = banker;
            DiscardPile = new List<Card>(shoe.Count);
            _totalCards = shoe.Count;
        }

        internal Player Banker { get; private set; }
        internal List<Player> Players { get; private set; }
        internal decimal StakePerRound { get; }
        internal List<Card> Shoe { get; private set; }
        internal List<Card> DiscardPile { get; private set; }
        internal RoundState State { get; private set; }
        internal int Rounds { get; private set; }

        public void Dispose()
        {
            Players = null;
            Shoe = null;
            Banker = null;
            DiscardPile = null;

            RaiseMessageEvent("Game disposed");
        }

        public event MessageEventHandler Message;

        internal Card GetTopCardFromShoe()
        {
            int cardsInShoe = Shoe.Count;
            if (cardsInShoe == 0)
            {
                // take the discard pile and shuffle it
                Shoe.AddRange(DiscardPile.Shuffle());
                DiscardPile = new List<Card>(Shoe.Count);
                cardsInShoe = Shoe.Count;
            }

            Card card = Shoe[cardsInShoe - 1];
            Shoe.RemoveAt(cardsInShoe - 1);
            return card;
        }

        public void PlayRound()
        {
            RaiseMessageEvent("= BEGIN ROUND ==================================================================");

            DiscardHands();
            RaiseMessageEvent("Discarding hands\r\n");

            RaiseMessageEvent(BuildMessage(MessageType.ContentsOfShoeAndDiscardPile));

            Banker.State = PlayerState.IsBanker;

            Rounds++;
            RaiseMessageEvent("Round:" + Rounds);

            State = RoundState.InPlay;
            TakeStakes();
            RaiseMessageEvent("Stakes taken");

            DealInitialCards();
            RaiseMessageEvent("Initial cards dealt");

            RaiseShowHandsMessage();

            // Player's turns
            foreach (Player player in Players.Where(player => player.State == PlayerState.InPlay))
            {
                PlayHand(player);
            }

            //Banker's turn
            if (Players.Any(x => x.State == PlayerState.Standing))
            {
                // there's someone standing
                // play the banker's hand

                PlayHand(Banker);

                switch (Banker.State)
                {
                    case PlayerState.Bust:
                        {
                            // pay everyone still standing
                            PayEveryoneStillStanding2To1();
                            break;
                        }
                    case PlayerState.Standing:
                        {
                            // compare all standing player's hands with banker's

                            if (Banker.Hand.IsBlackjack())
                            {
                                RaiseMessageEvent("Banker has blackjack");

                                // check if anyone else has blackjack
                                PayAllOtherBlackjacks();
                            }
                            else
                            {
                                // banker doesn't have blackjack
                                PayEqualOrGreaterThanBanker();
                            }
                            break;
                        }
                }
            }
            else
            {
                // everyone's gone bust - no payouts
                RaiseMessageEvent("Everyone's gone bust - no payouts");
            }

            State = RoundState.Finished;

            RaiseShowHandsMessage();
        }

        private void DiscardHands()
        {
            // Discard Player's cards
            foreach (Player player in Players)
            {
                DiscardCards(player.Hand);
                player.Hand = new List<Card>();
            }

            // discard banker's cards
            DiscardCards(Banker.Hand);
            Banker.Hand = new List<Card>();

            // make sure all cards are accounted for
            if (Shoe.Count + DiscardPile.Count != _totalCards)
            {
                throw new ApplicationException("Cards(s) have gone missing");
            }
        }

        private void DiscardCards(IEnumerable<Card> cards)
        {
            DiscardPile.AddRange(cards);
        }

        internal void DiscardCard(Card card)
        {
            DiscardPile.Add(card);
        }

        internal void PayEqualOrGreaterThanBanker()
        {
            int bankersHandValue = Banker.Hand.FinalValue();

            RaiseMessageEvent("Banker's cards total " + bankersHandValue);

            foreach (Player player in Players.Where(x => x.State == PlayerState.Standing))
            {
                int playersHandValue = player.Hand.FinalValue();

                if (playersHandValue == bankersHandValue)
                {
                    // same - player gets their money back
                    PayPlayer(player, StakePerRound);

                    RaiseMessageEvent(player.Name + " has same card value as banker. Player gets their money back.");
                }
                else
                {
                    if (playersHandValue > bankersHandValue)
                    {
                        // player has higher than banker. Pay 2:1
                        PayPlayer(player, StakePerRound * 2);

                        RaiseMessageEvent(player.Name + " has better cards than the banker. Player gets 2x their stake back (" + StakePerRound * 2 + ")");
                    }
                    else
                    {
                        // player has lower than banker - they lose
                        RaiseMessageEvent(player.Name + " has lower cards than the banker. Player doesn't win anything");
                    }
                }
            }
        }

        internal void PayAllOtherBlackjacks()
        {
            foreach (Player player in Players.Where(x => x.State == PlayerState.Standing))
            {
                if (player.Hand.IsBlackjack())
                {
                    // give the player his money back
                    PayPlayer(player, StakePerRound);

                    RaiseMessageEvent("Banker and " + player.Name + " have blackjack. Player gets their stake back");
                }
                else
                {
                    RaiseMessageEvent(player.Name + " does not have blackjack so wins nothing");
                }
            }
        }

        internal void PayEveryoneStillStanding2To1()
        {
            foreach (Player player in Players.Where(x => x.State == PlayerState.Standing))
            {
                // pay 2:1
                // 1 from table and 1 from banker
                PayPlayer(player, StakePerRound * 2);

                RaiseMessageEvent(player.Name + " is paid " + StakePerRound * 2);
            }
        }

        internal void PayPlayer(Player player, decimal amount)
        {
            Banker.DebitBalance(amount);
            player.CreditBalance(amount);
        }

        private void PlayHand(Player player)
        {
            while (player.State == PlayerState.InPlay || player.State == PlayerState.IsBanker)
            {
                switch (player.Decide)
                {
                    case PlayerAction.Hit:
                        {
                            HitPlayer(player);
                            RaiseMessageEvent("Hit " + player.Name + " with " + player.Hand.Last().ToCode());
                            break;
                        }
                    case PlayerAction.Stand:
                        {
                            player.State = PlayerState.Standing;
                            break;
                        }
                    case PlayerAction.Bust:
                        {
                            player.State = PlayerState.Bust;
                            break;
                        }
                }
            }

            RaiseMessageEvent(BuildMessage(MessageType.PlayerStats, player));
        }

        internal void HitPlayer(Player player)
        {
            player.Hand.Add(GetTopCardFromShoe());
        }

        internal void TakeStakes()
        {
            foreach (Player player in Players.Where(player => player.State != PlayerState.OutOfFunds))
            {
                if (player.Balance >= StakePerRound)
                {
                    TakeStakeFromPlayer(player);
                    player.State = PlayerState.InPlay;
                }
                else
                {
                    player.State = PlayerState.OutOfFunds;
                }
            }
        }

        private void TakeStakeFromPlayer(Player player)
        {
            Banker.CreditBalance(StakePerRound);
            player.DebitBalance(StakePerRound);
        }

        internal void DealInitialCards()
        {
            for (int i = 1; i <= 2; i++)
            {
                foreach (Player player in Players.Where(player => player.State != PlayerState.OutOfFunds))
                {
                    player.Hand.Add(GetTopCardFromShoe());
                }

                Banker.Hand.Add(GetTopCardFromShoe());
            }
        }

        #region Messages
        private void RaiseMessageEvent(string message)
        {
            Message?.Invoke(this, new MessageEventArgs(message));
        }

        private void RaiseShowHandsMessage()
        {
            StringBuilder message = new StringBuilder();

            // banker
            message.AppendLine(BuildMessage(MessageType.PlayerStats, Banker));
            message.AppendLine();

            // each player
            foreach (Player player in Players)
            {
                message.AppendLine(BuildMessage(MessageType.PlayerStats, player));
                message.AppendLine();
            }

            RaiseMessageEvent(message.ToString());
        }

        private enum MessageType
        {
            ContentsOfShoeAndDiscardPile,
            PlayerStats,
            CardCodes
        }

        private string BuildMessage(MessageType messageType, object passedObject = null)
        {
            switch (messageType)
            {
                case MessageType.ContentsOfShoeAndDiscardPile:
                    {
                        //string message = "";

                        //const int Col1Width = 38;
                        //const int Col2Start = 42;

                        //int col1Pos = 0;
                        //int col2pos = Col2Start;

                        //int shoeIndex = 0;
                        //int discardPileIndex = 0;

                        //while (shoeIndex <= Shoe.Count - 1 && discardPileIndex <= DiscardPile.Count - 1)
                        //{
                        //    if (shoeIndex <= Shoe.Count - 1)
                        //    {
                        //        if (col1Pos > Col1Width - 2)
                        //        {
                        //            col1Pos = 0;
                        //        }
                        //    }
                        //}


                        return $"SHOE:{(Shoe.Any() ? BuildMessage(MessageType.CardCodes, Shoe) : "[none]")}\r\nDISCARD PILE:{(DiscardPile.Any() ? BuildMessage(MessageType.CardCodes, DiscardPile) : "[none]")}\r\n";
                    }
                case MessageType.PlayerStats:
                    {
                        Player player = (Player)passedObject;
                        return player != null ? $"{player.Name}\t{BuildMessage(MessageType.CardCodes, player.Hand)} = {player.Hand.FinalValue()}\r\nBalance:{player.Balance}\tDecision:{player.Decide}\tState:{player.State}" : null;
                    }
                case MessageType.CardCodes:
                    {
                        List<Card> cards = (List<Card>)passedObject;
                        return cards != null && cards.Any() ? cards.Aggregate(string.Empty, (current, card) => current + (card.ToCode() + " ")).Trim() : "[none]";
                    }
            }
            throw new ArgumentException($"MessageType {messageType} not supported");
        }

        #endregion
    }

    public class MessageEventArgs : EventArgs
    {
        public MessageEventArgs(string message)
        {
            Message = message;
        }

        public string Message { get; }
    }
}