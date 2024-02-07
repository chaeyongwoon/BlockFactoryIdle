using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;

public class Rail : MonoBehaviour
{
    public Transform[] _prevNodes;


    public Transform _currentBlock;

    public StageManager _stageManager;
    public float _waitTime = 0f;

    public bool _isReady = true;


    // ==============================
    private void OnEnable()
    {
        _stageManager = GetComponentInParent<StageManager>();

        //_nextRail = _nextNode.GetComponent<Rail>();

        StartCoroutine(Cor_Update());
    }


    IEnumerator Cor_Update()
    {
        WaitForSeconds _wait = new WaitForSeconds(0.5f);
        while (true)
        {
            switch (_stageManager.isRvRailSpeedUp /*isUnLimit*/)
            {
                case false:
                    //yield return new WaitForSeconds(_stageManager._railSpeed * 0.5f);
                    yield return null;

                    break;

                case true:

                    yield return new WaitForSeconds(_stageManager._railSpeed * 0.5f);
                    break;
            }
            _waitTime += Time.deltaTime;

            if (_currentBlock == null)
            {
                if (_prevNodes.Length < 2)
                {

                    if (_prevNodes[0].GetComponent<Rail>()) // rail
                    {
                        if (_currentBlock == null && _prevNodes[0].GetComponent<Rail>()._currentBlock != null
                            && _prevNodes[0].GetComponent<Rail>()._isReady)
                        {
                            PullBlock(_prevNodes[0].GetComponent<Rail>()._currentBlock);
                            _prevNodes[0].GetComponent<Rail>()._currentBlock = null;
                            _prevNodes[0].GetComponent<Rail>()._waitTime = 0f;

                            //_isReady = true;
                        }
                    }
                    else     // blockMachine
                    {
                        if (_currentBlock == null && _prevNodes[0].GetComponent<BlockMachine>()._currentBlock != null)
                        {
                            PullBlock(_prevNodes[0].GetComponent<BlockMachine>()._currentBlock);
                            _prevNodes[0].GetComponent<BlockMachine>()._currentBlock = null;
                        }
                    }
                }
                else // over 2
                {
                    int _num = -1;
                    float _time = 0f; // _prevNodes[0].GetComponent<Rail>()._waitTime;

                    if (_currentBlock == null)
                    {
                        for (int i = 0; i < _prevNodes.Length; i++)
                        {
                            if (_prevNodes[i].GetComponent<Rail>()._currentBlock != null
                                && _prevNodes[i].GetComponent<Rail>()._isReady && _time < _prevNodes[i].GetComponent<Rail>()._waitTime)
                            {
                                _time = _prevNodes[i].GetComponent<Rail>()._waitTime;
                                _num = i;
                            }
                        }
                    }
                    if (_num > -1)
                    {
                        PullBlock(_prevNodes[_num].GetComponent<Rail>()._currentBlock);
                        _prevNodes[_num].GetComponent<Rail>()._currentBlock = null;
                        _prevNodes[_num].GetComponent<Rail>()._waitTime = 0f;
                    }



                }
            }

        }
    }


    public void PullBlock(Transform _Block)
    {

        _currentBlock = _Block;
        if (_stageManager.isRvRailSpeedUp/*isUnLimit*/ == false)
        {
            _isReady = false;

        }


        _currentBlock.DOMove(transform.position + Vector3.up * 0.5f, _stageManager._railSpeed)
            .OnComplete(() => { _isReady = true; });

    }

}
