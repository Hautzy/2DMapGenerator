using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Blocks;
using Assets.Scripts.Items;

namespace Assets.Scripts
{
    public class PrefabRepository
    {
        private static PrefabRepository _instance;

        public const string PrefabsFolderPath = "Prefabs/";

        public static PrefabRepository Instance
        {
            get { return _instance ?? (_instance = new PrefabRepository()); }
        }

        public Dictionary<BlockTypes, BlockDefinition> BlockDefinitions { get; set; }
        public Dictionary<ItemTypes, ItemDefinition> ItemDefinitions { get; set; }

		public GameObject GetBlockPrefab(BlockTypes type)
	   	{
	      	return BlockDefinitions[type].Prefab;
	   	}

	   	public GameObject GetItemPrefab (ItemTypes type)
		{
			return ItemDefinitions[type].Prefab;
		}

        public GameObject Map { get; private set; }
        public GameObject Player { get; private set; }
        public GameObject Generator { get; set; }

        public GameObject TxtMouseXCoord { get; set; }
        public GameObject TxtMouseYCoord { get; set; }
        public GameObject TxtBlockType { get; set; }

        public World World {
			get;
			set;
		}

        public GameObject BlockSelector { get; set; }

        private PrefabRepository()
        {
            LoadGameObjects();
            LoadBlockPrefabs();
            LoadItemPrefabs();
            LoadGameUI();

            DefineItemDrops();
        }

        private void DefineItemDrops ()
		{
			//Grass block -> 1 x Grass block drop
			BlockDefinitions[BlockTypes.Grass].ItemDrop = new ItemDrop(ItemDefinitions[ItemTypes.GrassDrop], 1);
		}

        private void LoadGameUI()
        {
            TxtMouseXCoord = GameObject.Find("TxtMouseXCoord");
            TxtMouseYCoord = GameObject.Find("TxtMouseYCoord");
            TxtBlockType = GameObject.Find("TxtBlockType");
        }

        private void LoadGameObjects()
        {
            Map = GameObject.Find("Map");
            Player = GameObject.Find("Player");
            Generator = GameObject.Find("Generator");
        }

        private void LoadItemPrefabs ()
		{
            ItemDefinitions = new Dictionary<ItemTypes, ItemDefinition>();

			LoadSingleItemPrefab("GrassDrop", ItemTypes.GrassDrop);
			LoadSingleItemPrefab("DirtDrop", ItemTypes.DirtDrop);
            LoadSingleItemPrefab("StoneDrop", ItemTypes.StoneDrop);
		}

        private void LoadBlockPrefabs()
        {
            BlockDefinitions = new Dictionary<BlockTypes, BlockDefinition>();

            LoadSingleBlockPrefab("Dirt", BlockTypes.Dirt);
            LoadSingleBlockPrefab("Stone", BlockTypes.Stone);
            LoadSingleBlockPrefab("Grass", BlockTypes.Grass);
            LoadSingleBlockPrefab("Tree", BlockTypes.Tree);
            LoadSingleBlockPrefab("Ore", BlockTypes.Ore);

            BlockSelector = Resources.Load(PrefabsFolderPath + "BlockSelector") as GameObject;
        }

        private void LoadSingleBlockPrefab(string name, BlockTypes type)
        {
            BlockDefinitions.Add(type, new BlockDefinition(
            	name,
            	type,
            	Resources.Load(PrefabsFolderPath + name) as GameObject
        	));
        }

        private void LoadSingleItemPrefab (string name, ItemTypes type)
		{
			ItemDefinitions.Add(type, new ItemDefinition(
				name, 
				type,
				Resources.Load(PrefabsFolderPath + name) as GameObject
			));
		}
    }
}
