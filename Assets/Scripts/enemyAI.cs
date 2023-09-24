using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    public bool action;//���Կ�ʼ�ж��ı�־

    public tileMapScript TMScript;//��ȡ��tilemap�Ľű�



    private UnitScript unitScript;//��ȡ��unit�Ľű�
    private HashSet<Node> finalMovementHighlight;//���嵥λ���ƶ��ķ�Χ�Ľڵ�

    public 
    // ��ʼ������
    void Start()
    {
        action = false;
        unitScript = GetComponent<UnitScript>();
        finalMovementHighlight = new HashSet<Node>();
    }

    // Update is called once per frame
    void Update()
    {
        if (action && unitScript.movementQueue.Count == 0 && unitScript.combatQueue.Count == 0)//�����ж����ҵ�λ�����ƶ�Ҳ���ڹ���,ʹ���ƶ�
        {
            TMScript.disableHighlightUnitRange();//ȡ����ͼ�����еĸ�����ʾ
            TMScript.selectedUnit = this.gameObject;//���õ�ͼ�ϵ�ѡ�е�λΪ�õ�λ
            unitScript.map = TMScript;//ѡ�е�λ�ĵ�ͼ����Ϊ��ǰ��ͼ
            unitScript.setMovementState(1);//ѡ�е�λ��״̬��Ϊ��ѡ��
            unitScript.setSelectedAnimation();//ѡ�е�λ�Ķ�����Ϊ��ѡ�е����
            TMScript.unitSelected = true;//�����Ѿ���ѡ�еĵ�λ��

            List<Node> temPath = null;
            List<Node> shortPath = null;
            

            //������Բ����ƶ�����ҡ���Ķ����ȵȣ��޸ĵ�λ��movespeed
            //�ҵ�ͼ�ϵ����е�λ�������ÿ����λ�����ڸ�����̾�����Ҫ���٣��ó���С���Ǹ���λ�ĸ��ӣ������ڿ��ƶ���Χ����ÿ����������С��λ�ĸ�������������ͣ��û�����ȡ��Сֵ���ƶ����������
            foreach (Transform team in TMScript.unitsOnBoard.transform)
            {
                foreach (Transform unitOnTeam in team)
                {
                    if (unitScript.teamNum != unitOnTeam.GetComponent<UnitScript>().teamNum)//�˴�ȡ�������е�ͼ�ϵķ��ѷ���λ
                    {
                        Debug.Log(unitOnTeam.GetComponent<UnitScript>().teamNum);
                        int unitX = unitOnTeam.GetComponent<UnitScript>().x;
                        int unitY = unitOnTeam.GetComponent<UnitScript>().y;
                        foreach (Node n in TMScript.graph[unitX, unitY].neighbours)//�ҵ����ѷ���λ�����ڴ�
                        {

                            TMScript.generatePathTo(n.x, n.y);//���㵱ǰ��λλ�õ����з��ѷ���λ���ڴ��ľ���
                            temPath = unitScript.path;
                            if(shortPath == null ||  shortPath.Count > temPath.Count)
                            {
                                shortPath = temPath;//�ҳ���̾���
                            }
                        }
                            
                    }
                }
                    
            }
            finalMovementHighlight = TMScript.getUnitMovementOptions();//��ȡ��λ���Ե����λ�ýڵ�
            shortPath.Reverse();//�������ƶ��õ��б���Ҫ��תһ��
            for(int i = 1; shortPath.Count > 0; i++)
            {
                if(finalMovementHighlight.Contains(shortPath[shortPath.Count - i]))
                {
                    break;
                }
            }

            shortPath.Reverse();//�����Ǽ���õ��б���Ҫ��תһ��

            unitScript.path = shortPath;//��Ҫ�ƶ���·��������λ
            //unitScript.setWalkingAnimation();//���õ�ǰ��λΪ�ƶ�����

            unitScript.MoveNextTile();//�ƶ�

            StartCoroutine(TMScript.moveUnitAndFinalize());//�ƶ����

            action = false;

        }

    }

   


   
}
