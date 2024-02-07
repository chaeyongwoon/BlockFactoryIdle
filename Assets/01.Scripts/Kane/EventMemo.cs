using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MondayOFF;
using static Cinemachine.DocumentationSortingAttribute;
using static GameManager;


public class EventMemo : MonoBehaviour
{

    GameManager gameManager;
    StageManager stageManager;

    public void Test()
    {
        {
            if (Managers.Game.infiniteTicket)
            {

            }
            else if (Managers.Game.ticketCount > 0)
            {
                Managers.Game.TicketUpdate(-1);

            }
            else
            {
                AdsManager.ShowRewarded(() =>
                {

                });
            }
        }

        //EventTracker.LogCustomEvent("Order", new Dictionary<string, string> { { "Order",
        //$"{((GameManager.ABType)Managers.Game.isA).ToString()}_OrderCountStack-{orderCountStack}"} });

        //EventTracker.LogCustomEvent("Order", new Dictionary<string, string> { { "Order",
        //$"{((GameManager.ABType)Managers.Game.isA).ToString()}_OrderLevel-{orderLevel}"} });


        //EventTracker.LogCustomEvent("IAP", new Dictionary<string, string> { { "IAP",
        //$"{((GameManager.ABType)Managers.Game.isA).ToString()}_BoostPack"} });

        //EventTracker.LogCustomEvent("Rv", new Dictionary<string, string> { { "Rv",
        //$"{((GameManager.ABType)Managers.Game.isA).ToString()}_BoostPack"} });

        //EventTracker.LogCustomEvent("Ticket", new Dictionary<string, string> { { "Ticket",
        //$"{((GameManager.ABType)Managers.Game.isA).ToString()}_~~"} });


    }
    //public void Memo()
    //{
    //    //======= Machine ===============

    //    //  머신 색상변
    //    EventTracker.LogCustomEvent("BlockMachine", new Dictionary<string, string> { { "BlockMachine",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{stageManager._stageLevel}_ChangeColor-{_spawnBlockType.ToString()}"} });


    //    // 머신 업그레이드
    //    EventTracker.LogCustomEvent("BlockMachine", new Dictionary<string, string> { { "BlockMachine",
    //            $" { ((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum{stageManager._stageLevel}_MachineNum-{_machineNum}_Upgrade-{_level}" } });

    //    // 머신 추가 
    //    EventTracker.LogCustomEvent("BlockMachine", new Dictionary<string, string> { { "BlockMachine",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_MachineCount-{_blockMachineCount}" } });


    //    // 레일 속도 업그레이드
    //    EventTracker.LogCustomEvent("BlockMachine", new Dictionary<string, string> { { "BlockMachine", $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_RailUpgrade-{_rail_Speed_Level}" } });


    //    // ======= Vehicle ================

    //    // Vehicle 추가 
    //    EventTracker.LogCustomEvent("Vehicle", new Dictionary<string, string> {{"Vehicle",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_AddVehicle-{_vehicle_Spawn_Level}"}});

    //    // Vehicle 속도 업그레이드 
    //    EventTracker.LogCustomEvent("Vehicle", new Dictionary<string, string> { { "Vehicle",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_SpeedUp-{_vehicle_Speed_Level}"}});

    //    // Vehicle 용량 업그레이
    //    EventTracker.LogCustomEvent("Vehicle", new Dictionary<string, string> { { "Vehicle",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_CapacityUp-{_vehicle_Capacity_Level}"}});

    //    //{ ((GameManager.ABType)Managers.Game.isA).ToString() }


    //    // ======= Village =================

    //    // 빌딩 완성 카운트 
    //    EventTracker.LogCustomEvent("Village", new Dictionary<string, string> { { "Village",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{stageManager._stageLevel}_BuildingCount-{_buildingNum}"}});

    //    // 플레이타임 카운트 
    //    EventTracker.LogCustomEvent("Village", new Dictionary<string, string> { { "Village",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_PlayTime-{playTime}"}});
    //    //$"PlayTime -{playTime}" } });

    //    // 스테이지 클리어
    //    EventTracker.LogCustomEvent("Village", new Dictionary<string, string> { { "Village",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_VillageClear-{stageLevel}"}});
    //    //$"VillageClear -{stageLevel}" } });

    //    // 스테이지 트라이 
    //    EventTracker.LogCustomEvent("Village", new Dictionary<string, string> { { "Village",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_VillageTry-{stageLevel}"}});


    //    // ======= Ads =================

    //    // IS 
    //    EventTracker.LogCustomEvent("Ads", new Dictionary<string, string> { { "Ads",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_IsCount-{IsCount}"}});

    //    // RV
    //    // Timer Rv
    //    EventTracker.LogCustomEvent("Rv", new Dictionary<string, string> { { "Rv",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_RvOrderRefresh"}});

    //    EventTracker.LogCustomEvent("Rv", new Dictionary<string, string> { { "Rv",
    //               $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_RvDoubleSpawn"}});

    //    EventTracker.LogCustomEvent("Rv", new Dictionary<string, string> { { "Rv",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}__RvVehicleSpeedUp"}});

    //    EventTracker.LogCustomEvent("Rv", new Dictionary<string, string> { { "Rv",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{_stageLevel}_RvRailSpeedUp"}});


    //    // Upgrade Rv
    //    EventTracker.LogCustomEvent("Rv", new Dictionary<string, string> { { "Rv",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{Managers.Game.currentStageLevel}_RvAddVehicle"}});

    //    EventTracker.LogCustomEvent("Rv", new Dictionary<string, string> { { "Rv",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{Managers.Game.currentStageLevel}_RvVehicleSpeed"}});

    //    EventTracker.LogCustomEvent("Rv", new Dictionary<string, string> { { "Rv",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{Managers.Game.currentStageLevel}_RvVehicleCapacity"}});

    //    EventTracker.LogCustomEvent("Rv", new Dictionary<string, string> { { "Rv",
    //            $"{((GameManager.ABType)Managers.Game.isA).ToString()}_StageNum-{Managers.Game.currentStageLevel}_RvRailSpeed"}});





    //}



}
