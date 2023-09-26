using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NewDice.Inventory;

public class DiceEffectManager : MonoBehaviour
{
    public Effect effectPrefab;

    private Transform effectParent;

    //private void OnEnable()
    //{
    //    EventHandler.InstantiateEffectInScene += OnInstantiateEffectInScene;
    //}

    //private void OnDisable()
    //{
    //    EventHandler.InstantiateEffectInScene -= OnInstantiateEffectInScene;
    //}

    private void Start()
    {
        effectParent = GameObject.FindWithTag("EffectParent").transform;
    }

    private void OnInstantiateEffectInScene(int ID,Vector3 pos)
    {
        var effect = Instantiate(effectPrefab, pos, Quaternion.identity, effectParent);
        effect.effectID = ID;
    }




}
