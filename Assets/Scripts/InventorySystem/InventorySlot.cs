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
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler,
        IEndDragHandler, IDragHandler
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
            Inventory.SelectedSlotPosition = new Vector2Int(X, Y);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("EXIT");
            IsSelected = false;
            Image image = transform.GetComponent<Image>();
            image.sprite = PrefabRepository.Instance.SlotSprite;
            Inventory.SelectedSlotPosition = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Drag-start event triggered");
            if (Inventory.CurrentDraggingImageItem != null)
            {
                GameObject.Destroy(Inventory.CurrentDraggingImageItem);
                Inventory.CurrentDraggingImageItem = null;
            }
            if (Inventory.Slots[Y, X] != null)
            {
                Debug.Log("START DRAG - " + Y + "/" + X);
                Inventory.CurrentDraggingInventoryItem = Inventory.Slots[Y, X];
                Inventory.Slots[Y, X] = null;
                Inventory.DraggingStartingPosition = new Vector2Int(X, Y);
                Inventory.DeleteCurrentSlotGui(X, Y);

                Inventory.CurrentDraggingImageItem = new GameObject("CurrentDraggingImageItem");
                Image image = Inventory.CurrentDraggingImageItem.AddComponent<Image>();
                image.sprite = PrefabRepository.Instance
                    .ItemDefinitions[Inventory.CurrentDraggingInventoryItem.ItemDefinition.ItemType].Sprite;
                image.transform.position = Input.mousePosition + new Vector3(20, -20);
                image.rectTransform.sizeDelta = new Vector2(Inventory.SlotSize * 0.6f, Inventory.SlotSize * 0.6f);
                image.transform.SetParent(PrefabRepository.Instance.GUIInventory.transform);

                GameObject prefab = PrefabRepository.Instance.InventoryCountText;
                GameObject slotCount = Instantiate(prefab, new Vector3(), Quaternion.identity);
                Text imageCount = slotCount.GetComponent<Text>();
                imageCount.text = Inventory.CurrentDraggingInventoryItem.Count.ToString();
                imageCount.transform.SetParent(image.transform);
                imageCount.transform.position = Input.mousePosition + new Vector3(30, -30);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("Drag-end event triggered");
            if (Inventory.CurrentDraggingInventoryItem != null)
            {
                GameObject.Destroy(Inventory.CurrentDraggingImageItem);
                Inventory.CurrentDraggingImageItem = null;
                if (Inventory.SelectedSlotPosition == null)
                {
                    Inventory.Slots[Y, X] = Inventory.CurrentDraggingInventoryItem;
                    Debug.Log("Reset dragging");
                }
                else
                {
                    Inventory.Slots[Inventory.SelectedSlotPosition.Value.y, Inventory.SelectedSlotPosition.Value.x] =
                        Inventory.CurrentDraggingInventoryItem;
                    Debug.Log("END DRAG - " + Inventory.SelectedSlotPosition.Value.y + "/" +
                              Inventory.SelectedSlotPosition.Value.x);
                }
                Inventory.DeleteGuiInventory();
                Inventory.DrawInventory();
            }
        }

        public void Update()
        {
            if (IsSelected && Input.GetKeyDown(KeyCode.G))
            {
                if (Inventory.Slots[Y, X] != null)
                {
                    Debug.Log("DROP");
                    // TODO: Item can bug into other blogs when dropped right next to them, because position is absolute
                    Vector3 newPos = new Vector3(Inventory.Owner.transform.position.x + 0.5f,
                        Inventory.Owner.transform.position.y + 0.5f);
                    GameObject current = Instantiate(InventoryItem.ItemDefinition.Prefab, newPos, Quaternion.identity);
                    current.transform.parent = PrefabRepository.Instance.Map.transform.GetChild(2);
                    ItemInstance itemInstance = new ItemInstance(InventoryItem.ItemDefinition.ItemType, current);
                    current.GetComponent<ItemBehaviour>().Instance = itemInstance;
                    PrefabRepository.Instance.World.Items.Add(itemInstance);
                    Inventory.Slots[Y, X] = null;
                    Destroy(gameObject);

                    Inventory.DeleteGuiInventory();
                    Inventory.DrawInventory();
                }
            }
        }

        // drag update method
        public void OnDrag(PointerEventData eventData)
        {
            if (Inventory.CurrentDraggingInventoryItem != null)
            {
                Inventory.CurrentDraggingImageItem.transform.position = Input.mousePosition + new Vector3(20, -20);
            }
        }
    }
}