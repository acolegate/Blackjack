using System;
using System.Collections.Generic;
using System.Linq;

using CardTable;

namespace ConsoleApp
{
    internal static class Program
    {
        private const decimal StakePerRound = 1;
        private const decimal PlayerStartingBalance = 1;

        private static void Main(string[] args)
        {
            List<Player> players = new List<Player>
                                       {
                                           new Player("Player 1", PlayerStartingBalance),
                                           new Player("Player 2", PlayerStartingBalance)
                                           //new Player("Player 3", PlayerStartingBalance),
                                           //new Player("Player 4", PlayerStartingBalance)
                                       };

            Player banker = new Player("Banker", isBanker: true);

            List<Card> shoe = HelperFunctions.CreateCards(1).Shuffle();

            using (Game game = new Game(players, banker, StakePerRound, shoe))
            {
                game.Message += Game_Message;

                while (players.Any(x => x.Balance >= StakePerRound))
                {
                    game.PlayRound();
                    Pause();
                }
            }
        }

        private static void Pause()
        {
            Console.WriteLine("Press any key to continue . . .");
            Console.ReadKey(true);
        }

        private static void Game_Message(object sender, MessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}