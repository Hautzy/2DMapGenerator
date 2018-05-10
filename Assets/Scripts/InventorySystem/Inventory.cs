using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.InventorySystem
{
    public class Inventory
    {
        public const int SlotsWidth = 4;
        public const int SlotsHeight = 4;
        public const float SlotSize = 50f;
        public const float SlotMargin = 5f;
        public const float InventoryTopPadding = 500;
        public const float InventoryLeftPadding = 500;
        public bool ShowInventory { get; set; }
        public InventoryItem[,] Slots { get; set; }
        public Player Owner {
			get;
			set;
		}

		public Inventory (Player owner)
		{
			Owner = owner;
			Slots = new InventoryItem[SlotsHeight, SlotsWidth];

            InventoryItem ii = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[ItemTypes.GrassDrop], 1);
            Slots[0, 0] = ii;

            PrefabRepository.Instance.TxtInventory.GetComponent<Text>().text = ToString();
		}

        public bool HasFreeSlot
        {
            get { return MaxSlotCount > CurrentSlotCount; }
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
                        if (Slots[y, x] != null)
                        {
                            count++;
                        }
                    }
                }
                return count;
            }
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

        public void DeleteGUIInventory()
        {
            foreach (Transform child in PrefabRepository.Instance.GUIInventory.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
    }
}
