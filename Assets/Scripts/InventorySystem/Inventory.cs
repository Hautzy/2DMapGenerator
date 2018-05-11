using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Items;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace Assets.Scripts.InventorySystem
{
    public class Inventory
    {
        public const int SlotsWidth = 6;
        public const int SlotsHeight = 4;
        public const int MaxItemCount = 4;
        public const float SlotSize = 50f;
        public const float SlotMargin = 5f;
        public const float InventoryTopPadding = 200;
        public const float InventoryLeftPadding = 1000;
        public bool ShowInventory { get; set; }
        public InventoryItem[,] Slots { get; set; }
        public Player Owner {
			get;
			set;
		}
        public InventoryItem CurrentDraggingInventoryItem { get; set; }
        public Vector2Int DraggingStartingPosition { get; set; }
        public Vector2Int? SelectedSlotPosition { get; set; }
        public GameObject CurrentDraggingImageItem { get; set; } // current dragged item as image

		public Inventory (Player owner)
		{
			Owner = owner;
			Slots = new InventoryItem[SlotsHeight, SlotsWidth];

            InventoryItem ii = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[ItemTypes.GrassDrop], 1);
            //InventoryItem iii = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[ItemTypes.GrassDrop], 2);
            //InventoryItem iiii = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[ItemTypes.GrassDrop], 3);
            //InventoryItem iiiii = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[ItemTypes.GrassDrop], 4);
            Slots[0, 0] = ii;
            //Slots[0, 1] = iii;
            //Slots[2, 2] = iiii;
            //Slots[3, 1] = iiiii;

            PrefabRepository.Instance.TxtInventory.GetComponent<Text>().text = ToString();
		}

        /*public bool HasFreeSlot
        {
            get { return MaxSlotCount > CurrentSlotCount; }
        }*/

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
                        if (Slots[y, x] != null)
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
                int currentPlaceable = MaxItemCount - notFullSlots[i].Count;
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
                int currentPlaceable = placeAbleCount - MaxItemCount;
                InventoryItem newIi = null;
                if (currentPlaceable < MaxItemCount)
                {
                    newIi = new InventoryItem(ii.ItemDefinition, placeAbleCount);
                    placeAbleCount = 0;
                }
                else
                {
                    newIi = new InventoryItem(ii.ItemDefinition, MaxItemCount);
                    placeAbleCount -= MaxItemCount;
                }

                AddToEmptySlot(newIi);
                freeSlotCnt--;
            }

            return ii.Count - placeAbleCount;
        }

        public List<InventoryItem> FindNoFullItemSlot(ItemTypes itemType)
        {
            List<InventoryItem> notFullSlots = new List<InventoryItem>();
            for (int y = 0; y < SlotsHeight; y++)
            {
                for (int x = 0; x < SlotsWidth; x++)
                {
                    if (Slots[y, x] != null && Slots[y, x].ItemDefinition.ItemType == itemType && Slots[y, x].Count < MaxItemCount)
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
                    if (Slots[y, x] != null)
                        sb.AppendLine(Slots[y, x].Count + " x " + Slots[y, x].ItemDefinition.Name);
                    else
                        sb.AppendLine("---------");
                }
            }
            return sb.ToString();
        }

        public void DrawInventory()
        {
            for (int y = 0; y < SlotsHeight; y++)
            {
                for (int x = 0; x < SlotsWidth; x++)
                {
                    GameObject slot = CreateSlot(y, x, "Slot");
                    InventoryItem ii = Slots[y, x];
                    InventorySlot inventorySlot = slot.AddComponent<InventorySlot>();
                    inventorySlot.InventoryItem = ii;
                    inventorySlot.Inventory = this;
                    inventorySlot.X = x;
                    inventorySlot.Y = y;
                    if (ii != null)
                    {
                        GameObject slotItem = CreateSlot(y, x, "Item");
                        Image image = slotItem.GetComponent<Image>();
                        image.sprite = ii.ItemDefinition.Sprite;
                        image.rectTransform.sizeDelta = new Vector2(SlotSize * 0.9f, SlotSize * 0.9f);
                        slotItem.transform.SetParent(slot.transform);

                        GameObject prefab = PrefabRepository.Instance.InventoryCountText;
                        GameObject slotCount = GameObject.Instantiate(prefab, new Vector3(), Quaternion.identity);
                        Text imageCount = slotCount.GetComponent<Text>();
                        imageCount.text = ii.Count.ToString();
                        slotCount.transform.position = new Vector3(InventoryLeftPadding + x * (SlotSize + SlotMargin) + (SlotSize) / 4, InventoryTopPadding - y * (SlotSize + SlotMargin) - (SlotSize) / 4);
                        slotCount.transform.SetParent(slot.transform);
                    }
                }
            }
        }

        public GameObject CreateSlot(int y, int x, string prefix)
        {
            GameObject slot = new GameObject(prefix + "_y" + y + "_x" + x);
            Image image = slot.AddComponent<Image>();
            slot.transform.position = new Vector3(InventoryLeftPadding + x * (SlotSize + SlotMargin), InventoryTopPadding - y * (SlotSize + SlotMargin));
            slot.transform.SetParent(PrefabRepository.Instance.GUIInventory.transform);
            image.rectTransform.sizeDelta = new Vector2(SlotSize, SlotSize);
            image.sprite = PrefabRepository.Instance.SlotSprite;
            return slot;
        }

        public void DeleteGuiInventory()
        {
            foreach (Transform child in PrefabRepository.Instance.GUIInventory.transform)
            {
                if(child.name != "CurrentDraggingImageItem")
                    GameObject.Destroy(child.gameObject);
            }
        }

        public Transform GetGuiTransformSlotByPos(int x, int y)
        {
            return PrefabRepository.Instance.GUIInventory.transform.Find("Slot_y" + y + "_x" + x);
        }

        public void DeleteCurrentSlotGui(int x, int y)
        {
            Transform currentSlot = GetGuiTransformSlotByPos(x, y);
            GameObject.Destroy(currentSlot.transform.GetChild(0).gameObject);
            GameObject.Destroy(currentSlot.transform.GetChild(1).gameObject);
        }
    }
}
