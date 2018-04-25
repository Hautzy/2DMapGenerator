using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Blocks;
using UnityEngine;
using System.Runtime.Serialization;
using System;
using System.Linq;

namespace Assets.Scripts
{
	[Serializable()]
	public class World: ISerializable
	{
		public static string WorldName = "map";
		
		public int Seed { get; set; }
	    public BlockInstance[,] Chunk { get; set; }
	    public BlockInstance[,] Changes { get; set; }
	    public List<BlockInstance> Obstacles { get; set; }

	    public int StartPointX { get; set; }
	    public int EndPointX { get; set; }
	    public int StartPointY { get; set; }
	    public int EndPointY { get; set; }

	    public World (SerializationInfo info, StreamingContext ctx)
		{
			//Changes = SerializationController.ListToArray((Block[,])info.GetValue("Changes", typeof(List<List<Block>>)));
			Changes = (BlockInstance[,])info.GetValue("Changes", typeof(BlockInstance[,]));
			Obstacles = (List<BlockInstance>)info.GetValue("Obstacles", typeof(List<BlockInstance>));
		}

	    public World(int startPointX, int endPointX, int startPointY, int endPointY)
	    {
	        StartPointX = startPointX;
	        EndPointX = endPointX;
	        StartPointY = startPointY;
	        EndPointY = endPointY;

	        Chunk = new BlockInstance[Mathf.Abs(StartPointY) + Mathf.Abs(EndPointY), Mathf.Abs(StartPointX) + Mathf.Abs(EndPointX)];
	        Changes = new BlockInstance[Mathf.Abs(StartPointY) + Mathf.Abs(EndPointY), Mathf.Abs(StartPointX) + Mathf.Abs(EndPointX)];
	        Obstacles = new List<BlockInstance>();
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
			SerializationController.Serialize(WorldName + "_" + Seed + ".txt", this);
			return true;
		}

		public static World LoadMap (string map, int seed)
		{
			try {
				Debug.Log("Load World");
				World world = (World)SerializationController.Deserialize(map + "_" + seed + ".txt");
				return world;
			} 
			catch (Exception ex) 
			{
				return null;
			}
		}
	}
}