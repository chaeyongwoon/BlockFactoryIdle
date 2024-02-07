using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : Vehicle
{


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



            switch (_state)
            {
                case State.Wait:

                    break;

                case State.Move:
                    Vector3 lookrotation = _agent.steeringTarget - transform.position;

                    if (lookrotation != Vector3.zero)
                    {
                        transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, Quaternion.LookRotation(lookrotation), extraRotationSpeed * Time.deltaTime);
                    }


                    _currentDis = Vector3.Distance(transform.position, _target.transform.position);
                    if (_currentDis  /*_agent.remainingDistance*/ <= _minDistance)
                    {
                        if (_currentCount == 0) // Arrive BlockStorage
                        {
                            _target = Managers.Game.currentStageManager.FindBuilding();
                            PullBlock();

                            if (_currentCount > 0)
                            {
                                _agent.Warp(_blockStorage.transform.Find("Out_Pos").position);
                                SetDest(_target);
                            }
                            else
                            {

                                _target = Managers.Game.currentStageManager.ReFindBuilding();
                                if (_target == null)
                                {
                                    Debug.Log("Target Null");
                                    _state = State.Wait;

                                    Managers.Game.currentStageManager._vehicleQueue.Enqueue(this);
                                    break;
                                }
                                PullBlock();

                                if (_currentCount > 0)
                                {
                                    _agent.Warp(_blockStorage.transform.Find("Out_Pos").position);
                                    SetDest(_target);
                                }
                                else
                                {
                                    //Debug.Log("CurrentCount = 0");
                                }


                            }

                        }
                        else
                        {
                            SetDest(transform);
                            while (_currentCount > 0) // Arrive Building
                            {

                                PushBlock();

                                yield return new WaitForSeconds(0.2f);

                            }
                            _boxMeshFilter.gameObject.SetActive(false);
                            yield return new WaitForSeconds(1f);

                            _target = _blockStorage.transform.Find("In_Pos");
                            SetDest(_target);

                            _state = State.Return;


                        }
                    }
                    break;

                case State.PickDown:
                    break;

                case State.Return:




                    _currentDis = Vector3.Distance(transform.position, _target.transform.position);
                    if (_currentDis <= _minDistance)
                    {
                        Managers.Game.currentStageManager._vehicleQueue.Enqueue(this);
                        _state = State.Sleep;
                    }
                    break;

            }
        }
    }


}
