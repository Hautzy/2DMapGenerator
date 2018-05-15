using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Assets.Scripts.InventorySystem;
using Assets.Scripts.Items;
using Assets.Scripts.SlotsObjectSystem;
using Assets.Scripts.SlotSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.ItemBarSystem
{
    [Serializable()]
    public class ItemBar: SlotsObject, ISlotObjectPersistable
    {
        public int SelectedSlot { get; set; }
        public ItemBar(Player owner) : base(
            owner,
            PrefabRepository.Instance.TxtItemBar.GetComponent<Text>(),
            PrefabRepository.Instance.GuiItemBar,
            6, 1, 35, 500, "ItemBar")
        {
            Slots[0, 0] = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[ItemTypes.GrassDrop], 1);
            SelectedSlot = 0;
        }

        public ItemBar(SerializationInfo info, StreamingContext ctx): base(null, null, null, 0, 0, 0, 0, "")
        {
            Slots = (InventoryItem[,]) info.GetValue("ItemBarSlots", typeof(InventoryItem[,]));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ItemBarSlots", Slots);
        }

        public void SaveChanges()
        {
            SerializationController.Serialize(PrefabRepository.PersistItemBarName + ".txt", this);
        }

        public ISlotObjectPersistable Load()
        {
            try
            {
                return (ItemBar) SerializationController.Deserialize(PrefabRepository.PersistItemBarName + ".txt");
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public override void DrawUi()
        {
            for (int y = 0; y < SlotsHeight; y++)
            {
                for (int x = 0; x < SlotsWidth; x++)
                {
                    bool isSelected = y == 0 && x == SelectedSlot;
                    GameObject slot = SlotController.DrawItemSlotWithSpriteAndDetails(
                        y,
                        x,
                        SlotPrefix,
                        Slots[y, x],
                        Parent.transform,
                        this,
                        DrawLeftPadding,
                        DrawBottomPadding,
                        isSelected);
                    if (isSelected)
                    {
                        InventorySlot invSlot = slot.GetComponent<InventorySlot>();
                        Transform selectedTransform = invSlot.transform;
                        selectedTransform.GetComponent<Image>().sprite = PrefabRepository.Instance.SlotSelectSprite;
                    }
                }
            }
        }

        public void UpdateSelectedItemGameObject()
        {
            if (Slots[0, SelectedSlot] != null)
                Owner.SelectedItemGameObject.GetComponent<SpriteRenderer>().sprite =
                    Slots[0, SelectedSlot].ItemDefinition.Sprite;
            else
                Owner.SelectedItemGameObject.GetComponent<SpriteRenderer>().sprite = null;
        }
    }
}
