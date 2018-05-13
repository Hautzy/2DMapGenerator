using Assets.Scripts.Contracts;
using Assets.Scripts.InventorySystem;
using Assets.Scripts.ItemBarSystem;
using Assets.Scripts.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.Scripts.SlotSystem
{
    public class InventorySlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler,
        IEndDragHandler, IDragHandler
    {
        public SlotsObject SlotsObject { get; set; }
        public ItemBar ItemBar { get; set; }
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
            SlotsObject.SelectedSlotPosition = new Vector2Int(X, Y);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("EXIT");
            IsSelected = false;
            Image image = transform.GetComponent<Image>();
            image.sprite = PrefabRepository.Instance.SlotSprite;
            SlotsObject.SelectedSlotPosition = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Drag-start event triggered");
            if (SlotsObject.CurrentDraggingImageItem != null)
            {
                GameObject.Destroy(SlotsObject.CurrentDraggingImageItem);
                SlotsObject.CurrentDraggingImageItem = null;
            }
            if (SlotsObject.Slots[Y, X] != null)
            {
                Debug.Log("START DRAG - " + Y + "/" + X);
                SlotsObject.CurrentDraggingItem = SlotsObject.Slots[Y, X];
                SlotsObject.Slots[Y, X] = null;
                SlotsObject.DraggingStartPosition = new Vector2Int(X, Y);
                SlotsObject.DeleteGuiSlotPerPosition(X, Y);

                SlotsObject.CurrentDraggingImageItem = new GameObject("CurrentDraggingImageItem");
                Image image = SlotsObject.CurrentDraggingImageItem.AddComponent<Image>();
                image.sprite = PrefabRepository.Instance
                    .ItemDefinitions[SlotsObject.CurrentDraggingItem.ItemDefinition.ItemType].Sprite;
                image.transform.position = Input.mousePosition + new Vector3(20, -20);
                image.rectTransform.sizeDelta = new Vector2(SlotController.SlotSize * 0.6f, SlotController.SlotSize * 0.6f);
                image.transform.SetParent(PrefabRepository.Instance.GuiInventory.transform);

                GameObject prefab = PrefabRepository.Instance.InventoryCountText;
                GameObject slotCount = Instantiate(prefab, new Vector3(), Quaternion.identity);
                Text imageCount = slotCount.GetComponent<Text>();
                imageCount.text = SlotsObject.CurrentDraggingItem.Count.ToString();
                imageCount.transform.SetParent(image.transform);
                imageCount.transform.position = Input.mousePosition + new Vector3(30, -30);
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("Drag-end event triggered");
            if (SlotsObject.CurrentDraggingItem != null)
            {
                GameObject.Destroy(SlotsObject.CurrentDraggingImageItem);
                SlotsObject.CurrentDraggingImageItem = null;
                if (SlotsObject.SelectedSlotPosition == null)
                {
                    SlotsObject.Slots[Y, X] = SlotsObject.CurrentDraggingItem;
                    Debug.Log("Reset dragging");
                }
                else
                {
                    SlotsObject.Slots[SlotsObject.SelectedSlotPosition.Value.y, SlotsObject.SelectedSlotPosition.Value.x] =
                        SlotsObject.CurrentDraggingItem;
                    Debug.Log("END DRAG - " + SlotsObject.SelectedSlotPosition.Value.y + "/" +
                              SlotsObject.SelectedSlotPosition.Value.x);
                }
                var inv = SlotsObject as Inventory;
                if(inv != null)
                    inv.SaveChanges();
                SlotsObject.DeleteGui();
                SlotsObject.DrawUi();
            }
        }

        public void Update()
        {
            if (IsSelected && Input.GetKeyDown(KeyCode.G))
            {
                if (SlotsObject.Slots[Y, X] != null)
                {
                    Debug.Log("DROP");
                    // TODO: Item can bug into other blogs when dropped right next to them, because position is absolute
                    Vector3 newPos = new Vector3(SlotsObject.Owner.transform.position.x + 0.5f,
                        SlotsObject.Owner.transform.position.y + 0.5f);
                    GameObject current = Instantiate(InventoryItem.ItemDefinition.Prefab, newPos, Quaternion.identity);
                    current.transform.parent = PrefabRepository.Instance.Map.transform.GetChild(2);
                    ItemInstance itemInstance = new ItemInstance(InventoryItem.ItemDefinition.ItemType, current);
                    current.GetComponent<ItemBehaviour>().Instance = itemInstance;
                    PrefabRepository.Instance.World.Items.Add(itemInstance);
                    if (SlotsObject.Slots[Y, X].Count - 1 < 1)
                    {
                        SlotsObject.Slots[Y, X] = null;
                    }
                    else
                    {
                        SlotsObject.Slots[Y, X].Count--;
                    }
                    Destroy(gameObject);

                    var inv = SlotsObject as Inventory;
                    if(inv != null)
                        inv.SaveChanges();
                    SlotsObject.DeleteGui();
                    SlotsObject.DrawUi();
                }
            }
        }

        // drag update method
        public void OnDrag(PointerEventData eventData)
        {
            if (SlotsObject.CurrentDraggingItem != null)
            {
                SlotsObject.CurrentDraggingImageItem.transform.position = Input.mousePosition + new Vector3(20, -20);
            }
        }
    }
}