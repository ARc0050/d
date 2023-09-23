using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class gameManagerScript : MonoBehaviour
{

    //很多用户界面并不需要公开，只是目前需要在inspector中快速更改时才公开。
    //将它们更改为private不会破坏任何东西，但您需要重新启用它们，使其显示在inspector中
    [Header("UI GameObjects")]
    public TMP_Text currentTeamUI;
    public Canvas displayWinnerUI;

    public TMP_Text UIunitCurrentHealth;
    public TMP_Text UIunitAttackDamage;
    public TMP_Text UIunitAttackRange;
    public TMP_Text UIunitMoveSpeed;
    public TMP_Text UIunitName;
    public UnityEngine.UI.Image UIunitSprite;

    public Canvas UIunitCanvas;
    public GameObject playerPhaseBlock;
    private Animator playerPhaseAnim;
    private TMP_Text playerPhaseText;

    //Raycast 用于更新 unitHover 信息
    private Ray ray;
    private RaycastHit hit;

    /// 团队数量被在代码里被定死了是2，如果将来有变化，该类中的一些函数也需要修改，以更新这一变化。

    public int numberOfTeams = 2;
    public int currentTeam;
    public GameObject unitsOnBoard;

    public GameObject team1;
    public GameObject team2;

    public GameObject unitBeingDisplayed;
    public GameObject tileBeingDisplayed;
    public bool displayingUnitInfo;              //用于控制是否显示目标的面板数值

    public tileMapScript TMS;

    //鼠标位置的坐标信息，给tileMapScript用
    public int cursorX;
    public int cursorY;
    //鼠标目前悬停在的区域
    public int selectedXTile;
    public int selectedYTile;

    //Variables for unitPotentialMovementRoute 计算潜在的运动路线需要的variables
    List<Node> currentPathForUnitRoute;
    List<Node> unitPathToCursor;

    public bool unitPathExists;

    public Material UIunitRoute;
    public Material UIunitRouteCurve;
    public Material UIunitRouteArrow;
    public Material UICursor;

    public int routeToX;
    public int routeToY;

    //This game object is to record the location of the 2 count path when it is reset to 0 this is used to remember what tile to disable
    //该游戏对象用于记录 2 个计数路径的位置，当其重置为 0 时，可用于记住要禁用的tile。
    public GameObject quadThatIsOneAwayFromUnit;

   
    public void Start()           
    {
        currentTeam = 0; //重置队伍数
        setCurrentTeamUI(); //激活UI
        teamHealthbarColorUpdate(); // 激活双方单位的血条颜色
        displayingUnitInfo = false; // 重置是否激活单位的数值面板
        playerPhaseAnim = playerPhaseBlock.GetComponent<Animator>(); //获得动画器的数据
        playerPhaseText = playerPhaseBlock.GetComponentInChildren<TextMeshProUGUI>(); //获得TMPUGUI中子单位的数据
        unitPathToCursor = new List<Node>(); //激活鼠标的路径指示，就是当玩家用鼠标移动单位时会出现的UI指示器
        unitPathExists = false;       //重置选中单位后UI显示的目标移动路径
      
        TMS = GetComponent<tileMapScript>(); //获得tileMapScript的公开数据

        
    }
    //2019/10/17 there is a small blink between disable and re-enable for path, its a bit jarring, try to fix it later
    public void Update()
    {
        
        ray = Camera.main.ScreenPointToRay(Input.mousePosition); //在Update中更新，这个function总是试图找到鼠标指向的位置，从相机处向指针位置发射一个射线，用于检测鼠标指向的位置

        if (Physics.Raycast(ray, out hit))  //如果鼠标指到了东西，这里的out是要求这个函数必
        {
            //Update cursorLocation and unit appearing in the topLeft
            cursorUIUpdate(); //激活这个函数时可以高亮鼠标悬停处的可选中格子
            unitUIUpdate();   //激活这个函数时如果当前的可选中格子里有个unit，那么就在UI界面里显示这个Unit的面板数值


            //如果选中了目标（选中的单位不等于null）以及该目标目前是未选中的（getMovementStateEnum的第一项（1）是unselected，它和目标当前的unitMoveState一致，都是unselected（1））
            if (TMS.selectedUnit != null && TMS.selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(1) == TMS.selectedUnit.GetComponent<UnitScript>().unitMoveState)
            {


                //检查光标是否在地图范围内，我们无法显示范围外的移动，因此如果在范围外就没有意义了
                if (TMS.selectedUnitMoveRange.Contains(TMS.graph[cursorX, cursorY])) //如果鼠标悬停的坐标在选中的Unit的移动范围内的话
                {

                    //生成到光标的新路径，尽量限制为每个新光标位置一次，否则计算量过大
                    //当鼠标目前的坐标不等于选中目标的坐标时，即选中目标并且将鼠标移动到移动范围内的一点时
                    if (cursorX != TMS.selectedUnit.GetComponent<UnitScript>().x || cursorY != TMS.selectedUnit.GetComponent<UnitScript>().y)
                    {
                        //当选中目标后还没出现指示路径，且选中的单位还没有计算出一条移动路径时（即该单位的移动方案的队列为0时）
                        if (!unitPathExists && TMS.selectedUnit.GetComponent<UnitScript>().movementQueue.Count==0)
                        {
                           
                            unitPathToCursor = generateCursorRouteTo(cursorX, cursorY); //用这个函数去生成一条指向鼠标悬停处的路径
                           
                            routeToX = cursorX; //把公共变量里的routeToX改成鼠标目前悬停方格的x轴数值
                            routeToY = cursorY; //同上，但是y轴

                            if (unitPathToCursor.Count != 0) //如果已经有一条用于鼠标指针显示用的路线被计算出来了
                            {
                                                                                              
                                for(int i = 0; i < unitPathToCursor.Count; i++)//遍历所有计算出的路线的格子（nodes）
                                {
                                    int nodeX = unitPathToCursor[i].x;
                                    int nodeY = unitPathToCursor[i].y;

                                    if (i == 0) //箭头的起始点
                                    {
                                        GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
                                        quadToUpdate.GetComponent<Renderer>().material = UICursor;
                                    }
                                    else if (i!=0 && (i+1)!=unitPathToCursor.Count) //鼠标移动的指示器箭头的中间部分
                                    {
                                        //This is used to set the indicator for tiles excluding the first/last tile
                                        setCorrectRouteWithInputAndOutput(nodeX, nodeY,i);
                                    }
                                    else if (i == unitPathToCursor.Count-1) //箭头的末端
                                    {
                                        //This is used to set the indicator for the final tile;
                                        setCorrectRouteFinalTile(nodeX, nodeY, i);
                                    }
                                    
                                    TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY].GetComponent<Renderer>().enabled = true; //在地图指示器中显示箭头
                                   
                                }
                                    
                            }
                            unitPathExists = true; //选中的棋子可以到达当前鼠标位置的方格

                        }
                        
                        else if (routeToX != cursorX || routeToY != cursorY) //当鼠标悬停的坐标不等于路线终点的坐标时，就是玩家的鼠标位置超出了路线的最远距离
                        {
                           
                            if (unitPathToCursor.Count != 0) //如果已经有一条用于鼠标指针显示用的路线被计算出来了
                            {
                                for (int i = 0; i < unitPathToCursor.Count; i++)                                                 //同上，依然遍历所有计算出的路线的格子（nodes）
                                {
                                    int nodeX = unitPathToCursor[i].x;
                                    int nodeY = unitPathToCursor[i].y;

                                    TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY].GetComponent<Renderer>().enabled = false;  //但鼠标移动到超出该棋子范围的格子时就不再延伸箭头了
                                }
                            }
                            
                            unitPathExists = false; //选中的棋子无法到达当前鼠标位置的方格
                        }
                    }

                    //当鼠标目前的坐标等于选中目标的坐标时，即选中目标并且没有将鼠标移动到移动范围内的一点时
                    else if (cursorX == TMS.selectedUnit.GetComponent<UnitScript>().x && cursorY == TMS.selectedUnit.GetComponent<UnitScript>().y)
                    {
                        
                        TMS.disableUnitUIRoute(); //在TMS脚本中，运行这个函数可以取消显示箭头
                        unitPathExists = false;   //选中的棋子无法到达当前鼠标位置的方格
                    }
                    
                }               
            }
        
        }
        
    }
    //In: 
    //Out: void
    //描述：在UI中显示队伍的编号
    public void setCurrentTeamUI()
    {
        currentTeamUI.SetText("Current Player is : Player " + (currentTeam+1).ToString());
    }

    //In: 
    //Out: void
    //描述：由Player1转换到Player2
    public void switchCurrentPlayer()
    {
        resetUnitsMovements(returnTeam(currentTeam)); //将之前行动过的一方的行动状态重置
        currentTeam++;
        if (currentTeam == numberOfTeams) //目前有两个队，当两个队都行动过了
        {
            currentTeam = 0;              //重置currentTeam
        }
        
    }

    //输入：currentTeam的值，这个值决定了当前行动的是哪一方
    //输出：Gameobject Team
    //描述：和上面的switchCurrentPlayer一起用，来实现让双方队伍轮流开始他们的回合
    public GameObject returnTeam(int i)
    {
        GameObject teamToReturn = null;
        if (i == 0)
        {
            teamToReturn = team1;
        }
        else if (i == 1)
        {
            teamToReturn = team2;
        }
        return teamToReturn;
    }

    //输入：一个已经行动过的队伍，让该队伍内的所有棋子的行动状态重置
    //Out: void
    //描述：让该队伍内的所有棋子的行动状态重置
    public void resetUnitsMovements(GameObject teamToReset)
    {
        foreach (Transform unit in teamToReset.transform)
        {
            unit.GetComponent<UnitScript>().moveAgain();
        }
    }

    //In: 
    //Out: void
    //描述：在回合结束时激活动画的trigger来播放双方回合结束和开始动画
    public void endTurn()
    {
        
        if (TMS.selectedUnit == null)
        {
            switchCurrentPlayer();
            if (currentTeam == 1)
            {
                playerPhaseAnim.SetTrigger("slideLeftTrigger");
                playerPhaseText.SetText("Player 2 Phase");
            }
            else if (currentTeam == 0)
            {
                playerPhaseAnim.SetTrigger("slideRightTrigger");
                playerPhaseText.SetText("Player 1 Phase");
            }
            teamHealthbarColorUpdate();
            setCurrentTeamUI();
        }
    }

    //输入：发动攻击的单位和受到攻击的单位
    //Out: void
    //描述: checks to see if units remain on a team 检查在被攻击后受击的单位是否还在那个队伍里
    public void checkIfUnitsRemain(GameObject unit, GameObject enemy)
    {
        //  Debug.Log(team1.transform.childCount);
        //  Debug.Log(team2.transform.childCount);
        StartCoroutine(checkIfUnitsRemainCoroutine(unit,enemy)); //这个协程是用来干嘛的
    }


    //In:
    //Out: void
    //描述：更新鼠标悬停处的UI指示器
    public void cursorUIUpdate()
    {
       //如果悬停在一个tile上就使它高亮
        if (hit.transform.CompareTag("Tile"))//如果选中的是个tile
        {
            if (tileBeingDisplayed == null) //如果没有tile被选中（鼠标悬停），也就是鼠标从地图外移动到第一个可被选中高亮的tile上面时。
                                            //这个tileBeingDisplayed是脚本开头定义的一个public的gameobject，
            {
                selectedXTile = hit.transform.gameObject.GetComponent<ClickableTileScript>().tileX;//把上面public的selectedXTile，就是选中的tile的x值更新成这个
                selectedYTile = hit.transform.gameObject.GetComponent<ClickableTileScript>().tileY;//同上，但是y值
                cursorX = selectedXTile;                                                           //更新鼠标悬停位置的坐标x
                cursorY = selectedYTile;                                                           //同上，更新的是坐标y
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true; //用TMS里的函数让这个tile高亮
                tileBeingDisplayed = hit.transform.gameObject;                                     //最后将这个tileBeingDisplayed更新成鼠标悬停在的tile
                
            }
            else if (tileBeingDisplayed != hit.transform.gameObject)//如果鼠标从一个选中的tile移动到另一个上面时
            {
                selectedXTile = tileBeingDisplayed.GetComponent<ClickableTileScript>().tileX;//重复上述操作
                selectedYTile = tileBeingDisplayed.GetComponent<ClickableTileScript>().tileY;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false; //先把之前的那个tile的高亮关掉

                selectedXTile = hit.transform.gameObject.GetComponent<ClickableTileScript>().tileX; //再重复上述操作，但是这次会更新鼠标悬停的位置
                selectedYTile = hit.transform.gameObject.GetComponent<ClickableTileScript>().tileY;
                cursorX = selectedXTile;                                                            
                cursorY = selectedYTile;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true; //然后把现在鼠标悬停位置的tile高亮
                tileBeingDisplayed = hit.transform.gameObject;
                
            }

        }
        //如果我们将鼠标移至一个Unit上，则高亮显示该Unit占用的tile
        else if (hit.transform.CompareTag("Unit"))                                                   //一二三四，再来一次
        {
            if (tileBeingDisplayed == null)
            {
                selectedXTile = hit.transform.parent.gameObject.GetComponent<UnitScript>().x;
                selectedYTile = hit.transform.parent.gameObject.GetComponent<UnitScript>().y;
                cursorX = selectedXTile;
                cursorY = selectedYTile;
                TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                tileBeingDisplayed = hit.transform.parent.gameObject.GetComponent<UnitScript>().tileBeingOccupied;

            }
            else if (tileBeingDisplayed != hit.transform.gameObject)
            {
                if (hit.transform.parent.gameObject.GetComponent<UnitScript>().movementQueue.Count == 0)
                {
                    selectedXTile = tileBeingDisplayed.GetComponent<ClickableTileScript>().tileX;
                    selectedYTile = tileBeingDisplayed.GetComponent<ClickableTileScript>().tileY;
                    TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false;

                    selectedXTile = hit.transform.parent.gameObject.GetComponent<UnitScript>().x;
                    selectedYTile = hit.transform.parent.gameObject.GetComponent<UnitScript>().y;
                    cursorX = selectedXTile;
                    cursorY = selectedYTile;
                    TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = true;
                    tileBeingDisplayed = hit.transform.parent.GetComponent<UnitScript>().tileBeingOccupied;
                   
                }
               
            }
        }
        //如果鼠标悬停的位置没有可交互的东西
        else
        {
            TMS.quadOnMapCursor[selectedXTile, selectedYTile].GetComponent<MeshRenderer>().enabled = false;
        }
    }


    //In and Out：void
    //描述：被高亮显示的Unit会在UI界面上显示其面板数值
    public void unitUIUpdate()
    {
        if (!displayingUnitInfo) //如果没有UnitInfo被显示
        {
            if (hit.transform.CompareTag("Unit")) //如果raycast射线点到的是一个unit
            {
                UIunitCanvas.enabled = true; //激活UIunitCanvas
                displayingUnitInfo = true;   //展示UnitInfo
                unitBeingDisplayed = hit.transform.parent.gameObject;  //更新unitBeingDisplayed
                var highlightedUnitScript = hit.transform.parent.gameObject.GetComponent<UnitScript>(); //获得这个位置（子级的数据）的位于父级的Unit的Script

                UIunitCurrentHealth.SetText(highlightedUnitScript.currentHealthPoints.ToString());     //在UI上展示该Unit的面板
                UIunitAttackDamage.SetText(highlightedUnitScript.attackDamage.ToString());
                UIunitAttackRange.SetText(highlightedUnitScript.attackRange.ToString());
                UIunitMoveSpeed.SetText(highlightedUnitScript.moveSpeed.ToString());
                UIunitName.SetText(highlightedUnitScript.unitName);
                UIunitSprite.sprite = highlightedUnitScript.unitSprite;
                
            }
            else if (hit.transform.CompareTag("Tile"))//如果raycast射线点到的是一个tile
            {
                if (hit.transform.GetComponent<ClickableTileScript>().unitOnTile != null) //如果这tile上面不是空的（有一个unit在这个tile上）
                {
                    

                    UIunitCanvas.enabled = true;       //二二三四，再来一次
                    displayingUnitInfo = true;
                    unitBeingDisplayed = hit.transform.GetComponent<ClickableTileScript>().unitOnTile;
                    var highlightedUnitScript = unitBeingDisplayed.GetComponent<UnitScript>();

                    UIunitCurrentHealth.SetText(highlightedUnitScript.currentHealthPoints.ToString());
                    UIunitAttackDamage.SetText(highlightedUnitScript.attackDamage.ToString());
                    UIunitAttackRange.SetText(highlightedUnitScript.attackRange.ToString());
                    UIunitMoveSpeed.SetText(highlightedUnitScript.moveSpeed.ToString());
                    UIunitName.SetText(highlightedUnitScript.unitName);
                    UIunitSprite.sprite = highlightedUnitScript.unitSprite;

                }
            }
        }
        else if (hit.transform.gameObject.CompareTag("Tile")) //如果raycast射线点到的是一个tile
        {
            if (hit.transform.GetComponent<ClickableTileScript>().unitOnTile == null) //如果这tile上面是空的
            {
                UIunitCanvas.enabled = false;
                displayingUnitInfo = false;
            }
            else if (hit.transform.GetComponent<ClickableTileScript>().unitOnTile != unitBeingDisplayed)//如果tile上的Unit和unitBeingDisplayed不一样，比如说鼠标从原先悬停的unit上离开了
            {
                UIunitCanvas.enabled = false; //让之前显示的数值面板UI消失
                displayingUnitInfo = false;
            }
        }
        else if (hit.transform.gameObject.CompareTag("Unit")) //如果射线点到的Unit和unitBeingDisplayed不一样，比如说鼠标从原先悬停的unit上移动到另一个Unit
        {
            if (hit.transform.parent.gameObject != unitBeingDisplayed)
            {
                UIunitCanvas.enabled = false; //让之前显示的数值面板UI消失（然后进入上面的if case生成当前选中的unit的数值UI）
                displayingUnitInfo = false;
            }
        }
    }

    //In: 
    //Out: void
    //描述：让当前正在行动的队伍的血条变成蓝色，另一队变成红色
    public void teamHealthbarColorUpdate()
    {
        for(int i = 0; i < numberOfTeams; i++)
        {
            GameObject team = returnTeam(i);
            if(team == returnTeam(currentTeam))
            {
                foreach (Transform unit in team.transform)
                {
                    unit.GetComponent<UnitScript>().changeHealthBarColour(0);
                }
            }
            else
            {
                foreach (Transform unit in team.transform)
                {
                    unit.GetComponent<UnitScript>().changeHealthBarColour(1);
                }
            }
        }
       
        
    }
    //输入：目标格子的x轴和y轴的值
    //输出：一个Node List作为路线
    //描述：生成一个到达目标格子x和y轴的鼠标路径
    public List<Node> generateCursorRouteTo(int x, int y)
    {

        if (TMS.selectedUnit.GetComponent<UnitScript>().x == x && TMS.selectedUnit.GetComponent<UnitScript>().y == y) //如果鼠标点击的位置是被选中的Unit的原来位置，在游戏里就是玩家点击了目标原地的位置
        {
            Debug.Log("clicked the same tile that the unit is standing on");
            currentPathForUnitRoute = new List<Node>(); //重置这个Node List
            

            return currentPathForUnitRoute;            //输出这个被重置的Node List，被重置的Node List里啥也没有，相当于无事发生
        }
        if (TMS.unitCanEnterTile(x, y) == false)
        {
            //无法移动到某些地方，但是必须要return些啥
            //无法移动到鼠标点击的tile，选择的终点（endpoint）无法作为这次路径的目标

            return null; //没反应
        }

        //TMS.selectedUnit.GetComponent<UnitScript>().path = null;
        currentPathForUnitRoute = null;
        //from wiki dijkstra's
        Dictionary<Node, float> dist = new Dictionary<Node, float>(); //这个字典存储每个节点到起始节点的最短已知距离。
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();   //这个字典用于回溯路径。对于每个节点，它存储了达到该节点的最佳路径上的前一个节点。
        Node source = TMS.graph[TMS.selectedUnit.GetComponent<UnitScript>().x, TMS.selectedUnit.GetComponent<UnitScript>().y]; //这行获取起始单位的当前位置，并将其转换为一个Node，作为路径搜索的起点。
        Node target = TMS.graph[x, y]; //这行获取目标位置，并将其转换为一个Node，作为路径搜索的终点。
        dist[source] = 0; //从起点到自己的距离总是0
        prev[source] = null; //起点没有前一个节点，因为它是路径上的第一个节点。
        List<Node> unvisited = new List<Node>();//这行代码初始化这个列表。接下来，你通常会将所有的节点加入这个列表，因为一开始它们都是未访问的。

        //初始化
        foreach (Node n in TMS.graph)    //这循环遍历了TMS.graph中的所有节点。这个graph很可能是一个2D数组，包含了整个战场的每个格子
        {


            if (n != source)             //这是检查当前节点n是否不是起点。之前已经为起点设置了dist和prev的值。
            {
                dist[n] = Mathf.Infinity;//对于所有非起点的节点，其初始到起点的距离都设置为无穷大。因为在这时，还不知道到这些点的实际距离。
                prev[n] = null;          //同样地，对于所有非起点的节点，它们的前一个节点都还未知，所以设置为null
            }
            unvisited.Add(n);            //将当前节点n添加到unvisited列表。这表示开始时，所有节点都是未访问的
        }

        while (unvisited.Count > 0)      //当unvisited列表中还有节点时，这个循环会继续执行。
        {
            
            Node u = null;
            foreach (Node possibleU in unvisited)
            {
                if (u == null || dist[possibleU] < dist[u])
                {
                    u = possibleU;       //这个内部的foreach循环是为了找到在unvisited列表中dist值最小的节点,就是u
                }
            }


            if (u == target)             //如果我们已经访问到了目标节点，则提前退出循环
            {
                break;
            }

            unvisited.Remove(u);         //从unvisited列表中移除节点u，因为我们现在正在访问它

            foreach (Node n in u.neighbours)        //这个循环访问节点u的所有邻居，通常为上、下、左、右的节点（在一个四方向的格子地图中）
            {

                //float alt = dist[u] + u.DistanceTo(n);
                float alt = dist[u] + TMS.costToEnterTile(n.x, n.y); //这里计算从源节点到节点n的新距离，通过节点u
                                                                     //TMS.costToEnterTile(n.x, n.y)是获取进入该节点付出的移动距离。例如，山地可能会比草地高
                if (alt < dist[n])       //如果通过节点u到达节点n的新距离alt比当前存储在dist[n]的距离更短，我们就更新dist[n]和prev[n]
                                         //这是选择最短路径的关键步骤。
                {
                    dist[n] = alt;
                    prev[n] = u;
                }
            }
        }
       
        if (prev[target] == null) //如果prev字典中的目标节点target为null，这意味着从source到target没有找到路径
        {
            //返回null表示无法到达。
            return null;
        }
        currentPathForUnitRoute = new List<Node>(); //存储从source到target的路径
        
        Node curr = target;     
        while (curr != null)    //开始于目标节点。这个循环是回溯的关键。通过prev字典，我们回溯每一步直到我们到达source,如此就得到了从target到source的路径
        {
            currentPathForUnitRoute.Add(curr);      //步进当前路径并将其添加到链中 
            curr = prev[curr];                      //然后我们会通过prev字典回溯到source 
        }
       
        currentPathForUnitRoute.Reverse(); //由于我们得到的路径是从target到source的，我们需要反转它，以得到从source到target的正确路径

        return currentPathForUnitRoute;    //返回计算出的路径




    }

    //输入：一个需要被重置的四边形gameobject（UI的那个箭头） 
    //描述：重置箭头的旋转
    public void resetQuad(GameObject quadToReset) //此方法将quad的材质设置为UICursor并重置它的旋转角度为(90, 0, 0)
    {
        quadToReset.GetComponent<Renderer>().material = UICursor;
        quadToReset.transform.eulerAngles = new Vector3(90, 0, 0);  //这意味着这个四边形将面向"上"或"正面"方向

    }

    //输入：一个2D坐标，代表我们要更改的位置（cursorPos）和一个3D向量（arrowRotationVector），表示我们将旋转四边形的角度
    //描述：生成一个指示箭头
    public void UIunitRouteArrowDisplay(Vector2 cursorPos,Vector3 arrowRotationVector)
    {
        GameObject quadToManipulate = TMS.quadOnMapForUnitMovementDisplay[(int)cursorPos.x, (int)cursorPos.y]; //在TMS.quadOnMapForUnitMovementDisplay二维数组中方法寻找一个与cursorPos对应的四边形
        quadToManipulate.transform.eulerAngles = arrowRotationVector;                                          //然后旋转这个四边形
        quadToManipulate.GetComponent<Renderer>().material = UIunitRouteArrow;                                 //激活并设置其材质为UIunitRouteArrow
        quadToManipulate.GetComponent<Renderer>().enabled = true;
    }

    //输入：当前的2D坐标/向量，下一个2D坐标/向量
    //输出：方向向量
    //描述：返回从当前向量到下一个向量的方向
    public Vector2 directionBetween(Vector2 currentVector, Vector2 nextVector)
    {

        
        Vector2 vectorDirection = (nextVector - currentVector).normalized; //通过nextVector - currentVector计算从当前向量到下一个向量的向量。
                                                                           //之后，它标准化此向量以获得方向（长度为1的向量）
       
        if (vectorDirection == Vector2.right)                              //检查此方向是否与已知的四个方向（右、左、上、下）匹配，并返回相应的方向
        {
            return Vector2.right;
        }
        else if (vectorDirection == Vector2.left)
        {
            return Vector2.left;
        }
        else if (vectorDirection == Vector2.up)
        {
            return Vector2.up;
        }
        else if (vectorDirection == Vector2.down)
        {
            return Vector2.down;
        }
        else
        {
            Vector2 vectorToReturn = new Vector2();                        //如果没有匹配的方向，则返回一个零向量（即new Vector2()，其x和y都是0）
            return vectorToReturn;
        }
    }

    //输入: nodex和nodey代表被操作的瓦片的坐标， i代表在路径列表中的位置
    //描述：基于两个方向向量（从前一个瓦片到当前瓦片的方向和从当前瓦片到下一个瓦片的方向）为特定的网格瓦片设置适当的UI
    public void setCorrectRouteWithInputAndOutput(int nodeX,int nodeY,int i)
    {
        Vector2 previousTile = new Vector2(unitPathToCursor[i - 1].x + 1, unitPathToCursor[i - 1].y + 1); //首先计算前一个瓦片、当前瓦片和下一个瓦片的位置
        Vector2 currentTile = new Vector2(unitPathToCursor[i].x + 1, unitPathToCursor[i].y + 1);
        Vector2 nextTile = new Vector2(unitPathToCursor[i + 1].x + 1, unitPathToCursor[i + 1].y + 1);

        Vector2 backToCurrentVector = directionBetween(previousTile, currentTile);       //计算两个方向向量：从前一个瓦片到当前瓦片的方向，以及从当前瓦片到下一个瓦片的方向
        Vector2 currentToFrontVector = directionBetween(currentTile, nextTile);


        //Right (UP/DOWN/RIGHT)
        if (backToCurrentVector == Vector2.right && currentToFrontVector == Vector2.right) //根据这两个方向向量设置瓦片的旋转和材质，以表示正确的路径方向和是否存在转弯
        {                                                                                  //if就完事了，奥利给！
            //Debug.Log("[IN[R]]->[Out[R]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.right && currentToFrontVector == Vector2.up)
        {
            //Debug.Log("[IN[R]]->[Out[UP]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 180);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;

        }
        else if (backToCurrentVector == Vector2.right && currentToFrontVector == Vector2.down)
        {
            //Debug.Log("[IN[R]]->[Out[DOWN]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        //Left (UP/DOWN/LEFT)
        else if (backToCurrentVector == Vector2.left && currentToFrontVector == Vector2.left)
        {
            //Debug.Log("[IN[L]]->[Out[L]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.left && currentToFrontVector == Vector2.up)
        {
            //Debug.Log("[IN[L]]->[Out[UP]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.left && currentToFrontVector == Vector2.down)
        {
            //Debug.Log("[IN[L]]->[Out[DOWN]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        //UP (UP/RIGHT/LEFT)
        else if (backToCurrentVector == Vector2.up && currentToFrontVector == Vector2.up)
        {
            //Debug.Log("[IN[UP]]->[Out[UP]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.up && currentToFrontVector == Vector2.right)
        {
            //Debug.Log("[IN[UP]]->[Out[R]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.up && currentToFrontVector == Vector2.left)
        {
            //Debug.Log("[IN[UP]]->[Out[L]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        //DOWN (DOWN/RIGHT/LEFT)
        else if (backToCurrentVector == Vector2.down && currentToFrontVector == Vector2.down)
        {
            //Debug.Log("[IN[DOWN]]->[Out[DOWN]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRoute;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.down && currentToFrontVector == Vector2.right)
        {
            //Debug.Log("[IN[DOWN]]->[Out[R]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;

        }
        else if (backToCurrentVector == Vector2.down && currentToFrontVector == Vector2.left)
        {
            //Debug.Log("[IN[DOWN]]->[Out[L]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 180);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteCurve;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
    }

    //输入: nodex和nodey代表被操作的瓦片的坐标， i代表在路径列表中的位置
    //描述：根据路径接近这个最后一个瓦片的方向，决定如何显示结束的箭头
    public void setCorrectRouteFinalTile(int nodeX,int nodeY,int i)
    {
        Vector2 previousTile = new Vector2(unitPathToCursor[i - 1].x + 1, unitPathToCursor[i - 1].y + 1);   //首先计算前一个瓦片和当前瓦片的位置
        Vector2 currentTile = new Vector2(unitPathToCursor[i].x + 1, unitPathToCursor[i].y + 1);
        Vector2 backToCurrentVector = directionBetween(previousTile, currentTile);                          //计算从前一个瓦片到当前瓦片的方向向量

        if (backToCurrentVector == Vector2.right)
        {
            //Debug.Log("[IN[R]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];                    //根据这个方向向量设置瓦片的旋转和材质，以表示正确的路径方向和是否存在转弯
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 270);                 //三二三四再来一次！
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.left)
        {
            //Debug.Log("[IN[L]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 90);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;

        }
        else if (backToCurrentVector == Vector2.up)
        {
            //Debug.Log("[IN[U]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 0);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
        else if (backToCurrentVector == Vector2.down)
        {
            //Debug.Log("[IN[D]]");
            GameObject quadToUpdate = TMS.quadOnMapForUnitMovementDisplay[nodeX, nodeY];
            quadToUpdate.GetComponent<Transform>().rotation = Quaternion.Euler(90, 0, 180);
            quadToUpdate.GetComponent<Renderer>().material = UIunitRouteArrow;
            quadToUpdate.GetComponent<Renderer>().enabled = true;
        }
    }

    //输入：两个Game object Unit
    //描述：一个协程，用于检查两个单位（或玩家）之间的战斗是否结束，并显示胜利者，同时确保在显示胜利者之前，所有战斗相关的动画和事件都已经完成
    public IEnumerator checkIfUnitsRemainCoroutine(GameObject unit, GameObject enemy)
    {
        while (unit.GetComponent<UnitScript>().combatQueue.Count != 0) //首先检查两个unit的combatQueue是否为空。这个combatQueue可能包含战斗动画或其他战斗相关的事件
        {
            yield return new WaitForEndOfFrame();
        }
        
        while (enemy.GetComponent<UnitScript>().combatQueue.Count != 0)
        {
            yield return new WaitForEndOfFrame();
        }
        if (team1.transform.childCount == 0)                           //当两个单位的combatQueue都为空时，这意味着战斗动画和事件都已经完成
        {
            displayWinnerUI.enabled = true;                            //接着检查team1或team2是否没有子对象，即是否没有存活的单位
            displayWinnerUI.GetComponentInChildren<TextMeshProUGUI>().SetText("Player 2 has won!");    //一方没有存活单位，另一方就胜出
           
            
        }
        else if (team2.transform.childCount == 0)
        {
            displayWinnerUI.enabled = true;
            displayWinnerUI.GetComponentInChildren<TextMeshProUGUI>().SetText("Player 1 has won!");

          
        }


    }



    //描述：胜利结算画面
    
    public void win()
    {
        displayWinnerUI.enabled = true;
        displayWinnerUI.GetComponentInChildren<TextMeshProUGUI>().SetText("Winner!");

    }

  
   
}
