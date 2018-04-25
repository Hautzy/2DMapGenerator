using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
	public class ItemDefinition
	{
		public string Name {
			get;
			set;
		}
		public GameObject Prefab {
			get;
			set;
		}
		public ItemTypes ItemType {
			get;
			set;
		}
		public ItemDefinition (string name, ItemTypes itemType, GameObject prefab)
		{
			Name = name;
			Prefab = prefab;
			ItemType = itemType;
		}
	}
}

