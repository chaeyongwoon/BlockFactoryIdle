using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Train : Vehicle
{
    public bool isHead = false;

    public Transform _headTarget;


    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Cor_Update());
    }

    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return null;
            Vector3 lookrotation = _agent.steeringTarget - transform.position;

            if (lookrotation != Vector3.zero)
            {
                transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.LookRotation(lookrotation), extraRotationSpeed * Time.deltaTime * 10f);

                //transform.rotation = Quaternion.se
            }


            switch (_state)
            {
                case State.Wait:

                    break;

                case State.Move:



                    _currentDis = Vector3.Distance(transform.position, _target.transform.position);
                    if (_currentDis <= _minDistance)
                    {
                        if (isHead)
                        {
                            _target = Managers.Game.currentStageManager.FindBuilding();
                            _headTarget = _target;
                        }
                        else
                        {
                            _target = Managers.Game.currentStageManager._vehicleHead._headTarget;
                        }

                        if (isHead == false)
                        {

                            PullBlock();
                        }
                        _agent.Warp(_blockStorage.transform.Find("Out_Pos").position);
                        SetDest(_target, State.PickDown);

                    }
                    break;

                //case State.PickUp:
                //    _currentDis = Vector3.Distance(transform.position, _target.transform.position);
                //    if (_currentDis <= _minDistance)
                //    {
                //        setde
                //    }
                //    break;

                case State.PickDown:
                    _currentDis = Vector3.Distance(transform.position, _target.transform.position);
                    if (_currentDis <= _minDistance)
                    {
                        if (isHead == false && _currentCount > 0) PushBlock2(_currentCount);
                        _boxMeshFilter.gameObject.SetActive(false);
                        _agent.SetDestination(_blockStorage.transform.Find("In_Pos").transform.position);
                        yield return new WaitForSeconds(1f);
                        _target = _blockStorage.transform.Find("In_Pos");
                        SetDest(_target, State.Return);
                        //_state = State.Return;
                    }
                    break;

                case State.Return:

                    _currentDis = Vector3.Distance(transform.position, _target.transform.position);
                    if (_currentDis <= 1f /*_minDistance*/)
                    {
                        _target = _blockStorage.transform.Find("Out_Pos");
                        _agent.Warp(_blockStorage.transform.Find("In_Pos2").position);
                        SetDest(_blockStorage.transform.Find("Out_Pos"), State.Return2);

                    }
                    break;

                case State.Return2:
                    _currentDis = Vector3.Distance(transform.position, _target.transform.position);
                    if (_currentDis <= 1f /*_minDistance*/)
                    {

                        Managers.Game.currentStageManager.VehicleEnqueue(this);

                        SetDest(_blockStorage.transform.Find("Out_Pos"), State.Sleep);
                        _agent.Warp(_blockStorage.transform.Find("Out_Pos").position);

                    }
                    break;

            }
        }
    }

    [Button]
    public void DebugPosition()
    {
        Debug.Log($"TargetPos : {_target.transform.position} / DestPos : {_agent.destination}");
    }


}
