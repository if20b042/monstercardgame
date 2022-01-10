using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebService_Lib.DB;

namespace WebService_Lib.Cards
{
    public class Stack
    {
        private List<Card> cards;

        public Stack()
        {
            cards = new List<Card>();
        }

        public void addCard(Card c)
        {
            cards.Add(c);
        }

        public void removeCard(Card c)
        {
            cards.Remove(c);
        }

        public static Stack getPlayerStack(string userID)
        {
            return Database.getUserStack(userID);
        }

        public static Stack getPlayerDeck(string userID)
        {
            return Database.getUserDeck(userID);
        }

        public static bool setPlayerDeck(string userID, Dictionary<string, object> cards)
        {
            return Database.setUserDeck(userID, cards);
        }

        public List<Dictionary<string, object>> ToList()
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (Card c in cards)
            {
                Dictionary<string, object> cDic = new Dictionary<string, object>();
                cDic["name"] = c.getName();
                cDic["damage"] = c.getDamage();
                cDic["type"] = c.getType();
                cDic["spell"] = c.getSpell();
                list.Add(cDic);
            }
            return list;
        }

        public int cardCount()
        {
            return cards.Count();
        }

        public Card randomCard(Random rand)
        {
            int index = rand.Next(cardCount());
            return cards[index];
        }

        public void saveStack(string username)
        {
            foreach(Card card in cards)
                Database.setCardOwner(card.getID(), username);
        }
    }
}