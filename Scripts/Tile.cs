using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Tile//定义格子的类型并赋予属性
{

    public string name;//名称
    public GameObject tileVisualPrefab;//tile的预制体
    public GameObject unitOnTile;//格子上的单位
    public float movementCost = 1;//移动消耗
    public bool isWalkable=true;//是否可移动
    
    /*
    private int x;
    private int y;

    
   
    
    public Tile( int xLocation, int yLocation)
    {
        x = xLocation;
        y = yLocation;
    }

    public void setCoords(int xLocation, int yLocation)
    {
        x = xLocation;
        y = yLocation;
    }
    */
}
