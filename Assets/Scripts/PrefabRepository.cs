using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Blocks;
using Assets.Scripts.Items;
using Assets.Scripts.WorldSystem;

namespace Assets.Scripts
{
    public class PrefabRepository
    {
        private static PrefabRepository _instance;

        public const string PrefabsFolderPath = "Prefabs/";
        public const string SpritesFolderPath = "Sprites/";
        public const string PersistInventoryName = "Inventory";
        public const string PersistItemBarName = "ItemBar";

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

        public GameObject GetItemPrefab(ItemTypes type)
        {
            return ItemDefinitions[type].Prefab;
        }

        public GameObject Map { get; private set; }
        public GameObject Player { get; private set; }
        public GameObject Generator { get; set; }

        public GameObject TxtMouseXCoord { get; set; }
        public GameObject TxtMouseYCoord { get; set; }
        public GameObject TxtBlockType { get; set; }
        public GameObject TxtInventory { get; set; }
        public GameObject TxtItemBar { get; set; }
        public GameObject GuiInventory { get; set; }
        public GameObject GuiItemBar { get; set; }
        public GameObject InventoryCountText { get; set; }

        public Sprite SlotSprite { get; set; }
        public Sprite SlotHoverSprite { get; set; }
        public Sprite SlotSelectSprite { get; set; }
        public Sprite SlotSelectHoverSprite { get; set; }

        public World World { get; set; }

        public GameObject BlockSelector { get; set; }

        private PrefabRepository()
        {
            LoadGameObjects();
            LoadBlockPrefabs();
            LoadItemPrefabs();
            LoadGameUi();
            LoadSprites();

            DefineItemDrops();
        }

        private void LoadSprites()
        {
            //SlotSprite = Resources.Load<Sprite>(SpritesFolderPath + "slot");
            //SlotHoverSprite = Resources.Load<Sprite>(SpritesFolderPath + "hovered_slot");
            //SlotSelectSprite = Resources.Load<Sprite>(SpritesFolderPath + "selected_slot");
            //SlotSelectHoverSprite = Resources.Load<Sprite>(SpritesFolderPath + "selected_hovered_slot");
            var test = Resources.LoadAll<Sprite>(SpritesFolderPath + "Inventory");
            Debug.Log("Test");


            var invSprites = Resources.LoadAll<Sprite>(SpritesFolderPath + "Inventory");
            SlotSelectSprite = invSprites[2];
            SlotSelectHoverSprite = invSprites[1];
            SlotHoverSprite = invSprites[0];
            SlotSprite = invSprites[3];
        }

        private void DefineItemDrops()
        {
            //Grass block -> 1 x Grass block drop
            BlockDefinitions[BlockTypes.Grass].ItemDrop = new ItemDrop(ItemDefinitions[ItemTypes.GrassDrop], 1);
            BlockDefinitions[BlockTypes.Dirt].ItemDrop = new ItemDrop(ItemDefinitions[ItemTypes.DirtDrop], 1);
        }

        private void LoadGameUi()
        {
            TxtMouseXCoord = GameObject.Find("TxtMouseXCoord");
            TxtMouseYCoord = GameObject.Find("TxtMouseYCoord");
            TxtBlockType = GameObject.Find("TxtBlockType");
            GuiInventory = GameObject.Find("GuiInventory");
            TxtInventory = GameObject.Find("TxtInventory");
            GuiItemBar = GameObject.Find("GuiItemBar");
            TxtItemBar = GameObject.Find("TxtItemBar");
            InventoryCountText = Resources.Load(PrefabsFolderPath + "InventoryCountText") as GameObject;
        }

        private void LoadGameObjects()
        {
            Map = GameObject.Find("Map");
            Player = GameObject.Find("Player");
            Generator = GameObject.Find("Generator");
        }

        private void LoadItemPrefabs()
        {
            ItemDefinitions = new Dictionary<ItemTypes, ItemDefinition>();

            string pre = "Items/";

            LoadSingleItemPrefab("GrassDrop", ItemTypes.GrassDrop, pre + "GrassDrop");
            LoadSingleItemPrefab("DirtDrop", ItemTypes.DirtDrop, pre + "DirtDrop");
            LoadSingleItemPrefab("StoneDrop", ItemTypes.StoneDrop, pre + "StoneDrop");
        }

        private void LoadBlockPrefabs()
        {
            BlockDefinitions = new Dictionary<BlockTypes, BlockDefinition>();

            string pre = "Blocks/";

            LoadSingleBlockPrefab(pre + "Dirt", BlockTypes.Dirt);
            LoadSingleBlockPrefab(pre + "Stone", BlockTypes.Stone);
            LoadSingleBlockPrefab(pre + "Grass", BlockTypes.Grass);
            LoadSingleBlockPrefab(pre + "Ore", BlockTypes.Ore);
            
            LoadSingleBlockPrefab("Tree", BlockTypes.Tree);

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

        private void LoadSingleItemPrefab(string name, ItemTypes type, string prefabName)
        {
            GameObject go = Resources.Load(PrefabsFolderPath + prefabName) as GameObject;
            var itemDefinition = new ItemDefinition(
                name,
                type,
                go
            );

            itemDefinition.Sprite = go.GetComponent<SpriteRenderer>().sprite; 
            
            ItemDefinitions.Add(type, itemDefinition);
        }
    }
}