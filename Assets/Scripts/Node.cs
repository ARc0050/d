using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node //定义节点
{ 
    
        public List<Node> neighbours;//节点的相邻节点
        public int x;
        public int y;//定义x、y
        //Edges
        public Node()//初始化节点的相邻节点
        {
            neighbours = new List<Node>();
        }

        public float DistanceTo(Node n)//返回[x,y]到节点的二维距离
        {
            return Vector2.Distance(new Vector2(x, y), new Vector2(n.x, n.y));
        }


    
}
