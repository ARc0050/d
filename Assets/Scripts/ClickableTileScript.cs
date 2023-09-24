using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickableTileScript : MonoBehaviour    
{
    //这个脚本是储存每个可点击的tile里的数据的
    //每个tile的x和y数据
    public int tileX;
    public int tileY;
    //站在这个tile上的gameobject
    public GameObject unitOnTile;
    public tileMapScript map;


    /*
     * This was used in Quill18Create'sTutorial, I no longer use this portion
    private void OnMouseDown()
    {
       
        Debug.Log("tile has been clicked");
        if (map.selectedUnit != null)
        {
            map.generatePathTo(tileX, tileY);
        }
        
    }*/
}
