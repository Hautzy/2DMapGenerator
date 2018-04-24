using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class World
{
    public Block[,] Chunk { get; set; }
    public Block[,] Changes { get; set; }
    public List<Block> Obstacles { get; set; }

    public int StartPointX { get; set; }
    public int EndPointX { get; set; }
    public int StartPointY { get; set; }
    public int EndPointY { get; set; }

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

}
