using System;
using Assets.Scripts.Items;
using Assets.Scripts.Blocks;

namespace Assets.Scripts.Items
{
	public class ItemDrop
	{
		public ItemDefinition Item {
			get;
			set;
		}
		public int DropCount {
			get;
			set;
		}
		public ItemDrop (ItemDefinition item, int dropCount)
		{
			Item = item;
			DropCount = dropCount;
		}
	}
}

