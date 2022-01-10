using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService_Lib.DB;

namespace WebService_Lib.Cards
{
    public class Battle
    {
        public static string battle(string battleId, string firstUsername, string secondUsername, Stack firstUser, Stack secondUser)
        {
            string ret = "";
            string msgTemplate = "[FIGHT] {0}'s {1} hat mit {2} Damage gegen {3}'s {4} gewonnen mit {5} damage.\n";
            int roundCount = 0;
            Random rand = new Random();
            while (firstUser.cardCount() != 0 && secondUser.cardCount() != 0 && roundCount != 100)
            {
                Card randomFirst = firstUser.randomCard(rand);
                Card randomSecond = secondUser.randomCard(rand);

                double firstMultiplicator = 1.0;
                double secondMultiplicator = 1.0;

                // Goblin afraid of dragon
                if (randomFirst.getName().ToLower().Contains("goblin") && randomSecond.getName().ToLower().Contains("dragon"))
                    firstMultiplicator = 0.0;
                else if (randomSecond.getName().ToLower().Contains("goblin") && randomFirst.getName().ToLower().Contains("dragon"))
                    secondMultiplicator = 0.0;

                // Ork cant damage wizzard
                if (randomFirst.getName().ToLower().Contains("ork") && randomSecond.getSpell())
                    firstMultiplicator = 0.0;
                else if (randomSecond.getName().ToLower().Contains("ork") && randomFirst.getSpell())
                    secondMultiplicator = 0.0;

                // Knights drown
                if (randomFirst.getName().ToLower().Contains("knight") && randomSecond.getSpell() && randomSecond.getName().ToLower().Contains("Water"))
                    firstMultiplicator = 0.0;
                else if (randomSecond.getName().ToLower().Contains("knight") && randomFirst.getSpell() && randomFirst.getName().ToLower().Contains("Water"))
                    secondMultiplicator = 0.0;

                // kraken immune
                if (randomFirst.getName().ToLower().Contains("kraken") && randomSecond.getSpell())
                    secondMultiplicator = 0.0;
                else if (randomSecond.getName().ToLower().Contains("kraken") && randomFirst.getSpell())
                    firstMultiplicator = 0.0;

                // FireElves friends with dragon
                if (randomFirst.getName().ToLower().Contains("fireelve") && randomSecond.getName().ToLower().Contains("dragon"))
                    secondMultiplicator = 0.0;
                else if (randomSecond.getName().ToLower().Contains("fireelve") && randomFirst.getName().ToLower().Contains("dragon"))
                    firstMultiplicator = 0.0;

                if (!randomFirst.getSpell() && !randomSecond.getSpell())
                {
                    if (randomFirst.getDamage() * firstMultiplicator > randomSecond.getDamage() * secondMultiplicator)
                    {
                        secondUser.removeCard(randomSecond);
                        firstUser.addCard(randomSecond);
                        ret += String.Format(msgTemplate, firstUsername, randomFirst.getName(), randomFirst.getDamage() * firstMultiplicator, secondUsername, randomSecond.getName(), randomSecond.getDamage() * secondMultiplicator);
                    }
                    else if (randomFirst.getDamage() * firstMultiplicator < randomSecond.getDamage() * secondMultiplicator)
                    {
                        firstUser.removeCard(randomFirst);
                        secondUser.addCard(randomFirst);
                        ret += String.Format(msgTemplate, secondUsername, randomSecond.getName(), randomSecond.getDamage() * secondMultiplicator, firstUsername, randomFirst.getName(), randomFirst.getDamage() * firstMultiplicator);
                    }
                }
                else
                {
                    if ((randomFirst.getType() == ElementType.Water && randomSecond.getType() == ElementType.Fire) ||
                        (randomSecond.getType() == ElementType.Water && randomFirst.getType() == ElementType.Fire))
                    {
                        if (randomFirst.getType() == ElementType.Water)
                        {
                            firstMultiplicator *= 2.0;
                            secondMultiplicator *= 0.5;
                        }
                        else
                        {
                            firstMultiplicator *= 0.5;
                            secondMultiplicator *= 2.0;
                        }
                    }
                    else if ((randomFirst.getType() == ElementType.Fire && randomSecond.getType() == ElementType.Normal) ||
                            (randomSecond.getType() == ElementType.Fire && randomFirst.getType() == ElementType.Normal))
                    {
                        if (randomFirst.getType() == ElementType.Fire)
                        {
                            firstMultiplicator *= 2.0;
                            secondMultiplicator *= 0.5;
                        }
                        else
                        {
                            firstMultiplicator *= 0.5;
                            secondMultiplicator *= 2.0;
                        }
                    }
                    else if ((randomFirst.getType() == ElementType.Water && randomSecond.getType() == ElementType.Normal) ||
                            (randomSecond.getType() == ElementType.Water && randomFirst.getType() == ElementType.Normal))
                    {
                        if (randomFirst.getType() == ElementType.Normal)
                        {
                            firstMultiplicator *= 2.0;
                            secondMultiplicator *= 0.5;
                        }
                        else
                        {
                            firstMultiplicator *= 0.5;
                            secondMultiplicator *= 2.0;
                        }
                    }
                    if (randomFirst.getDamage() * firstMultiplicator > randomSecond.getDamage() * secondMultiplicator)
                    {
                        secondUser.removeCard(randomSecond);
                        firstUser.addCard(randomSecond);
                        ret += String.Format(msgTemplate, firstUsername, randomFirst.getName(), randomFirst.getDamage() * firstMultiplicator, secondUsername, randomSecond.getName(), randomSecond.getDamage() * secondMultiplicator);
                    }
                    else if (randomFirst.getDamage() * firstMultiplicator < randomSecond.getDamage() * secondMultiplicator)
                    {
                        firstUser.removeCard(randomFirst);
                        secondUser.addCard(randomFirst);
                        ret += String.Format(msgTemplate, secondUsername, randomSecond.getName(), randomSecond.getDamage() * secondMultiplicator, firstUsername, randomFirst.getName(), randomFirst.getDamage() * firstMultiplicator);
                    }
                }
                System.Console.WriteLine("Round: " + roundCount);
                roundCount++;
            }
            string winner = "";
            if (roundCount == 100)
                ret += "!!!DRAW (too many rounds)!!!";
            else if (firstUser.cardCount() == 0)
            {
                winner = secondUsername;
                secondUser.saveStack(winner);
                Database.changeElo(winner, 3);
                Database.changeElo(firstUsername, -5);
                ret += String.Format("{0} won, {1} lost.", secondUsername, firstUsername);
            }
            else
            {
                winner = firstUsername;
                firstUser.saveStack(winner);
                Database.changeElo(winner, 3);
                Database.changeElo(secondUsername, -5);
                ret += String.Format("{0} won, {1} lost.", firstUsername, secondUsername);
            }
            Database.addBattle(battleId, firstUsername, secondUsername, ret, winner);
            return ret;
        }
    }
}
