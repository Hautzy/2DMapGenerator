using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.Scripts.Contracts;
using Assets.Scripts.InventorySystem;
using Assets.Scripts.SlotSystem;
using UnityEngine.UI;

namespace Assets.Scripts.ItemBarSystem
{
    public class ItemBar: SlotsObject, ISlotObjectPersistable
    {
        public ItemBar(Player owner) : base(
            owner,
            PrefabRepository.Instance.TxtItemBar.GetComponent<Text>(),
            PrefabRepository.Instance.GuiItemBar,
            6, 1, 35, 500, "ItemBar")
        {
        }

        public ItemBar(SerializationInfo info, StreamingContext ctx): base(null, null, null, 0, 0, 0, 0, "")
        {
            Slots = (InventoryItem[,]) info.GetValue("ItemBarSlots", typeof(InventoryItem[,]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ItemBarSlots", Slots);
        }

        public void SaveChanges()
        {
            SerializationController.Serialize(PrefabRepository.PersistInventoryItemBarName + ".txt", this);
        }

        public ISlotObjectPersistable Load()
        {
            try
            {
                return (ItemBar) SerializationController.Deserialize(PrefabRepository.PersistInventoryItemBarName + ".txt");
            }
            catch (Exception e)
            {
                return null;
            }
        }   
    }
}
