using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Block
    {
        public BlockTypes BlockType { get; set; }
        public GameObject GameObject { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public Block(BlockTypes blockType, GameObject gameObject, float x, float y)
        {
            BlockType = blockType;
            GameObject = gameObject;
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return BlockType + " / x= " + X + " / y= " + Y;
        }
    }
}
