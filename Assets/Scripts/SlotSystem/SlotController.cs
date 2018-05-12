using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.InventorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.SlotSystem
{
    public class SlotController
    {
        public const float SlotSize = 50f;
        public const float SlotMargin = 5f;

        public static void DrawItemSlotWithSpriteAndDetails(
            int y, 
            int x, 
            string mainPrefix, 
            InventoryItem currentInventoryItem, 
            Transform parent, 
            Inventory inventory,
            float leftPadding,
            float bottomPadding)
        {
            // create basic slot
            GameObject slot = CreateBasicEmptySlot(y, x, mainPrefix + "Slot", parent, leftPadding, bottomPadding);
            InventorySlot inventorySlot = slot.AddComponent<InventorySlot>();
            inventorySlot.InventoryItem = currentInventoryItem;
            inventorySlot.Inventory = inventory;
            inventorySlot.X = x;
            inventorySlot.Y = y;
            if (currentInventoryItem != null)
            {
                // create image for sprite
                GameObject slotItem = CreateBasicEmptySlot(y, x, mainPrefix + "Item", parent, leftPadding, bottomPadding);
                Image image = slotItem.GetComponent<Image>();
                image.sprite = currentInventoryItem.ItemDefinition.Sprite;
                image.rectTransform.sizeDelta = new Vector2(SlotSize * 0.9f, SlotSize * 0.9f);
                slotItem.transform.SetParent(slot.transform);

                //create image for count
                GameObject prefab = PrefabRepository.Instance.InventoryCountText;
                GameObject slotCount = GameObject.Instantiate(prefab, new Vector3(), Quaternion.identity);
                Text imageCount = slotCount.GetComponent<Text>();
                imageCount.text = currentInventoryItem.Count.ToString();
                slotCount.transform.position =
                    new Vector3(leftPadding + x * (SlotSize + SlotMargin) + (SlotSize) / 4,
                        bottomPadding - y * (SlotSize + SlotMargin) - (SlotSize) / 4);
                slotCount.transform.SetParent(slot.transform);
            }
        }

        public static GameObject CreateBasicEmptySlot(int y, int x, string prefix, Transform parent, float leftPadding, float bottomPadding)
        {
            // create basic and empty slot
            GameObject slot = new GameObject(prefix + "_y" + y + "_x" + x);
            Image image = slot.AddComponent<Image>();
            slot.transform.position = new Vector3(leftPadding + x * (SlotSize + SlotMargin),
                bottomPadding - y * (SlotSize + SlotMargin));
            slot.transform.SetParent(parent);
            image.rectTransform.sizeDelta = new Vector2(SlotSize, SlotSize);
            image.sprite = PrefabRepository.Instance.SlotSprite;
            return slot;
        }
    }
}
