using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Blocks;
using Assets.Scripts.InventorySystem;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Items;

namespace Assets.Scripts
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D _rb;

        public float SpeedX;
        public float SpeedY;

        public Vector3 CurrentMousePosition { get; set; }
        public GameObject BlockSelector { get; private set; }
        public World World { get; set; }
        public Inventory Inventory { get; set; }

        // Use this for initialization
        void Start()
        {
            World = PrefabRepository.Instance.World;
            Inventory = new Inventory(this);
            _rb = GetComponent<Rigidbody2D>();
            BlockSelector = PrefabRepository.Instance.BlockSelector;
            BlockSelector = Instantiate(BlockSelector, new Vector3(0, 0), Quaternion.identity);
        }

        // Update is called once per frame
        void Update()
        {
            PlayerMovement();
            HandleMouseFocus();
            if(!Inventory.ShowInventory)
                HandlePlayerClick();
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
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _rb.AddForce(new Vector2(-SpeedX * Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _rb.AddForce(new Vector2(SpeedX * Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.Space))
            {
                _rb.AddForce(new Vector2(0, SpeedY * Time.deltaTime));
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
                Inventory.ShowInventory = !Inventory.ShowInventory;
                if (Inventory.ShowInventory)
                    Inventory.DrawInventory();
                else
                    Inventory.DeleteGuiInventory();
            }
        }

        public void OnCollisionEnter2D(Collision2D collision)
        {
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
                    if (Inventory.ShowInventory)
                    {
                        Inventory.DeleteGuiInventory();
                        Inventory.DrawInventory();
                    }
                }
            }
        }
    }
}