using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camShakeScript : MonoBehaviour
{

    public IEnumerator camShake(float duration,float camShakeStrength,Vector3 direction)
    {
        float updatedShakeStrength = camShakeStrength;         //相机抖动力度
        if (camShakeStrength > 10)
        {
            camShakeStrength = 10;                             //力度最高为10
        }
        Vector3 originalPos = transform.position;                                             
        Vector3 endPoint = new Vector3(direction.x, 0, direction.z)*(camShakeStrength/2); 
        
        float timePassed = 0f; //已经过的相机的抖动时间
        while (timePassed < duration) //当相机的抖动时间还没结束时
        {

            float xPos = Random.Range(-.1f, .1f)*camShakeStrength; //随机选定x和z轴的一个方向
            float zPos = Random.Range(-.1f, .1f)*camShakeStrength;
            Vector3 newPos = new Vector3(transform.position.x + xPos, transform.position.y, transform.position.z + zPos); //根据随机选定的x和z轴的位置确定抖动的坐标
            //Vector3 newPos = endPoint + originalPos;
            transform.position = Vector3.Lerp(transform.position,newPos,0.15f);//这是什么，抖一下
            timePassed += Time.deltaTime; //经历的时间增加
            yield return new WaitForEndOfFrame();

        }

        
           
            
    }
}
