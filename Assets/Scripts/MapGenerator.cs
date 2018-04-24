using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    private PrefabRepository _repo;
    public World World;

    public GameObject Map { get; set; }

    public int Seed = 123456;

    private PerlinNoise noise;

	void Start ()
	{
        World = new World(-128, 128, -32, 32);
	    _repo = PrefabRepository.Instance;
        noise = new PerlinNoise(Seed);
	    Map = PrefabRepository.Instance.Map;
        Random.InitState(Seed);
	    LoadMap();
	    RegenerateMap();
	}

    private void LoadMap()
    {
        var changes = new Block[Mathf.Abs(World.StartPointY) + Mathf.Abs(World.EndPointY), 
            Mathf.Abs(World.StartPointX) + Mathf.Abs(World.EndPointX)];

    }


    private void RegenerateMap()
    {
        var chunk = new Block[Mathf.Abs(World.StartPointY) + Mathf.Abs(World.EndPointY), Mathf.Abs(World.StartPointX) + Mathf.Abs(World.EndPointX)];
        var obstacles = new List<Block>();

        float blockWidth = _repo.BlockPrefabs[BlockTypes.Dirt].transform.lossyScale.x;
        float blockHeight = _repo.BlockPrefabs[BlockTypes.Dirt].transform.lossyScale.y;

        int yOffset = Mathf.Abs(World.StartPointY);
        int xOffset = Mathf.Abs(World.StartPointX);

        for (int x = World.StartPointX; x < World.EndPointX; x++)
        {
            int curHeight = noise.GetNoise(x - World.StartPointX, World.EndPointY - World.StartPointY);
            for (int y = World.StartPointY; y < World.EndPointY; y++)
            {
                if (y == World.StartPointY + curHeight)
                {
                    float rand = Random.Range(0.0f, 1.0f);
                    if (rand > 0.95)
                    {
                        GameObject current = Instantiate(_repo.BlockPrefabs[BlockTypes.Tree], new Vector3(x, y + 1.5f),
                            Quaternion.identity);
                        current.transform.parent = Map.transform.GetChild(1).transform;
                    }
                }
                else if (y < World.StartPointY + curHeight)
                {
                    GameObject current = null;
                    if (y == World.StartPointY + curHeight - 1)
                    {
                        current = Instantiate(_repo.BlockPrefabs[BlockTypes.Grass], new Vector2(x * blockWidth, y * blockHeight),
                            Quaternion.identity);
                        chunk[yOffset + y, xOffset + x] = new Block(BlockTypes.Grass, current, x, y);
                    }
                    else if (y < World.StartPointY + 0.5 * curHeight)
                    {

                        if (y < World.StartPointY + 0.25 * curHeight)
                        {
                            float rand = Random.Range(0.0f, 1.0f);
                            if (rand > 0.975)
                            {
                                current = Instantiate(_repo.BlockPrefabs[BlockTypes.Ore],
                                    new Vector2(x * blockWidth, y * blockHeight),
                                    Quaternion.identity);
                                chunk[yOffset + y, xOffset + x] = new Block(BlockTypes.Ore, current, x, y);
                            }
                            else
                            {
                                current = Instantiate(_repo.BlockPrefabs[BlockTypes.Stone],
                                    new Vector2(x * blockWidth, y * blockHeight),
                                    Quaternion.identity);
                                chunk[yOffset + y, xOffset + x] = new Block(BlockTypes.Stone, current, x, y);
                            }
                        }
                        else
                        {
                            current = Instantiate(_repo.BlockPrefabs[BlockTypes.Stone],
                                new Vector2(x * blockWidth, y * blockHeight),
                                Quaternion.identity);
                            chunk[yOffset + y, xOffset + x] = new Block(BlockTypes.Stone, current, x, y);
                        }
                    }
                    else
                    {
                        current = Instantiate(_repo.BlockPrefabs[BlockTypes.Dirt], new Vector2(x * blockWidth, y * blockHeight),
                            Quaternion.identity);
                        chunk[yOffset + y, xOffset + x] = new Block(BlockTypes.Dirt, current, x, y);
                    }
                    current.transform.parent = Map.transform.GetChild(0).transform;
                }
            }
        
        }

        World.Chunk = chunk;
        World.Obstacles = obstacles;
    }
}
