using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.AI;
using MondayOFF;
using System;



public class GameManager : MonoBehaviour
{

    public StageManager[] stageManagers;
    public StageManager currentStageManager;

    public int stageLevel;
    public int currentStageLevel;
    public double money = 0d;
    public int playTime = 0;
    public bool isISReady = true;

    //=== Ads Values

    public int isA = 0;
    public int IsInterval = 120;
    public int IsCount = 0;
    public int RvCount = 0;
    public enum ABType
    {
        A,
        B
    }
    // ===  IAP ==============
    public int ticketCount = 0;
    //public bool noAds = false;
    public bool allBoost = false;
    public bool infiniteTicket = false;


    // =====================


    [ShowInInspector] RaycastHit _tempHit;
    public GameObject[] _tempObj = new GameObject[2];


    public int orderLevel = 0;
    public int orderCount = 0;
    public int orderCountStack = 0;

    public bool isSound = true;
    public bool readyDailyTicket = true;
    public DateTime lastGetTicketTime;

    public Material _seaMat;

    // ===================================

    public void Init() // 처음 초기화
    {
        LoadData();

        currentStageLevel = stageLevel;
        ChangeStage();

        CalcMoney(0);
        TicketUpdate(0);


        // consumable
        //MondayOFF.IAPManager.RegisterProduct("blockmatchhro_ticket_1", BuyTicket_1);
        MondayOFF.IAPManager.RegisterProduct("blockmatchhro_ticket_2", BuyTicket_2);
        MondayOFF.IAPManager.RegisterProduct("blockmatchhro_ticket_3", BuyTicket_3);

        // non consumable
        MondayOFF.IAPManager.RegisterProduct("blockmatchhro_boostpack", BuyAllPack);
        MondayOFF.IAPManager.RegisterProduct("blockmatchhro_ticket_pack", BuyTicketPack);
        MondayOFF.IAPManager.RegisterProduct("blockmatchhro_infinite_pack", BuyInfinitePack);

        MondayOFF.IAPManager.OnAfterPurchase += (isSuccess) => Debug.Log("구매 완료!");


        _seaMat.DOOffset(new Vector2(0f, 0f), 0);
        _seaMat.DOOffset(new Vector2(1f, 0f), 5).SetLoops(-1, LoopType.Restart).SetEase(Ease.Linear);

        if (stageLevel == 0)
        {
            if (currentStageManager.buildingCompleteCount < 3)
            {
                Managers._gameUi.RvDoubleSpawn_Button.gameObject.SetActive(false);
                Managers._gameUi.RvRailSpeedUp_Button.gameObject.SetActive(false);
                Managers._gameUi.RvVehicleSpeedUp_Button.gameObject.SetActive(false);
            }
        }
        else
        {
            Managers._gameUi.RvDoubleSpawn_Button.gameObject.SetActive(true);
            Managers._gameUi.RvRailSpeedUp_Button.gameObject.SetActive(true);
            Managers._gameUi.RvVehicleSpeedUp_Button.gameObject.SetActive(true);
        }


    }

    public void BuyTicket_1()
    {
        Debug.Log("buy ticket_1");

        TicketUpdate(3);

        EventTracker.LogEvent("Rv", new Dictionary<string, object> { { "Rv",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_GetTicketX3"} });

    }
    public void BuyTicket_2()
    {
        //Debug.Log("buy ticket_2");

        TicketUpdate(5);

        EventTracker.LogEvent("IAP", new Dictionary<string, object> { { "IAP",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_TicketX5"} });
    }
    public void BuyTicket_3()
    {
        //Debug.Log("buy ticket_3");

        TicketUpdate(15);
        EventTracker.LogEvent("IAP", new Dictionary<string, object> { { "IAP",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_TicketX15"} });

    }

    public void BuyAllPack()
    {
        //Debug.Log("buy All Pack");

        allBoost = true;
        ES3.Save<bool>("allBoost", allBoost);

        NoAds.Purchase();
        //noAds = true;
        //ES3.Save<bool>("noAds", noAds);

        ProductsCheck();

        EventTracker.LogEvent("IAP", new Dictionary<string, object> { { "IAP",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_BoostPack"} });

    }

    public void BuyTicketPack()
    {
        //Debug.Log("buy Ticket Pack");

        TicketUpdate(15);

        NoAds.Purchase();
        //noAds = true;
        //ES3.Save<bool>("noAds", noAds);

        ProductsCheck();

        EventTracker.LogEvent("IAP", new Dictionary<string, object> { { "IAP",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_TicketPack"} });

    }

    public void BuyInfinitePack()
    {
        //Debug.Log("buy infiniteTicket Pack");

        infiniteTicket = true;
        ES3.Save<bool>("infiniteTicket", infiniteTicket);
        TicketUpdate(99999);

        NoAds.Purchase();
        //noAds = true;
        //ES3.Save<bool>("noAds", noAds);

        ProductsCheck();

        EventTracker.LogEvent("IAP", new Dictionary<string, object> { { "IAP",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_InfinitePack"} });

    }

    // ==== End IAP =============



    public void Clear() // 씬 전환할
    {



    }

    // ====================================
    [Button]
    public void ChangeStage(int _num = 0)
    {
        for (int i = 0; i < stageManagers.Length; i++)
        {
            if (stageManagers[i] != null)
                stageManagers[i].gameObject.SetActive(false);
        }

        currentStageLevel += _num;
        if (currentStageLevel > stageManagers.Length - 1) currentStageLevel = stageManagers.Length - 1;
        else if (currentStageLevel < 0) currentStageLevel = 0;

        currentStageManager = stageManagers[currentStageLevel];
        currentStageManager.gameObject.SetActive(true);
        currentStageManager.SetStage();

        GetComponent<NavMeshSurface>().BuildNavMesh();


        //CheckMoney();
        CheckScrollUpgradePrice();


        Managers._gameUi.InitRvPanel();

        //money = 0;
        CalcMoney(0);

    }


    private void Start()
    {
        StartCoroutine(Cor_Update());
        StartCoroutine(Cor_IsUpdate());
    }

    IEnumerator Cor_Update()
    {
        while (true)
        {
            WaitForSeconds term = new WaitForSeconds(1f);
            yield return term;

            playTime++;
            if ((playTime % 10) == 0)
            {
                ES3.Save<int>("playTime", playTime);

                EventTracker.LogEvent("Village", new Dictionary<string, object> { { "Village",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_PlayTime-{playTime}"}});
            }

            if (Managers._gameUi.Shop_Panel.activeSelf == true)
                DailyTicketUpdate();


        }
    }

    IEnumerator Cor_IsUpdate()
    {
        IsInterval = MondayOFF.RemoteConfig.GetInt("IsInterval");

        yield return new WaitForSeconds(IsInterval);

        while (true)
        {
            //if (noAds == true) break;
            if (NoAds.IsNoAds == true) break;

            IsInterval = MondayOFF.RemoteConfig.GetInt("IsInterval");
            yield return new WaitForSeconds(IsInterval);

            if (NoAds.IsNoAds == false && stageManagers[0]._blockMachineCount >= 4 && AdsManager.IsInterstitialReady())
            {
                Managers._gameUi.AdBreak_Panel.SetActive(true);
                yield return new WaitForSeconds(1.5f);
                Managers._gameUi.AdBreak_Panel.SetActive(false);
                AdsManager.ShowInterstitial();
                IsCount++;

                EventTracker.LogEvent("Ads", new Dictionary<string, object> { { "Ads",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_IsCount-{IsCount}"}});

                ES3.Save<int>("IsCount", IsCount);
            }

        }
    }

    [Button]
    public void OdeeoTest()
    {
        AdsManager.ShowPlayOn();
    }


    private void Update()
    {
        //#if UNITY_EDITOR
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    CalcMoney(10000);
        //}


        //// ================= Mouse ====================
        //if (Input.GetMouseButtonDown(0))
        //{
        //    if (!EventSystem.current.IsPointerOverGameObject())// 
        //    {
        //        Managers._gameUi.ChangePanel(0);
        //    }
        //}


        //else if (Input.GetMouseButtonUp(0))
        //{
        //    if (!EventSystem.current.IsPointerOverGameObject())// 
        //    {
        //        //Debug.Log("None Ui");
        //        Ray ray;
        //        RaycastHit hit;
        //        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //        if (Physics.Raycast(ray, out hit))
        //        {
        //            Debug.DrawLine(ray.origin, hit.point, Color.red, 1.5f);
        //            switch (hit.collider.tag)
        //            {
        //                case "BlockMachine":
        //                    currentStageManager._selectBlockMachine = hit.transform.GetComponent<BlockMachine>();
        //                    Managers._gameUi.ChangePanel(1);
        //                    Managers._gameUi.BlockMachine_SetColor((int)currentStageManager._selectBlockMachine._spawnBlockType);
        //                    break;

        //            }
        //        }
        //    }
        //    //else
        //    //{
        //    //    //Debug.Log("On Ui");
        //    //}
        //}

        //#elif !UNITY_EDITOR

        if (Input.touchCount > 0)
        {
            Touch _touch = Input.GetTouch(0);
            if (_touch.phase == TouchPhase.Began)
            {

                if (!EventSystem.current.IsPointerOverGameObject(_touch.fingerId))
                {
                    Managers._gameUi.ChangePanel(0);

                    Ray ray;
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    Physics.Raycast(ray, out _tempHit);

                    if (_tempHit.collider == null) { }
                    else
                    {
                        _tempObj[0] = _tempHit.collider.gameObject;
                    }

                }
            }

            if (_touch.phase == TouchPhase.Ended)
            {
                //if (!EventSystem.current.IsPointerOverGameObject(_touch.fingerId))
                if (!Managers._gameUi.BlockMachine_Panel.activeSelf)
                {
                    //Debug.Log("None Ui");
                    Ray ray;
                    RaycastHit hit;
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    if (Physics.Raycast(ray, out hit))
                    {
                        Debug.DrawLine(ray.origin, hit.point, Color.red, 1.5f);

                        if (hit.collider == null) { }
                        else
                        {
                            _tempObj[1] = hit.collider.gameObject;
                        }

                        //if (_tempHit.Equals(hit))
                        if (_tempObj[0].Equals(_tempObj[1]))
                        {

                            switch (hit.collider.tag)
                            {
                                case "BlockMachine":
                                    currentStageManager._selectBlockMachine = hit.transform.GetComponent<BlockMachine>();
                                    Managers._gameUi.ChangePanel(1);
                                    Managers._gameUi.BlockMachine_SetColor((int)currentStageManager._selectBlockMachine._spawnBlockType);
                                    break;

                            }
                        }
                    }
                }

            }
        }
        //#endif
    }





    public void CalcMoney(double _value)
    {
        money += (_value * (1d + orderLevel * 0.1d));
        if (money < 0) money = 0;

        Managers._gameUi.Money_Text.text = $"{money:F0}";

        ES3.Save<double>("Money", money);

        CheckMoney();
        CheckScrollUpgradePrice();


    }

    void CheckMoney()
    {
        currentStageManager.CheckMoney();
    }

    void CheckScrollUpgradePrice()
    {
        //currentStageManager.CheckScrollUpgradePrice();
        currentStageManager.ChangeUI();
    }


    public void LoadData()
    {
        stageLevel = ES3.Load<int>("stageLevel", 0);
        money = ES3.Load<double>("money", 0);
        //money = ES3.Load<double>("money", 100000);
        playTime = ES3.Load<int>("playTime", 0);

        orderLevel = ES3.Load<int>("orderLevel", 0);
        orderCount = ES3.Load<int>("orderCount", 0);
        orderCountStack = ES3.Load<int>("orderCountStack", 0);
        ticketCount = ES3.Load<int>("ticketCount", 0);

        //noAds = ES3.Load<bool>("noAds", false);
        allBoost = ES3.Load<bool>("allBoost", false);
        infiniteTicket = ES3.Load<bool>("infiniteTicket", false);

        IsCount = ES3.Load<int>("IsCount", 0);
        RvCount = ES3.Load<int>("RvCount", 0);

        readyDailyTicket = ES3.Load<bool>("readyDailyTicket", true);
        lastGetTicketTime = ES3.Load<DateTime>("lastGetTicketTime", lastGetTicketTime);

        // ===========================

        ProductsCheck();
        UpdateOrderPanel();

    }

    public void ProductsCheck()
    {
        if (NoAds.IsNoAds == true)
        {
            AdsManager.DisableAdType(AdType.Banner | AdType.Interstitial);

            Managers._gameUi.Product_TicketPack.gameObject.SetActive(false);
            //Managers._gameUi.Product_TicketPack.interactable = false;
            //Managers._gameUi.Product_TicketPack.transform.Find("SoldOut_Img").gameObject.SetActive(true);
        }

        if (allBoost == true)
        {
            Managers._gameUi.Product_BoostPack.gameObject.SetActive(false);
            //Managers._gameUi.Product_BoostPack.interactable = false;
            //Managers._gameUi.Product_BoostPack.transform.Find("SoldOut_Img").gameObject.SetActive(true);
        }

        if (infiniteTicket == true)
        {
            Managers._gameUi.Product_InfinitePack.gameObject.SetActive(false);
            //Managers._gameUi.Product_InfinitePack.interactable = false;
            //Managers._gameUi.Product_InfinitePack.transform.Find("SoldOut_Img").gameObject.SetActive(true);
        }
    }



    public void SaveData()
    {
        ES3.Save<int>("stageLevel", stageLevel);
        ES3.Save<double>("money", money);

        ES3.Save<int>("orderLevel", orderLevel);
        ES3.Save<int>("orderCount", orderCount);
    }

    public void OrderComplete()
    {
        orderCount++;
        orderCountStack++;
        if (orderCount == 10)
        {
            orderCount = 0;
            orderLevel++;

            EventTracker.LogEvent("Order", new Dictionary<string, object> { { "Order",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_OrderLevel-{orderLevel}"} });

        }
        ES3.Save<int>("orderLevel", orderLevel);
        ES3.Save<int>("orderCount", orderCount);
        ES3.Save<int>("orderCountStack", orderCountStack);
        UpdateOrderPanel();

        EventTracker.LogEvent("Order", new Dictionary<string, object> { { "Order",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_OrderCountStack-{orderCountStack}"} });


    }



    public void ClearStage()
    {

        EventTracker.LogEvent("Village", new Dictionary<string, object> { { "Village",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_VillageClear-{stageLevel}"}});

        stageLevel++;
        ES3.Save<int>("stageLevel", stageLevel);

        DOTween.Sequence()
                  .AppendCallback(() =>
                  {
                      Managers._gameUi.ChangePanel(3);

                  }).AppendInterval(2f)
                  .AppendCallback(() => Managers._gameUi.PanelOnOff(Managers._gameUi.Unlock_Panel, false))
                  .OnComplete(() => Managers._gameUi.NextStage_Button.gameObject.SetActive(true));


    }

    public void RvCountFunc()
    {
        RvCount++;
        EventTracker.LogEvent("Ads", new Dictionary<string, object> { { "Ads",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}RvCount-{RvCount}"}});


        ES3.Save<int>("RvCount", RvCount);

    }

    public void TicketUpdate(int _count)
    {
        ticketCount += _count;
        if (ticketCount < 0) ticketCount = 0;
        ES3.Save<int>("ticketCount", ticketCount);


        if (ticketCount >= 9999)
        {
            Managers._gameUi.Ticket_Text.text = $"9999";
            //$"∞";
        }
        else
        {
            Managers._gameUi.Ticket_Text.text = $"{ticketCount}";
        }


    }

    public void UpdateOrderPanel()
    {
        Transform orderLevelGroup = Managers._gameUi.Order_Level_Group.transform;

        orderLevelGroup.Find("Order_Level_Text").GetComponent<Text>().text
            = $"Level {orderLevel}";

        orderLevelGroup.Find("Order_Benefit_Text").GetComponent<Text>().text
            = $"{(int)(100 + orderLevel * 10)}%";

        orderLevelGroup.Find("Order_Exp_Text").GetComponent<Text>().text
            = $"{orderCount} / 10";

        orderLevelGroup.Find("Order_Guage_Img").GetComponent<Image>().fillAmount
            = (float)(orderCount * 0.1f);

    }

    public void DailyFreeTicket()
    {
        readyDailyTicket = false;
        ES3.Save<bool>("readyDailyTicket", readyDailyTicket);

        lastGetTicketTime = DateTime.Now;
        ES3.Save<DateTime>("lastGetTicketTime", lastGetTicketTime);

        DailyTicketUpdate();
    }


    public void DailyTicketUpdate()
    {

        if (readyDailyTicket)
        {
            Managers._gameUi.Product_Ticket_1.interactable = true;
            Managers._gameUi.Product_Ticket_1.transform.Find("Free_Img").gameObject.SetActive(true);
            Managers._gameUi.Product_Ticket_1.transform.Find("Time_Img").gameObject.SetActive(false);
        }
        else
        {
            Managers._gameUi.Product_Ticket_1.interactable = false;
            Managers._gameUi.Product_Ticket_1.transform.Find("Free_Img").gameObject.SetActive(false);
            Managers._gameUi.Product_Ticket_1.transform.Find("Time_Img").gameObject.SetActive(true);

            Managers._gameUi.Product_Ticket_1.transform.Find("Time_Img").Find("Time_Text").GetComponent<Text>().text
                = $"{DateTime.Today.AddDays(1).Subtract(DateTime.Now).ToString(@"hh\:mm\:ss")}";

            //Debug.Log(System.DateTime.Today.AddDays(1).Subtract(System.DateTime.Now).ToString(@"hh\:mm\:ss"));

            if (lastGetTicketTime.Day != DateTime.Now.Day)
            {
                readyDailyTicket = true;
                ES3.Save<bool>("readyDailyTicket", readyDailyTicket);
                DailyTicketUpdate();
            }

        }

    }



}
