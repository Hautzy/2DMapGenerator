﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Assets.Scripts.Blocks;
using Assets.Scripts.InventorySystem;
using Assets.Scripts.ItemBarSystem;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Items;
using Assets.Scripts.WorldSystem;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts
{
    public class Player : MovingObject
    {
        public Vector3 CurrentMousePosition { get; set; }
        public GameObject BlockSelector { get; private set; }
        public GameObject SelectedItemGameObject { get; private set; }
        public World World { get; set; }
        public Inventory Inventory { get; set; }
        public ItemBar ItemBar { get; set; }

        // Use this for initialization
        public override void Start()
        {
            base.Start();
            World = PrefabRepository.Instance.World;
            BlockSelector = PrefabRepository.Instance.BlockSelector;
            BlockSelector = Instantiate(BlockSelector, new Vector3(0, 0), Quaternion.identity);
            SelectedItemGameObject = GameObject.Find("SelectedItemGameObject");
            Inventory = new Inventory(this);
            ItemBar = new ItemBar(this);
            ItemBar loadedItemBar = (ItemBar) ItemBar.Load();
            Inventory loadedInventory = (Inventory) Inventory.Load();
            if (loadedInventory != null)
                Inventory.Slots = loadedInventory.Slots;
            if (loadedItemBar != null)
                ItemBar.Slots = loadedItemBar.Slots;

            Inventory.Show = false;
            Inventory.DrawUi();
            Inventory.ToggleSlots(false);

            ItemBar.Show = true;
            ItemBar.DrawUi();
            ItemBar.UpdateSelectedItemGameObject();
        }

        // Update is called once per frame
        void Update()
        {
            HandleMouseFocus();
            if (!Inventory.Show)
                HandlePlayerClick();
            GetItemBarChange();
        }

        void FixedUpdate()
        {
            PlayerMovement();
        }

        private void GetItemBarChange()
        {
            if (Input.anyKey && !Input.GetMouseButton(0) && !Input.GetMouseButton(1))
            {
                bool numBtnPressed = false;
                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    ItemBar.SelectedSlot = 0;
                    numBtnPressed = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    ItemBar.SelectedSlot = 1;
                    numBtnPressed = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    ItemBar.SelectedSlot = 2;
                    numBtnPressed = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    ItemBar.SelectedSlot = 3;
                    numBtnPressed = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha5))
                {
                    ItemBar.SelectedSlot = 4;
                    numBtnPressed = true;
                }
                else if (Input.GetKeyDown(KeyCode.Alpha6))
                {
                    ItemBar.SelectedSlot = 5;
                    numBtnPressed = true;
                }
                if (numBtnPressed)
                {
                    ItemBar.DeleteGui();
                    ItemBar.DrawUi();

                    ItemBar.UpdateSelectedItemGameObject();
                }
            }
        }

        private void HandlePlayerClick()
        {
            var chunk = World.Chunk;
            var changes = World.Changes;
            int height = chunk.GetLength(0);
            int width = chunk.GetLength(1);

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                int yCoord = height / 2 + Mathf.RoundToInt(CurrentMousePosition.y - 0.5f);
                int xCoord = width / 2 + Mathf.RoundToInt(CurrentMousePosition.x);

                BlockInstance block = (xCoord >= 0 && xCoord < width && yCoord >= 0 && yCoord < height)
                    ? chunk[yCoord, xCoord]
                    : null;
                if (block != null && Input.GetMouseButtonDown(0))
                {
                    BlockInstance toDeleteBlock = chunk[yCoord, xCoord];
                    BlockInstance voidBlock = new BlockInstance(BlockTypes.Void, null,
                        toDeleteBlock.GameObject.transform.position.x, toDeleteBlock.GameObject.transform.position.y);

                    ItemDrop dropDefinition =
                        PrefabRepository.Instance.BlockDefinitions[toDeleteBlock.BlockType].ItemDrop;
                    if (dropDefinition != null)
                    {
                        GameObject itemGameObject = Instantiate(dropDefinition.Item.Prefab,
                            toDeleteBlock.GameObject.transform.position, Quaternion.identity);
                        itemGameObject.transform.parent = PrefabRepository.Instance.Map.transform.GetChild(2).transform;
                        ItemInstance drop = new ItemInstance(dropDefinition.Item.ItemType, itemGameObject);
                        itemGameObject.GetComponent<ItemBehaviour>().Instance = drop;
                        World.Items.Add(drop);
                    }

                    Destroy(toDeleteBlock.GameObject);
                    chunk[yCoord, xCoord] = null;
                    changes[yCoord, xCoord] = voidBlock;
                    World.SaveChanges();
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (block != null)
                    {
                        BlockInstance toDeleteBlock = chunk[yCoord, xCoord];
                        Destroy(toDeleteBlock.GameObject);
                    }
                    var current = Instantiate(PrefabRepository.Instance.GetBlockPrefab(BlockTypes.Grass),
                        new Vector3(xCoord - width / 2, yCoord - height / 2), Quaternion.identity) as GameObject;
                    current.transform.parent = PrefabRepository.Instance.Map.transform.GetChild(0).transform;
                    BlockInstance grassBlock = new BlockInstance(BlockTypes.Grass, current, xCoord - width / 2,
                        yCoord - height / 2);
                    chunk[yCoord, xCoord] = grassBlock;
                    changes[yCoord, xCoord] = grassBlock;
                    World.SaveChanges();
                }
            }
        }

        private void HandleMouseFocus()
        {
            CurrentMousePosition = Input.mousePosition;

            CurrentMousePosition = Camera.main.ScreenToWorldPoint(CurrentMousePosition);

            PrefabRepository.Instance.TxtMouseXCoord.GetComponent<Text>().text = "x: " + CurrentMousePosition.x;
            PrefabRepository.Instance.TxtMouseYCoord.GetComponent<Text>().text = "y: " + CurrentMousePosition.y;

            SearchForBlock();
            BlockSelector.transform.position = new Vector3(Mathf.RoundToInt(CurrentMousePosition.x),
                Mathf.RoundToInt(CurrentMousePosition.y - 0.5f));
        }

        private void SearchForBlock()
        {
            var chunk = World.Chunk;
            int height = chunk.GetLength(0);
            int width = chunk.GetLength(1);

            int yCoord = height / 2 + Mathf.RoundToInt(CurrentMousePosition.y - 0.5f);
            int xCoord = width / 2 + Mathf.RoundToInt(CurrentMousePosition.x);

            BlockInstance block = (xCoord >= 0 && xCoord < width && yCoord >= 0 && yCoord < height)
                ? chunk[yCoord, xCoord]
                : null;

            if (block != null)
            {
                PrefabRepository.Instance.TxtBlockType.GetComponent<Text>().text = block.ToString();
            }
            else
            {
                PrefabRepository.Instance.TxtBlockType.GetComponent<Text>().text = "void";
            }
        }

        private void PlayerMovement()
        {
            float h = Input.GetAxis("Horizontal");

            _rb.AddForce(Vector2.right * _speedX * h);

            if (Mathf.Abs(_rb.velocity.x) > _maxSpeedX)
            {
                _rb.velocity = new Vector2((Mathf.Abs(_rb.velocity.x) / _rb.velocity.x ) * _maxSpeedX, _rb.velocity.y);
            }
            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _rb.AddForce(Vector2.up * _jumpForce);
            }

            float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
            if (Camera.main.orthographicSize + mouseWheel * Time.deltaTime * 100 > 0)
                Camera.main.orthographicSize += mouseWheel * Time.deltaTime * 100;
            if (Input.GetKey(KeyCode.F))
            {
                Camera.main.orthographicSize = 11.4f;
            }
            if (Input.GetKeyDown(KeyCode.I))
            {
                Inventory.Show = !Inventory.Show;
                if (Inventory.Show)
                    Inventory.ToggleSlots(true);
                else
                    Inventory.ToggleSlots(false);
            }
        }

        

        public override void OnCollisionEnter2D(Collision2D collision)
        {
            base.OnCollisionEnter2D(collision);

            string currentTag = collision.gameObject.tag;
            if (currentTag == "Item")
            {
                Debug.Log("Bumped into item!");
                ItemInstance drop = collision.gameObject.GetComponent<ItemBehaviour>().Instance;

                InventoryItem ii = new InventoryItem(PrefabRepository.Instance.ItemDefinitions[drop.ItemType], 1);
                int placedItemCount = Inventory.FillFreeSlots(ii);
                if (placedItemCount > 0)
                {
                    PrefabRepository.Instance.World.Items.Remove(drop);
                    Destroy(drop.GameObject);
                    if (Inventory.Show)
                    {
                        Inventory.DeleteGui();
                        Inventory.DrawUi();
                    }
                }
            }
        }
    }
}