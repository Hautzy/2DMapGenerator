using System;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Assets.Scripts;
using System.Collections.Generic;
using Assets.Scripts.Blocks;

namespace Assets.Scripts
{
	public static class SerializationController
	{
		public static void Serialize(string fileName, ISerializable s) 
		{
			Stream stream = File.Open(fileName, FileMode.Create);
			BinaryFormatter formatter = new BinaryFormatter();

			formatter.Serialize(stream, s);
			stream.Close();
		}

		public static ISerializable Deserialize (string fileName)
		{
			ISerializable obj = null;
			Stream stream = File.Open(fileName, FileMode.Open);
			BinaryFormatter formatter = new BinaryFormatter();

			obj = (ISerializable)formatter.Deserialize(stream);
			stream.Close();
			return obj;
		}

		public static List<List<BlockInstance>> ArrayToList (BlockInstance[,] arr)
		{
			List<List<BlockInstance>> list = new List<List<BlockInstance>>();
			int height = arr.GetLength(0);
			int width = arr.GetLength(1);

			for (int y = 0; y < height; y++) {
				List<BlockInstance> row = new List<BlockInstance>();
				for (int x = 0; x < width; x++) {
					row.Add(arr[y, x]);
				}
				list.Add(row);
			}
			return list;
		}

		public static BlockInstance[,] ListToArray (List<List<BlockInstance>> list)
		{
			int height = list.Count;
			int width = list[0].Count;
			BlockInstance[,] arr = new BlockInstance[height, width];

			for (int y = 0; y < height; y++) {
				for (int x = 0; x < width; x++) {
					arr[y, x] = list[y][x];
				}
			}
			return arr;
		}
	}
}
