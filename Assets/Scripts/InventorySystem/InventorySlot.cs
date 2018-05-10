using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Assets.Scripts.Items;

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
            //Debug.Log("ENTER");
            IsSelected = true;
            Image image = transform.GetComponent<Image>();
            image.sprite = PrefabRepository.Instance.SlotHoverSprite;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("EXIT");
            IsSelected = false;
            Image image = transform.GetComponent<Image>();
            image.sprite = PrefabRepository.Instance.SlotSprite;
        }

        public void OnBeginDrag (PointerEventData eventData)
		{
		Debug.Log("START DRAG");
		}

		public void OnEndDrag (PointerEventData eventData)
		{
		Debug.Log("END DRAG");
		}

        public void Update ()
		{
			if (IsSelected && Input.GetKeyDown (KeyCode.G)) {
				// TODO: create method for creating new item in world
				if (Inventory.Slots [Y, X] != null) {
					Debug.Log ("DROP");
					Vector3 newPos = new Vector3(Inventory.Owner.transform.position.x, Inventory.Owner.transform.position.y + 3);
					GameObject current = Instantiate (InventoryItem.ItemDefinition.Prefab, newPos, Quaternion.identity);
					current.transform.parent = PrefabRepository.Instance.Map.transform.GetChild (2);
					ItemInstance itemInstance = new ItemInstance (InventoryItem.ItemDefinition.ItemType, current);
					current.GetComponent<ItemBehaviour> ().Instance = itemInstance;
					PrefabRepository.Instance.World.Items.Add (itemInstance);
					Inventory.Slots [Y, X] = null;
					Destroy (gameObject);

					Inventory.DeleteGUIInventory();
					Inventory.DrawInventory();
				}
			}
        }
    }
}
