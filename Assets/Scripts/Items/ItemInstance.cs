using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
	public class ItemInstance
	{
		public ItemTypes ItemType { get; set; }
		public GameObject GameObject { get; set; }

		public ItemInstance (ItemTypes itemType, GameObject gameObject)
		{
			ItemType = itemType;
			GameObject = gameObject;
		}


	}
}

