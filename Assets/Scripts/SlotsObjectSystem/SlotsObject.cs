﻿using System.Collections.Generic;
using System.Text;
using Assets.Scripts.InventorySystem;
using Assets.Scripts.SlotSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.SlotsObjectSystem
{
    public abstract class SlotsObject
    {
        public int SlotsWidth { get; set; }
        public int SlotsHeight { get; set; }
        public float DrawBottomPadding { get; set; }
        public float DrawLeftPadding { get; set; }
        public string SlotPrefix { get; set; }

        public bool Show { get; set; }
        public InventoryItem[,] Slots { get; set; }
        public GameObject[,] GameObjectSlots { get; set; }
        public Player Owner { get; set; }

        public InventoryItem CurrentDraggingItem { get; set; }
        public Vector2Int DraggingStartPosition { get; set; }
        public Vector2Int? SelectedSlotPosition { get; set; }
        public GameObject CurrentDraggingImageItem { get; set; }    // current dragged item as image
        public GameObject Parent { get; set; }
        public Text DebugUi { get; set; }

        protected SlotsObject(
            Player owner,
            Text debugUi,
            GameObject parent,
            int slotWidth,
            int slotHeight,
            float drawBottomPadding,
            float drawLeftPadding,
            string slotPrefix)
        {
            Owner = owner;
            DebugUi = debugUi;
            Parent = parent;
            SlotsWidth = slotWidth;
            SlotsHeight = slotHeight;
            DrawBottomPadding = drawBottomPadding;
            DrawLeftPadding = drawLeftPadding;
            SlotPrefix = slotPrefix;

            Slots = new InventoryItem[SlotsHeight, SlotsWidth];
            GameObjectSlots = new GameObject[SlotsHeight, SlotsWidth];
            if(DebugUi != null)
                DebugUi.text = ToString();
        }

        public int MaxSlotCount { get { return SlotsHeight * SlotsWidth; } }

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

        public virtual void DrawUi()
        {
            for (int y = 0; y < SlotsHeight; y++)
            {
                for (int x = 0; x < SlotsWidth; x++)
                {
                    SlotController.DrawItemSlotWithSpriteAndDetails(
                        y,
                        x,
                        SlotPrefix,
                        Slots[y, x],
                        Parent.transform,
                        this,
                        DrawLeftPadding,
                        DrawBottomPadding,
                        false,
                        GameObjectSlots);
                }
            }
        }

        public void ToggleSlots(bool status)
        {
            for (int y = 0; y < SlotsHeight; y++)
            {
                for (int x = 0; x < SlotsWidth; x++)
                {
                    GameObjectSlots[y, x].gameObject.SetActive(status);
                }
            }
        }

        public void DeleteGui()
        {
            foreach (Transform child in Parent.transform)
            {
                //if (child.name != "CurrentDraggingImageItem")
                child.gameObject.SetActive(false);
            }
        }

        public Transform GetGuiTransformSlotByPos(int x, int y, string prefix)
        {
            return GameObjectSlots[y, x].transform; //Parent.transform.Find(prefix + "Slot_y" + y + "_x" + x);
        }

        public void DeleteGuiSlotAtPosition(int x, int y)
        {
            Transform currentSlot = GetGuiTransformSlotByPos(x, y, SlotPrefix);
            if (currentSlot.transform.childCount >= 2)
            {
                GameObject.Destroy(currentSlot.transform.GetChild(0).gameObject);
                GameObject.Destroy(currentSlot.transform.GetChild(1).gameObject);
            }
        }

        public void DrawGuiSlotAtPosition(int x, int y, bool isSelected)
        {
            SlotController.DrawItemSlotWithSpriteAndDetails(
                y,
                x,
                SlotPrefix,
                Slots[y, x],
                Parent.transform,
                this,
                DrawLeftPadding,
                DrawBottomPadding,
                isSelected, GameObjectSlots);
        }
    }
}
