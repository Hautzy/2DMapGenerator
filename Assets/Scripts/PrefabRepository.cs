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
        public const string PersistInventoryItemBarName = "Inventory_ItemBar";

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
            SlotSprite = Resources.Load<Sprite>(SpritesFolderPath + "slot");
            SlotHoverSprite = Resources.Load<Sprite>(SpritesFolderPath + "slot_select");
        }

        private void DefineItemDrops()
        {
            //Grass block -> 1 x Grass block drop
            BlockDefinitions[BlockTypes.Grass].ItemDrop = new ItemDrop(ItemDefinitions[ItemTypes.GrassDrop], 1);
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

            LoadSingleItemPrefab("GrassDrop", ItemTypes.GrassDrop, "GrassDrop", "GrassDrop_Item");
            LoadSingleItemPrefab("DirtDrop", ItemTypes.DirtDrop, "DirtDrop", "DirtDrop_Item");
            LoadSingleItemPrefab("StoneDrop", ItemTypes.StoneDrop, "StoneDrop", "StoneDrop_item");
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

        private void LoadSingleItemPrefab(string name, ItemTypes type, string prefabName, string spriteName)
        {
            var itemDefinition = new ItemDefinition(
                name,
                type,
                Resources.Load(PrefabsFolderPath + prefabName) as GameObject
            );

            if (spriteName != null)
            {
                itemDefinition.Sprite = Resources.Load<Sprite>(SpritesFolderPath + spriteName);
            }
            ItemDefinitions.Add(type, itemDefinition);
        }
    }
}