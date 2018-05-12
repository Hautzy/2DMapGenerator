using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.InventorySystem;

namespace Assets.Scripts.ItemBarSystem
{
    public class ItemBar
    {
        public const string ItemBarSlotPrefix = "ItemBar_";

        public const int SlotWidth = 6;
        public const float ItemBarBottomPadding = 200;
        public const float ItemBarLeftPadding = 750;

        public InventoryItem ItemBarSlots { get; set; }

        public bool ShowItemBar { get; set; }
        public Player Owner { get; set; }

    }
}
