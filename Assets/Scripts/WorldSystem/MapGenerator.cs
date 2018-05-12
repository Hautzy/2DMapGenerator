using System.Collections.Generic;
using Assets.Scripts.Blocks;
using UnityEngine;

namespace Assets.Scripts.WorldSystem 
{

	public class MapGenerator : MonoBehaviour
	{

	    private PrefabRepository _repo;
	    public World World;

	    public GameObject Map { get; set; }

	    public int Seed = 123456;

	    private PerlinNoise noise;

		void Start ()
		{
	        noise = new PerlinNoise(Seed);
		    Map = PrefabRepository.Instance.Map;
		    _repo = PrefabRepository.Instance;
	        Random.InitState(Seed);
			World = new World(-128, 128, -32, 32);
			World.Seed = Seed;
		    _repo.World = World;
		    RegenerateMap();
		    LoadMap();
		}

	    private void LoadMap ()
		{
			World loadedWorld = World.LoadMap (World.WorldName, Seed);
			if(loadedWorld == null)
				return;
			var changes = loadedWorld.Changes;
			var chunk = World.Chunk;
			World.Changes = changes;

			int height = chunk.GetLength (0);
			int width = chunk.GetLength (1);

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					var block = changes [y, x];
					if (block != null) {
						Debug.Log(block);
						if (block.BlockType == BlockTypes.Void) {
							Destroy (chunk[y, x].GameObject);
							chunk [y, x] = null;
						} 
						else 
						{
							var current = Instantiate(PrefabRepository.Instance.GetBlockPrefab(block.BlockType), new Vector3(block.X, block.Y), Quaternion.identity) as GameObject;
							current.transform.parent = Map.transform.GetChild(0).transform;
							BlockInstance newBlock = new BlockInstance(block.BlockType, current, block.X, block.Y);
							chunk[y, x] = newBlock;
						}
					}
				}
	        }
	    }


	    private void RegenerateMap()
	    {
			var chunk = new BlockInstance[Mathf.Abs(World.StartPointY) + Mathf.Abs(World.EndPointY), Mathf.Abs(World.StartPointX) + Mathf.Abs(World.EndPointX)];
			var obstacles = new List<BlockInstance>();

	        float blockWidth = _repo.GetBlockPrefab(BlockTypes.Dirt).transform.lossyScale.x;
	        float blockHeight = _repo.GetBlockPrefab(BlockTypes.Dirt).transform.lossyScale.y;

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
	                        GameObject current = Instantiate(_repo.GetBlockPrefab(BlockTypes.Tree), new Vector3(x, y + 1.5f),
	                            Quaternion.identity);
	                        current.transform.parent = Map.transform.GetChild(1).transform;
	                    }
	                }
	                else if (y < World.StartPointY + curHeight)
	                {
	                    GameObject current = null;
	                    if (y == World.StartPointY + curHeight - 1)
	                    {
	                        current = Instantiate(_repo.GetBlockPrefab(BlockTypes.Grass), new Vector2(x * blockWidth, y * blockHeight),
	                            Quaternion.identity);
							chunk[yOffset + y, xOffset + x] = new BlockInstance(BlockTypes.Grass, current, x, y);
	                    }
	                    else if (y < World.StartPointY + 0.5 * curHeight)
	                    {

	                        if (y < World.StartPointY + 0.25 * curHeight)
	                        {
	                            float rand = Random.Range(0.0f, 1.0f);
	                            if (rand > 0.975)
	                            {
	                                current = Instantiate(_repo.GetBlockPrefab(BlockTypes.Ore),
	                                    new Vector2(x * blockWidth, y * blockHeight),
	                                    Quaternion.identity);
									chunk[yOffset + y, xOffset + x] = new BlockInstance(BlockTypes.Ore, current, x, y);
	                            }
	                            else
	                            {
	                                current = Instantiate(_repo.GetBlockPrefab(BlockTypes.Stone),
	                                    new Vector2(x * blockWidth, y * blockHeight),
	                                    Quaternion.identity);
									chunk[yOffset + y, xOffset + x] = new BlockInstance(BlockTypes.Stone, current, x, y);
	                            }
	                        }
	                        else
	                        {
	                            current = Instantiate(_repo.GetBlockPrefab(BlockTypes.Stone),
	                                new Vector2(x * blockWidth, y * blockHeight),
	                                Quaternion.identity);
								chunk[yOffset + y, xOffset + x] = new BlockInstance(BlockTypes.Stone, current, x, y);
	                        }
	                    }
	                    else
	                    {
	                        current = Instantiate(_repo.GetBlockPrefab(BlockTypes.Dirt), new Vector2(x * blockWidth, y * blockHeight),
	                            Quaternion.identity);
							chunk[yOffset + y, xOffset + x] = new BlockInstance(BlockTypes.Dirt, current, x, y);
	                    }
	                    current.transform.parent = Map.transform.GetChild(0).transform;
	                }
	            }
	        
	        }

	        World.Chunk = chunk;
	        World.Obstacles = obstacles;
	    }
	}
}