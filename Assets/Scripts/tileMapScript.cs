using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tileMapScript : MonoBehaviour    
{
    //引用其他核心脚本
    [Header("Manager Scripts")]
    public battleManagerScript BMS;//战斗核心
    public gameManagerScript GMS;//游戏核心

    //传入地图的类型（泥土、水等等）
    [Header("Tiles")]
    public Tile[] tileTypes;
    public int[,] tiles;

    //存在于地图上的所有单位
    [Header("Units on the board")]
    public GameObject unitsOnBoard;

    //2d数组，格子对象在地图上的列表
    public GameObject[,] tilesOnMap;

    //2d数组，格子对象在地图上高亮图形的列表
    public GameObject[,] quadOnMap;//移动攻击范围
    public GameObject[,] quadOnMapForUnitMovementDisplay;//移动的箭头
    public GameObject[,] quadOnMapCursor;//选中的格子的高亮
    
    //public is only to set them in the inspector, if you change these to private then you will
    //need to re-enable them in the inspector
    //叠加以显示可能移动的对象
    public GameObject mapUI;
    //在鼠标位置显示的高光
    public GameObject mapCursorUI;
    //突出路径的高光
    public GameObject mapUnitMovementUI;

    //当前路径，到达当前选择目标的最短路径
    public List<Node> currentPath = null;

    //寻路的节点图
    public Node[,] graph;

    //UI产生的容器（父对象）
    [Header("Containers")]
    public GameObject tileContainer;
    public GameObject UIQuadPotentialMovesContainer;
    public GameObject UIQuadCursorContainer;
    public GameObject UIUnitMovementPathContainer;
    

    //地图大小
    [Header("Board Size")]
    public int mapSizeX;
    public int mapSizeY;

    //鼠标光线追踪投射此单元，即鼠标选中的位置对象
    [Header("Selected Unit Info")]
    public GameObject selectedUnit;
    //主要用于高光检查，也用于移动检查等
    public HashSet<Node> selectedUnitTotalRange;
    public HashSet<Node> selectedUnitMoveRange;

    public bool unitSelected = false;

    public int unitSelectedPreviousX;
    public int unitSelectedPreviousY;

    public GameObject previousOccupiedTile;


    //public AudioSource selectedSound;
    //public AudioSource unselectedSound;
    //public area to set the material for the quad material for UI purposes
    [Header("Materials")]
    public Material greenUIMat;
    public Material redUIMat;
    public Material blueUIMat;


    private void Start()
    {
        //Get the battlemanager running
        //BMS = GetComponent<battleManagerScript>();
        //GMS = GetComponent<gameManagerScript>();
        
        generateMapInfo();//为地图的所有格子定义为相应的类型值
        
        generatePathFindingGraph();//为地图的每一个点创造一个图形
        
        generateMapVisuals();//实例化地图的图形信息
        
        setIfTileIsOccupied();//设置已经被单位占用的格子为已占用


    }

    private void Update()
    {

        //如果鼠标左键单击
        if (Input.GetMouseButtonDown(0))
        {
            if (selectedUnit == null)//如果选中单位为空
            {
                //mouseClickToSelectUnit();
                mouseClickToSelectUnitV2();//根据鼠标位置选中单位

            }
            //检查单位是否进入选中状态
            else if (selectedUnit.GetComponent<UnitScript>().unitMoveState == selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(1) && selectedUnit.GetComponent<UnitScript>().movementQueue.Count == 0)//如果选中单位的状态为选中且不在移动
            {

               
                if ( selectTileToMoveTo())//如果目标可以移动到
                {
                    
                    unitSelectedPreviousX = selectedUnit.GetComponent<UnitScript>().x;
                    unitSelectedPreviousY = selectedUnit.GetComponent<UnitScript>().y;//暂存当前单位的位置，取消时返回
                    previousOccupiedTile = selectedUnit.GetComponent<UnitScript>().tileBeingOccupied;//暂存当前占据的格子，取消时返回
                    selectedUnit.GetComponent<UnitScript>().setWalkingAnimation();//设置当前单位为移动动画
                    moveUnit();//移动所选单位
                    
                    StartCoroutine(moveUnitAndFinalize());//移动完成
                    
                    
                    
                }

            }
            //至此，完成移动
            else if(selectedUnit.GetComponent<UnitScript>().unitMoveState == selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(2))//如果当前单位的状态是已经移动完成
            {
                if (selectedUnit.GetComponent<UnitScript>().teamNum >= 2)//如果是怪物的队伍
                {
                    
                }
                else
                {
                    finalizeOption();//攻击确认
                }
                
            }
            
        }
        
        //右键单击，取消选中
        if (Input.GetMouseButtonDown(1))
        {
            if (selectedUnit != null)//如果有已经选中的单位
            {
                if (selectedUnit.GetComponent<UnitScript>().movementQueue.Count == 0 && selectedUnit.GetComponent<UnitScript>().combatQueue.Count==0)//如果选中单位不在移动不在战斗
                {
                    if (selectedUnit.GetComponent<UnitScript>().unitMoveState != selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(3))//如果选中单位不在等待中
                    {
                        //unselectedSound.Play();
                        selectedUnit.GetComponent<UnitScript>().setIdleAnimation();//设置待机动画
                        deselectUnit();//取消选中
                    }
                }
                else if (selectedUnit.GetComponent<UnitScript>().movementQueue.Count == 1)//如果选中单位在移动中
                {
                    selectedUnit.GetComponent<UnitScript>().visualMovementSpeed = 0.5f;//移动速度设置为0.5
                }
            }
        }
       
        
    }
    

    public void generateMapInfo()//为地图的所有格子定义为相应的类型值
    {
        tiles = new int[mapSizeX, mapSizeY];
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                tiles[x, y] = 0;
            }
        }
        tiles[2, 7] = 2;
        tiles[3, 7] = 2;
       
        tiles[6, 7] = 2;
        tiles[7, 7] = 2;

        tiles[2, 2] = 2;
        tiles[3, 2] = 2;
       
        tiles[6, 2] = 2;
        tiles[7, 2] = 2;

        tiles[0, 3] = 3;
        tiles[1, 3] = 3;
        tiles[0, 2] = 3;
        tiles[1, 2] = 3;

        tiles[0, 6] = 3;
        tiles[1, 6] = 3;
        tiles[2, 6] = 3;
        tiles[0, 7] = 3;
        tiles[1, 7] = 3;

        tiles[2, 3] = 3;
        tiles[0, 4] = 1;
        tiles[0, 5] = 1;
        tiles[1, 4] = 1;
        tiles[1, 5] = 1;
        tiles[2, 4] = 3;
        tiles[2, 5] = 3;

        tiles[4, 4] = 1;
        tiles[5, 4] = 1;
        tiles[4, 5] = 1;
        tiles[5, 5] = 1;

        tiles[7, 3] = 3;
        tiles[8, 3] = 3;
        tiles[9, 3] = 3;
        tiles[8, 2] = 3;
        tiles[9, 2] = 3;
        tiles[7, 4] = 3;
        tiles[7, 5] = 3;
        tiles[7, 6] = 3;
        tiles[8, 6] = 3;
        tiles[9, 6] = 3;
        tiles[8, 7] = 3;
        tiles[9, 7] = 3;
        tiles[8, 4] = 1;
        tiles[8, 5] = 1;
        tiles[9, 4] = 1;
        tiles[9, 5] = 1;


    }
    
    public void generatePathFindingGraph()//为寻路创建图形
    {
        graph = new Node[mapSizeX, mapSizeY];

        //初始化图形，为地图的每个点都创建了一个图形
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;
            }
        }
        //计算相邻位置，放在graph[x, y].neighbours里
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {              
                //X is not 0, then we can add left (x - 1)
                if (x > 0)
                {                   
                    graph[x, y].neighbours.Add(graph[x - 1, y]);
                }
                //X is not mapSizeX - 1, then we can add right (x + 1)
                if (x < mapSizeX-1)
                {                   
                    graph[x, y].neighbours.Add(graph[x + 1, y]);
                }
                //Y is not 0, then we can add downwards (y - 1 ) 
                if (y > 0)
                {
                    graph[x, y].neighbours.Add(graph[x, y - 1]);
                }
                //Y is not mapSizeY -1, then we can add upwards (y + 1)
                if (y < mapSizeY - 1)
                {
                    graph[x, y].neighbours.Add(graph[x, y + 1]);
                }
               
               
            }
        }
    }


    
    public void generateMapVisuals()//实例化地图四边形以及相关信息
    {
        //生成实际图块的对象
        tilesOnMap = new GameObject[mapSizeX, mapSizeY];
        quadOnMap = new GameObject[mapSizeX, mapSizeY];
        quadOnMapForUnitMovementDisplay = new GameObject[mapSizeX, mapSizeY];
        quadOnMapCursor = new GameObject[mapSizeX, mapSizeY];
        int index;
        for (int x = 0; x < mapSizeX; x++)
        {
            for (int y = 0; y < mapSizeY; y++)
            {
                index = tiles[x, y];
                GameObject newTile = Instantiate(tileTypes[index].tileVisualPrefab, new Vector3(x, 0, y), Quaternion.identity);//根据已经设定好的相应位置的格子的类型值，创建相应类型的格子
                newTile.GetComponent<ClickableTileScript>().tileX = x;
                newTile.GetComponent<ClickableTileScript>().tileY = y;
                newTile.GetComponent<ClickableTileScript>().map = this;//设置格子的x、y值，并将？
                newTile.transform.SetParent(tileContainer.transform);//设置父对象为tile的容器
                tilesOnMap[x, y] = newTile;//将格子和地图上的位置绑定

                //创建并设置显示潜在移动路径的网格UI
                GameObject gridUI = Instantiate(mapUI, new Vector3(x, 0.501f, y),Quaternion.Euler(90f,0,0));
                gridUI.transform.SetParent(UIQuadPotentialMovesContainer.transform);
                quadOnMap[x, y] = gridUI;
                //创建并设置显示单位移动路径的网格UI
                GameObject gridUIForPathfindingDisplay = Instantiate(mapUnitMovementUI, new Vector3(x, 0.502f, y), Quaternion.Euler(90f, 0, 0));
                gridUIForPathfindingDisplay.transform.SetParent(UIUnitMovementPathContainer.transform);
                quadOnMapForUnitMovementDisplay[x, y] = gridUIForPathfindingDisplay;
                //创建并设置显示光标位置的网格UI
                GameObject gridUICursor = Instantiate(mapCursorUI, new Vector3(x, 0.503f, y), Quaternion.Euler(90f, 0, 0));
                gridUICursor.transform.SetParent(UIQuadCursorContainer.transform);              
                quadOnMapCursor[x, y] = gridUICursor;

            }
        }
    }

    
    public void moveUnit()//移动该单位
    {
        if (selectedUnit != null)//如果有选中任意单位
        {
            selectedUnit.GetComponent<UnitScript>().MoveNextTile();//使用MoveNextTile()方法移动
        }
    }

    
    public Vector3 tileCoordToWorldCoord(int x, int y)//通过格子的x和y值返回世界坐标
    {
        return new Vector3(x, 0.75f, y);
    }

    public void setIfTileIsOccupied()//如果格子上有单位，则设置为已占用
    {
        foreach (Transform team in unitsOnBoard.transform)//找到所有单位中的所有队伍
        {
            //Debug.Log("Set if Tile is Occupied is Called");
            foreach (Transform unitOnTeam in team) //找到队伍中的所有单位
            { 
                int unitX = unitOnTeam.GetComponent<UnitScript>().x;
                int unitY = unitOnTeam.GetComponent<UnitScript>().y;
                unitOnTeam.GetComponent<UnitScript>().tileBeingOccupied = tilesOnMap[unitX, unitY];//在单位上绑定单位和相应位置的格子
                tilesOnMap[unitX, unitY].GetComponent<ClickableTileScript>().unitOnTile = unitOnTeam.gameObject;//在地图上绑定单位和相应位置
            }
            
        }
    }
    
    public void generatePathTo(int x, int y)//传入X和Y生成到达此处的最短路径
    {

        if (selectedUnit.GetComponent<UnitScript>().x == x && selectedUnit.GetComponent<UnitScript>().y == y)
        {
            Debug.Log("选到了自己单位在的格子");
            currentPath = new List<Node>();//初始化当前路径
            selectedUnit.GetComponent<UnitScript>().path = currentPath;//选中的单位的路径为当前路径
            
            return;
        }
        if (unitCanEnterTile(x, y) == false)//如果此地不能进入则直接返回
        {
            
            return;
        }

        selectedUnit.GetComponent<UnitScript>().path = null;//所选单位的路径为空
        currentPath = null;//当前路径为空

        Dictionary<Node, float> dist = new Dictionary<Node, float>();//创建一个字典，通过节点来找距离
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();//创建一个字典，通过节点来找上一个节点
        Node source = graph[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y];//设置起始节点
        Node target = graph[x, y];//设置目标节点
        dist[source] = 0;
        prev[source] = null;
        //Unchecked nodes
        List<Node> unvisited = new List<Node>();//创建一个新列表，存储所有还未检查的节点

        //初始化所有节点
        foreach (Node n in graph)//对于每一个图中的节点
        {

            if (n != source)//如果n不是起始节点
            {
                dist[n] = Mathf.Infinity;//设置到所有节点距离为无限大
                prev[n] = null;//设置所有节点的上一个到达的节点为空
            }
            unvisited.Add(n);//在未检查过的列表里加入所有节点
        }
        //检查未见过的列表里的节点
        while (unvisited.Count > 0)
        {
            //u 是到达距离最短的未访问节点
            Node u = null;//初始化未访问节点
            foreach (Node possibleU in unvisited)//对于未检查列表里的所有节点进行一次判断，找出目前到达距离最小的u节点
            {
                if (u == null || dist[possibleU] < dist[u])//如果u是空的或者到该节点的距离小于到u节点的距离，则将该节点设置为u节点
                {
                    u = possibleU;
                }
            }


            if (u == target)//如果u节点是目的地，则中断循环
            {
                break;
            }

            unvisited.Remove(u);//将u节点从未检查列表中移除

            foreach (Node n in u.neighbours)//检查u节点的所有相邻格子n
            {

                //float alt = dist[u] + u.DistanceTo(n);
                float alt = dist[u] + costToEnterTile(n.x, n.y);//计算到达u节点的距离和到达n位置的移动力消耗的和
                if (alt < dist[n])//如果计算结果小于到n的距离
                {
                    dist[n] = alt;//设置到达n的距离为计算结果
                    prev[n] = u;//设置上一个到达的节点为u
                }
            }
        }
        //如果找到了最短路径或者没有路径存在
        if (prev[target] == null)//如果目标节点没有上一个节点
        {
            return;//直接中断
        }
        currentPath = new List<Node>();//初始化当前路径
        Node curr = target;//创建一个当前节点，设置为目标
        //执行当前路径并添加到链中
        while (curr != null)//当前节点不是空时
        {
            currentPath.Add(curr);//当前路径中添加当前节点
            
            curr = prev[curr];//当前节点设置为其上一个节点
        }
        //当前路径中的节点是从目标到起点，需要反转
        currentPath.Reverse();

        selectedUnit.GetComponent<UnitScript>().path = currentPath;//将当前列表传给单位
       



    }

    
    public float costToEnterTile(int x, int y)//传入X和Y，确认进入该处需要花费的移动力
    {

        if (unitCanEnterTile(x, y) == false)
        {
            return Mathf.Infinity;

        }//如果是不可进入的地方，则移动力消耗为无穷大

        Tile t = tileTypes[tiles[x, y]];
        float dist = t.movementCost;

        return dist;//根据地块类型决定消耗的移动力
    }

    
    public bool unitCanEnterTile(int x, int y)//传入X和Y确认该处是否可以通行
    {
        if (tilesOnMap[x, y].GetComponent<ClickableTileScript>().unitOnTile != null)
        {
            if (tilesOnMap[x, y].GetComponent<ClickableTileScript>().unitOnTile.GetComponent<UnitScript>().teamNum != selectedUnit.GetComponent<UnitScript>().teamNum)
            {
                
            }
            return false;//当有其他单位占据时不可通行
        }
        return tileTypes[tiles[x, y]].isWalkable;//根据该地的类型确认是否可通行
    }


    
    public void mouseClickToSelectUnit()//根据鼠标位置选中单位【已废弃的代码】
    {
        GameObject tempSelectedUnit;
        
        RaycastHit hit;       
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        
       
        if (Physics.Raycast(ray, out hit))
        {


            //Debug.Log(hit.transform.tag);
            if (unitSelected == false)
            {
               
                if (hit.transform.gameObject.CompareTag("Tile"))
                {
                    if (hit.transform.GetComponent<ClickableTileScript>().unitOnTile != null)
                    {


                        tempSelectedUnit = hit.transform.GetComponent<ClickableTileScript>().unitOnTile;
                        if (tempSelectedUnit.GetComponent<UnitScript>().unitMoveState == tempSelectedUnit.GetComponent<UnitScript>().getMovementStateEnum(0)
                            && tempSelectedUnit.GetComponent<UnitScript>().teamNum == GMS.currentTeam
                            )
                        {
                            disableHighlightUnitRange();
                            selectedUnit = tempSelectedUnit;
                            selectedUnit.GetComponent<UnitScript>().map = this;
                            selectedUnit.GetComponent<UnitScript>().setMovementState(1);
                            unitSelected = true;
                            
                            highlightUnitRange();
                        }
                    }
                }

                else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Unit"))
                {
                    
                    tempSelectedUnit = hit.transform.parent.gameObject;
                    if (tempSelectedUnit.GetComponent<UnitScript>().unitMoveState == tempSelectedUnit.GetComponent<UnitScript>().getMovementStateEnum(0)
                          && tempSelectedUnit.GetComponent<UnitScript>().teamNum == GMS.currentTeam
                        )
                    {

                        disableHighlightUnitRange();
                        selectedUnit = tempSelectedUnit;
                        selectedUnit.GetComponent<UnitScript>().setMovementState(1);
                        //These were here before I don't think they do anything the unit location is set beforehand
                        //selectedUnit.GetComponent<UnitScript>().x = (int)selectedUnit.transform.position.x;
                        // selectedUnit.GetComponent<UnitScript>().y = (int)selectedUnit.transform.position.z;
                        selectedUnit.GetComponent<UnitScript>().map = this;
                        unitSelected = true;
                       
                        highlightUnitRange();
                    }
                }
            }

         }
    }



    
    public void finalizeMovementPosition()//完成移动后的设置，将单位所站的格子绑定之类的
    {
        tilesOnMap[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y].GetComponent<ClickableTileScript>().unitOnTile = selectedUnit;
        //在格子上把单位和格子绑定


        selectedUnit.GetComponent<UnitScript>().setMovementState(2);//设置目标单位的状态为已经移动过
       
        highlightUnitAttackOptionsFromPosition();
        highlightTileUnitIsOccupying();
    }




    
    public void mouseClickToSelectUnitV2()//根据鼠标位置选中单位2.0
    {
        
        if (unitSelected == false && GMS.tileBeingDisplayed!=null)//如果没有已经选中任何单位，且光标对象不为空
        {

            if (GMS.tileBeingDisplayed.GetComponent<ClickableTileScript>().unitOnTile != null)//如果光标对象的单位站的格子不是空的
            {
                GameObject tempSelectedUnit = GMS.tileBeingDisplayed.GetComponent<ClickableTileScript>().unitOnTile;//创建一个object，是光标对象的格子上的单位
                if (tempSelectedUnit.GetComponent<UnitScript>().unitMoveState == tempSelectedUnit.GetComponent<UnitScript>().getMovementStateEnum(0)
                               && tempSelectedUnit.GetComponent<UnitScript>().teamNum == GMS.currentTeam && (GMS.currentTeam == 0|| GMS.currentTeam == 1))//如果想要选择的单位处于未选中状态，且角色队伍等于当前队伍,且角色队伍是可以选中的队伍，即主角或召唤物
                {
                    disableHighlightUnitRange();//取消地图上所有的高亮显示
                    //selectedSound.Play();
                    selectedUnit = tempSelectedUnit;//选中单位设置为想要选择的单位
                    selectedUnit.GetComponent<UnitScript>().map = this;//选中单位的地图设置为当前地图
                    selectedUnit.GetComponent<UnitScript>().setMovementState(1);//选中单位的状态改为被选中
                    selectedUnit.GetComponent<UnitScript>().setSelectedAnimation();//选中单位的动画改为被选中的情况
                    unitSelected = true;//声明已经有选中的单位了
                    highlightUnitRange();//高亮选中单位的范围
                   
                }
            }
        }
        
    }


    
    public void finalizeOption()//攻击的确认
    {
    
    RaycastHit hit;
    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//创建了一条摄像机到鼠标位置的射线
    HashSet<Node> attackableTiles = getUnitAttackOptionsFromPosition();//获取当前可攻击的节点

    if (Physics.Raycast(ray, out hit))//执行射线检测
    {

        //此部分用于确认已经点击了tile，并且检查其上是否有单位
        if (hit.transform.gameObject.CompareTag("Tile"))//如果是tile类型的object
        {
            if (hit.transform.GetComponent<ClickableTileScript>().unitOnTile != null)//如果有单位
            {
                GameObject unitOnTile = hit.transform.GetComponent<ClickableTileScript>().unitOnTile;//创建一个对象代表该单位
                int unitX = unitOnTile.GetComponent<UnitScript>().x;
                int unitY = unitOnTile.GetComponent<UnitScript>().y;

                if (unitOnTile == selectedUnit)//如果单位是被选中的单位
                {
                    disableHighlightUnitRange();//取消所有高亮
                    selectedUnit.GetComponent<UnitScript>().wait();//将单位设置为等待不可移动
                    selectedUnit.GetComponent<UnitScript>().setWaitIdleAnimation();//将单位设置为等待动画
                    selectedUnit.GetComponent<UnitScript>().setMovementState(3);//将单位状态设置为等待
                    deselectUnit();//取消选中单位


                }
                else if (unitOnTile.GetComponent<UnitScript>().teamNum != selectedUnit.GetComponent<UnitScript>().teamNum && attackableTiles.Contains(graph[unitX,unitY]))//如果选中单位和目标单位不在同一队伍，并且该节点被包含在可攻击节点中
                {
                        if (unitOnTile.GetComponent<UnitScript>().currentHealthPoints > 0)//如果目标血量大于0
                        {
                            //Debug.Log("We clicked an enemy that should be attacked");
                            //Debug.Log(selectedUnit.GetComponent<UnitScript>().currentHealthPoints);

                            StartCoroutine(BMS.attack(selectedUnit, unitOnTile));//攻击目标

                            
                            StartCoroutine(deselectAfterMovements(selectedUnit, unitOnTile));//执行操作后，取消选中单位
                        }
                }                                     
            }
        }
        else if (hit.transform.parent != null && hit.transform.parent.gameObject.CompareTag("Unit"))//如果是单位类型的object
        {
            GameObject unitClicked = hit.transform.parent.gameObject;
            int unitX = unitClicked.GetComponent<UnitScript>().x;
            int unitY = unitClicked.GetComponent<UnitScript>().y;

            if (unitClicked == selectedUnit)
            {
                disableHighlightUnitRange();//取消所有高亮
                selectedUnit.GetComponent<UnitScript>().wait();//将单位设置为等待不可移动
                selectedUnit.GetComponent<UnitScript>().setWaitIdleAnimation();//将单位设置为等待动画
                selectedUnit.GetComponent<UnitScript>().setMovementState(3);//将单位状态设置为等待
                deselectUnit();//取消选中单位    
            }
            else if (unitClicked.GetComponent<UnitScript>().teamNum != selectedUnit.GetComponent<UnitScript>().teamNum && attackableTiles.Contains(graph[unitX, unitY]))//如果选中单位和目标单位不在同一队伍，并且该节点被包含在可攻击节点中
            {
                    if (unitClicked.GetComponent<UnitScript>().currentHealthPoints > 0)//如果目标血量大于0
                    {
                        //Debug.Log("We clicked an enemy that should be attacked");
                        //Debug.Log(selectedUnit.GetComponent<UnitScript>().currentHealthPoints);

                        StartCoroutine(BMS.attack(selectedUnit, unitClicked));//攻击目标


                        StartCoroutine(deselectAfterMovements(selectedUnit, unitClicked));//执行操作后，取消选中单位
                    }
                    
            }

        }
    }
    
}


    public void deselectUnit()//取消选中单位
    {
        
        if (selectedUnit != null)//如果确实有选中的单位
        {
            if (selectedUnit.GetComponent<UnitScript>().unitMoveState == selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(1))//如果是选中状态
            {
                disableHighlightUnitRange();//取消所有范围高亮
                disableUnitUIRoute();//取消所有箭头显示
                selectedUnit.GetComponent<UnitScript>().setMovementState(0);//将单位状态设置为未选中
                selectedUnit = null;//选中单位设置为空
                unitSelected = false;//设置当前未选中
            }
            else if (selectedUnit.GetComponent<UnitScript>().unitMoveState == selectedUnit.GetComponent<UnitScript>().getMovementStateEnum(3) )//如果是wait状态
            {
                disableHighlightUnitRange();//取消所有范围高亮
                disableUnitUIRoute();//取消所有箭头显示
                selectedUnit = null;//选中单位设置为空
                unitSelected = false;//设置当前未选中
            }
            else
            {
                disableHighlightUnitRange();//取消所有范围高亮
                disableUnitUIRoute();//取消所有箭头显示
                tilesOnMap[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y].GetComponent<ClickableTileScript>().unitOnTile = null;//设置当前单位占据的格子与当前单位解绑
                tilesOnMap[unitSelectedPreviousX, unitSelectedPreviousY].GetComponent<ClickableTileScript>().unitOnTile = selectedUnit;//之前暂存位置的格子与当前选中单位绑定

                selectedUnit.GetComponent<UnitScript>().x = unitSelectedPreviousX;
                selectedUnit.GetComponent<UnitScript>().y = unitSelectedPreviousY;//当前单位的坐标回到暂存位置
                selectedUnit.GetComponent<UnitScript>().tileBeingOccupied = previousOccupiedTile;//当前单位占据的格子定义为之前暂存的格子
                selectedUnit.transform.position = tileCoordToWorldCoord(unitSelectedPreviousX, unitSelectedPreviousY);//调整当前单位的实际位置
                selectedUnit.GetComponent<UnitScript>().setMovementState(0);//设置当前单位不在移动中
                selectedUnit = null;//选中单位设置为空
                unitSelected = false;//设置当前未选中
            }
        }
    }


    
    public void highlightUnitRange()//高亮显示选中单位的移动攻击范围
    {
       
       
        HashSet<Node> finalMovementHighlight = new HashSet<Node>();
        HashSet<Node> totalAttackableTiles = new HashSet<Node>();
        HashSet<Node> finalEnemyUnitsInMovementRange = new HashSet<Node>();
      
        int attRange = selectedUnit.GetComponent<UnitScript>().attackRange;//获取攻击范围
        int moveSpeed = selectedUnit.GetComponent<UnitScript>().moveSpeed;//获取移动速度


        Node unitInitialNode = graph[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y];
        finalMovementHighlight = getUnitMovementOptions();//获取单位可以到达的位置节点
        totalAttackableTiles = getUnitTotalAttackableTiles(finalMovementHighlight, attRange, unitInitialNode);//获取所有最远攻击范围的节点【未移动完成时】

        foreach (Node n in totalAttackableTiles)//对于最远可攻击的所有节点遍历
        {

            if (tilesOnMap[n.x, n.y].GetComponent<ClickableTileScript>().unitOnTile != null)//如果其上有单位
            {
                GameObject unitOnCurrentlySelectedTile = tilesOnMap[n.x, n.y].GetComponent<ClickableTileScript>().unitOnTile;//获取当前选中节点上的单位
                if (unitOnCurrentlySelectedTile.GetComponent<UnitScript>().teamNum != selectedUnit.GetComponent<UnitScript>().teamNum)//如果不在同一队伍中
                {
                    finalEnemyUnitsInMovementRange.Add(n);//在该列表中加入该节点
                }
            }
        }

        
        highlightEnemiesInRange(totalAttackableTiles);//高亮显示所有可攻击的节点
        //highlightEnemiesInRange(finalEnemyUnitsInMovementRange);
        highlightMovementRange(finalMovementHighlight);//高亮显示所有可移动的节点
        //Debug.Log(finalMovementHighlight.Count);
        selectedUnitMoveRange = finalMovementHighlight;//选中单位的移动范围即为高亮显示的所有可移动的节点

        //This final bit sets the selected Units tiles, which can be accessible in other functions
        //Probably bad practice, but I'll need to get things to work for now (2019-09-30)
        selectedUnitTotalRange = getUnitTotalRange(finalMovementHighlight, totalAttackableTiles);//设置全部移动+可攻击到的节点
        //Debug.Log(unionTiles.Count);
        
        //Debug.Log("exiting the while loop");
       

    }


    
    public void disableUnitUIRoute()//取消显示箭头图形
    {
        foreach(GameObject quad in quadOnMapForUnitMovementDisplay)
        {
            if (quad.GetComponent<Renderer>().enabled == true)
            {
                
                quad.GetComponent<Renderer>().enabled = false;
            }
        }
    }


    public HashSet<Node> getUnitMovementOptions()//返回单位可以到达的位置节点
    {
        float[,] cost = new float[mapSizeX, mapSizeY];//定义一个二维浮点数组以获取相应节点的移动消耗
        HashSet<Node> UIHighlight = new HashSet<Node>();//定义UI高亮列表
        HashSet<Node> tempUIHighlight = new HashSet<Node>();//定义暂时UI高亮列表
        HashSet<Node> finalMovementHighlight = new HashSet<Node>();//定义最终要高亮的列表
        int moveSpeed = selectedUnit.GetComponent<UnitScript>().moveSpeed;//获取当前单位的移动速度
        Node unitInitialNode = graph[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y];//存储一个当前单位的位置

        //设置相邻位置的初始消耗
        finalMovementHighlight.Add(unitInitialNode);//当前单位的位置加入最终高亮列表
        
        foreach (Node n in unitInitialNode.neighbours)//对于当前单位位置的相邻位置遍历
        {
            
            cost[n.x, n.y] = costToEnterTile(n.x, n.y);//获取该位置的移动力消耗
            //Debug.Log(cost[n.x, n.y]);
            if (moveSpeed - cost[n.x, n.y] >= 0)//如果移动速度大于等于到该位置的移动力消耗
            {
                UIHighlight.Add(n);//将相邻位置加入UI高亮
            }
        }

        finalMovementHighlight.UnionWith(UIHighlight);//将UI高亮列表中的节点全部加入最终高亮列表

        while (UIHighlight.Count != 0)//如果UI高亮列表不为空
        {
           

            foreach (Node n in UIHighlight)//对UI高亮列表中的每个节点遍历
            {
                foreach (Node neighbour in n.neighbours)//对于每个节点的相邻位置遍历
                {
                    if (!finalMovementHighlight.Contains(neighbour))//如果最终高亮列表中没有该节点
                    {
                        
                        cost[neighbour.x, neighbour.y] = costToEnterTile(neighbour.x, neighbour.y) + cost[n.x, n.y];//移动力消耗等于相邻位置的移动消耗加上其再相邻位置的移动消耗
                        //Debug.Log(cost[neighbour.x, neighbour.y]);
                        if (moveSpeed - cost[neighbour.x, neighbour.y] >= 0)//如果移动速度大于等于到该位置的移动力消耗
                        {
                            
                            //Debug.Log(cost[neighbour.x, neighbour.y]);
                            tempUIHighlight.Add(neighbour);//暂时UI高亮列表中加入该节点
                        }
                    }
                }

            }

            UIHighlight = tempUIHighlight;//UI高亮列表等于暂时UI高亮列表
            finalMovementHighlight.UnionWith(UIHighlight);//将UI高亮列表中的节点全部加入最终高亮列表
            tempUIHighlight = new HashSet<Node>();//暂时UI高亮列表初始化
           
        }
        
        //Debug.Log("The total amount of movable spaces for this unit is: " + finalMovementHighlight.Count);
        //Debug.Log("We have used the function to calculate it this time");
        return finalMovementHighlight;//返回最终高亮列表
    }


    public HashSet<Node> getUnitTotalRange(HashSet<Node> finalMovementHighlight, HashSet<Node> totalAttackableTiles)//返回全部移动+攻击范围的节点
    {
        HashSet<Node> unionTiles = new HashSet<Node>();
        unionTiles.UnionWith(finalMovementHighlight);
        //unionTiles.UnionWith(finalEnemyUnitsInMovementRange);
        unionTiles.UnionWith(totalAttackableTiles);
        return unionTiles;//结合移动高亮的节点和攻击高亮的节点
    }


    public HashSet<Node> getUnitTotalAttackableTiles(HashSet<Node> finalMovementHighlight, int attRange, Node unitInitialNode)//返回所有最远攻击范围的节点【移动时】
    {
        HashSet<Node> tempNeighbourHash = new HashSet<Node>();//暂存位置列表
        HashSet<Node> neighbourHash = new HashSet<Node>();//位置列表
        HashSet<Node> seenNodes = new HashSet<Node>();//不可攻击到的节点列表
        HashSet<Node> totalAttackableTiles = new HashSet<Node>();//最终可攻击的节点列表
        foreach (Node n in finalMovementHighlight)//对于可以移动到的所有节点进行遍历
        {
            neighbourHash = new HashSet<Node>();//初始化位置列表
            neighbourHash.Add(n);//在位置列表中加入节点
            for (int i = 0; i < attRange; i++)//i小于攻击距离时循环遍历
            {
                foreach (Node t in neighbourHash)//遍历位置中的所有节点
                {
                    foreach (Node tn in t.neighbours)//遍历位置列表中所有节点的相邻位置
                    {
                        tempNeighbourHash.Add(tn);//将所有节点加入暂存的位置列表中
                    }
                }

                neighbourHash = tempNeighbourHash;//将位置列表销毁，重新储存暂存的位置列表中的点
                tempNeighbourHash = new HashSet<Node>();//暂存位置列表初始化
                if (i < attRange - 1)//如果i比攻击距离小1
                {
                    seenNodes.UnionWith(neighbourHash);//将位置列表中的节点全部加入不可攻击到的节点列表中
                }

            }
            neighbourHash.ExceptWith(seenNodes);//在位置列表中除去不可攻击到的节点
            seenNodes = new HashSet<Node>();//不可攻击列表初始化
            totalAttackableTiles.UnionWith(neighbourHash);//将位置列表中的节点全部加入可攻击的节点列表中
        }
        totalAttackableTiles.Remove(unitInitialNode);//在可攻击的节点列表中移除初始节点
        
        //Debug.Log("The unit node has this many attack options" + totalAttackableTiles.Count);
        return (totalAttackableTiles);//返回可攻击的节点列表
    }


    public HashSet<Node> getUnitAttackOptionsFromPosition()//返回当前位置可攻击的节点
    {
        HashSet<Node> tempNeighbourHash = new HashSet<Node>();
        HashSet<Node> neighbourHash = new HashSet<Node>();
        HashSet<Node> seenNodes = new HashSet<Node>();
        Node initialNode = graph[selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y];//目前选中单位的位置
        int attRange = selectedUnit.GetComponent<UnitScript>().attackRange;//选中对象的攻击范围


        neighbourHash = new HashSet<Node>();
        neighbourHash.Add(initialNode);//在该列表中加入选中单位的位置作为一个节点
        for (int i = 0; i < attRange; i++)//当i小于攻击范围时遍历
        {
            foreach (Node t in neighbourHash)//对于列表里的每个节点遍历
            {
                foreach (Node tn in t.neighbours)//将每个节点的相邻格子作为新节点
                {
                    tempNeighbourHash.Add(tn);//将所有新节点加入临时表
                }
            }
            neighbourHash = tempNeighbourHash;//原有列表销毁，临时列表的内容设置其中
            tempNeighbourHash = new HashSet<Node>();//新列表初始化
            if (i < attRange - 1)//如果i小于攻击范围1
            {
                seenNodes.UnionWith(neighbourHash);//将列表的所有节点添加进不可攻击范围的列表
            }
        }
        neighbourHash.ExceptWith(seenNodes);//移除不可攻击范围列表的节点
        neighbourHash.Remove(initialNode);//移除初始节点
        return neighbourHash;//返回可攻击列表
    }


    public HashSet<Node> getTileUnitIsOccupying()//返回当前选中单位所占据的格子
    {
       
        int x = selectedUnit.GetComponent<UnitScript>().x;
        int y = selectedUnit.GetComponent<UnitScript>().y;
        HashSet<Node> singleTile = new HashSet<Node>();
        singleTile.Add(graph[x, y]);
        return singleTile;
        
    }


    public void highlightTileUnitIsOccupying()//高亮显示所选单位
    {
        if (selectedUnit != null)
        {
            highlightMovementRange(getTileUnitIsOccupying());//以蓝色高亮被所选单位占据的格子
        }
    }


    public void highlightUnitAttackOptionsFromPosition()//高亮显示所选单位的攻击范围
    {
        if (selectedUnit != null)//如果有选中的单位
        {
            highlightEnemiesInRange(getUnitAttackOptionsFromPosition());//高亮显示可攻击的节点
        }
    }


    public void highlightMovementRange(HashSet<Node> movementToHighlight)//蓝色高亮显示传入参数的节点
    {
        foreach (Node n in movementToHighlight)
        {
            quadOnMap[n.x, n.y].GetComponent<Renderer>().material = blueUIMat;
            quadOnMap[n.x, n.y].GetComponent<MeshRenderer>().enabled = true;
        }
    }




    public void highlightEnemiesInRange(HashSet<Node> enemiesToHighlight)//高亮显示可攻击的节点
    {
        foreach (Node n in enemiesToHighlight)
        {
            quadOnMap[n.x, n.y].GetComponent<Renderer>().material = redUIMat;
            quadOnMap[n.x, n.y].GetComponent<MeshRenderer>().enabled = true;
        }
    }


    
    public void disableHighlightUnitRange()//取消地图上所有的高亮显示
    {
        foreach(GameObject quad in quadOnMap)//寻找地图上的所有图形
        {
            if(quad.GetComponent<Renderer>().enabled == true)
            {
                quad.GetComponent<Renderer>().enabled = false;
            }
        }
    }


    public IEnumerator moveUnitAndFinalize()//完成移动，取消高亮，解绑
    {
        disableHighlightUnitRange();//取消所有范围高亮
        disableUnitUIRoute();//取消显示箭头图像
        while (selectedUnit.GetComponent<UnitScript>().movementQueue.Count != 0)
        {
            yield return new WaitForEndOfFrame();
        }
        finalizeMovementPosition(); //完成移动后的设置，将单位所站的格子绑定之类的
        selectedUnit.GetComponent<UnitScript>().setSelectedAnimation();//设置选中单位为被选中的动画
    }


    
    public IEnumerator deselectAfterMovements(GameObject unit, GameObject enemy)//执行操作后，取消选中单位
    {
        //selectedSound.Play();
        
        selectedUnit.GetComponent<UnitScript>().setMovementState(3);//设置单位为等待状态
        disableHighlightUnitRange();//取消所有范围高亮
        disableUnitUIRoute();//取消显示箭头图像
        
        yield return new WaitForSeconds(.25f);//在此处添加延迟以确保协程的顺利进行

        while (unit.GetComponent<UnitScript>().combatQueue.Count > 0)
        {
            yield return new WaitForEndOfFrame();
        }
        while (enemy.GetComponent<UnitScript>().combatQueue.Count > 0)
        {
            yield return new WaitForEndOfFrame();
          
        }
        //Debug.Log("All animations done playing");

        if(selectedUnit.GetComponent<enemyAI>() != null)
        {
            selectedUnit.GetComponent<enemyAI>().action = false;
        }
        
        deselectUnit();//取消选中单位

    }


    public bool selectTileToMoveTo()//检查选中单位想要前往的位置是否可以移动
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//获取摄像机至鼠标的射线
        if (Physics.Raycast(ray, out hit))
        {
           
            if (hit.transform.gameObject.CompareTag("Tile"))//如果选中的对象是tile
            {
               
                int clickedTileX = hit.transform.GetComponent<ClickableTileScript>().tileX;
                int clickedTileY = hit.transform.GetComponent<ClickableTileScript>().tileY;
                Node nodeToCheck = graph[clickedTileX, clickedTileY];//获取选中对象的位置
                //var unitScript = selectedUnit.GetComponent<UnitScript>();

                if (selectedUnitMoveRange.Contains(nodeToCheck)) //如果选中对象在当前单位可移动范围内
                {
                    if ((hit.transform.gameObject.GetComponent<ClickableTileScript>().unitOnTile == null || hit.transform.gameObject.GetComponent<ClickableTileScript>().unitOnTile == selectedUnit) && (selectedUnitMoveRange.Contains(nodeToCheck)))
                    {
                        //Debug.Log("We have finally selected the tile to move to");
                        generatePathTo(clickedTileX, clickedTileY);//检查到目标位置的最短路径并设置为当前路径
                        return true;
                    }
                }
            }
            else if (hit.transform.gameObject.CompareTag("Unit"))//如果选中对象是单位
            {
              
                if (hit.transform.parent.GetComponent<UnitScript>().teamNum != selectedUnit.GetComponent<UnitScript>().teamNum)//选中的是敌人
                {
                    //Debug.Log("Clicked an Enemy");
                }
                else if(hit.transform.parent.gameObject == selectedUnit)//选中的是自己
                {
                   
                    generatePathTo(selectedUnit.GetComponent<UnitScript>().x, selectedUnit.GetComponent<UnitScript>().y);//检查到自己的位置的最短路径并设置为当前路径
                    return true;
                }
            }

        }
        return false;
    }


}
