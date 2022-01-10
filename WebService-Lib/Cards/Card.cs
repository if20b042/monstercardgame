using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebService_Lib.Cards
{
    public class Card
    {
        private readonly string _id;
        private string name;
        private readonly double damage;
        private ElementType type;
        private bool spell;

        public Card(string _id, string name, double damage)
        {
            this._id = _id;
            this.name = name;
            this.damage = damage;
            this.spell = this.name.ToLower().Contains("spell");
            if (this.name.ToLower().Contains("fire"))
                this.type = ElementType.Fire;
            else if (this.name.ToLower().Contains("water"))
                this.type = ElementType.Water;
            else
                this.type = ElementType.Normal;
        }

        public string getID()
        {
            return _id;
        }

        public string getName()
        {
            // return name + "(" + this.getID() + ")";
            return name;
        }

        public double getDamage()
        {
            return damage;
        }

        public ElementType getType()
        {
            return type;
        }

        public bool getSpell()
        {
            return spell;
        }
    }
}
