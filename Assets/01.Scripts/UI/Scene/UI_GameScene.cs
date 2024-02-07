using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
//using System.Drawing;
using MondayOFF;

public class UI_GameScene : UI_Scene
{
    enum Texts
    {
        Money_Text,
        Ticket_Text,
        BlockMachine_Upgrade_Price_Text,
        BlockMachine_Status_Text,
        BlockMachine_UpgradeValue_Text,
    }
    enum Buttons
    {
        Scroll_Button,
        Speed_RV_Button,
        View_Button,
        Cpi_Rail_Button,
        NextStage_Button,
        Order_Button,
        Shop_Button,
        Setting_Button,
        RvDoubleSpawn_Button,
        RvRailSpeedUp_Button,
        RvVehicleSpeedUp_Button,
        ViewChange_Button,
        AddCar_Button,
        BlockMachine_Close_Button,
        BlockMachine_Upgrade_Button,
        Scroll_Close_Button,
        Order_Close_Button,
        Test_PreStage_Button,
        Test_NextStage_Button,
        Product_BoostPack,
        Product_TicketPack,
        Product_InfinitePack,
        Product_Ticket_1,
        Product_Ticket_2,
        Product_Ticket_3,
        Sound_Button,
        Restore_Button,
    }
    enum GameObjects
    {
        BlockCount_Group,
        RV_Panel,
        FactoryBase_Panel,
        BlockMachine_Panel,
        BlockMachine_Color_Buttons_Group,
        Scroll_Panel,
        Scroll_Content,
        MaskImg,
        Unlock_Panel,
        Order_Panel,
        Order_Level_Group,
        Order_Group,
        Mask_Panel,
        Rv_Popup_Panel,
        Shop_Panel,
        AdBreak_Panel,
        Setting_Panel,
    }

    DOTween _camTween;
    // ======================================================

    public GameObject BlockMachine_Panel
        , FactoryBase_Panel
        , BlockMachine_Color_Buttons_Group
        , BlockCount_Group
        , MaskImg
        , Scroll_Panel
        , Scroll_Content
        , Unlock_Panel
        , Order_Panel
        , Order_Group
        , RV_Panel
        , Mask_Panel
        , Rv_Popup_Panel
        , AdBreak_Panel
        , Shop_Panel
        , Setting_Panel
        , Order_Level_Group
        ;
    //Recipe_RawImage;


    public Button
         BlockMachine_Close_Button
        , BlockMachine_Upgrade_Button
        , ViewChange_Button
        , AddCar_Button
        , Scroll_Close_Button
        , Scroll_Button
        , Speed_RV_Button
        , View_Button
        , Cpi_Rail_Button
        , NextStage_Button
        , Order_Close_Button
        , Order_Button
        , Test_PreStage_Button
        , Test_NextStage_Button
        , RvDoubleSpawn_Button,
        RvRailSpeedUp_Button,
        RvVehicleSpeedUp_Button
        , Product_BoostPack,
        Product_TicketPack,
        Product_InfinitePack,
        Product_Ticket_1,
        Product_Ticket_2,
        Product_Ticket_3
        , Shop_Button
        , Setting_Button
        , Sound_Button
        , Restore_Button

        ;

    public Button[] _blockMachineColorButtons = new Button[4];



    public Text
        Money_Text
        , BlockMachine_Upgrade_Price_Text
        , BlockMachine_Status_Text
        , BlockMachine_UpgradeValue_Text
        , Ticket_Text
        ;

    public Text[] _blockCountTexts = new Text[4];




    public Color _color;

    public GameObject[] _scrollUpgContent = new GameObject[4];
    public Button[] _scrollUpgButtons = new Button[4];




    // =======================================================
    private void Awake()
    {
        Bind<UnityEngine.UI.Text>(typeof(Texts));
        Bind<UnityEngine.UI.Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
        ColorUtility.TryParseHtmlString("#4F9BCD", out _color);


        Managers._gameUi = this;


        base.Init();

        // ========= GameObjects
        BlockMachine_Panel = GetObject(GameObjects.BlockMachine_Panel);

        FactoryBase_Panel = GetObject(GameObjects.FactoryBase_Panel);

        BlockMachine_Color_Buttons_Group = GetObject(GameObjects.BlockMachine_Color_Buttons_Group);

        BlockCount_Group = GetObject(GameObjects.BlockCount_Group);
        MaskImg = GetObject(GameObjects.MaskImg);
        Scroll_Panel = GetObject(GameObjects.Scroll_Panel);
        Scroll_Content = GetObject(GameObjects.Scroll_Content);

        for (int i = 0; i < Scroll_Content.transform.childCount; i++)
        {
            _scrollUpgContent[i] = Scroll_Content.transform.GetChild(i).gameObject;
        }


        Unlock_Panel = GetObject(GameObjects.Unlock_Panel);
        Order_Panel = GetObject(GameObjects.Order_Panel);
        Order_Group = GetObject(GameObjects.Order_Group);

        RV_Panel = GetObject(GameObjects.RV_Panel);

        Mask_Panel = GetObject(GameObjects.Mask_Panel);
        Rv_Popup_Panel = GetObject(GameObjects.Rv_Popup_Panel);
        AdBreak_Panel = GetObject(GameObjects.AdBreak_Panel);
        Shop_Panel = GetObject(GameObjects.Shop_Panel);
        Setting_Panel = GetObject(GameObjects.Setting_Panel);
        Order_Level_Group = GetObject(GameObjects.Order_Level_Group);

        // ========= Buttons


        for (int i = 0; i < BlockMachine_Color_Buttons_Group.transform.childCount; i++)
        {
            _blockMachineColorButtons[i] = BlockMachine_Color_Buttons_Group.transform.GetChild(i).GetComponent<Button>();
        }



        BlockMachine_Close_Button = GetButton(Buttons.BlockMachine_Close_Button);
        BlockMachine_Upgrade_Button = GetButton(Buttons.BlockMachine_Upgrade_Button);
        ViewChange_Button = GetButton(Buttons.ViewChange_Button);
        AddCar_Button = GetButton(Buttons.AddCar_Button);

        Scroll_Close_Button = GetButton(Buttons.Scroll_Close_Button);
        Scroll_Button = GetButton(Buttons.Scroll_Button);
        Speed_RV_Button = GetButton(Buttons.Speed_RV_Button);
        Cpi_Rail_Button = GetButton(Buttons.Cpi_Rail_Button);
        for (int i = 0; i < Scroll_Content.transform.childCount; i++)
        {
            _scrollUpgButtons[i] = _scrollUpgContent[i].transform.Find("Upgrade_Button").GetComponent<Button>();
        }

        View_Button = GetButton(Buttons.View_Button);

        NextStage_Button = GetButton(Buttons.NextStage_Button);
        Order_Close_Button = GetButton(Buttons.Order_Close_Button);

        for (int i = 0; i < 3; i++)
        {
            int _num = i;
            Order_Group.transform.GetChild(_num).Find("Claim_Button").GetComponent<Button>().AddButtonEvent(() => Managers.Game.currentStageManager.RewardOrder(_num));

        }

        Order_Button = GetButton(Buttons.Order_Button);
        Test_PreStage_Button = GetButton(Buttons.Test_PreStage_Button);
        Test_NextStage_Button = GetButton(Buttons.Test_NextStage_Button);

        RvDoubleSpawn_Button = GetButton(Buttons.RvDoubleSpawn_Button);
        RvRailSpeedUp_Button = GetButton(Buttons.RvRailSpeedUp_Button);
        RvVehicleSpeedUp_Button = GetButton(Buttons.RvVehicleSpeedUp_Button);

        Shop_Button = GetButton(Buttons.Shop_Button);
        Setting_Button = GetButton(Buttons.Setting_Button);

        Sound_Button = GetButton(Buttons.Sound_Button);
        Restore_Button = GetButton(Buttons.Restore_Button);

        //== button IAP
        Product_BoostPack = GetButton(Buttons.Product_BoostPack);
        Product_TicketPack = GetButton(Buttons.Product_TicketPack);
        Product_InfinitePack = GetButton(Buttons.Product_InfinitePack);
        Product_Ticket_1 = GetButton(Buttons.Product_Ticket_1);
        Product_Ticket_2 = GetButton(Buttons.Product_Ticket_2);
        Product_Ticket_3 = GetButton(Buttons.Product_Ticket_3);


        // ========= Img






        // ========= Text



        Money_Text = GetText(Texts.Money_Text);

        BlockMachine_Upgrade_Price_Text = GetText(Texts.BlockMachine_Upgrade_Price_Text);
        BlockMachine_Status_Text = GetText(Texts.BlockMachine_Status_Text);
        BlockMachine_UpgradeValue_Text = GetText(Texts.BlockMachine_UpgradeValue_Text);



        for (int i = 0; i < 4; i++)
        {
            _blockCountTexts[i] = BlockCount_Group.transform.GetChild(i).GetChild(0).GetComponent<Text>();
        }

        Ticket_Text = GetText(Texts.Ticket_Text);


        // ================ Add Button Listner========================================


        for (int i = 0; i < _blockMachineColorButtons.Length; i++)
        {
            int _num = i;
            _blockMachineColorButtons[i].AddButtonEvent(() =>
            {

                Managers.Game.currentStageManager._selectBlockMachine.SetBlockType(_num);
                BlockMachine_SetColor(_num);

                if (TutorialManager._instance._tutorial_Level == 6)
                {
                    TutorialManager._instance.Tutorial_Complete();
                    BlockMachine_Panel.SetActive(false);
                    Managers.Game.currentStageManager._cams[0].transform.position = TutorialManager._instance._cams[6].transform.position;
                }

            });
        }



        BlockMachine_Close_Button.AddButtonEvent(() => ChangePanel(0));

        BlockMachine_Upgrade_Button.AddButtonEvent(() =>
        {
            Managers.Game.currentStageManager.SelectBlockMachine_Upgrade();
        });

        ViewChange_Button.AddButtonEvent(() =>
        {
            if (Managers.Game.currentStageManager._cams[0].activeSelf) Managers.Game.currentStageManager.ChangeCam(1);
            else Managers.Game.currentStageManager.ChangeCam(0);
        });

        AddCar_Button.AddButtonEvent(() =>
        {
            Managers.Game.currentStageManager.AddVehicle();
        });

        Scroll_Close_Button.AddButtonEvent(() => Scroll_Panel.SetActive(false));


        Scroll_Button.AddButtonEvent(() =>
        {
            if ((Scroll_Panel.activeSelf == false)) ChangePanel(2);
            else PanelOnOff(Scroll_Panel, false);

            if (TutorialManager._instance._tutorial_Level == 1 && Managers.Game.currentStageManager._vehicle_Spawn_Level == 0)
            {
                TutorialManager._instance.Tutorial_Complete();

                Managers.Game.currentStageManager._cams[0].transform.position = TutorialManager._instance._cams[2].transform.position;
                TutorialManager._instance.Tutorial_Img();

            }
        });
        Speed_RV_Button.AddButtonEvent(() => { /* add func */});


        for (int i = 0; i < _scrollUpgButtons.Length; i++)
        {
            int _num = i;
            _scrollUpgButtons[i].AddButtonEvent(() => Managers.Game.currentStageManager.VehicleUpgrade(_num));

            _scrollUpgButtons[i].transform.Find("Rv_Button").GetComponent<Button>().AddButtonEvent(() =>
            {
                if (Managers.Game.infiniteTicket)
                {

                    ScrollUpgButtonFunc(_num, 2);
                }
                else if (Managers.Game.ticketCount > 0)
                {


                    ScrollUpgButtonFunc(_num, 1);
                    Managers.Game.TicketUpdate(-1);


                }
                else
                {
                    AdsManager.ShowRewarded(() =>
                    {

                        ScrollUpgButtonFunc(_num, 0);
                    });

                }

            });
        }

        void ScrollUpgButtonFunc(int _num, int isTicketType = 0) // 0 : not use / 1 : use ticket / 2 : use Infinite
        {
            Debug.Log("ScrollUpgButtonFunc");
            Managers.Game.currentStageManager.VehicleUpgrade(_num, false);

            switch (isTicketType)
            {
                case 0:
                    Managers.Game.RvCountFunc();
                    switch (_num)
                    {
                        case 0:
                            EventTracker.LogEvent("Rv", new Dictionary<string, object> { { "Rv",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{Managers.Game.currentStageLevel}_RvAddVehicle"}});
                            break;

                        case 1:
                            EventTracker.LogEvent("Rv", new Dictionary<string, object> { { "Rv",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{Managers.Game.currentStageLevel}_RvVehicleSpeed"}});
                            break;

                        case 2:
                            EventTracker.LogEvent("Rv", new Dictionary<string, object> { { "Rv",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{Managers.Game.currentStageLevel}_RvVehicleCapacity"}});
                            break;

                        case 3:
                            EventTracker.LogEvent("Rv", new Dictionary<string, object> { { "Rv",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{Managers.Game.currentStageLevel}_RvRailSpeed"}});

                            break;
                    }
                    break;

                case 1:
                    switch (_num)
                    {
                        case 0:

                            EventTracker.LogEvent("Ticket", new Dictionary<string, object> { { "Ticket",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_Use_StageNum-{Managers.Game.currentStageLevel}_AddVehicle"} });


                            break;

                        case 1:
                            EventTracker.LogEvent("Ticket", new Dictionary<string, object> { { "Ticket",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_Use_StageNum-{Managers.Game.currentStageLevel}_RvVehicleSpeed"} });

                            break;

                        case 2:
                            EventTracker.LogEvent("Ticket", new Dictionary<string, object> { { "Ticket",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_Use_StageNum-{Managers.Game.currentStageLevel}_RvVehicleCapacity"} });

                            break;

                        case 3:
                            EventTracker.LogEvent("Ticket", new Dictionary<string, object> { { "Ticket",
        $"{((GameManager.ABType)Managers.Game.isA).ToString()}_Use_StageNum-{Managers.Game.currentStageLevel}_RvRailSpeed"} });


                            break;
                    }
                    break;
            }



        }

        Cpi_Rail_Button.AddButtonEvent(() =>
        {
            if (Managers.Game.currentStageManager._rail_Speed_Level < 10)
            {
                Managers.Game.currentStageManager._rail_Speed_Level++;
                Managers.Game.currentStageManager._railSpeed = 0.5f - (0.05f * Managers.Game.currentStageManager._rail_Speed_Level);
            }
        });

        View_Button.AddButtonEvent(() =>
        {
            Vector3 _pos = Managers.Game.currentStageManager._cams[0].transform.position;


            if (Managers.Game.currentStageManager._cams[0].transform.position.z < -22)
            {


                Managers.Game.currentStageManager._cams[0].transform.DOMoveZ(-7f, 0.5f).SetEase(Ease.Linear);
            }
            else
            {

                Managers.Game.currentStageManager._cams[0].transform.DOMoveZ(-43f, 0.5f).SetEase(Ease.Linear);
            }

        });

        NextStage_Button.AddButtonEvent(() =>
        {

            Managers.Game.ChangeStage(1);
            NextStage_Button.gameObject.SetActive(false);

        });

        Order_Close_Button.AddButtonEvent(() => PanelOnOff(Order_Panel, false));

        Order_Button.AddButtonEvent(() => ChangePanel(4));
        Test_PreStage_Button.AddButtonEvent(() => Managers.Game.ChangeStage(-1));
        Test_NextStage_Button.AddButtonEvent(() => Managers.Game.ChangeStage(1));

        for (int i = 0; i < 3; i++)
        {
            int _num = i;
            Order_Group.transform.GetChild(_num).Find("Wait_Img").Find("RV_Order_Refesh_Button")
                .GetComponent<Button>().AddButtonEvent(() =>
                {
                    if (Managers.Game.infiniteTicket)
                    {
                        Managers.Game.currentStageManager.RV_Order_Refresh(_num);
                    }
                    else if (Managers.Game.ticketCount > 0)
                    {
                        Managers.Game.TicketUpdate(-1);
                        Managers.Game.currentStageManager.RV_Order_Refresh(_num);
                    }
                    else
                    {
                        AdsManager.ShowRewarded(() =>
                        {
                            Managers.Game.currentStageManager.RV_Order_Refresh(_num);
                        });
                    }
                });


        }

        RvDoubleSpawn_Button.AddButtonEvent(() =>
        {
            if (Managers.Game.allBoost)
            {
                Managers.Game.currentStageManager.RV_DoubleSpawn();
            }

            else if (Managers.Game.infiniteTicket)
            {
                Managers.Game.currentStageManager.RV_DoubleSpawn();
            }
            else if (Managers.Game.ticketCount > 0)
            {
                Managers.Game.TicketUpdate(-1);
                Managers.Game.currentStageManager.RV_DoubleSpawn();
            }
            else
            {
                AdsManager.ShowRewarded(() =>
                {
                    Managers.Game.currentStageManager.RV_DoubleSpawn();
                });
            }
        }


        );
        RvRailSpeedUp_Button.AddButtonEvent(() =>
        {
            if (Managers.Game.allBoost)
            {
                Managers.Game.currentStageManager.RV_RailSpeedUp();
            }

            else if (Managers.Game.infiniteTicket)
            {
                Managers.Game.currentStageManager.RV_RailSpeedUp();
            }
            else if (Managers.Game.ticketCount > 0)
            {
                Managers.Game.TicketUpdate(-1);
                Managers.Game.currentStageManager.RV_RailSpeedUp();
            }
            else
            {
                AdsManager.ShowRewarded(() =>
                {
                    Managers.Game.currentStageManager.RV_RailSpeedUp();
                });
            }
        }


        );
        RvVehicleSpeedUp_Button.AddButtonEvent(() =>
        {
            if (Managers.Game.allBoost)
            {
                Managers.Game.currentStageManager.RV_VehicleSpeedUp();
            }

            else if (Managers.Game.infiniteTicket)
            {
                Managers.Game.currentStageManager.RV_VehicleSpeedUp();
            }
            else if (Managers.Game.ticketCount > 0)
            {
                Managers.Game.TicketUpdate(-1);
                Managers.Game.currentStageManager.RV_VehicleSpeedUp();
            }
            else
            {
                AdsManager.ShowRewarded(() =>
                {
                    Managers.Game.currentStageManager.RV_VehicleSpeedUp();
                });
            }
        }


        );



        Rv_Popup_Panel.transform.Find("Free_Claim_Button").GetComponent<Button>()
            .AddButtonEvent(() => Managers.Game.currentStageManager.FreeRvFunc());


        Rv_Popup_Panel.transform.Find("Close_Button").GetComponent<Button>()
            .AddButtonEvent(() => Rv_Popup_Panel.SetActive(false));


        Shop_Button.AddButtonEvent(() =>
        {
            ChangePanel(-1);
            PanelOnOff(Shop_Panel, true);
            Managers.Game.DailyTicketUpdate();
        });
        Setting_Button.AddButtonEvent(() =>
        {
            ChangePanel(-1);
            PanelOnOff(Setting_Panel, true);
        });


        Sound_Button.AddButtonEvent(() =>
        {
            Managers.Game.isSound = !Managers.Game.isSound;
            Sound_Button.transform.GetChild(0).gameObject.SetActive(Managers.Game.isSound);
            Managers.Game.GetComponent<AudioSource>().mute = !Managers.Game.isSound;
        });

        Restore_Button.AddButtonEvent(() =>
        {
            IAPManager.RestorePurchase();
        });


        // add IAP AddListner ====================================================

        Product_BoostPack.AddButtonEvent(() => IAPManager.PurchaseProduct("blockmatchhro_boostpack"));
        Product_TicketPack.AddButtonEvent(() => IAPManager.PurchaseProduct("blockmatchhro_ticket_pack"));
        Product_InfinitePack.AddButtonEvent(() => IAPManager.PurchaseProduct("blockmatchhro_infinite_pack"));
        Product_Ticket_1.AddButtonEvent(() => AdsManager.ShowRewarded(() =>
        {
            Managers.Game.DailyFreeTicket();
            Managers.Game.TicketUpdate(3);
        }));
        Product_Ticket_2.AddButtonEvent(() => IAPManager.PurchaseProduct("blockmatchhro_ticket_2"));
        Product_Ticket_3.AddButtonEvent(() => IAPManager.PurchaseProduct("blockmatchhro_ticket_3"));


    }

    // ====================================================



    private void Start()
    {
        //StartCoroutine(Cor_Update());

    }

    //IEnumerator Cor_Update()
    //{
    //    while (true)
    //    {
    //        yield return null;




    //    }
    //}







    public void ChangePanel(int _num)
    {
        //Debug.Log("Panel OnOff");
        PanelOnOff(FactoryBase_Panel, false);
        PanelOnOff(BlockMachine_Panel, false);
        PanelOnOff(Scroll_Panel, false);
        PanelOnOff(Order_Panel, false);
        PanelOnOff(Setting_Panel, false);
        PanelOnOff(Shop_Panel, false);


        switch (_num)
        {
            case 0:

                //PanelOnOff(FactoryBase_Panel, true, 0);
                break;

            case 1:
                PanelOnOff(BlockMachine_Panel, true);
                //BlockMachine_Upgrade_Button.interactable =
                Managers.Game.currentStageManager._selectBlockMachine.CheckPrice();

                if (TutorialManager._instance._tutorial_Level == 5 && Managers.Game.currentStageManager._selectBlockMachine._machineNum == 0)
                {
                    TutorialManager._instance.Tutorial_Complete();
                    TutorialManager._instance.Tutorial_Img();
                }

                break;

            case 2:
                PanelOnOff(Scroll_Panel, true);
                Managers.Game.currentStageManager.CheckScrollUpgradePrice();
                break;

            case 3:
                PanelOnOff(Unlock_Panel, true);

                break;

            case 4:
                PanelOnOff(Order_Panel, true);
                break;



        }

    }

    public void PanelOnOff(GameObject _obj, bool isOn, float _time = 0.3f)
    {
        if (isOn)
        {
            //Debug.Log("On");
            _obj.SetActive(true);
            _obj.transform.localScale = Vector3.zero;
            _obj.transform.DOScale(Vector3.one, _time).SetEase(Ease.OutBack);

        }
        else
        {
            //Debug.Log("Off");
            //_obj.transform.DOScale(Vector3.zero, _time).SetEase(Ease.InBack).OnComplete(() => _obj.SetActive(false));
            _obj.SetActive(false);
        }


    }



    public void BlockMachine_SetColor(int _num)
    {
        for (int i = 0; i < 4; i++)
        {
            BlockMachine_Color_Buttons_Group.transform
                .GetChild(i).GetChild(0).gameObject.SetActive(false);
            BlockMachine_Color_Buttons_Group.transform
            .GetChild(i).GetComponent<Button>().interactable = true;
        }
        BlockMachine_Color_Buttons_Group.transform
            .GetChild(_num).GetChild(0).gameObject.SetActive(true);
        BlockMachine_Color_Buttons_Group.transform
            .GetChild(_num).GetComponent<Button>().interactable = false;



    }

    public void SetOrderPanel(int _num, StageManager.OrderStruct _newOrder)
    {

        Order_Group.transform.GetChild(_num).Find("Person_Img").GetComponent<Image>().sprite = _newOrder.personSprite;

        Order_Group.transform.GetChild(_num).Find("Order_BG_0").Find("OrderBlock_Img").GetComponent<Image>().sprite =
            _newOrder.blockSprite[0];
        Order_Group.transform.GetChild(_num).Find("Order_BG_0").Find("Order_Count_Text").GetComponent<Text>().text = $"{_newOrder.blockCount[0]}";

        if (_newOrder.orderCount < 2)
        {
            Order_Group.transform.GetChild(_num).Find("Order_BG_1").gameObject.SetActive(false);
        }
        else
        {
            Order_Group.transform.GetChild(_num).Find("Order_BG_1").gameObject.SetActive(true);
            Order_Group.transform.GetChild(_num).Find("Order_BG_1").Find("OrderBlock_Img").GetComponent<Image>().sprite =
            _newOrder.blockSprite[1];
            Order_Group.transform.GetChild(_num).Find("Order_BG_1").Find("Order_Count_Text").GetComponent<Text>().text = $"{_newOrder.blockCount[1]}";
        }

        Order_Group.transform.GetChild(_num).Find("Claim_Button").Find("Reward_Text").GetComponent<Text>().text = $"{_newOrder.rewardCount}";
        //Order_Group.transform.GetChild(_num).Find("Wait_Img").gameObject.SetActive(false);



    }


    public void InitRvPanel()
    {
        for (int i = 0; i < 3; i++)
        {
            RV_Panel.transform.GetChild(i).GetComponent<Button>().interactable = true;
            DOTween.Kill(RV_Panel.transform.GetChild(i).Find("Guage_Group").Find("Guage_Img").GetComponent<Image>()); DOTween.Kill(RV_Panel.transform.GetChild(i).Find("Guage_Group").Find("Time_Text").GetComponent<Text>());
            RV_Panel.transform.GetChild(i).Find("Guage_Group").gameObject.SetActive(false);
            RV_Panel.transform.GetChild(i).Find("Rv_Img").gameObject.SetActive(true);

        }
    }


    public void RvPopupPanelOnOff(int _num, bool isOn = true, float _time = 60f)
    {
        if (isOn)
        {
            Rv_Popup_Panel.SetActive(true);
            //DOTween.Sequence()
            //    .Append(Rv_Popup_Panel.transform.Find("Close_Button").GetChild(0).GetComponent<Text>().DOColor(new Color(1f, 1f, 1f, 0f), 0f))
            //    .AppendInterval(2f)
            //    .Append(Rv_Popup_Panel.transform.Find("Close_Button").GetChild(0).GetComponent<Text>().DOColor(new Color(1f, 1f, 1f, 0.8f), 2f).SetEase(Ease.Linear));




            for (int i = 0; i < 3; i++)
            {
                Rv_Popup_Panel.transform.Find("Img_Group").GetChild(i).gameObject.SetActive(false);
            }
            Rv_Popup_Panel.transform.Find("Img_Group").GetChild(_num).gameObject.SetActive(true);


            switch (_num)
            {
                case 0:
                    Rv_Popup_Panel.transform.Find("Title_Text").GetComponent<Text>().text
                        = $"Machine Boost";
                    Rv_Popup_Panel.transform.Find("Explain_Text").GetComponent<Text>().text
                        = $"Block Spawn X 2 \n {(int)_time} Seconds";
                    break;

                case 1:
                    Rv_Popup_Panel.transform.Find("Title_Text").GetComponent<Text>().text
                        = $"Rail Boost";
                    Rv_Popup_Panel.transform.Find("Explain_Text").GetComponent<Text>().text
                        = $"Rail Speed Up X 2 \n {(int)_time} Seconds";
                    break;

                case 2:
                    Rv_Popup_Panel.transform.Find("Title_Text").GetComponent<Text>().text
                        = $"Truck Boost";
                    Rv_Popup_Panel.transform.Find("Explain_Text").GetComponent<Text>().text
                        = $"Vhicle Speed Up X 2 \n {(int)_time} Seconds";
                    break;
            }


        }
        else
        {
            Rv_Popup_Panel.SetActive(false);
        }

    }




}
