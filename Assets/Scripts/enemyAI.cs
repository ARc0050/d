using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyAI : MonoBehaviour
{
    public bool action;//���Կ�ʼ�ж��ı�־

    public tileMapScript TMScript;//��ȡ��tilemap�Ľű�,Ҳ��ȡ������Ϸ����


    public GameObject targetunit;//��ȡĿ�굥λ



    private UnitScript unitScript;//��ȡ������unit�Ľű�
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
        if (action && unitScript.unitMoveState == unitScript.getMovementStateEnum(0))//�����ж����ҵ�λΪδ��ѡ��״̬
        {
            TMScript.disableHighlightUnitRange();//ȡ����ͼ�����еĸ�����ʾ
            TMScript.selectedUnit = this.gameObject;//���õ�ͼ�ϵ�ѡ�е�λΪ�õ�λ
            unitScript.map = TMScript;//ѡ�е�λ�ĵ�ͼ����Ϊ��ǰ��ͼ
            unitScript.setMovementState(1);//ѡ�е�λ��״̬��Ϊ��ѡ��
            unitScript.setSelectedAnimation();//ѡ�е�λ�Ķ�����Ϊ��ѡ�е����
            TMScript.unitSelected = true;//�����Ѿ���ѡ�еĵ�λ��

            MoveAndFindEnemy();//�ƶ����ҵ�Ŀ��


            

        }
        else if(action && unitScript.unitMoveState == unitScript.getMovementStateEnum(2))//�����ж����Լ��ƶ�����ˣ�׼������
        {
            if (targetunit != null)//�ҵ���Ŀ�겢�ҿ��Դ�
            {
                //StartCoroutine(TMScript.BMS.attack(this.gameObject, targetunit));//����
                //StartCoroutine(TMScript.deselectAfterMovements(this.gameObject, targetunit));//�ڵ�ͼ�ű���ȡ��ѡ�е�λ��Э�����ӳ�����ս������

                Debug.Log("�˴�Ӧ�ù���" + targetunit);

                targetunit.GetComponent<UnitScript>().currentHealthPoints = targetunit.GetComponent<UnitScript>().currentHealthPoints - 1;

                unitScript.wait();//����λ����Ϊ�ȴ������ƶ�
                unitScript.setWaitIdleAnimation();//����λ����Ϊ�ȴ�����
                unitScript.setMovementState(3);//����λ״̬����Ϊ�ȴ�

                TMScript.deselectUnit();//�ڵ�ͼ�ű���ȡ��ѡ�е�λ

                action = false;
            }
            else
            {
                unitScript.wait();//����λ����Ϊ�ȴ������ƶ�
                unitScript.setWaitIdleAnimation();//����λ����Ϊ�ȴ�����
                unitScript.setMovementState(3);//����λ״̬����Ϊ�ȴ�

                TMScript.deselectUnit();//�ڵ�ͼ�ű���ȡ��ѡ�е�λ

                action = false;
            }
            
            
            
        }
        else if (!action && unitScript.unitMoveState == unitScript.getMovementStateEnum(3))//�����ж��ҵ�λ�Ѿ��ж������
        {

            TMScript.GMS.endTurn();//����Ϸ���Ĺ����н����غ�
            
        }



    }

   public void MoveAndFindEnemy()//�ƶ�V1 �ҵ������Լ�����ĵ��˲����������ڵĸ��ӣ����һ�ȡ��Ŀ��ĵ�λ�ű�
    {
        List<Node> temPath = null;
        List<Node> shortPath = null;


        //������Բ����ƶ�����ҡ���Ķ����ȵȣ��޸ĵ�λ��movespeed

        
        foreach (Transform team in TMScript.unitsOnBoard.transform)
        {
            foreach (Transform unitOnTeam in team)
            {
                if (unitScript.teamNum != unitOnTeam.GetComponent<UnitScript>().teamNum)//�˴�ȡ�������е�ͼ�ϵķ��ѷ���λ
                {
                    
                    int unitX = unitOnTeam.GetComponent<UnitScript>().x;
                    int unitY = unitOnTeam.GetComponent<UnitScript>().y;
                    foreach (Node n in TMScript.graph[unitX, unitY].neighbours)//�ҵ����ѷ���λ�����ڴ�
                    {
                        if(TMScript.unitCanEnterTile(n.x, n.y))
                        {
                            TMScript.generatePathTo(n.x, n.y);//���㵱ǰ��λλ�õ����з��ѷ���λ���ڴ��ľ���
                            temPath = unitScript.path;
                            if (shortPath == null || shortPath.Count > temPath.Count)
                            {

                                shortPath = temPath;//�ҳ���̾���
                                targetunit = unitOnTeam.gameObject;//��Ŀ��ȷ��

                            }
                        }
                        
                    }

                }
            }
            
        }
        temPath = shortPath;//����̾�����ʱ��ֵ����ʱ·��
        finalMovementHighlight = TMScript.getUnitMovementOptions();//��ȡ��λ���Ե����λ�ýڵ�
        if(shortPath != null)
        {
            for (int i = 1; shortPath.Count > 0; i++)
            {

                if (finalMovementHighlight.Contains(temPath[temPath.Count - 1]))//���Ŀ���ڿ��ƶ����ķ�Χ��
                {
                    break;
                }
                if (finalMovementHighlight.Contains(temPath[temPath.Count - i]))//�ҵ���Ŀ������Ŀ��ƶ���
                {
                    targetunit = null;//Ŀ���ÿգ��޷��ƶ����ɹ���Ŀ�괦
                    break;
                }
                targetunit = null;
                shortPath.RemoveAt(shortPath.Count - 1);
            }
        }
        

        unitScript.path = shortPath;//��Ҫ�ƶ���·��������λ
                                    //unitScript.setWalkingAnimation();//���õ�ǰ��λΪ�ƶ�����

        unitScript.MoveNextTile();//�ƶ�

        StartCoroutine(TMScript.moveUnitAndFinalize());//�ƶ����
    }


   public void AttackTarget(UnitScript target)
    {
        
    }
}
