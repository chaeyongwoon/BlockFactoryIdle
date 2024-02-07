using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using MondayOFF;

public class Building : MonoBehaviour
{
    public int _buildingNum = 0;
    //public int _maxCount = 10;
    //public int _currentCount = 0;


    //public int[] _blockArray = new int[4];

    public Block.BlockType _blockType;
    public int _maxCount = 50;
    public int _currentCount = 0;

    public bool isBuildComplete = false;
    public double _rewardPrice = 100d;
    public Transform _buildingDeco;
    public Canvas _buildingCanvas;

    public GameObject _floating_Text_Pref;
    //public Transform _floating_Group;

    public Sprite[] _blockSprites;

    [SerializeField] StageManager stageManager;
    // =================


    private void Awake()
    {
        //if (_villageManager == null) _villageManager = Managers._stageManager._currentVillageManager;
        if (stageManager == null) stageManager = transform.GetComponentInParent<StageManager>();

        _buildingDeco = transform.Find("BuildingDeco");
        _buildingCanvas = transform.Find("Building_Canvas").GetComponent<Canvas>();

        //Debug.Log("canvas off");
        _buildingDeco.localScale = Vector3.zero;
        _buildingCanvas.gameObject.SetActive(false);

        _buildingCanvas.transform.Find("Build_Button").GetComponent<Button>().AddButtonEvent(() => Build_Button());

        _currentCount = 0; // _maxCount;
        _buildingCanvas.transform.Find("BlockCountImg").GetChild(0).GetComponent<Text>().text = $"{_currentCount}/{_maxCount}";

        _buildingCanvas.transform.Find("BlockCountImg").GetComponent<Image>().sprite = _blockSprites[(int)_blockType];



        LoadData();

        _buildingCanvas.transform.Find("BlockCountImg").GetChild(0).GetComponent<Text>().text
            = $"{_currentCount}/{_maxCount}";

        CheckBuild();

        if (isBuildComplete)
        {

            _buildingDeco.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
            _buildingCanvas.gameObject.SetActive(false);
            Transform _spark = Managers.Pool.Pop(Resources.Load<GameObject>("Building-Sparks"), transform).transform;
            _spark.localPosition = Vector3.zero;

        }


        if (_floating_Text_Pref == null) _floating_Text_Pref = Resources.Load<GameObject>("Floating_Text_Pref");

        _rewardPrice = _maxCount * 10d;  //_maxCount * 10d > 100 ? _maxCount * 5d : _maxCount * 10d;
    }

    public void LoadData()
    {
        _currentCount = ES3.Load<int>($"Stage_{stageManager._stageLevel}_buildingNum_{_buildingNum}_currentCount", 0);
        isBuildComplete = ES3.Load<bool>($"Stage_{stageManager._stageLevel}_buildingNum_{_buildingNum}_isBuildComplete", false);

    }

    public void SaveData()
    {
        ES3.Save<int>($"Stage_{stageManager._stageLevel}_buildingNum_{_buildingNum}_currentCount", _currentCount);
        ES3.Save<bool>($"Stage_{stageManager._stageLevel}_buildingNum_{_buildingNum}_isBuildComplete", isBuildComplete);
    }


    public void PushBlock(int _count = 1)
    {
        //Debug.Log(transform.name + " : " + _currentCount);
        _currentCount += _count;
        _buildingCanvas.transform.Find("BlockCountImg").GetChild(0).GetComponent<Text>().text
            = $"{_currentCount}/{_maxCount}";
        CheckBuild();

        SaveData();

    }

    public void SetCanvas()
    {


        if (_buildingDeco == null) _buildingDeco = transform.Find("BuildingDeco");
        if (_buildingCanvas == null) _buildingCanvas = transform.Find("Building_Canvas").GetComponent<Canvas>();


        _buildingDeco.localScale = Vector3.zero;
        _buildingCanvas.gameObject.SetActive(true);

        _buildingCanvas.transform.Find("Build_Button").Find("Reward_Text").GetComponent<Text>().text = $"{_rewardPrice}";
    }


    public void CheckBuild()
    {
        if (isBuildComplete == false)
        {
            if (_currentCount >= _maxCount)
            {
                isBuildComplete = true;
                //Debug.Log($"{_buildingNum}_Complete");

                _buildingCanvas.transform.Find("BlockCountImg").gameObject.SetActive(false);
                _buildingCanvas.transform.Find("Build_Button").gameObject.SetActive(true);

                // build button on
                EventTracker.LogEvent("Village", new Dictionary<string, object> { { "Village",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{stageManager._stageLevel}_BuildingCount-{_buildingNum}"}});

                stageManager.BuildComplete();

                if (TutorialManager._instance._tutorial_Level == 3 && _buildingNum==0)
                {
                    TutorialManager._instance.Tutorial_Img();



                }

            }
        }



    }

    public void Build_Button()
    {
        if (TutorialManager._instance._tutorial_Level == 3)
        {
            TutorialManager._instance.Tutorial_Complete();
            Managers.Game.currentStageManager._cams[0].transform.position = TutorialManager._instance._cams[3].transform.position;

            DOTween.Sequence().AppendInterval(2f).AppendCallback(() =>
            {
                if (TutorialManager._instance._tutorial_Level == 4 && _buildingNum==0)
                {

                    TutorialManager._instance.Tutorial_Img();
                }
            });

        }


        GameObject _makeingParticle = Managers.Pool.Pop(Resources.Load<GameObject>("Building-Making"), transform).gameObject;
        DOTween.Sequence()
            .AppendCallback(() =>
            {
                _makeingParticle.transform.DOScale(Vector3.one, 0.5f);
                _makeingParticle.transform.localPosition = Vector3.up * 2f;
                _buildingCanvas.gameObject.SetActive(false);
            })
            .AppendInterval(1.5f)
            .Append(_buildingDeco.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce))
            .Join(_makeingParticle.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.Linear))
            .AppendCallback(() =>
            {
                Managers.Pool.Push(_makeingParticle.GetComponent<Poolable>());

                Managers.Game.CalcMoney(_rewardPrice);

                Floating_Text(_rewardPrice);

                Transform _spark = Managers.Pool.Pop(Resources.Load<GameObject>("Building-Sparks"), transform).transform;
                _spark.localPosition = Vector3.zero;

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



        //_floatingTrans.localPosition = new Vector3(0f, 3f, 0f);
        _floatingTrans.position = transform.position + new Vector3(0f, 1.5f, 0);

        _floatingTrans.DOMoveZ(_floatingTrans.position.z + 2f, 1f).SetEase(Ease.Linear)
            .OnComplete(() => Managers.Pool.Push(_floatingTrans.GetComponent<Poolable>()));

    }




}
