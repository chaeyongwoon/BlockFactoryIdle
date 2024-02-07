using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Stage_Data", menuName = "Scriptable Object/Stage Data", order = int.MaxValue)]

public class StageData : ScriptableObject
{

    //public int _stageLevel;

    public int _maxEnemyCount = 1;
    public float _rewardCoin = 100f; // example

    public GameObject _bossPref;

    public float _rewards = 100f;



}
