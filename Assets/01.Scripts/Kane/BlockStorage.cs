using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using MondayOFF;


public class BlockStorage : MonoBehaviour
{

    public Vector3 _scale_1 = new Vector3(1f, 1.1f, 1f);
    public Vector3 _scale_2 = new Vector3(1.05f, 0.9f, 1.05f);
    public float _scaleTime = 0.25f;


    public GameObject _floating_Pref;

    public Transform _floating_Group;

    // ====================================

    public int[] _blockCountArray
        = new int[System.Enum.GetValues(typeof(Block.BlockType)).Length];


    public Rail _prevNode;

    StageManager stageManager;
    // ===================================

    private void OnEnable()
    {
        //if (_floating_Pref == null) _floating_Pref = Resources.Load<GameObject>("Floating_Pref");
        if (_floating_Group == null)
        {
            _floating_Group = Instantiate(new GameObject("Floating_Group")).transform;
            _floating_Group.transform.SetParent(transform);
            _floating_Group.SetAsLastSibling();
        }

        if (stageManager == null) stageManager = transform.GetComponentInParent<StageManager>();


        StartCoroutine(Cor_Update());
        LoadData();

        UpdateBlockCount();
    }


    IEnumerator Cor_Update()
    {
        while (true)
        {
            yield return null;

            if (_prevNode._isReady && _prevNode.GetComponent<Rail>()._currentBlock != null)
            {
                PullBlock(_prevNode.GetComponent<Rail>()._currentBlock);
                _prevNode.GetComponent<Rail>()._currentBlock = null;
            }

        }
    }

    public void PullBlock(Transform _block)
    {
        DOTween.Sequence()
                  .Append(transform.DOScale(_scale_1, _scaleTime).SetEase(Ease.Linear))
                  .Append(transform.DOScale(_scale_2, _scaleTime).SetEase(Ease.Linear));

        DOTween.Sequence()
             .Append(_block.transform.DOMove(transform.position + Vector3.up, Managers.Game.currentStageManager._railSpeed))
             .OnComplete(() =>
             {
                 int _typeNum = (int)_block.GetComponent<Block>()._blockType;
                 _blockCountArray[_typeNum]++;
                 Managers.Pool.Push(_block.GetComponent<Poolable>());

                 //Managers._gameUi.MakeButtonOnOff();
                 Floating_Text(_typeNum);

                 UpdateBlockCount();

                 if (TutorialManager._instance._tutorial_Level == 1 && Managers.Game.currentStageManager._vehicle_Spawn_Level == 0)
                 {
                     TutorialManager._instance.Tutorial_Img();
                     _blockCountArray[0] += 3;
                 }



             });

    }

    public void UpdateBlockCount()
    {
        if (transform.gameObject.activeSelf)
        {

            for (int i = 0; i < 4; i++)
            {
                Managers._gameUi._blockCountTexts[i].text = _blockCountArray[i].ToString();
            }

            SaveData();
        }
    }


    public void Floating_Text(int _num)
    {
        Transform _floatingTrans = Managers.Pool.Pop(_floating_Pref, _floating_Group).GetComponent<Transform>();
        _floatingTrans.localScale = Vector3.one * 0.01f;
        _floatingTrans.SetAsLastSibling();

        //Debug.Log("Spawn Floating");
        //UnityEditor.EditorApplication.isPaused = true;
        for (int i = 0; i < 4; i++)
        {
            _floatingTrans.GetChild(i).gameObject.SetActive(false);
        }
        _floatingTrans.GetChild(_num).gameObject.SetActive(true);


        _floatingTrans.localPosition = new Vector3(0f, 4f, 0f);

        _floatingTrans.DOLocalMoveZ(1f, 1f).SetEase(Ease.OutCirc)
            .OnComplete(() => Managers.Pool.Push(_floatingTrans.GetComponent<Poolable>()));

    }


    public void SaveData()
    {
        ES3.Save<int[]>($"Stage_{stageManager._stageLevel}_blockCount", _blockCountArray);
    }

    public void LoadData()
    {
        _blockCountArray = ES3.Load<int[]>($"Stage_{stageManager._stageLevel}_blockCount", new int[4]);
    }



}
