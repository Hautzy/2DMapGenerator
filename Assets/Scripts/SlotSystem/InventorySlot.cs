using Assets.Scripts.InventorySystem;
using Assets.Scripts.ItemBarSystem;
using Assets.Scripts.Items;
using Assets.Scripts.SlotsObjectSystem;
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
        public bool IsHovered { get; set; }
        public bool IsSelected { get; set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("ENTER");
            IsHovered = true;
            Image image = transform.GetComponent<Image>();
            if (IsSelected)
                image.sprite = PrefabRepository.Instance.SlotSelectHoverSprite;
            else
                image.sprite = PrefabRepository.Instance.SlotHoverSprite;
            SlotsObject.SelectedSlotPosition = new Vector2Int(X, Y);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("EXIT");
            IsHovered = false;
            Image image = transform.GetComponent<Image>();
            if (IsSelected)
                image.sprite = PrefabRepository.Instance.SlotSelectSprite;
            else
                image.sprite = PrefabRepository.Instance.SlotSprite;
            SlotsObject.SelectedSlotPosition = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            Debug.Log("Drag-start event triggered");
            SlotsObject.CurrentDraggingItem = null;
            if (SlotsObject.CurrentDraggingImageItem != null)
            {
                GameObject.Destroy(SlotsObject.CurrentDraggingImageItem);
                SlotsObject.CurrentDraggingImageItem = null;
            }
            if (SlotsObject.Slots[Y, X] != null)
            {
                Debug.Log("START DRAG - " + Y + "/" + X);
                if (Input.GetMouseButton(0) && Input.GetKey(KeyCode.LeftControl))
                {
                    if (SlotsObject.Slots[Y, X] != null)
                    {
                        int cnt = SlotsObject.Slots[Y, X].Count;
                        cnt /= 2;
                        if (cnt <= 0)
                            cnt = SlotsObject.Slots[Y, X].Count;

                        SlotsObject.CurrentDraggingItem =
                            new InventoryItem(SlotsObject.Slots[Y, X].ItemDefinition, cnt);
                        if (SlotsObject.Slots[Y, X].Count - cnt <= 0)
                        {
                            SlotsObject.Slots[Y, X] = null;
                        }
                        else
                        {
                            SlotsObject.Slots[Y, X].Count -= cnt;
                        }
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    if (SlotsObject.Slots[Y, X] != null)
                    {
                        SlotsObject.CurrentDraggingItem = SlotsObject.Slots[Y, X];
                        SlotsObject.Slots[Y, X] = null;
                    }
                }
                else if (Input.GetMouseButton(1))
                {
                    if (SlotsObject.Slots[Y, X] != null)
                    {
                        SlotsObject.CurrentDraggingItem = new InventoryItem(SlotsObject.Slots[Y, X].ItemDefinition, 1);
                        SlotsObject.Slots[Y, X].Count--;
                        if (SlotsObject.Slots[Y, X].Count <= 0)
                        {
                            SlotsObject.Slots[Y, X] = null;
                        }
                    }
                }
                SlotsObject.DraggingStartPosition = new Vector2Int(X, Y);
                SlotsObject.DeleteGuiSlotAtPosition(X, Y);
                SlotsObject.DrawGuiSlotAtPosition(X, Y, IsSelected);

                SlotsObject.CurrentDraggingImageItem = new GameObject("CurrentDraggingImageItem");
                Image image = SlotsObject.CurrentDraggingImageItem.AddComponent<Image>();
                image.sprite = PrefabRepository.Instance
                    .ItemDefinitions[SlotsObject.CurrentDraggingItem.ItemDefinition.ItemType].Sprite;
                image.transform.position = Input.mousePosition + new Vector3(20, -20);
                image.rectTransform.sizeDelta =
                    new Vector2(SlotController.SlotSize * 0.6f, SlotController.SlotSize * 0.6f);
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

                SlotsObject curSlotObject = null;
                if (SlotsObject.Owner.Inventory.SelectedSlotPosition != null)
                    curSlotObject = SlotsObject.Owner.Inventory;
                else if (SlotsObject.Owner.ItemBar.SelectedSlotPosition != null)
                    curSlotObject = SlotsObject.Owner.ItemBar;

                if (curSlotObject == null)
                {
                    SlotsObject.Slots[Y, X] = SlotsObject.CurrentDraggingItem;
                    SlotsObject.DeleteGui();
                    SlotsObject.DrawUi();
                    Debug.Log("Reset dragging");
                }
                else
                {
                    InventoryItem help = curSlotObject.Slots[curSlotObject.SelectedSlotPosition.Value.y,
                        curSlotObject.SelectedSlotPosition.Value.x];
                    if (help != null &&
                        help.ItemDefinition.ItemType == SlotsObject.CurrentDraggingItem.ItemDefinition.ItemType &&
                        help.Count + SlotsObject.CurrentDraggingItem.Count <= ItemDefinition.MaxItemCount)
                    {
                        curSlotObject.Slots[curSlotObject.SelectedSlotPosition.Value.y,
                                curSlotObject.SelectedSlotPosition.Value.x].Count +=
                            SlotsObject.CurrentDraggingItem.Count;
                    }
                    else if (help != null)
                    {
                        // help.Count + SlotsObject.CurrentDraggingItem.Count <
                        SlotsObject.Slots[SlotsObject.DraggingStartPosition.y,
                            SlotsObject.DraggingStartPosition.x] = help;
                        SlotsObject.DeleteGui();
                        SlotsObject.DrawUi();
                        curSlotObject.Slots[curSlotObject.SelectedSlotPosition.Value.y,
                                curSlotObject.SelectedSlotPosition.Value.x] =
                            SlotsObject.CurrentDraggingItem;
                    }
                    else
                    {
                        curSlotObject.Slots[curSlotObject.SelectedSlotPosition.Value.y,
                                curSlotObject.SelectedSlotPosition.Value.x] =
                            SlotsObject.CurrentDraggingItem;
                    }
                    curSlotObject.DeleteGui();
                    curSlotObject.DrawUi();

                    var itemBar = curSlotObject as ItemBar;
                    if(itemBar != null)
                        itemBar.UpdateSelectedItemGameObject();

                    SaveChangesInventoryItemBar(SlotsObject.Owner);
                    Debug.Log("END DRAG - " + curSlotObject.SelectedSlotPosition.Value.y + "/" +
                              curSlotObject.SelectedSlotPosition.Value.x);
                }
            }
        }

        public void Update()
        {
            if (IsHovered && Input.GetKeyDown(KeyCode.G))
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

                    SaveChangesInventoryItemBar(SlotsObject.Owner);

                    SlotsObject.DeleteGui();
                    SlotsObject.DrawUi();
                }
            }
        }

        private void SaveChangesInventoryItemBar(Player player)
        {
            player.Inventory.SaveChanges();
            player.ItemBar.SaveChanges();
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