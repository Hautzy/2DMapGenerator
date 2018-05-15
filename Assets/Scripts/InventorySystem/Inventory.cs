using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.Scripts.Items;
using Assets.Scripts.SlotsObjectSystem;
using Assets.Scripts.SlotSystem;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Assets.Scripts.InventorySystem
{
    [Serializable()]
    public class Inventory : SlotsObject, ISlotObjectPersistable
    {
        public Inventory(Player owner) : base(
            owner, 
            PrefabRepository.Instance.TxtInventory.GetComponent<Text>(), 
            PrefabRepository.Instance.GuiInventory, 
            6, 4, 200, 1000, "Inventory")
        {
        }

        public Inventory(SerializationInfo info, StreamingContext ctx): base(null, null, null, 0, 0, 0, 0, "")
        {
            Slots = (InventoryItem[,]) info.GetValue("InventorySlots", typeof(InventoryItem[,]));
        }

        public int FillFreeSlots(InventoryItem ii)
        {
            int newItemCnt = ii.Count;
            int freeSlotCnt = MaxSlotCount - CurrentSlotCount;
            int placeAbleCount = newItemCnt;
            List<InventoryItem> notFullSlots = FindNoFullItemSlot(ii.ItemDefinition.ItemType);

            for (int i = 0; i < notFullSlots.Count; i++)
            {
                int currentPlaceable = ItemDefinition.MaxItemCount - notFullSlots[i].Count;
                if (placeAbleCount < currentPlaceable)
                {
                    notFullSlots[i].Count += placeAbleCount;
                }
                else
                {
                    notFullSlots[i].Count += currentPlaceable;
                }
                placeAbleCount = placeAbleCount - currentPlaceable;
                if (placeAbleCount <= 0)
                {
                    placeAbleCount = 0;
                    break;
                }
            }

            while (placeAbleCount > 0 && freeSlotCnt > 0)
            {
                int currentPlaceable = placeAbleCount - ItemDefinition.MaxItemCount;
                InventoryItem newIi = null;
                if (currentPlaceable < ItemDefinition.MaxItemCount)
                {
                    newIi = new InventoryItem(ii.ItemDefinition, placeAbleCount);
                    placeAbleCount = 0;
                }
                else
                {
                    newIi = new InventoryItem(ii.ItemDefinition, ItemDefinition.MaxItemCount);
                    placeAbleCount -= ItemDefinition.MaxItemCount;
                }

                AddToEmptySlot(newIi);
                freeSlotCnt--;
            }

            SaveChanges();
            return ii.Count - placeAbleCount;
        }

        public List<InventoryItem> FindNoFullItemSlot(ItemTypes itemType)
        {
            List<InventoryItem> notFullSlots = new List<InventoryItem>();
            for (int y = 0; y < SlotsHeight; y++)
            {
                for (int x = 0; x < SlotsWidth; x++)
                {
                    if (Slots[y, x] != null && Slots[y, x].ItemDefinition.ItemType == itemType &&
                        Slots[y, x].Count < ItemDefinition.MaxItemCount)
                        notFullSlots.Add(Slots[y, x]);
                }
            }
            return notFullSlots;
        }

        public void AddToEmptySlot(InventoryItem ii)
        {
            for (int y = 0; y < SlotsHeight; y++)
            {
                for (int x = 0; x < SlotsWidth; x++)
                {
                    if (Slots[y, x] == null)
                    {
                        Slots[y, x] = ii;

                        PrefabRepository.Instance.TxtInventory.GetComponent<Text>().text = ToString();
                        SaveChanges();
                        return;
                    }
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("InventorySlots", Slots);
        }
        

        public void SaveChanges()
        {
            SerializationController.Serialize(PrefabRepository.PersistInventoryName + ".txt", this);
        }

        public ISlotObjectPersistable Load()
        {
            try
            {
                return (Inventory) SerializationController.Deserialize(PrefabRepository.PersistInventoryName + ".txt");
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}