using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using System.Runtime.Serialization;
using System;
using System.Linq;

[Serializable()]
public class World: ISerializable
{
	public static string WorldName = "map.txt";

    public Block[,] Chunk { get; set; }
    public Block[,] Changes { get; set; }
    public List<Block> Obstacles { get; set; }

    public int StartPointX { get; set; }
    public int EndPointX { get; set; }
    public int StartPointY { get; set; }
    public int EndPointY { get; set; }

    public World (SerializationInfo info, StreamingContext ctx)
	{
		//Changes = SerializationController.ListToArray((Block[,])info.GetValue("Changes", typeof(List<List<Block>>)));
		Changes = (Block[,])info.GetValue("Changes", typeof(Block[,]));
		Obstacles = (List<Block>)info.GetValue("Obstacles", typeof(List<Block>));
	}

    public World(int startPointX, int endPointX, int startPointY, int endPointY)
    {
        StartPointX = startPointX;
        EndPointX = endPointX;
        StartPointY = startPointY;
        EndPointY = endPointY;

        Chunk = new Block[Mathf.Abs(StartPointY) + Mathf.Abs(EndPointY), Mathf.Abs(StartPointX) + Mathf.Abs(EndPointX)];
        Changes = new Block[Mathf.Abs(StartPointY) + Mathf.Abs(EndPointY), Mathf.Abs(StartPointX) + Mathf.Abs(EndPointX)];
        Obstacles = new List<Block>();
    }

    public void GetObjectData (SerializationInfo info, StreamingContext ctx)
	{
		info.AddValue("Changes", Changes);
		//info.AddValue("Changes", SerializationController.ArrayToList(Changes));
		info.AddValue("Obstacles", Obstacles);
	}

	public bool SaveChanges ()
	{
		Debug.Log("Save World");
		SerializationController.Serialize(WorldName, this);
		return true;
	}

	public static World LoadMap (string map)
	{
		try {
			Debug.Log("Load World");
			World world = (World)SerializationController.Deserialize (map);
			return world;
		} 
		catch (Exception ex) 
		{
			return null;
		}
	}

}
