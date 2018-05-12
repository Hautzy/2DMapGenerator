using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.Scripts.Items;
using Assets.Scripts.SlotSystem;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Assets.Scripts.InventorySystem
{
    [Serializable()]
    public class Inventory : ISerializable
    {

        public static string InventoryName = "Inventory_";
        public const string InventorySlotPrefix = "Inventory";

        public const int SlotsWidth = 6;
        public const int SlotsHeight = 4;
        public const float InventoryBottomPadding = 200;
        public const float InventoryLeftPadding = 1000;
        public bool ShowInventory { get; set; }
        public InventoryItem[,] InventorySlots { get; set; }
        public Player Owner { get; set; }
        public InventoryItem CurrentDraggingInventoryItem { get; set; }
        public Vector2Int DraggingStartingPosition { get; set; }
        public Vector2Int? SelectedSlotPosition { get; set; }
        public GameObject CurrentDraggingImageItem { get; set; } // current dragged item as image

        public Inventory(Player owner)
        {
            Owner = owner;
            InventorySlots = new InventoryItem[SlotsHeight, SlotsWidth];

            //InventoryItem ii = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[ItemTypes.GrassDrop], 1);
            //InventoryItem iii = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[ItemTypes.GrassDrop], 2);
            //InventoryItem iiii = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[ItemTypes.GrassDrop], 3);
            //InventoryItem iiiii = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[ItemTypes.GrassDrop], 4);
            //InventorySlots[0, 0] = ii;
            //InventorySlots[0, 1] = iii;
            //InventorySlots[2, 2] = iiii;
            //InventorySlots[3, 1] = iiiii;

            PrefabRepository.Instance.TxtInventory.GetComponent<Text>().text = ToString();
        }

        public Inventory(SerializationInfo info, StreamingContext ctx)
        {
            InventorySlots = (InventoryItem[,]) info.GetValue("InventorySlots", typeof(InventoryItem[,]));
        }

        public int MaxSlotCount
        {
            get { return SlotsWidth * SlotsHeight; }
        }

        public int CurrentSlotCount
        {
            get
            {
                int count = 0;
                for (int y = 0; y < SlotsHeight; y++)
                {
                    for (int x = 0; x < SlotsWidth; x++)
                    {
                        if (InventorySlots[y, x] != null)
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
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
                    if (InventorySlots[y, x] != null && InventorySlots[y, x].ItemDefinition.ItemType == itemType &&
                        InventorySlots[y, x].Count < ItemDefinition.MaxItemCount)
                        notFullSlots.Add(InventorySlots[y, x]);
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
                    if (InventorySlots[y, x] == null)
                    {
                        InventorySlots[y, x] = ii;

                        PrefabRepository.Instance.TxtInventory.GetComponent<Text>().text = ToString();
                        SaveChanges();
                        return;
                    }
                }
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < SlotsHeight; y++)
            {
                for (int x = 0; x < SlotsWidth; x++)
                {
                    if (InventorySlots[y, x] != null)
                        sb.AppendLine(InventorySlots[y, x].Count + " x " + InventorySlots[y, x].ItemDefinition.Name);
                    else
                        sb.AppendLine("---------");
                }
            }
            return sb.ToString();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("InventorySlots", InventorySlots);
        }

        public void DrawInventory()
        {
            for (int y = 0; y < SlotsHeight; y++)
            {
                for (int x = 0; x < SlotsWidth; x++)
                {
                    SlotController.DrawItemSlotWithSpriteAndDetails(
                        y, 
                        x, 
                        InventorySlotPrefix, 
                        InventorySlots[y, x], 
                        PrefabRepository.Instance.GUIInventory.transform, 
                        this,
                        InventoryLeftPadding,
                        InventoryBottomPadding);
                }
            }
        }

        public void DeleteGuiInventory()
        {
            foreach (Transform child in PrefabRepository.Instance.GUIInventory.transform)
            {
                if (child.name != "CurrentDraggingImageItem")
                    GameObject.Destroy(child.gameObject);
            }
        }

        public Transform GetGuiTransformSlotByPos(int x, int y, string prefix)
        {
            return PrefabRepository.Instance.GUIInventory.transform.Find(prefix + "Slot_y" + y + "_x" + x);
        }

        public void DeleteGuiSlotPerPosition(int x, int y)
        {
            Transform currentSlot = GetGuiTransformSlotByPos(x, y, InventorySlotPrefix);
            GameObject.Destroy(currentSlot.transform.GetChild(0).gameObject);
            GameObject.Destroy(currentSlot.transform.GetChild(1).gameObject);
        }

        public void SaveChanges()
        {
            SerializationController.Serialize(InventoryName + ".txt", this);
        }

        public Inventory LoadInventory()
        {
            try
            {
                return (Inventory) SerializationController.Deserialize(InventoryName + ".txt");
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}