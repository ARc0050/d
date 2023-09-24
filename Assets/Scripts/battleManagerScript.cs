using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class battleManagerScript : MonoBehaviour
{
    //该脚本用于本游戏使用的战斗系统
    //T以下变量为公共变量，便于在unity的inspector中设置
    //如果能将它们拉到inspector中，就可以将它们设置为private
    public camShakeScript CSS;
    public gameManagerScript GMS;

   //这个battleStatus用于检测战斗是否结束
    private bool battleStatus;

    //输入 In：两个 "单位 "游戏对象中，initiator是发起攻击的单位，recipient是受到攻击的单位
    //输出 Out：不输出（void） 如果被攻击的一方生命值减少，单位会受到伤害或被摧毁。 
    //说明 Desc： 通常由另一个脚本调用，该脚本可以访问两个单位，然后将单位设置为函数的参数

    public void battle(GameObject initiator, GameObject recipient)
    {
        battleStatus = true; //战斗开始
        var initiatorUnit = initiator.GetComponent<UnitScript>(); //获得攻击单位的属性
        var recipientUnit = recipient.GetComponent<UnitScript>(); //获得受击单位的属性
        int initiatorAtt = initiatorUnit.attackDamage;            //获得攻击者的攻击
        int recipientAtt = recipientUnit.attackDamage;            //获得受击者的攻击

        //如果两个单位的攻击范围相同，那么它们可以互相攻击
        if (initiatorUnit.attackRange == recipientUnit.attackRange)
        {
            //克隆出unitScript里的一个粒子效果，在受击者的位置和角度方向上出现
            GameObject tempParticle = Instantiate( recipientUnit.GetComponent<UnitScript>().damagedParticle,recipient.transform.position, recipient.transform.rotation); 
            Destroy(tempParticle, 2f); //2f后粒子效果消失


            recipientUnit.dealDamage(initiatorAtt); //攻击者对受击者发动攻击
            if (checkIfDead(recipient)) //检查受击者是否死亡
            {
                //先设置为null，然后移除，如果游戏对象在移除前被销毁，将无法被正确地检测到
                //这将导致游戏无法真正结束，因为在从父对象中移除对象之前，需要检查是否仍有单位，因此我们需要在销毁 gameObject 之前将父对象置为 null。
                recipient.transform.parent = null;
                recipientUnit.unitDie(); //该对象死亡，运行UnitScript里的死亡函数
                battleStatus = false;    //战斗结束
                GMS.checkIfUnitsRemain(initiator, recipient);//检查场上是否还有敌方单位
                return;
            }

           
            initiatorUnit.dealDamage(recipientAtt); //受击者对攻击者发动攻击
            if (checkIfDead(initiator))             //以下代码作用同上
            {
                initiator.transform.parent = null;
                initiatorUnit.unitDie();
                battleStatus = false;
                GMS.checkIfUnitsRemain(initiator, recipient);
                return;

            }
        }
        //如果单位的攻击范围不同，如剑士对弓箭手，则受攻击者不能反击
        else
        {
            GameObject tempParticle = Instantiate(recipientUnit.GetComponent<UnitScript>().damagedParticle, recipient.transform.position, recipient.transform.rotation);
            Destroy(tempParticle, 2f);               //播放粒子效果，同上
           
            recipientUnit.dealDamage(initiatorAtt);  //攻击者单方面造成伤害
            if (checkIfDead(recipient))
            {
                recipient.transform.parent = null;
                recipientUnit.unitDie();
                battleStatus = false;
                GMS.checkIfUnitsRemain(initiator, recipient);

                return;
            }
        }

        battleStatus = false;                       //战斗结束

    }

    //输入In：需要检测是否死亡的游戏对象
    //输出Out：一个bool，true说明挂了，false说明没挂
    //说明：检查游戏对象的生命值（被检查对象必须为 "Unit"），否则会脚本就崩溃了
    public bool checkIfDead(GameObject unitToCheck)
    {
        if (unitToCheck.GetComponent<UnitScript>().currentHealthPoints <= 0)
        {
            return true;
        }
        return false;
    }

    //输入：游戏对象
    //不输出
    //说明：参数中的游戏对象被销毁
    //当游戏对象挂了就摧毁这一单位
    public void destroyObject(GameObject unitToDestroy)
    {
        Destroy(unitToDestroy);
    }

    //输入：两个游戏单位，攻击者和受击者
    //输出：攻击并播放战斗动画
    //说明：该函数调用战斗有关的所有函数 
    public IEnumerator attack(GameObject unit, GameObject enemy)
    {
        battleStatus = true;                                      //战斗开始
        float elapsedTime = 0;                                    //定义并重置动画持续时间
        Vector3 startingPos = unit.transform.position;            //攻击动画播放时角色的方向
        Vector3 endingPos = enemy.transform.position;             //动画结束时角色的方向
        unit.GetComponent<UnitScript>().setWalkingAnimation();    //获得角色的移动动画
        while (elapsedTime < .25f)                                //当动画持续时间小于0.25f
        {
            //设置动画的播放时间，播放动画，用Lerp来改变角色的方位，具体表现在攻击时角色往前撞的那一下
            unit.transform.position = Vector3.Lerp(startingPos, startingPos+((((endingPos - startingPos) / (endingPos - startingPos).magnitude)).normalized*.5f), (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            
            yield return new WaitForEndOfFrame();
        }
        
        
        //使攻击命中后飘出伤害数字
        while (battleStatus)      //当处于战斗状态时
        {

            StartCoroutine(CSS.camShake(.2f, unit.GetComponent<UnitScript>().attackDamage, getDirection(unit, enemy))); //用协程来启动相机震动

            //当双方都在互相的攻击范围内，且双方的攻击都没击杀对面的时候，就是双方互相攻击时
            if (unit.GetComponent<UnitScript>().attackRange == enemy.GetComponent<UnitScript>().attackRange && enemy.GetComponent<UnitScript>().currentHealthPoints - unit.GetComponent<UnitScript>().attackDamage > 0)
            {
                StartCoroutine(unit.GetComponent<UnitScript>().displayDamageEnum(enemy.GetComponent<UnitScript>().attackDamage)); //飘出敌方受到的伤害
                StartCoroutine(enemy.GetComponent<UnitScript>().displayDamageEnum(unit.GetComponent<UnitScript>().attackDamage)); //飘出我方受到的伤害
            }
           
            else
            {
                StartCoroutine(enemy.GetComponent<UnitScript>().displayDamageEnum(unit.GetComponent<UnitScript>().attackDamage)); //飘出攻击造成的伤害
            }
            
            //unit.GetComponent<UnitScript>().displayDamage(enemy.GetComponent<UnitScript>().attackDamage);
            //enemy.GetComponent<UnitScript>().displayDamage(unit.GetComponent<UnitScript>().attackDamage);
            
            battle(unit, enemy);  //执行战斗函数
            
            yield return new WaitForEndOfFrame();
        }
        
        if (unit != null)
        {
           StartCoroutine(returnAfterAttack(unit, startingPos));  //通过协程触发下面的函数
          
        }
       
        

       
        //unit.GetComponent<UnitScript>().wait();

    }

    //输入：攻击完即将返回的角色，返回的位置
    //输出：角色将返回到攻击前的位置
    //Desc: the gameObject in the parameter is returned to the endPoint
    public IEnumerator returnAfterAttack(GameObject unit, Vector3 endPoint) {
        float elapsedTime = 0;
        

        while (elapsedTime < .30f)
        {
            unit.transform.position = Vector3.Lerp(unit.transform.position, endPoint, (elapsedTime / .25f));
            elapsedTime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        unit.GetComponent<UnitScript>().setWaitIdleAnimation();  //返回原点后进入待机动画
        unit.GetComponent<UnitScript>().wait();                  //进入等待状态
       
        
    }

    //输入：两个游戏单位
    //输出：单位应当朝向的方向
    //Desc: the vector3 which the unit needs to moveTowards is returned by this function
    public Vector3 getDirection(GameObject unit, GameObject enemy)
    {
        Vector3 startingPos = unit.transform.position;
        Vector3 endingPos = enemy.transform.position;
        return (((endingPos - startingPos) / (endingPos - startingPos).magnitude)).normalized;
    }
    




}
