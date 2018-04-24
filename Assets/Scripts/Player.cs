using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    private Rigidbody2D _rb;

    public float SpeedX;
    public float SpeedY;

    public Vector3 CurrentMousePosition { get; set; }
    public GameObject BlockSelector { get; private set; }

    // Use this for initialization
    void Start ()
	{
	    _rb = GetComponent<Rigidbody2D>();
	    BlockSelector = PrefabRepository.Instance.BlockSelector;
	    BlockSelector = Instantiate(BlockSelector, new Vector3(0, 0), Quaternion.identity);
    }
	
	// Update is called once per frame
	void Update ()
	{
	    PlayerMovement();
	    HandleMouseFocus();
	    HandlePlayerClick();
	}

    private void HandlePlayerClick()
    {
        var chunk = PrefabRepository.Instance.Generator.GetComponent<MapGenerator>().World.Chunk;
        var changes = PrefabRepository.Instance.Generator.GetComponent<MapGenerator>().World.Changes;
        int height = chunk.GetLength(0);
        int width = chunk.GetLength(1);

        if (Input.GetMouseButtonDown(0))
        {
            int yCoord = height / 2 + Mathf.RoundToInt(CurrentMousePosition.y - 0.5f);
            int xCoord = width / 2 + Mathf.RoundToInt(CurrentMousePosition.x);

            Block block = (xCoord >= 0 && xCoord < width && yCoord >= 0 && yCoord < height) ? chunk[yCoord, xCoord] : null;
            if (block != null)
            {
                Block toDeleteBlock = chunk[yCoord, xCoord];
                Block voidBlock = new Block(BlockTypes.Void, null, toDeleteBlock.GameObject.transform.position.x, toDeleteBlock.GameObject.transform.position.y);
                Destroy(toDeleteBlock.GameObject);
                chunk[yCoord, xCoord] = null;
                changes[yCoord, xCoord] = voidBlock;
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
        BlockSelector.transform.position = new Vector3(Mathf.RoundToInt(CurrentMousePosition.x), Mathf.RoundToInt(CurrentMousePosition.y - 0.5f));
    }

    private void SearchForBlock()
    {
        var chunk = PrefabRepository.Instance.Generator.GetComponent<MapGenerator>().World.Chunk;
        int height = chunk.GetLength(0);
        int width = chunk.GetLength(1);

        int yCoord = height / 2 + Mathf.RoundToInt(CurrentMousePosition.y - 0.5f);
        int xCoord = width / 2 + Mathf.RoundToInt(CurrentMousePosition.x);

        Block block = (xCoord >= 0 && xCoord < width && yCoord >= 0 && yCoord < height) ? chunk[yCoord, xCoord] : null;

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
    }
}
