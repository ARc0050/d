using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    public bool action;//可以开始行动的标志

    public tileMapScript TMScript;//获取到tilemap的脚本,也获取到了游戏核心


    public GameObject targetunit;//获取目标单位



    private UnitScript unitScript;//获取到自身unit的脚本
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
        if (action && unitScript.unitMoveState == unitScript.getMovementStateEnum(0))//可以行动，且单位为未被选中状态
        {
            TMScript.disableHighlightUnitRange();//取消地图上所有的高亮显示
            TMScript.selectedUnit = this.gameObject;//设置地图上的选中单位为该单位
            unitScript.map = TMScript;//选中单位的地图设置为当前地图
            unitScript.setMovementState(1);//选中单位的状态改为被选中
            unitScript.setSelectedAnimation();//选中单位的动画改为被选中的情况
            TMScript.unitSelected = true;//声明已经有选中的单位了

            MoveAndFindEnemy();//移动并找到目标


            

        }
        else if(action && unitScript.unitMoveState == unitScript.getMovementStateEnum(2))//可以行动且以及移动完成了，准备开打
        {
            if (targetunit != null)//找到了目标并且可以打！
            {
                //StartCoroutine(TMScript.BMS.attack(this.gameObject, targetunit));//攻击
                //StartCoroutine(TMScript.deselectAfterMovements(this.gameObject, targetunit));//在地图脚本中取消选中单位，协程有延迟来放战斗动画

                Debug.Log("此处应该攻击" + targetunit);

                targetunit.GetComponent<UnitScript>().currentHealthPoints = targetunit.GetComponent<UnitScript>().currentHealthPoints - 1;

                unitScript.wait();//将单位设置为等待不可移动
                unitScript.setWaitIdleAnimation();//将单位设置为等待动画
                unitScript.setMovementState(3);//将单位状态设置为等待

                TMScript.deselectUnit();//在地图脚本中取消选中单位

                action = false;
            }
            else
            {
                unitScript.wait();//将单位设置为等待不可移动
                unitScript.setWaitIdleAnimation();//将单位设置为等待动画
                unitScript.setMovementState(3);//将单位状态设置为等待

                TMScript.deselectUnit();//在地图脚本中取消选中单位

                action = false;
            }
            
            
            
        }
        else if (!action && unitScript.unitMoveState == unitScript.getMovementStateEnum(3))//不可行动且单位已经行动完成了
        {

            TMScript.GMS.endTurn();//在游戏核心管理中结束回合
            
        }



    }

   public void MoveAndFindEnemy()//移动V1 找到距离自己最近的敌人并到达其相邻的格子，并且获取该目标的单位脚本
    {
        List<Node> temPath = null;
        List<Node> shortPath = null;


        //这里可以插入移动骰子摇动的动画等等，修改单位的movespeed

        
        foreach (Transform team in TMScript.unitsOnBoard.transform)
        {
            foreach (Transform unitOnTeam in team)
            {
                if (unitScript.teamNum != unitOnTeam.GetComponent<UnitScript>().teamNum)//此处取到了所有地图上的非友方单位
                {
                    
                    int unitX = unitOnTeam.GetComponent<UnitScript>().x;
                    int unitY = unitOnTeam.GetComponent<UnitScript>().y;
                    foreach (Node n in TMScript.graph[unitX, unitY].neighbours)//找到非友方单位的相邻处
                    {
                        if(TMScript.unitCanEnterTile(n.x, n.y))
                        {
                            TMScript.generatePathTo(n.x, n.y);//计算当前单位位置到所有非友方单位相邻处的距离
                            temPath = unitScript.path;
                            if (shortPath == null || shortPath.Count > temPath.Count)
                            {

                                shortPath = temPath;//找出最短距离
                                targetunit = unitOnTeam.gameObject;//将目标确认

                            }
                        }
                        
                    }

                }
            }
            
        }
        temPath = shortPath;//把最短距离暂时赋值给临时路径
        finalMovementHighlight = TMScript.getUnitMovementOptions();//获取单位可以到达的位置节点
        if(shortPath != null)
        {
            for (int i = 1; shortPath.Count > 0; i++)
            {

                if (finalMovementHighlight.Contains(temPath[temPath.Count - 1]))//如果目标在可移动到的范围内
                {
                    break;
                }
                if (finalMovementHighlight.Contains(temPath[temPath.Count - i]))//找到离目标最近的可移动点
                {
                    targetunit = null;//目标置空，无法移动至可攻击目标处
                    break;
                }
                targetunit = null;
                shortPath.RemoveAt(shortPath.Count - 1);
            }
        }
        

        unitScript.path = shortPath;//将要移动的路径传给单位
                                    //unitScript.setWalkingAnimation();//设置当前单位为移动动画

        unitScript.MoveNextTile();//移动

        StartCoroutine(TMScript.moveUnitAndFinalize());//移动完成
    }


   public void AttackTarget(UnitScript target)
    {
        
    }
}
