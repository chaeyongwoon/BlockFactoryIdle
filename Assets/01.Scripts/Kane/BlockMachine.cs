using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using MondayOFF;



public class BlockMachine : MonoBehaviour
{
    [FoldoutGroup("Upgrade")] public int _machineNum = 0;
    [FoldoutGroup("Upgrade")] public int _level;
    [FoldoutGroup("Upgrade")] public double[] _upgradePrices = new double[5];

    [FoldoutGroup("BlockMachine")] public Block.BlockType _spawnBlockType;
    [FoldoutGroup("BlockMachine")] public GameObject _blockPref;

    [FoldoutGroup("BlockMachine")] public float _spawnInterval = 1f;

    [FoldoutGroup("BlockMachine")] public Transform _currentBlock;
    [FoldoutGroup("BlockMachine")] public MeshFilter _connectMeshfilter;
    [FoldoutGroup("BlockMachine")] public Mesh _connectMesh;
    //MeshRenderer _cubeObj;


    [FoldoutGroup("Setting")] public Mesh[] _topMeshes;
    [FoldoutGroup("Setting")] public Mesh[] _bodyMeshes;

    Transform _factoryTop_Obj;
    MeshFilter _topMeshfilter;
    MeshFilter _bodyMeshfilter;

    public Vector3 _scale_1 = new Vector3(0.9f, 1.1f, 0.9f);

    public float _scaleTime = 0.25f;

    //public GameObject _machineCanvas;
    //Button _machineBuyButton;
    //Text _machinePriceText;


    // =======private ============
    [SerializeField] StageManager stageManager;

    // =======================================
    private void OnEnable()
    {
        if (_blockPref == null) Resources.Load<GameObject>("Block_Pref");
        if (stageManager == null) stageManager = GetComponentInParent<StageManager>();

        _factoryTop_Obj = transform.Find("Top_Obj");
        _topMeshfilter = _factoryTop_Obj.GetComponent<MeshFilter>();
        _bodyMeshfilter = transform.GetComponent<MeshFilter>();


        if (_connectMeshfilter != null)
            _connectMeshfilter.sharedMesh = _connectMesh;

        SetBlockType((int)_spawnBlockType, false);


        //_machineBuyButton.AddButtonEvent(() => AddBlockMachine());



        LoadData();

        StartCoroutine(Cor_Update());



        if (transform.Find("Rail_Group")) transform.Find("Rail_Group").SetParent(stageManager.transform.Find("3.RailGroup"));

    }




    public void LoadData()
    {
        _level = ES3.Load<int>($"BlockMachine_{stageManager._stageLevel}_{_machineNum}", 0);
        _spawnInterval = 6f - 1f * _level;
    }

    public void SetBlockType(int _num, bool isLog = true)
    {
        _spawnBlockType = (Block.BlockType)_num;


        _topMeshfilter.sharedMesh = _topMeshes[_num];
        _bodyMeshfilter.sharedMesh = _bodyMeshes[_num];

        if (isLog)
        {

            EventTracker.LogEvent("BlockMachine", new Dictionary<string, object> { { "BlockMachine",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{stageManager._stageLevel}_ChangeColor-{_spawnBlockType.ToString()}"} });

        }

        // add material change

    }

    public void UpgradeMachine()
    {


        Managers.Game.CalcMoney(-_upgradePrices[_level]);
        _level++;
        ES3.Save<int>($"BlockMachine_{stageManager._stageLevel}_{_machineNum}", _level);

        EventTracker.LogEvent("BlockMachine", new Dictionary<string, object> { { "BlockMachine",
                $" { ((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum{stageManager._stageLevel}_MachineNum-{_machineNum}_Upgrade-{_level}" } });

        _spawnInterval = 6f - 1f * _level;
        CheckPrice();
    }



    IEnumerator Cor_Update()
    {
        yield return new WaitForSeconds(1f);

        while (true)
        {

            Spawnblock();
            //if (stageManager.isFree)
            //{
            //    yield return new WaitForSeconds(0.5f);
            //}
            //else
            //if (stageManager.isRvDoubleSpawn)
            //{
            //    yield return new WaitForSeconds(_spawnInterval * 0.5f);
            //}
            //else
            //{
            //    yield return new WaitForSeconds(_spawnInterval);
            //}

            yield return new WaitForSeconds(stageManager.isRvDoubleSpawn ? /*_spawnInterval **/ 0.5f : _spawnInterval);
        }
    }



    [Button]
    public void Spawnblock()
    {
        if (_currentBlock == null)
        {
            Block _block = Managers.Pool.Pop(_blockPref).GetComponent<Block>();
            _block.SetInit(_spawnBlockType);
            _block.transform.SetParent(stageManager.transform.Find("2.BlockPool"));

            _block.transform.position = transform.position + Vector3.up * 0.5f;
            transform.DOScale(Vector3.one, 0f);
            DOTween.Sequence().Append(_factoryTop_Obj.DOLocalMoveY(1f, 0.25f)).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);
            DOTween.Sequence().Append(transform.DOScale(_scale_1, _scaleTime)).SetLoops(2, LoopType.Yoyo).SetEase(Ease.Linear);

            _currentBlock = _block.transform;


        }

    }


    public void CheckPrice()
    {


        if (_level < _upgradePrices.Length)
        {
            Managers._gameUi.BlockMachine_Upgrade_Price_Text.text = $"{_upgradePrices[_level]}";
            Managers._gameUi.BlockMachine_Status_Text.text = $"{_spawnInterval}s";

            Managers._gameUi.BlockMachine_UpgradeValue_Text.text = $"-1s";

            if (Managers.Game.money >= _upgradePrices[_level])
            {
                Managers._gameUi.BlockMachine_Upgrade_Button.interactable = true;
                Managers._gameUi.BlockMachine_Upgrade_Price_Text.color = Color.white;

            }
            else
            {
                Managers._gameUi.BlockMachine_Upgrade_Button.interactable = false;
                Managers._gameUi.BlockMachine_Upgrade_Price_Text.color
                    = Managers._gameUi.BlockMachine_Upgrade_Button.colors.disabledColor;
            }
        }
        else
        {
            Managers._gameUi.BlockMachine_Upgrade_Price_Text.text = $"Max";
            Managers._gameUi.BlockMachine_Status_Text.text = $"{_spawnInterval}s";
            Managers._gameUi.BlockMachine_UpgradeValue_Text.text = $"";
            Managers._gameUi.BlockMachine_Upgrade_Button.interactable = false;
            Managers._gameUi.BlockMachine_Upgrade_Price_Text.color
                    = Managers._gameUi.BlockMachine_Upgrade_Button.colors.disabledColor;
        }
    }





}
