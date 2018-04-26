using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.InventorySystem
{
    public class InventorySlot: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Inventory Inventory { get; set; }
        public InventoryItem InventoryItem { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public GameObject Hover { get; set; }
        public bool IsSelected { get; set; }

        /*public void OnPointerEnter(PointerEventData eventData)
        {
            IsSelected = true;
            LastTime = Time.time;
            Debug.Log(Y + "/" + X + " - ENTERED");
            if (Hover == null)
            {
                Hover = new GameObject("selected_slot");
                Image image = Hover.AddComponent<Image>();
                Hover.transform.position =
                    new Vector3(Inventory.InventoryLeftPadding + X * (Inventory.SlotSize + Inventory.SlotMargin),
                        Inventory.InventoryTopPadding - Y * (Inventory.SlotSize + Inventory.SlotMargin));
                Hover.transform.SetParent(PrefabRepository.Instance.GUIInventory.transform);
                image.rectTransform.sizeDelta = new Vector2(Inventory.SlotSize, Inventory.SlotSize);
                image.sprite = PrefabRepository.Instance.SlotHoverSprite;
            }
        }*/
        public void OnPointerEnter(PointerEventData eventData)
        {
            Debug.Log("ENTER");
            IsSelected = true;
            Image image = transform.GetComponent<Image>();
            image.sprite = PrefabRepository.Instance.SlotHoverSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Debug.Log("EXIT");
            IsSelected = false;
            Image image = transform.GetComponent<Image>();
            image.sprite = PrefabRepository.Instance.SlotSprite;
        }

        public void Update()
        {
        }
    }
}
