using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem
{
    public class PassiveItem : ItemData
    {
        public override ItemType Type => ItemType.Passive;
    }
}
