using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Items;

namespace Assets.Scripts.InventorySystem
{
    public class InventoryItem
    {
        public ItemDefinition ItemDefinition { get; set; }
        public int Count { get; set; }

        public InventoryItem(ItemDefinition itemDefinition, int count)
        {
            ItemDefinition = itemDefinition;
            Count = count;
        }
    }
}
