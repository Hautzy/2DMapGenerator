using System;
using UnityEngine;

namespace Assets.Scripts.Items
{
    public class ItemDefinition
    {
        public string Name { get; set; }
        public GameObject Prefab { get; set; }
        public ItemTypes ItemType { get; set; }
        public Sprite Sprite { get; set; }

        public ItemDefinition(string name, ItemTypes itemType, GameObject prefab, Sprite sprite)
        {
            Name = name;
            Prefab = prefab;
            ItemType = itemType;
            Sprite = sprite;
        }

        public ItemDefinition(string name, ItemTypes itemType, GameObject prefab)
        {
            Name = name;
            Prefab = prefab;
            ItemType = itemType;
        }
    }
}