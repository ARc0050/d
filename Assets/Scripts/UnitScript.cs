using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitScript : MonoBehaviour
{
    public int teamNum;
    public int x;
    public int y;

    //这是一个不太好的想法，不要用
    public bool coroutineRunning;

    //元定义
    public Queue<int> movementQueue;
    public Queue<int> combatQueue;
    //移动速度
    public float visualMovementSpeed = .15f;

    //动画控制器
    public Animator animator;


    public GameObject tileBeingOccupied;

    public GameObject damagedParticle;
    //单位属性
    public string unitName;
    public int moveSpeed = 2;    
    public int attackRange = 1;
    public int attackDamage = 1;
    public int maxHealthPoints = 5;
    public int currentHealthPoints;
    public Sprite unitSprite;

    [Header("UI Elements")]
    //UI界面
    public Canvas healthBarCanvas;
    public TMP_Text hitPointsText;
    public Image healthBar;

    public Canvas damagePopupCanvas;
    public TMP_Text damagePopupText;
    public Image damageBackdrop;
    

    //如果用2D素材的话可能会改变
    //public Material unitMaterial;
    //public Material unitWaitMaterial;

    public tileMapScript map;

    //更新位置需要用到的出发点和终点
    public Transform startPoint;
    public Transform endPoint;
    public float moveSpeedTime = 1f;
    
    //检查使用的是3D模型还是2D精灵
    //确保只使用了其中一种
    //public GameObject holder3D;
    public GameObject holder2D;
    // 总计距离
    private float journeyLength;

    //确认单位是否在移动的bool
    public bool unitInMovement;


    
    public movementStates unitMoveState;//单位的状态类型
   
    //寻路

    public List<Node> path = null;//路径节点的列表为空

    //移动单元的路径变换
    public List<Node> pathForMovement = null;//用于移动的路径节点列表为空
    public bool completedMovement = false;

    private void Awake()//在游戏一开始就执行，初始化
    {

        animator = holder2D.GetComponent<Animator>();//获取相应单位的动画控制器
        movementQueue = new Queue<int>();//定义移动队列
        combatQueue = new Queue<int>();//定义战斗队列
       
        
        x = (int)transform.position.x;//将当前单位的X坐标转为int存储
        y = (int)transform.position.z;//将当前单位的Y坐标转为int存储
        unitMoveState = movementStates.Unselected;//将单位的状态类型初始化为未选中
        currentHealthPoints = maxHealthPoints;//将当前HP初始化为最大HP
        hitPointsText.SetText(currentHealthPoints.ToString());//显示UI文字
        
     
    }

    public void LateUpdate()//每帧都会调用，在update方法后，这里是保证角色的朝向始终面对相机
    {
        healthBarCanvas.transform.forward = Camera.main.transform.forward;
        //damagePopupCanvas.transform.forward = Camera.main.transform.forward;
        holder2D.transform.forward = Camera.main.transform.forward;
    }

    public void MoveNextTile()//前进到下一个路径中的下一个格子
    {
        if (path.Count == 0)//检查路径长度是否为零
        {
            return;
        }
        else
        {
            StartCoroutine(moveOverSeconds(transform.gameObject, path[path.Count - 1]));//协程代码，使用moveOverSeconds方法，传进当前对象及路径最底部的位置的节点
        }
        
     }

   
    public void moveAgain()//到该玩家的回合后再次行动
    {
        
        path = null;//将路径定义为空
        setMovementState(0);//将单位初始化为未选中
        completedMovement = false;//初始化移动为未完成状态
        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.white;//将单位设置为白色
        //setIdleAnimation();//设置待机动画
        //gameObject.GetComponentInChildren<Renderer>().material = unitMaterial;//3D情况下的设置
    }
    public movementStates getMovementStateEnum(int i)//获取当前单位的状态
    {
        if (i == 0)
        {
            return movementStates.Unselected;
        }
        else if (i == 1)
        {
            return movementStates.Selected;
        }
        else if (i == 2)
        {
            return movementStates.Moved;
        }
        else if (i == 3)
        {
            return movementStates.Wait;
        }
        return movementStates.Unselected;
        
    }
    public void setMovementState(int i)//定义单位的状态
    {
        if (i == 0)
        {
            unitMoveState =  movementStates.Unselected;
        }
        else if (i == 1)
        {
            unitMoveState = movementStates.Selected;
        }
        else if (i == 2)
        {
            unitMoveState = movementStates.Moved;
        }
        else if (i == 3)
        {
            unitMoveState = movementStates.Wait;
        }
       

    }
    public void updateHealthUI()//刷新生命值UI显示
    {
        healthBar.fillAmount = (float)currentHealthPoints / maxHealthPoints;
        hitPointsText.SetText(currentHealthPoints.ToString());
    }
    public void dealDamage(int x)//造成伤害的函数
    {
        currentHealthPoints = currentHealthPoints - x;
        updateHealthUI();
    }
    public void wait()//等待，不可移动
    {

        gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.gray;//将单位设置为灰色
        //gameObject.GetComponentInChildren<Renderer>().material = unitWaitMaterial;//3D
    }
    public void changeHealthBarColour(int i)//改变血条UI颜色，传入0则为蓝色，传入1则为红色
    {
        if (i == 0)
        {
            healthBar.color = Color.blue;
        }
        else if (i == 1)
        {
           
            healthBar.color = Color.red;
        }
    }
    public void unitDie()//单位死亡
    {
        if (holder2D.activeSelf)//检查单位是否可行动，可以行动则执行销毁代码
        {
            StartCoroutine(fadeOut());//单位销毁时颜色渐变
            StartCoroutine(checkIfRoutinesRunning());//单位销毁时删除对象
           
        }
       
        //Destroy(gameObject,2f);
        /*
        Renderer rend = GetComponentInChildren<SpriteRenderer>();
        Color c = rend.material.color;
        c.a = 0f;
        rend.material.color = c;
        StartCoroutine(fadeOut(rend));*/
       
    }
    public IEnumerator checkIfRoutinesRunning()
    {
        while (combatQueue.Count>0)//当战斗队列大于零时
        {
          
            yield return new WaitForEndOfFrame();
        }
        
        Destroy(gameObject);//删除单位对象

    }    
    public IEnumerator fadeOut()//逐步修改单位颜色的方法
    {

        combatQueue.Enqueue(1);//战斗队列中加入数字1
        //setDieAnimation();
        //yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        Renderer rend = GetComponentInChildren<SpriteRenderer>();//获取单位的外观
        
        for (float f = 1f; f >= .05; f -= 0.01f)//逐步缩小f，修改单位透明度
        {
            Color c = rend.material.color;
            c.a = f;
            rend.material.color = c;
            yield return new WaitForEndOfFrame();
        }
        combatQueue.Dequeue();//移除战斗队列元素
       

    }
    public IEnumerator moveOverSeconds(GameObject objectToMove,Node endNode)//移动方法，传入需要开始移动的对象和结束节点
    {
        movementQueue.Enqueue(1);//将数字1添加到移动队列中

        
        
        path.RemoveAt(0);//删除路径上的第一个点，因为这是当前站立的位置
        while (path.Count != 0)//
        {
            
            Vector3 endPos = map.tileCoordToWorldCoord(path[0].x, path[0].y);//使用了地图脚本中的一个方法，将当前节点的xy坐标转化为世界坐标，并赋值给endPos变量
            objectToMove.transform.position = Vector3.Lerp(transform.position, endPos, visualMovementSpeed);//使用线性插值移动传入的对象至endPos，传入当前位置，目标位置和移动速度
            if ((transform.position - endPos).sqrMagnitude < 0.001)//如果当前目标和目标极度接近
            {

                path.RemoveAt(0);//删除路径上的第一个点

            }
            yield return new WaitForEndOfFrame();//暂时停止循环并等待下一帧启用
        }
        visualMovementSpeed = 0.15f;//定义移动速度
        transform.position = map.tileCoordToWorldCoord(endNode.x, endNode.y);//使用了地图脚本中的一个方法，将结束节点的xy坐标转化为世界坐标，并赋值给当前位置

        x = endNode.x;
        y = endNode.y;
        tileBeingOccupied.GetComponent<ClickableTileScript>().unitOnTile = null;//使原本站立的格子与单位解绑
        tileBeingOccupied = map.tilesOnMap[x, y];//绑定单位和单位正在站立的位置的格子
        movementQueue.Dequeue();//移除移动队列的第一个元素

    }



    public IEnumerator displayDamageEnum(int damageTaken)//传入伤害变量，展示伤害数字的方法
    {

        combatQueue.Enqueue(1);//战斗队列添加元素1
       
        damagePopupText.SetText(damageTaken.ToString());//飘出伤害数字
        damagePopupCanvas.enabled = true;//伤害可见
        for (float f = 1f; f >=-0.01f; f -= 0.01f)//逐步缩小f值
        {
            
            Color backDrop = damageBackdrop.GetComponent<Image>().color;//获取伤害数字背景图片颜色
            Color damageValue = damagePopupText.color;//获取伤害数字的颜色

            backDrop.a = f;
            damageValue.a = f;
            damageBackdrop.GetComponent<Image>().color = backDrop;
            damagePopupText.color = damageValue;//以上都是修改伤害数字的颜色变化
           yield return new WaitForEndOfFrame();
        }

        //damagePopup.enabled = false;
        combatQueue.Dequeue();//移除战斗队列元素
       
    }
    public void resetPath()//移动初始化
    {
        path = null;//路径初始化
        completedMovement = false;//完成移动初始化
    }
    public void displayDamage(int damageTaken)//飘出伤害数字
    {
        damagePopupCanvas.enabled = true;
        damagePopupText.SetText(damageTaken.ToString());
    }
    public void disableDisplayDamage()//伤害数字不可见
    {
        damagePopupCanvas.enabled = false;
    }

    public void setSelectedAnimation()//设置动画状态触发为选中
    {
        
        animator.SetTrigger("toSelected");
    }
    public void setIdleAnimation()//设置动画状态触发为待机
    {        
        animator.SetTrigger("toIdle");
    }
    public void setWalkingAnimation()//设置动画状态触发为移动
    {
        animator.SetTrigger("toWalking");
    }

    public void setAttackAnimation()//设置动画状态触发为攻击
    {
       animator.SetTrigger("toAttacking");
    }
    public void setWaitIdleAnimation()//设置动画状态触发为待机等待
    {
        
        animator.SetTrigger("toIdleWait");
    }
       
    public void setDieAnimation()//设置动画状态触发为死亡
    {
        animator.SetTrigger("dieTrigger");
    }
}
