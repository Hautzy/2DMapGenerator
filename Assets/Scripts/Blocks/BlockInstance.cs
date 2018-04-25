using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Assets.Scripts.Blocks
{
	[Serializable()]
    public class BlockInstance: ISerializable
    {
        public BlockTypes BlockType { get; set; }
        public GameObject GameObject { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public BlockInstance(BlockTypes blockType, GameObject gameObject, float x, float y)
        {
            BlockType = blockType;
            GameObject = gameObject;
            X = x;
            Y = y;
        }

        public BlockInstance(SerializationInfo info, StreamingContext ctx) 
        {
        	X = (float)info.GetValue("X", typeof(float));
        	Y = (float)info.GetValue("Y", typeof(float));
        	BlockType = (BlockTypes)info.GetValue("BlockType", typeof(BlockTypes));
        }

        public void GetObjectData (SerializationInfo info, StreamingContext ctx)
		{
			info.AddValue("X", X);
			info.AddValue("Y", Y);
			info.AddValue("BlockType", BlockType);
		}

		public static void Serialize (string fileName, BlockInstance block)
		{
			Stream stream = File.Open(fileName, FileMode.Create);
			BinaryFormatter formatter = new BinaryFormatter();

			formatter.Serialize(stream, block);
			stream.Close();
		}

		public static BlockInstance Deserialize (string fileName)
		{
			BlockInstance block = null;
			Stream stream = File.Open(fileName, FileMode.Open);
			BinaryFormatter formatter = new BinaryFormatter();

			block = (BlockInstance) formatter.Deserialize(stream);
			stream.Close();
			return block;
		}

        public override string ToString()
        {
            return BlockType + " / x= " + X + " / y= " + Y;
        }
    }
}
