using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using UnityEngine.UI;


using Sirenix.OdinInspector;

public class Vehicle : MonoBehaviour
{



    public Vector3 _scale = new Vector3(0.9f, 1.2f, 0.9f);
    public float _scaleTime = 0.25f;

    public GameObject _blockPref;

    public enum State
    {
        Wait,
        Move,
        PickUp,
        PickDown,
        Return,
        Return2,
        Sleep


    }
    public State _state;
    public Block.BlockType _blockType;

    public int _speed_Level = 0;
    public int _capacity_Level = 0;

    public int _maxCount = 10;
    public int _currentCount;

    public float _minDistance = 2f;
    public float _currentDis;
    //


    public GameObject _floating_Text_Pref;

    protected NavMeshAgent _agent;
    public Transform _target;

    public Mesh[] _meshes;

    public float extraRotationSpeed = 5f;

    protected BlockStorage _blockStorage;

    protected MeshFilter _boxMeshFilter;

    public double priceValue = 2d;
    // =====================================


    protected virtual void OnEnable()
    {

        if (_blockStorage == null) _blockStorage = Managers.Game.currentStageManager._blockStorage;


        if (_agent == null) _agent = transform.GetComponent<NavMeshAgent>();
        _boxMeshFilter = transform.GetChild(0).GetComponent<MeshFilter>();


        if (_floating_Text_Pref == null) _floating_Text_Pref = Resources.Load<GameObject>("Floating_Text_Pref");


        DOTween.Sequence().Append(transform.DOScale(_scale, _scaleTime)).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);

    }



    public void SetDest(Transform _trans, State _selectState = State.Move)
    {
        _agent.SetDestination(_trans.position);

        _state = _selectState;
    }

    public void PullBlock()
    {
        //Debug.Log("Pull Block");
        Building _building = _target.GetComponent<Building>();
        int _blockTypeNum = -1;

        _blockTypeNum = (int)_building._blockType;

        _blockType = (Block.BlockType)_blockTypeNum;
        _boxMeshFilter.sharedMesh = _meshes[(int)_blockType];


        if (_building._currentCount < _building._maxCount)
        {

            if (_blockStorage._blockCountArray[_blockTypeNum] >= _maxCount)
            {
                _blockStorage._blockCountArray[_blockTypeNum] -= _maxCount;
                _currentCount = _maxCount;
            }
            else
            {
                int _tempCount = _blockStorage._blockCountArray[_blockTypeNum];
                _blockStorage._blockCountArray[_blockTypeNum] -= _tempCount;
                _currentCount = _tempCount;

            }
        }
        else
        {
            _currentCount = 1;
            if (_blockStorage._blockCountArray[_blockTypeNum] >= 1)
            {
                _blockStorage._blockCountArray[_blockTypeNum] -= 1;
            }
        }
        _blockStorage.UpdateBlockCount();

        if (_currentCount != 0)
            _boxMeshFilter.gameObject.SetActive(true);
        else
            _boxMeshFilter.gameObject.SetActive(false);





    }

    public void PushBlock()
    {
        Transform _block = Managers.Pool.Pop(_blockPref, _target.transform).transform;

        _block.transform.position = transform.position;
        _block.GetComponent<MeshFilter>().sharedMesh = _boxMeshFilter.sharedMesh;

        _currentCount--;

        _block.DOJump(_target.transform.position, 5f, 1, 0.3f).SetEase(Ease.Linear)
            .Join(_block.DOScale(Vector3.one * 0.5f, 0.5f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                Managers.Pool.Push(_block.GetComponent<Poolable>());
                priceValue = 2d * (Managers.Game.currentStageLevel + 1);
                Floating_Text(priceValue);
                Managers.Game.CalcMoney(priceValue);
                //Debug.Log("Push : " + _target.name);
                _target.GetComponent<Building>().PushBlock();
            });
        //_target.GetComponent<Building>().CheckBuild();
    }

    public void PushBlock2(int _count)
    {
        Transform _block = Managers.Pool.Pop(_blockPref, _target.transform).transform;

        _block.transform.position = transform.position;
        _block.GetComponent<MeshFilter>().sharedMesh = _boxMeshFilter.sharedMesh;

        _currentCount = 0;

        _block.DOJump(_target.transform.position, 5f, 1, 0.3f).SetEase(Ease.Linear)
            .Join(_block.DOScale(Vector3.one * 0.5f, 0.5f).SetEase(Ease.Linear))
            .OnComplete(() =>
            {
                Managers.Pool.Push(_block.GetComponent<Poolable>());
                priceValue = 2d * (Managers.Game.currentStageLevel + 1);
                Floating_Text(priceValue * _count);
                Managers.Game.CalcMoney(priceValue * _count);

                _target.GetComponent<Building>().PushBlock(_count);
            });
    }





    public void Floating_Text(double _num)
    {
        Transform _floatingTrans = Managers.Pool.Pop(_floating_Text_Pref, Managers.Game.currentStageManager.transform.Find("4.Floating_Group")).transform;
        _floatingTrans.localScale = Vector3.one * 0.015f;
        _floatingTrans.SetAsLastSibling();
        _floatingTrans.GetChild(0).GetComponent<Text>().text =
            $"${_num * (1d + Managers.Game.orderLevel * 0.1d)}";

        _floatingTrans.rotation = Quaternion.Euler(new Vector3(80f, 0f, 0f));

        _floatingTrans.position = transform.position + Vector3.up * 2f;

        _floatingTrans.DOMoveZ(transform.position.z + 2f, 1f).SetEase(Ease.OutCirc)
            .OnComplete(() => Managers.Pool.Push(_floatingTrans.GetComponent<Poolable>()));

    }



    public void SetLevel(int _SpdLevel, int _CapLevel)
    {
        if (_agent == null) _agent = transform.GetComponent<NavMeshAgent>();

        _speed_Level = _SpdLevel;
        _capacity_Level = _CapLevel;


        _agent.speed = 2f + 0.2f * _speed_Level;
        _maxCount = 2 + 1 * _capacity_Level;

        if (Managers.Game.currentStageManager.isRvVehicleSpeedUp)
        {
            _agent.speed *= 2f;
        }


    }

    public void SetReturn()
    {
        if (_blockStorage == null) _blockStorage = Managers.Game.currentStageManager._blockStorage;

        _blockStorage._blockCountArray[(int)_blockType] += _currentCount;
        _currentCount = 0;

        _state = State.Wait;


    }






}
