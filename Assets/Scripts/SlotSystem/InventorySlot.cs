using Assets.Scripts.InventorySystem;
using Assets.Scripts.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.SlotSystem
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
            if (Inventory.InventorySlots[Y, X] != null)
            {
                Debug.Log("START DRAG - " + Y + "/" + X);
                Inventory.CurrentDraggingInventoryItem = Inventory.InventorySlots[Y, X];
                Inventory.InventorySlots[Y, X] = null;
                Inventory.DraggingStartingPosition = new Vector2Int(X, Y);
                Inventory.DeleteGuiSlotPerPosition(X, Y);

                Inventory.CurrentDraggingImageItem = new GameObject("CurrentDraggingImageItem");
                Image image = Inventory.CurrentDraggingImageItem.AddComponent<Image>();
                image.sprite = PrefabRepository.Instance
                    .ItemDefinitions[Inventory.CurrentDraggingInventoryItem.ItemDefinition.ItemType].Sprite;
                image.transform.position = Input.mousePosition + new Vector3(20, -20);
                image.rectTransform.sizeDelta = new Vector2(SlotController.SlotSize * 0.6f, SlotController.SlotSize * 0.6f);
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
                    Inventory.InventorySlots[Y, X] = Inventory.CurrentDraggingInventoryItem;
                    Debug.Log("Reset dragging");
                }
                else
                {
                    Inventory.InventorySlots[Inventory.SelectedSlotPosition.Value.y, Inventory.SelectedSlotPosition.Value.x] =
                        Inventory.CurrentDraggingInventoryItem;
                    Debug.Log("END DRAG - " + Inventory.SelectedSlotPosition.Value.y + "/" +
                              Inventory.SelectedSlotPosition.Value.x);
                }
                Inventory.SaveChanges();
                Inventory.DeleteGuiInventory();
                Inventory.DrawInventory();
            }
        }

        public void Update()
        {
            if (IsSelected && Input.GetKeyDown(KeyCode.G))
            {
                if (Inventory.InventorySlots[Y, X] != null)
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
                    if (Inventory.InventorySlots[Y, X].Count - 1 < 1)
                    {
                        Inventory.InventorySlots[Y, X] = null;
                    }
                    else
                    {
                        Inventory.InventorySlots[Y, X].Count--;
                    }
                    Destroy(gameObject);

                    Inventory.SaveChanges();
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