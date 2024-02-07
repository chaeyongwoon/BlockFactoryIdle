using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.AI;
using MondayOFF;

public class StageManager : MonoBehaviour
{
    public int _stageLevel = 0;
    public enum VehicleType
    {
        Car,
        Train,
        Boat
    }
    public VehicleType vehicleType;

    [FoldoutGroup("BlockMachine")] public List<BlockMachine> _blockMachineList = new List<BlockMachine>();
    [FoldoutGroup("BlockMachine")] public float _railSpeed = 0.5f;
    [FoldoutGroup("BlockMachine")] public BlockMachine _selectBlockMachine;
    [FoldoutGroup("BlockMachine")] public SkinnedMeshRenderer[] _skinnedBlock;
    [FoldoutGroup("BlockMachine")] public int _blockMachineCount;
    [FoldoutGroup("BlockMachine")] public double[] _blockMachine_Prices = new double[8];

    [FoldoutGroup("Vehicle")] public GameObject _vehicle_Head_Pref;
    [FoldoutGroup("Vehicle")] public GameObject _vehicle_Pref;
    [FoldoutGroup("Vehicle")] public int _vehicle_Spawn_Level = 0;
    [FoldoutGroup("Vehicle")] public int _vehicle_Capacity_Level = 0;
    [FoldoutGroup("Vehicle")] public int _vehicle_Speed_Level = 0;
    [FoldoutGroup("Vehicle")] public int _rail_Speed_Level = 0;

    [FoldoutGroup("Vehicle")] public double[] _spawnLevel_Prices = new double[50];
    [FoldoutGroup("Vehicle")] public double[] _speedLevel_Prices = new double[5];
    [FoldoutGroup("Vehicle")] public double[] _capacityLevel_Prices = new double[5];
    [FoldoutGroup("Vehicle")] public double[] _railSpeedLevel_Prices = new double[5];
    [FoldoutGroup("Vehicle")] public List<Vehicle> _vehicleList = new List<Vehicle>();
    [FoldoutGroup("Vehicle")][ShowInInspector] public Queue<Vehicle> _vehicleQueue = new Queue<Vehicle>();
    [FoldoutGroup("Vehicle")] public Train _vehicleHead;

    //[FoldoutGroup("Village")] public double _money = 1000d;
    [FoldoutGroup("Village")] public List<Building> buildingList = new List<Building>();
    [FoldoutGroup("Village")] public int buildingCompleteCount = 0;
    [FoldoutGroup("Village")] public bool _villageComplete = false;
    //[FoldoutGroup("Village")] public Transform[] AreaGroups;

    [FoldoutGroup("UI")] public Sprite[] scrollUpImg;
    [FoldoutGroup("UI")] public string[] scrollUpName;
    [FoldoutGroup("UI")] public string[] scrollUpExplain;
    [FoldoutGroup("UI")] public Sprite[] blockCountSprites;


    [FoldoutGroup("Order")] public Sprite[] peopleSprites;
    [FoldoutGroup("Order")] public Sprite[] blockSprites;

    [FoldoutGroup("RV")] public bool isRvDoubleSpawn;
    [FoldoutGroup("RV")] public float rvDoubleSpawnTime;
    [FoldoutGroup("RV")] public bool isRvVehicleSpeedUp;
    [FoldoutGroup("RV")] public float rvVehicleSpeedUpTime;
    [FoldoutGroup("RV")] public bool isRvRailSpeedUp;
    [FoldoutGroup("RV")] public float rvRailSpeedUpTime = 60f;


    public struct OrderStruct
    {

        public Sprite personSprite;
        public Sprite[] blockSprite;
        public int orderCount;
        public int[] orderType;
        public int[] blockCount;
        public double rewardCount;

        //////
        public bool isComplete;
        public float waitTerm;
        public float currentTerm;
    }
    [ShowInInspector]
    public OrderStruct[] orderStructs = new OrderStruct[3];



    public GameObject[] _cams;
    public GameObject _machineCanvas;
    Button _machineBuyButton;
    Text _machinePriceText;


    public BlockStorage _blockStorage;
    Transform _vehicleGroup;

    UI_GameScene _gameUi;
    Color _redColor;

    public float _vehicleTerm = 1f;


    //public int _playTime = 0;

    [SerializeField] int queueCount;


    public int popUpType = 0;
    [SerializeField] bool isFirstFree = true;
    //public bool isFree = false;
    // =================================================




    private void Awake()
    {
        ColorUtility.TryParseHtmlString("#FF4D00", out _redColor);

        _vehicleList = new List<Vehicle>();
        _gameUi = Managers._gameUi;
        _vehicleGroup = transform.Find("5.Vehicle_Group");
        int _count = transform.Find("6.MapAreaGroup").childCount;
        //AreaGroups = new Transform[_count];
        //for (int i = 0; i < _count; i++)
        //{
        //    AreaGroups[i] = transform.Find("6.MapAreaGroup").GetChild(i);
        //}


    }

    private void OnEnable()
    {
        LoadData();
        CheckMoney();

        //if (buildingCompleteCount < 3)
        //{
        //    Managers._gameUi.RvDoubleSpawn_Button.gameObject.SetActive(false);
        //    Managers._gameUi.RvRailSpeedUp_Button.gameObject.SetActive(false);
        //    Managers._gameUi.RvVehicleSpeedUp_Button.gameObject.SetActive(false);
        //}
        //else
        //{
        //    Managers._gameUi.RvDoubleSpawn_Button.gameObject.SetActive(true);
        //    Managers._gameUi.RvRailSpeedUp_Button.gameObject.SetActive(true);
        //    Managers._gameUi.RvVehicleSpeedUp_Button.gameObject.SetActive(true);
        //}



        _machineBuyButton.AddButtonEvent(() => AddBlockMachine());
        //SetStage();

        StartCoroutine(Cor_Update());

        switch (vehicleType)
        {
            case VehicleType.Car:
                StartCoroutine(Cor_Update_Car());
                break;

            case VehicleType.Train:
                StartCoroutine(Cor_Upgrade_Train());
                break;
        }

        //InitOrder();
        //for (int i = 0; i < 3; i++)
        //{
        //    ES3.Save<OrderStruct>($"Order_{_stageLevel}_{i}", orderStructs[i]);
        //}
        isRvDoubleSpawn = false;
        isRvRailSpeedUp = false;
        isRvVehicleSpeedUp = false;


    }

    IEnumerator Cor_Update_Car()
    {
        while (true)
        {
            yield return new WaitForSeconds(_vehicleTerm);

            if (_vehicleQueue.Count > 0 && (_blockStorage._blockCountArray[0] > 0 || _blockStorage._blockCountArray[1] > 0 || _blockStorage._blockCountArray[2] > 0 || _blockStorage._blockCountArray[3] > 0))
            {

                Vehicle _vehicle = _vehicleQueue.Dequeue();
                _vehicle._state = Vehicle.State.Move;
                _vehicle._target = _blockStorage.transform.Find("Out_Pos");
                _vehicle.GetComponent<NavMeshAgent>()
                    .Warp(_blockStorage.transform.Find("Out_Pos").position);

            }
        }
    }


    IEnumerator Cor_Upgrade_Train()
    {
        //Debug.Log("Cor Update Train");

        yield return new WaitForSeconds(_vehicleTerm);
        queueCount = _vehicleList.Count;
        for (int i = 0; i < _vehicleList.Count; i++)
        {
            Vehicle _vehicle = _vehicleList[i];
            if (_vehicle._state == Vehicle.State.Wait || _vehicle._state == Vehicle.State.Sleep)
                _vehicle._state = Vehicle.State.Move;
            _vehicle._target = _blockStorage.transform.Find("Out_Pos");
            _vehicle.GetComponent<NavMeshAgent>()
                .Warp(_blockStorage.transform.Find("Out_Pos").position);
            yield return new WaitForSeconds(1.15f / _vehicleList[0].GetComponent<NavMeshAgent>().speed);  // new WaitForSeconds(1f);
        }

    }

    IEnumerator Cor_Update()
    {
        while (true)
        {
            WaitForSeconds _1s = new WaitForSeconds(1f);
            yield return _1s;

            for (int i = 0; i < 3; i++)
            {
                int num = i;
                if (orderStructs[i].isComplete && orderStructs[i].currentTerm > 0)
                {
                    orderStructs[i].currentTerm--;
                }
                CheckOrder(num);
            }
        }
    }


    private void Start()
    {
        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingList[i].isBuildComplete == false)
            {
                buildingList[i].SetCanvas();
                break;
            }
        }
    }


    public void LoadData()
    {
        _blockMachineCount = ES3.Load<int>($"Stage_{_stageLevel}_BlockMachineCount", 0);
        buildingCompleteCount = ES3.Load<int>($"Stage_{_stageLevel}_buildingCompleteCount", 0);




        orderStructs[0] = ES3.Load<OrderStruct>($"Order_{_stageLevel}_{0}", CreateOrder());
        orderStructs[1] = ES3.Load<OrderStruct>($"Order_{_stageLevel}_{1}", CreateOrder());
        orderStructs[2] = ES3.Load<OrderStruct>($"Order_{_stageLevel}_{2}", CreateOrder());
        Managers._gameUi.SetOrderPanel(0, orderStructs[0]);
        Managers._gameUi.SetOrderPanel(1, orderStructs[1]);
        Managers._gameUi.SetOrderPanel(2, orderStructs[2]);
        CheckOrder(0);
        CheckOrder(1);
        CheckOrder(2);


        //_playTime = ES3.Load<int>("PlayTime", _playTime);


        for (int i = 0; i < _blockMachineCount; i++)
        {
            _blockMachineList[i].gameObject.SetActive(true);

            switch (i)
            {
                case 2:
                    _skinnedBlock[0].SetBlendShapeWeight(0, 100);
                    break;

                case 4:
                    _skinnedBlock[0].SetBlendShapeWeight(1, 100);
                    break;

                case 6:
                    _skinnedBlock[1].SetBlendShapeWeight(0, 100);
                    break;

                case 8:
                    _skinnedBlock[1].SetBlendShapeWeight(1, 100);
                    break;
                case 10:
                    _skinnedBlock[2].SetBlendShapeWeight(0, 100);
                    break;

                case 12:
                    _skinnedBlock[2].SetBlendShapeWeight(1, 100);
                    break;
                case 14:
                    _skinnedBlock[3].SetBlendShapeWeight(0, 100);
                    break;

                case 16:
                    _skinnedBlock[3].SetBlendShapeWeight(1, 100);
                    break;
                case 18:
                    _skinnedBlock[4].SetBlendShapeWeight(0, 100);
                    break;

            }

        }

        // Vehicle
        _vehicle_Spawn_Level = ES3.Load<int>($"Stage_{_stageLevel}_Vehicle_Spawn_Level", 0);
        _vehicle_Speed_Level = ES3.Load<int>($"Stage_{_stageLevel}_Vehicle_Speed_Level", 0);
        _vehicle_Capacity_Level = ES3.Load<int>($"Stage_{_stageLevel}_Vehicle_Capacity_Level", 0);
        _rail_Speed_Level = ES3.Load<int>($"Stage_{_stageLevel}_Rail_Level", 0);

        _railSpeed = 0.5f - (0.025f * _rail_Speed_Level);

        if (_vehicleList.Count == 0)
        {


            switch (vehicleType)
            {
                case VehicleType.Train:
                    NavMeshAgent _vehicle = Managers.Pool.Pop(_vehicle_Head_Pref, _vehicleGroup).GetComponent<NavMeshAgent>();
                    _vehicle.GetComponent<Vehicle>()._state = Vehicle.State.Wait;
                    _vehicle.Warp(_blockStorage.transform.Find("Out_Pos").position);
                    _vehicle.destination = _blockStorage.transform.Find("Out_Pos").position;

                    Vehicle _newVehicle = _vehicle.GetComponent<Vehicle>();
                    _newVehicle.SetLevel(_vehicle_Speed_Level, _vehicle_Capacity_Level);
                    _vehicleList.Add(_newVehicle);
                    _vehicleHead = (Train)_newVehicle;
                    break;
            }


            for (int i = 0; i < _vehicle_Spawn_Level; i++)
            {
                AddVehicle();
            }
            AllVehicleSetLevel();
        }

    }
    public void SaveData()
    {
        ES3.Save<int>($"Stage_{_stageLevel}_Vehicle_Spawn_Level", _vehicle_Spawn_Level);
        ES3.Save<int>($"Stage_{_stageLevel}_Vehicle_Speed_Level", _vehicle_Speed_Level);
        ES3.Save<int>($"Stage_{_stageLevel}_Vehicle_Capacity_Level", _vehicle_Capacity_Level);

        ES3.Save<int>($"Stage_{_stageLevel}_BlockMachineCount", _blockMachineCount);
        ES3.Save<int>($"Stage_{_stageLevel}_Rail_Level", _rail_Speed_Level);

        //ES3.Save<int[]>("rvCounts", rvCounts);

        //for (int i = 0; i < 3; i++)
        //{
        //    ES3.Save<OrderStruct>($"Order_{_stageLevel}_{i}", orderStructs[i]);
        //}

    }

    // ====== Block Machine ============================================================

    public void AddBlockMachine(bool isPay = true)
    {
        if (_blockMachineCount < _blockMachineList.Count)
        {
            if (isPay)
            {
                Managers.Game.CalcMoney(-_blockMachine_Prices[_blockMachineCount]);
            }
            _blockMachineList[_blockMachineCount].gameObject.SetActive(true);
            if (_stageLevel == 0 && _blockMachineCount == 0) TutorialManager._instance.Tutorial_Complete();
            if (_stageLevel == 0 && _blockMachineCount == 1)
            {
                TutorialManager._instance.Tutorial_Complete();
                TutorialManager._instance.Tutorial_Img();
            }
            _blockMachineCount++;
            ES3.Save<int>($"Stage_{_stageLevel}_BlockMachineCount", _blockMachineCount);

            EventTracker.LogEvent("BlockMachine", new Dictionary<string, object> { { "BlockMachine",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_MachineCount-{_blockMachineCount}" } });


            //Debug.Log("Save BlockMachine Count :" + _blockMachineCount);

            switch (_blockMachineCount)
            {
                case 3:
                    _skinnedBlock[0].SetBlendShapeWeight(0, 100);
                    break;

                case 5:
                    _skinnedBlock[0].SetBlendShapeWeight(1, 100);
                    break;

                case 7:
                    _skinnedBlock[1].SetBlendShapeWeight(0, 100);
                    break;

                case 9:
                    _skinnedBlock[1].SetBlendShapeWeight(1, 100);
                    break;
                case 11:
                    _skinnedBlock[2].SetBlendShapeWeight(0, 100);
                    break;

                case 13:
                    _skinnedBlock[2].SetBlendShapeWeight(1, 100);
                    break;
                case 15:
                    _skinnedBlock[3].SetBlendShapeWeight(0, 100);
                    break;
                case 17:
                    _skinnedBlock[3].SetBlendShapeWeight(1, 100);
                    break;
                case 19:
                    _skinnedBlock[4].SetBlendShapeWeight(0, 100);
                    break;


            }
            CheckMoney();
        }
    }
    public void SelectBlockMachine_Upgrade()
    {
        _selectBlockMachine.UpgradeMachine();

    }

    // ====== Vehicle =====================

    public void AddVehicle()
    {
        NavMeshAgent _vehicle = Managers.Pool.Pop(_vehicle_Pref, _vehicleGroup).GetComponent<NavMeshAgent>();
        _vehicle.GetComponent<Vehicle>()._state = Vehicle.State.Wait;
        _vehicle.Warp(_blockStorage.transform.Find("Out_Pos").position);
        _vehicle.destination = _blockStorage.transform.Find("Out_Pos").position;

        Vehicle _newVehicle = _vehicle.GetComponent<Vehicle>();
        _newVehicle.SetLevel(_vehicle_Speed_Level, _vehicle_Capacity_Level);
        _vehicleList.Add(_newVehicle);


        switch (vehicleType)
        {
            case VehicleType.Car:
                _vehicleQueue.Enqueue(_newVehicle);

                break;

            case VehicleType.Train:

                break;
        }



    }
    public void AllVehicleSetLevel()
    {
        for (int i = 0; i < _vehicleList.Count; i++)
        {
            _vehicleList[i].SetLevel(_vehicle_Speed_Level, _vehicle_Capacity_Level);
        }
    }
    public void VehicleUpgrade(int _num, bool isPay = true)
    {
        //Debug.Log("vehicle Upgrade :" + _num);
        switch (_num)
        {
            case 0:
                if (isPay) Managers.Game.CalcMoney(-_spawnLevel_Prices[_vehicle_Spawn_Level]);
                AddVehicle();
                _vehicle_Spawn_Level++;
                if (TutorialManager._instance._tutorial_Level == 2)
                {
                    TutorialManager._instance.Tutorial_Complete();
                    Managers._gameUi.Scroll_Panel.SetActive(false);
                    //TutorialManager._instance.Tutorial_Img();
                }

                EventTracker.LogEvent("Vehicle", new Dictionary<string, object> {{"Vehicle",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_AddVehicle-{_vehicle_Spawn_Level}"}});




                break;

            case 1:
                if (isPay) Managers.Game.CalcMoney(-_speedLevel_Prices[_vehicle_Speed_Level]);
                _vehicle_Speed_Level++;
                AllVehicleSetLevel();

                EventTracker.LogEvent("Vehicle", new Dictionary<string, object> { { "Vehicle",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_SpeedUp-{_vehicle_Speed_Level}"}});

                break;

            case 2:
                if (isPay) Managers.Game.CalcMoney(-_capacityLevel_Prices[_vehicle_Capacity_Level]);
                _vehicle_Capacity_Level++;
                AllVehicleSetLevel();

                EventTracker.LogEvent("Vehicle", new Dictionary<string, object> { { "Vehicle",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_CapacityUp-{_vehicle_Capacity_Level}"}});

                break;

            case 3:
                if (isPay) Managers.Game.CalcMoney(-_railSpeedLevel_Prices[_rail_Speed_Level]);
                _rail_Speed_Level++;
                _railSpeed = 0.5f - (0.025f * _rail_Speed_Level);

                EventTracker.LogEvent("BlockMachine", new Dictionary<string, object> { { "BlockMachine",
                        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_RailUpgrade-{_rail_Speed_Level}" } });
                break;
        }

        SaveData();
        CheckScrollUpgradePrice();

    }

    // ======  Village =====================

    public void BuildComplete()
    {
        if (buildingCompleteCount == 2 && _stageLevel == 0)
        {
            this.TaskDelay(3f, () =>
            {
                popUpType = 0;
                Managers._gameUi.RvPopupPanelOnOff(popUpType, true, 20f);
            });
        }

        buildingCompleteCount++;
        ES3.Save<int>($"Stage_{_stageLevel}_buildingCompleteCount", buildingCompleteCount);
        if (buildingCompleteCount < buildingList.Count)
            buildingList[buildingCompleteCount].SetCanvas();

        if (buildingCompleteCount >= buildingList.Count)
        {
            _villageComplete = true;
            ES3.Save<bool>($"VillageComplete_{_stageLevel}", _villageComplete);

            Managers.Game.ClearStage();

        }




    }

    public Transform FindBuilding()
    {

        for (int i = 0; i < buildingList.Count; i++)
        {
            if (buildingList[i]._currentCount < buildingList[i]._maxCount)
            {
                return buildingList[i].transform;
            }
        }
        return buildingList[Random.Range(0, buildingList.Count)].transform;

    }

    public Transform ReFindBuilding()
    {

        //for (int i = 0; i < buildingList.Count; i++)
        //{
        int _num = Random.Range(0, buildingCompleteCount);
        if (Managers.Game.currentStageManager
            ._blockStorage._blockCountArray[(int)buildingList[_num]._blockType] > 0)
        {
            return buildingList[_num].transform;
        }
        //}

        return buildingList[_num].transform;
    }



    // ======== UI =========================

    public void CheckMoney()
    {
        // ==== Machine Check ============== 
        if (_machinePriceText == null)
        {
            _machineBuyButton = _machineCanvas.transform.GetChild(1).GetComponent<Button>();
            _machinePriceText = _machineBuyButton.transform.Find("MachinePriceText").GetComponent<Text>();
        }

        if (_blockMachineCount < _blockMachineList.Count)
        {
            _machineCanvas.SetActive(true);
            _machineCanvas.transform.position = _blockMachineList[_blockMachineCount].transform.position + Vector3.up * 0.05f;
            if (Managers.Game.money >= _blockMachine_Prices[_blockMachineCount])
            {
                _machineBuyButton.interactable = true;
                _machinePriceText.text = $"{_blockMachine_Prices[_blockMachineCount]}";
                _machinePriceText.color = Color.white;
            }
            else
            {
                _machineBuyButton.interactable = false;
                _machinePriceText.text = $"{_blockMachine_Prices[_blockMachineCount]}";
                _machinePriceText.color = _machineBuyButton.colors.disabledColor;
            }
        }
        else
        {
            _machineCanvas.SetActive(false);
        }







    }


    public void ChangeUI()
    {
        CheckScrollUpgradePrice();
        CheckOrderPanel();
    }

    public void CheckOrderPanel()
    {

    }


    [Button]
    public void CheckScrollUpgradePrice()
    {

        for (int i = 0; i < 4; i++)
        {
            _gameUi._scrollUpgContent[i].transform.Find("Image").GetComponent<Image>().sprite =
                    scrollUpImg[i];
            _gameUi._scrollUpgContent[i].transform.Find("Name_Text").GetComponent<Text>().text = scrollUpName[i];
            _gameUi._scrollUpgContent[i].transform.Find("Explain_Text").GetComponent<Text>().text = scrollUpExplain[i];

        }



        if (_vehicle_Spawn_Level < _spawnLevel_Prices.Length)
        {
            _gameUi._scrollUpgContent[0].SetActive(true);
            _gameUi._scrollUpgButtons[0].transform.Find("UpgradePrice_Text").GetComponent<Text>().text = $"{_spawnLevel_Prices[_vehicle_Spawn_Level]}";

            if (Managers.Game.money >= _spawnLevel_Prices[_vehicle_Spawn_Level])
            {
                _gameUi._scrollUpgButtons[0].interactable = true;
                _gameUi._scrollUpgButtons[0].transform.Find("Rv_Button").gameObject.SetActive(false);
                _gameUi._scrollUpgButtons[0].transform.Find("UpgradePrice_Text").GetComponent<Text>().color
                    = Color.white;
                _gameUi._scrollUpgButtons[0].transform.Find("Coin_Img").GetComponent<Image>().color
                    = Color.white;
            }
            else
            {
                _gameUi._scrollUpgButtons[0].interactable = false;
                _gameUi._scrollUpgButtons[0].transform.Find("Rv_Button").gameObject.SetActive(true);
                _gameUi._scrollUpgButtons[0].transform.Find("UpgradePrice_Text").GetComponent<Text>().color
                = _redColor; //= _gameUi._scrollUpgButtons[0].colors.disabledColor;

            }
        }
        else
        {

            _gameUi._scrollUpgButtons[0].transform.Find("UpgradePrice_Text").GetComponent<Text>().text = $"Max";
            _gameUi._scrollUpgButtons[0].transform.Find("UpgradePrice_Text").GetComponent<Text>().color
                    = Color.white;
            _gameUi._scrollUpgButtons[0].interactable = false;
            _gameUi._scrollUpgButtons[0].transform.Find("Rv_Button").gameObject.SetActive(false);

        }
        // ============

        if (_vehicle_Speed_Level < _speedLevel_Prices.Length)
        {
            _gameUi._scrollUpgContent[1].SetActive(true);
            _gameUi._scrollUpgButtons[1].transform.Find("UpgradePrice_Text").GetComponent<Text>().text = $"{_speedLevel_Prices[_vehicle_Speed_Level]}";

            if (Managers.Game.money >= _speedLevel_Prices[_vehicle_Speed_Level])
            {
                _gameUi._scrollUpgButtons[1].interactable = true;
                _gameUi._scrollUpgButtons[1].transform.Find("Rv_Button").gameObject.SetActive(false);

                _gameUi._scrollUpgButtons[1].transform.Find("UpgradePrice_Text")
                    .GetComponent<Text>().color = Color.white;
                _gameUi._scrollUpgButtons[1].transform.Find("Coin_Img")
                    .GetComponent<Image>().color = Color.white;
            }
            else
            {
                _gameUi._scrollUpgButtons[1].interactable = false;
                _gameUi._scrollUpgButtons[1].transform.Find("Rv_Button").gameObject.SetActive(true);

                _gameUi._scrollUpgButtons[1].transform.Find("UpgradePrice_Text")
                    .GetComponent<Text>().color = _redColor;//= _gameUi._scrollUpgButtons[1].colors.disabledColor;

            }
        }
        else
        {
            //_gameUi._scrollUpgContent[1].SetActive(false);
            _gameUi._scrollUpgButtons[1].transform.Find("UpgradePrice_Text").GetComponent<Text>().text = $"Max";
            _gameUi._scrollUpgButtons[1].transform.Find("UpgradePrice_Text").GetComponent<Text>().color
                   = Color.white;
            _gameUi._scrollUpgButtons[1].interactable = false;
            _gameUi._scrollUpgButtons[1].transform.Find("Rv_Button").gameObject.SetActive(false);

        }
        // ==
        if (_vehicle_Capacity_Level < _capacityLevel_Prices.Length)
        {
            _gameUi._scrollUpgContent[2].SetActive(true);
            _gameUi._scrollUpgButtons[2].transform.Find("UpgradePrice_Text").GetComponent<Text>().text = $"{_capacityLevel_Prices[_vehicle_Capacity_Level]}";

            if (Managers.Game.money >= _capacityLevel_Prices[_vehicle_Capacity_Level])
            {
                _gameUi._scrollUpgButtons[2].interactable = true;
                _gameUi._scrollUpgButtons[2].transform.Find("Rv_Button").gameObject.SetActive(false);
                _gameUi._scrollUpgButtons[2].transform.Find("UpgradePrice_Text").GetComponent<Text>().color
                    = Color.white;
                _gameUi._scrollUpgButtons[2].transform.Find("Coin_Img").GetComponent<Image>().color
                    = Color.white;
            }
            else
            {
                _gameUi._scrollUpgButtons[2].interactable = false;
                _gameUi._scrollUpgButtons[2].transform.Find("Rv_Button").gameObject.SetActive(true);
                _gameUi._scrollUpgButtons[2].transform.Find("UpgradePrice_Text").GetComponent<Text>().color
                   = _redColor;// = gameUi._scrollUpgButtons[2].colors.disabledColor;

            }
        }
        else
        {

            _gameUi._scrollUpgButtons[2].transform.Find("UpgradePrice_Text").GetComponent<Text>().text = $"Max";
            _gameUi._scrollUpgButtons[2].transform.Find("UpgradePrice_Text").GetComponent<Text>().color
                   = Color.white;
            _gameUi._scrollUpgButtons[2].interactable = false;
            _gameUi._scrollUpgButtons[2].transform.Find("Rv_Button").gameObject.SetActive(false);

        }
        // ==
        if (_rail_Speed_Level < _railSpeedLevel_Prices.Length)
        {
            _gameUi._scrollUpgContent[3].SetActive(true);
            _gameUi._scrollUpgButtons[3].transform.Find("UpgradePrice_Text").GetComponent<Text>().text = $"{_railSpeedLevel_Prices[_rail_Speed_Level]}";

            if (Managers.Game.money >= _railSpeedLevel_Prices[_rail_Speed_Level])
            {
                _gameUi._scrollUpgButtons[3].interactable = true;
                _gameUi._scrollUpgButtons[3].transform.Find("Rv_Button").gameObject.SetActive(false);
                _gameUi._scrollUpgButtons[3].transform.Find("UpgradePrice_Text").GetComponent<Text>().color
                    = Color.white;
                _gameUi._scrollUpgButtons[3].transform.Find("Coin_Img").GetComponent<Image>().color
                    = Color.white;
            }
            else
            {
                _gameUi._scrollUpgButtons[3].interactable = false;
                _gameUi._scrollUpgButtons[3].transform.Find("Rv_Button").gameObject.SetActive(true);
                _gameUi._scrollUpgButtons[3].transform.Find("UpgradePrice_Text").GetComponent<Text>().color
                    = _redColor; //_gameUi._scrollUpgButtons[3].colors.disabledColor;

            }
        }
        else
        {

            _gameUi._scrollUpgButtons[3].transform.Find("UpgradePrice_Text").GetComponent<Text>().text = $"Max";
            _gameUi._scrollUpgButtons[3].transform.Find("UpgradePrice_Text").GetComponent<Text>().color
                   = Color.white;
            _gameUi._scrollUpgButtons[3].interactable = false;
            _gameUi._scrollUpgButtons[3].transform.Find("Rv_Button").gameObject.SetActive(false);

        }


    }

    [Button]
    public void SetStage()
    {
        for (int i = 0; i < 4; i++)
        {
            Managers._gameUi.BlockCount_Group.transform
                 .GetChild(i).GetComponent<Image>().sprite = blockCountSprites[i];

        }
        _blockStorage.UpdateBlockCount();


        _vehicleQueue.Clear();


        for (int i = 0; i < _vehicleList.Count; i++)
        {
            _vehicleList[i].SetReturn();

            _vehicleList[i]._target = _blockStorage.transform.Find("Out_Pos");
            _vehicleList[i].GetComponent<NavMeshAgent>()
                .Warp(_blockStorage.transform.Find("Out_Pos").position);
            //_vehicleList[i].SetDest(_blockStorage.transform.Find("Out_Pos"));
            _vehicleList[i].GetComponent<NavMeshAgent>().destination = _blockStorage.transform.Find("Out_Pos").position;

            switch (vehicleType)
            {
                case VehicleType.Car:
                    _vehicleQueue.Enqueue(_vehicleList[i]);

                    break;
            }


        }


        // add UI Change
        if (transform.gameObject.activeSelf)
            _blockStorage.UpdateBlockCount();
        //dd



    }

    public void ChangeCam(int _num, float _time = 0.5f)
    {
        for (int i = 0; i < _cams.Length; i++)
        {
            if (_num == i) _cams[i].SetActive(true);
            else _cams[i].SetActive(false);

            Camera.main.transform.GetComponent<Cinemachine.CinemachineBrain>()
                .m_DefaultBlend.m_Time = _time;

            //_cams[i].SetActive(false);
        }

    }

    public void VehicleEnqueue(Vehicle _vehicle)
    {
        _vehicleQueue.Enqueue(_vehicle);
        switch (vehicleType)
        {
            case VehicleType.Train:
                if (_vehicleQueue.Count >= queueCount)
                {
                    StartCoroutine(Cor_Upgrade_Train());
                    _vehicleQueue.Clear();
                }

                break;
        }
    }

    //[Button]
    //public void InitOrder()
    //{
    //    //orderStructs = new OrderStruct[3];
    //    for (int i = 0; i < 3; i++)
    //    {
    //        ES3.Save<OrderStruct>($"Order_{_stageLevel}_{i}", orderStructs[i]);
    //    }
    //    if (!ES3.KeyExists($"Order_{_stageLevel}_0"))
    //    {

    //        for (int i = 0; i < 3; i++)
    //        {
    //            int num = i;
    //            orderStructs[num] = CreateOrder();
    //            Managers._gameUi.SetOrderPanel(num, orderStructs[num]);
    //            CheckOrder(num);
    //            //UnityEditor.EditorApplication.isPaused = true;
    //        }
    //    }
    //}

    [Button]
    public void CheckOrder(int _num)
    {

        if (orderStructs[_num].isComplete == true)
        {
            if (Managers._gameUi.Order_Group.transform.GetChild(_num)
                .Find("Wait_Img").gameObject.activeSelf == false)
            {
                Managers._gameUi.Order_Group.transform.GetChild(_num)
                    .Find("Wait_Img").gameObject.SetActive(true);
            }
            Managers._gameUi.Order_Group.transform.GetChild(_num).Find("Wait_Img").Find("Wait_Text").GetComponent<Text>().text = $"{(int)(orderStructs[_num].currentTerm / 60):00} : {(orderStructs[_num].currentTerm % 60):00}";

            if (orderStructs[_num].currentTerm <= 0)
            {
                orderStructs[_num].isComplete = false;
                orderStructs[_num] = CreateOrder();
                Managers._gameUi.SetOrderPanel(_num, orderStructs[_num]);
                orderStructs[_num].currentTerm = orderStructs[_num].waitTerm;
                Managers._gameUi.Order_Group.transform.GetChild(_num).Find("Wait_Img").gameObject.SetActive(false);
            }
        }
        else
        {
            bool isClaim = true;
            for (int i = 0; i < orderStructs[_num].orderCount; i++)
            {
                if (_blockStorage._blockCountArray[orderStructs[_num].orderType[i]] >= orderStructs[_num].blockCount[i])
                {

                }
                else
                {
                    isClaim = false;
                }
            }
            if (isClaim)
            {
                Managers._gameUi.Order_Group.transform.GetChild(_num).Find("Claim_Button").GetComponent<Button>().interactable = true;
            }
            else
            {
                Managers._gameUi.Order_Group.transform.GetChild(_num).Find("Claim_Button").GetComponent<Button>().interactable = false;
            }

            //orderType
            //blockCount

            for (int i = 0; i < 3; i++)
            {
                ES3.Save<OrderStruct>($"Order_{_stageLevel}_{i}", orderStructs[i]);
            }

        }
    }



    public OrderStruct CreateOrder()
    {
        //Debug.Log("Create Order  ");
        OrderStruct newOrder = new OrderStruct();
        newOrder.personSprite = peopleSprites[Random.Range(0, peopleSprites.Length)];

        newOrder.orderCount = Random.Range(1, 3);
        newOrder.orderType = new int[newOrder.orderCount];
        newOrder.blockCount = new int[newOrder.orderCount];
        newOrder.blockSprite = new Sprite[2];
        newOrder.waitTerm =
                             //5f;
                             30f * 60f;
        newOrder.rewardCount = 0;
        for (int i = 0; i < newOrder.orderCount; i++)
        {
            newOrder.orderType[i] = Random.Range(0, 4);
            int _count = Random.Range(1, 11) * 10;
            newOrder.blockCount[i] = _count;
            newOrder.blockSprite[i] = blockSprites[newOrder.orderType[i]];
            newOrder.rewardCount += (_count * 5d);
        }

        if (newOrder.orderCount > 1)
        {
            if (newOrder.orderType[0] == newOrder.orderType[1])
            {
                Debug.Log("Same Type is Change");

                newOrder.orderType[1] += Random.Range(1, 4);
                if (newOrder.orderType[1] >= 4) newOrder.orderType[1] -= 4;
                newOrder.blockSprite[1] = blockSprites[newOrder.orderType[1]];
            }
        }

        //newOrder.rewardCount = 100;



        return newOrder;
    }

    public void RewardOrder(int _num)
    {
        Managers.Game.CalcMoney(orderStructs[_num].rewardCount);
        orderStructs[_num].isComplete = true;
        orderStructs[_num].currentTerm = orderStructs[_num].waitTerm;
        Managers._gameUi.Order_Group.transform.GetChild(_num).Find("Wait_Img").gameObject.SetActive(true);


        for (int i = 0; i < orderStructs[_num].orderCount; i++)
        {
            _blockStorage._blockCountArray[orderStructs[_num].orderType[i]] -=
            orderStructs[_num].blockCount[i];

            //Debug.Log($"Order Type :{orderStructs[_num].orderType[i]} / Count :{orderStructs[_num].blockCount[i]}");
        }


        _blockStorage.UpdateBlockCount();
        CheckOrder(_num);
        Managers.Game.OrderComplete();
        //
    }

    // ====================================================

    public void RV_Order_Refresh(int _num)
    {
        EventTracker.LogEvent("Rv", new Dictionary<string, object> { { "Rv",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_RvOrderRefresh"}});


        orderStructs[_num].isComplete = false;
        orderStructs[_num] = CreateOrder();
        Managers._gameUi.SetOrderPanel(_num, orderStructs[_num]);
        orderStructs[_num].currentTerm = orderStructs[_num].waitTerm;
        Managers._gameUi.Order_Group.transform.GetChild(_num)
            .Find("Wait_Img").gameObject.SetActive(false);

        CheckOrder(_num);


    }

    public void RV_DoubleSpawn(bool notFree = true, float _time = 60f)
    {
        if (Managers._gameUi.RvDoubleSpawn_Button.gameObject.activeSelf == false)
            Managers._gameUi.RvDoubleSpawn_Button.gameObject.SetActive(true);



        if (notFree)
            EventTracker.LogEvent("Rv", new Dictionary<string, object> { { "Rv",
                   $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_RvDoubleSpawn"}});
        isRvDoubleSpawn = true;

        DOTween.Sequence().
            AppendCallback(() =>
            {

                Managers._gameUi.RvDoubleSpawn_Button.interactable = false;
                Managers._gameUi.RvDoubleSpawn_Button.transform.Find("Guage_Group").gameObject.SetActive(true);
                Managers._gameUi.RvDoubleSpawn_Button.transform.Find("Guage_Group").transform
               .Find("Guage_Img").GetComponent<Image>().fillAmount = 1f;
                Managers._gameUi.RvDoubleSpawn_Button.transform.Find("Guage_Group")
                .transform.Find("Guage_Img").GetComponent<Image>().DOFillAmount(0f, _time).SetEase(Ease.Linear);
                Managers._gameUi.RvDoubleSpawn_Button.transform.Find("Guage_Group").transform
                .Find("Time_Text").GetComponent<Text>().DOCounter((int)_time, 0, _time).SetEase(Ease.Linear);
                Managers._gameUi.RvDoubleSpawn_Button.transform.Find("Rv_Img").gameObject.SetActive(false);

            })
            .AppendInterval(_time)
            .OnComplete(() =>
            {
                isRvDoubleSpawn = false;
                Managers._gameUi.RvDoubleSpawn_Button.interactable = true;
                Managers._gameUi.RvDoubleSpawn_Button.transform.Find("Guage_Group").gameObject.SetActive(false);
                Managers._gameUi.RvDoubleSpawn_Button.transform.Find("Guage_Group").transform
                .Find("Guage_Img").GetComponent<Image>().fillAmount = 1f;
                Managers._gameUi.RvDoubleSpawn_Button.transform.Find("Rv_Img").gameObject.SetActive(true);

                //isFree = false;
            });
    }

    public void RV_VehicleSpeedUp(bool notFree = true, float _time = 60f)
    {
        if (notFree)
            EventTracker.LogEvent("Rv", new Dictionary<string, object> { { "Rv",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}__RvVehicleSpeedUp"}});

        isRvVehicleSpeedUp = true;

        DOTween.Sequence().
            AppendCallback(() =>
            {
                for (int i = 0; i < _vehicleList.Count; i++)
                {
                    _vehicleList[i].SetLevel(_vehicle_Speed_Level, _vehicle_Capacity_Level);
                }
                Managers._gameUi.RvVehicleSpeedUp_Button.interactable = false;
                Managers._gameUi.RvVehicleSpeedUp_Button.transform.Find("Guage_Group").gameObject.SetActive(true);
                Managers._gameUi.RvVehicleSpeedUp_Button.transform.Find("Guage_Group").transform
                .Find("Guage_Img").GetComponent<Image>().fillAmount = 1f;
                Managers._gameUi.RvVehicleSpeedUp_Button.transform.Find("Guage_Group")
                .transform.Find("Guage_Img").GetComponent<Image>().DOFillAmount(0f, _time).SetEase(Ease.Linear);
                Managers._gameUi.RvVehicleSpeedUp_Button.transform.Find("Guage_Group").transform
                .Find("Time_Text").GetComponent<Text>().DOCounter((int)_time, 0, _time).SetEase(Ease.Linear);
                Managers._gameUi.RvVehicleSpeedUp_Button.transform.Find("Rv_Img").gameObject.SetActive(false);


            })
            .AppendInterval(_time)
            .OnComplete(() =>
            {
                isRvVehicleSpeedUp = false;
                for (int i = 0; i < _vehicleList.Count; i++)
                {
                    _vehicleList[i].SetLevel(_vehicle_Speed_Level, _vehicle_Capacity_Level);
                }
                Managers._gameUi.RvVehicleSpeedUp_Button.interactable = true;
                Managers._gameUi.RvVehicleSpeedUp_Button.transform.Find("Guage_Group").gameObject.SetActive(false);
                Managers._gameUi.RvVehicleSpeedUp_Button.transform.Find("Guage_Group").transform
                .Find("Guage_Img").GetComponent<Image>().fillAmount = 1f;
                Managers._gameUi.RvVehicleSpeedUp_Button.transform.Find("Rv_Img").gameObject.SetActive(true);


            });
    }

    public void RV_RailSpeedUp(bool notFree = true, float _time = 60f)
    {
        if (Managers._gameUi.RvRailSpeedUp_Button.gameObject.activeSelf == false)
            Managers._gameUi.RvRailSpeedUp_Button.gameObject.SetActive(true);


        if (notFree)
            EventTracker.LogEvent("Rv", new Dictionary<string, object> { { "Rv",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_RvRailSpeedUp"}});

        isRvRailSpeedUp = true;
        _railSpeed = _railSpeed * 0.5f < 0.05 ? 0.05f : _railSpeed;
        //isUnLimit = true;
        DOTween.Sequence().
            AppendCallback(() =>
            {
                Managers._gameUi.RvRailSpeedUp_Button.interactable = false;
                Managers._gameUi.RvRailSpeedUp_Button.transform.Find("Guage_Group").gameObject.SetActive(true);
                Managers._gameUi.RvRailSpeedUp_Button.transform.Find("Guage_Group").transform
                .Find("Guage_Img").GetComponent<Image>().fillAmount = 1f;
                Managers._gameUi.RvRailSpeedUp_Button.transform.Find("Guage_Group")
                .transform.Find("Guage_Img").GetComponent<Image>().DOFillAmount(0f, _time).SetEase(Ease.Linear);
                Managers._gameUi.RvRailSpeedUp_Button.transform.Find("Guage_Group").transform
                .Find("Time_Text").GetComponent<Text>().DOCounter((int)_time, 0, _time).SetEase(Ease.Linear);
                Managers._gameUi.RvRailSpeedUp_Button.transform.Find("Rv_Img").gameObject.SetActive(false);

            })
            .AppendInterval(_time)
            .OnComplete(() =>
            {
                isRvRailSpeedUp = false;
                Managers._gameUi.RvRailSpeedUp_Button.interactable = true;
                Managers._gameUi.RvRailSpeedUp_Button.transform.Find("Guage_Group").gameObject.SetActive(false);
                Managers._gameUi.RvRailSpeedUp_Button.transform.Find("Guage_Group").transform
                .Find("Guage_Img").GetComponent<Image>().fillAmount = 1f;
                Managers._gameUi.RvRailSpeedUp_Button.transform.Find("Rv_Img").gameObject.SetActive(true);

                _railSpeed = 0.5f - (0.025f * _rail_Speed_Level);

            });
    }


    public void FreeRvFunc()
    {
        float _time = 20f;
        switch (popUpType)
        {
            case 0:
                //isFree = true;
                RV_DoubleSpawn(false, _time);
                break;

            case 1:
                RV_RailSpeedUp(false, _time);
                break;

            case 2:
                RV_VehicleSpeedUp(false, _time);
                break;
        }

        Managers._gameUi.RvPopupPanelOnOff(0, false);

        if (buildingCompleteCount == 3 && isFirstFree && _stageLevel == 0)
        {
            isFirstFree = false;
            this.TaskDelay(10f, () =>
        {
            popUpType = 1;
            Managers._gameUi.RvPopupPanelOnOff(popUpType, true, 20f);
            Managers._gameUi.RvVehicleSpeedUp_Button.gameObject.SetActive(true);
        });
        }
    }

    [Button]
    public void Test(int _num)
    {
        popUpType = _num;
        Managers._gameUi.RvPopupPanelOnOff(popUpType);

    }


}

