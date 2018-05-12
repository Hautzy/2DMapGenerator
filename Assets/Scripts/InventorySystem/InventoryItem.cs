using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.Scripts.Items;

namespace Assets.Scripts.InventorySystem
{
    [Serializable()]
    public class InventoryItem: ISerializable
    {
        public ItemDefinition ItemDefinition { get; set; }
        public int Count { get; set; }

        public InventoryItem(ItemDefinition itemDefinition, int count)
        {
            ItemDefinition = itemDefinition;
            Count = count;
        }

        public InventoryItem(SerializationInfo info, StreamingContext ctx)
        {
            Count = (int) info.GetValue("Count", typeof(int));
            ItemDefinition =
                PrefabRepository.Instance.ItemDefinitions[(ItemTypes) info.GetValue("ItemType", typeof(ItemTypes))];
        }

        public void GetObjectData(SerializationInfo info, StreamingContext ctx)
        {
            info.AddValue("Count", Count);
            info.AddValue("ItemType", ItemDefinition.ItemType);
        }
    }
}
