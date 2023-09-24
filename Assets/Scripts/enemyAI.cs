using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    public bool action;//可以开始行动的标志

    public tileMapScript TMScript;//获取到tilemap的脚本



    private UnitScript unitScript;//获取到unit的脚本
    private HashSet<Node> finalMovementHighlight;//定义单位可移动的范围的节点

    public 
    // 初始化敌人
    void Start()
    {
        action = false;
        unitScript = GetComponent<UnitScript>();
        finalMovementHighlight = new HashSet<Node>();
    }

    // Update is called once per frame
    void Update()
    {
        if (action && unitScript.movementQueue.Count == 0 && unitScript.combatQueue.Count == 0)//可以行动，且单位不在移动也不在攻击,使其移动
        {
            TMScript.disableHighlightUnitRange();//取消地图上所有的高亮显示
            TMScript.selectedUnit = this.gameObject;//设置地图上的选中单位为该单位
            unitScript.map = TMScript;//选中单位的地图设置为当前地图
            unitScript.setMovementState(1);//选中单位的状态改为被选中
            unitScript.setSelectedAnimation();//选中单位的动画改为被选中的情况
            TMScript.unitSelected = true;//声明已经有选中的单位了

            List<Node> temPath = null;
            List<Node> shortPath = null;
            

            //这里可以插入移动骰子摇动的动画等等，修改单位的movespeed
            //找地图上的所有单位，算出到每个单位的相邻格子最短距离需要多少，得出最小的那个单位的格子，计算在可移动范围内算每个格子离最小单位的格子最近，到零就停，没到零就取最小值，移动到这个格子
            foreach (Transform team in TMScript.unitsOnBoard.transform)
            {
                foreach (Transform unitOnTeam in team)
                {
                    if (unitScript.teamNum != unitOnTeam.GetComponent<UnitScript>().teamNum)//此处取到了所有地图上的非友方单位
                    {
                        Debug.Log(unitOnTeam.GetComponent<UnitScript>().teamNum);
                        int unitX = unitOnTeam.GetComponent<UnitScript>().x;
                        int unitY = unitOnTeam.GetComponent<UnitScript>().y;
                        foreach (Node n in TMScript.graph[unitX, unitY].neighbours)//找到非友方单位的相邻处
                        {

                            TMScript.generatePathTo(n.x, n.y);//计算当前单位位置到所有非友方单位相邻处的距离
                            temPath = unitScript.path;
                            if(shortPath == null ||  shortPath.Count > temPath.Count)
                            {
                                shortPath = temPath;//找出最短距离
                            }
                        }
                            
                    }
                }
                    
            }
            finalMovementHighlight = TMScript.getUnitMovementOptions();//获取单位可以到达的位置节点
            shortPath.Reverse();//现在是移动用的列表，需要反转一下
            for(int i = 1; shortPath.Count > 0; i++)
            {
                if(finalMovementHighlight.Contains(shortPath[shortPath.Count - i]))
                {
                    break;
                }
            }

            shortPath.Reverse();//现在是检查用的列表，需要反转一下

            unitScript.path = shortPath;//将要移动的路径传给单位
            //unitScript.setWalkingAnimation();//设置当前单位为移动动画

            unitScript.MoveNextTile();//移动

            StartCoroutine(TMScript.moveUnitAndFinalize());//移动完成

            action = false;

        }

    }

   


   
}
