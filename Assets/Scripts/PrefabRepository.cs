using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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

        public Dictionary<BlockTypes, GameObject> BlockPrefabs;

        public GameObject Map { get; private set; }
        public GameObject Player { get; private set; }
        public GameObject Generator { get; set; }

        public GameObject TxtMouseXCoord { get; set; }
        public GameObject TxtMouseYCoord { get; set; }
        public GameObject TxtBlockType { get; set; }

        public GameObject BlockSelector { get; set; }

        private PrefabRepository()
        {
            LoadGameObjects();
            LoadBlockPrefabs();
            LoadGameUI();
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

        private void LoadBlockPrefabs()
        {
            BlockPrefabs = new Dictionary<BlockTypes, GameObject>();

            LoadSinglePrefab("Dirt", BlockTypes.Dirt);
            LoadSinglePrefab("Stone", BlockTypes.Stone);
            LoadSinglePrefab("Grass", BlockTypes.Grass);
            LoadSinglePrefab("Tree", BlockTypes.Tree);
            LoadSinglePrefab("Ore", BlockTypes.Ore);

            BlockSelector = Resources.Load(PrefabsFolderPath + "BlockSelector") as GameObject;
        }

        private void LoadSinglePrefab(string name, BlockTypes type)
        {
            BlockPrefabs.Add(type, Resources.Load(PrefabsFolderPath + name) as GameObject);
        }
    }
}
