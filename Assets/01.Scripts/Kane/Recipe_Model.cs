using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class Recipe_Model : MonoBehaviour
{

    //public string _recipeName;
    //public Hero.HeroType _heroType;
    //public Sprite _bluePrint_Sprite;

    //public Mesh[] _partsMeshes;
    //public Mesh[,] _2arrayMeshes;


    //public Mesh[] _selectMeshes;
    //public int _partsCount = 0;
    //public int _currentParts_Num = 0;
    //public List<Block.BlockType> _tempBlockList = new List<Block.BlockType>();



    //// =======================================
    //[FoldoutGroup("Status_Base")] public float _baseDamage = 10f;
    //[FoldoutGroup("Status_Base")] public float _baseAttackInterval = 1f;
    //[FoldoutGroup("Status_Base")] public float _baseSpeed = 5f;
    //[FoldoutGroup("Status_Base")] public float _baseHP = 10f;
    //[FoldoutGroup("Status_Base")] public float _baseDefense = 5f;

    //[FoldoutGroup("Status_addValue")] public float _damageValue = 5f;
    //[FoldoutGroup("Status_addValue")] public float _attackIntervalValue = 0.1f;
    //[FoldoutGroup("Status_addValue")] public float _speedValue = 2f;
    //[FoldoutGroup("Status_addValue")] public float _HPValue = 10f;
    //[FoldoutGroup("Status_addValue")] public float _defenseValue = 1f;

    //[FoldoutGroup("Status")] public float _damage = 10f;
    //[FoldoutGroup("Status")] public float _attackInterval = 1f;
    //[FoldoutGroup("Status")] public float _speed = 5f;
    //[FoldoutGroup("Status")] public float _maxHP = 10f;
    //[FoldoutGroup("Status")] public float _defense = 5f;


    //// ======================================================

    //private void Start()
    //{

    //    int _count = _partsMeshes.Length / 4;
    //    _2arrayMeshes = ConvertTo2DArray(_partsMeshes, _count, 4);
    //    _selectMeshes = new Mesh[_partsCount];


    //    _damage = _baseDamage;
    //    _attackInterval = _baseAttackInterval;
    //    _speed = _baseSpeed;
    //    _maxHP = _baseHP;
    //    _defense = _baseDefense;

    //}



    //public void SetColor(int _num)
    //{
    //    if (_currentParts_Num < _partsCount)
    //    {
    //        _selectMeshes[_currentParts_Num] = _2arrayMeshes[_currentParts_Num, _num];

    //        switch (_num)
    //        {
    //            case 0:
    //                _damage += _damageValue;
    //                break;
    //            case 1:
    //                _speed += _speedValue;
    //                break;

    //            case 2:
    //                _maxHP += _HPValue;
    //                break;

    //            case 3:
    //                _defense += _defenseValue;
    //                break;
    //        }


    //        _currentParts_Num++;
    //        Managers._stageManager._blockStorage._blockCountArray[_num]--;
    //        _tempBlockList.Add((Block.BlockType)_num);
    //        Managers._stageManager.FactoryCheckButtons();
    //        Managers._gameUi.Recipe_Block_Count_Text.text = $"X {_partsCount}";
    //        Managers._gameUi.SetColorImg(this);


    //        Managers._gameUi.Recipe_Status_Text.text = $"ATK : {_damage} Speed : {_speed} HP : {_maxHP} DEF : {_defense}";



    //    }

    //}



    //public void UndoColor()
    //{
    //    if (_currentParts_Num > 0)
    //    {
    //        _currentParts_Num--;

    //        Managers._gameUi.SetColorImg(this);
    //        int _tempblocknum = (int)_tempBlockList[_currentParts_Num];
    //        _tempBlockList.RemoveAt(_currentParts_Num);
    //        Managers._stageManager._blockStorage._blockCountArray[_tempblocknum]++;

    //        switch (_tempblocknum)
    //        {
    //            case 0:
    //                _damage -= _damageValue;
    //                break;
    //            case 1:
    //                _speed -= _speedValue;
    //                break;

    //            case 2:
    //                _maxHP -= _HPValue;
    //                break;

    //            case 3:
    //                _defense -= _defenseValue;
    //                break;
    //        }


    //        Managers._gameUi.Recipe_Status_Text.text = $"ATK : {_damage} Speed : {_speed} HP : {_maxHP} DEF : {_defense}";
    //        Managers._stageManager.FactoryCheckButtons();



    //    }
    //}

    //public void Reset()
    //{
    //    //for (int i = 0; i < _renderers.Length; i++)
    //    //{
    //    //    Color _color = Color.white;
    //    //    _color.a = 0.6f;
    //    //    _renderers[i].material.color = _color;
    //    //    _renderers[i].gameObject.SetActive(false);
    //    //    Vector3 _pos = _renderers[i].transform.position;
    //    //    _pos.y = i + 4f;
    //    //    _renderers[i].transform.position = _pos;

    //    //}
    //    _currentParts_Num = 0;


    //    for (int i = _tempBlockList.Count - 1; i >= 0; i--)
    //    {
    //        int _tempblocknum = (int)_tempBlockList[i];
    //        _tempBlockList.RemoveAt(i);
    //        Managers._stageManager._blockStorage._blockCountArray[_tempblocknum]++;
    //        //Managers._gameUi.SetColorImg(i, 0, false);
    //    }
    //    Managers._gameUi.SetColorImg(this);

    //    _damage = _baseDamage;
    //    _attackInterval = _baseAttackInterval;
    //    _speed = _baseSpeed;
    //    _maxHP = _baseHP;
    //    _defense = _baseDefense;

    //    Managers._gameUi.Recipe_Status_Text.text = $"ATK : {_damage} Speed : {_speed} HP : {_maxHP} DEF : {_defense}";
    //    Managers._stageManager.FactoryCheckButtons();

    //}




    //public Mesh[,] ConvertTo2DArray(Mesh[] oneDArray, int rows, int cols)
    //{
    //    Mesh[,] twoDArray = new Mesh[rows, cols];
    //    int index = 0;
    //    for (int i = 0; i < rows; i++)
    //    {
    //        for (int j = 0; j < cols; j++)
    //        {
    //            twoDArray[i, j] = oneDArray[index];
    //            index++;
    //        }
    //    }
    //    return twoDArray;
    //}

    ////public int[] ConvertTo1DArray(int[,] twoDArray)
    ////{
    ////    int rows = twoDArray.GetLength(0);
    ////    int cols = twoDArray.GetLength(1);
    ////    int[] oneDArray = new int[rows * cols];
    ////    int index = 0;
    ////    for (int i = 0; i < rows; i++)
    ////    {
    ////        for (int j = 0; j < cols; j++)
    ////        {
    ////            oneDArray[index] = twoDArray[i, j];
    ////            index++;
    ////        }
    ////    }
    ////    return oneDArray;
    ////}


}
