using System;
using Assets.Scripts;
using UnityEngine;
using Assets.Scripts.Items;

namespace Assets.Scripts.Blocks
{
	public class BlockDefinition
	{
		public string Name{ get; set; }
		public BlockTypes BlockType { get; set; }
		public GameObject Prefab { get; set; }

		// define drop
		public ItemDrop ItemDrop {
			get;
			set;
		}

		public BlockDefinition (string name, BlockTypes blockType, GameObject prefab, ItemDrop itemDrop)
		{
			Name = name;
			BlockType = blockType;
			Prefab = prefab;
			ItemDrop = itemDrop;
		}

		public BlockDefinition (string name, BlockTypes blockType, GameObject prefab)
		{
			Name = name;
			BlockType = blockType;
			Prefab = prefab;
		}
	}
}

