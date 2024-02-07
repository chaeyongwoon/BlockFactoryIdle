using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

public class CpiController : MonoBehaviour
{

    public GameObject _fullUi_Canvas;

    // =============================
    //#if UNITY_EDITOR

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Managers.Game.CalcMoney(10000);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            for (int i = 0; i < 4; i++)
            {
                Managers.Game.currentStageManager._blockStorage._blockCountArray[i] += 100;
                Managers.Game.currentStageManager._blockStorage.UpdateBlockCount();
            }
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            Managers.Game.currentStageManager.AddBlockMachine(false);
        }


        if (Input.GetKeyDown(KeyCode.U))
        {
            Managers._gameUi.View_Button.gameObject.SetActive(!Managers._gameUi.View_Button.gameObject.activeSelf);
            Managers._gameUi.Scroll_Button.gameObject.SetActive(Managers._gameUi.View_Button.gameObject.activeSelf);
        }

        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    Managers._gameUi.Cpi_Rail_Button.gameObject.SetActive(!Managers._gameUi.Cpi_Rail_Button.gameObject.activeSelf);
        //}

        if (Input.GetKeyDown(KeyCode.P))
        {
            _fullUi_Canvas.SetActive(!_fullUi_Canvas.activeSelf);
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Managers._gameUi.gameObject.SetActive(!Managers._gameUi.gameObject.activeSelf);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            Time.timeScale += 0.2f;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            Time.timeScale -= 0.2f;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            Time.timeScale = 1f;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.DeleteAll();
            //Managers.Game.ChangeStage(0);
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            Managers.Game.currentStageManager._rail_Speed_Level++;
            if (Managers.Game.currentStageManager._rail_Speed_Level > 10) Managers.Game.currentStageManager._rail_Speed_Level = 10;
            Managers.Game.currentStageManager._railSpeed = 0.5f - (0.05f * Managers.Game.currentStageManager._rail_Speed_Level);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Managers.Game.currentStageManager._rail_Speed_Level--;
            Managers.Game.currentStageManager._railSpeed = 0.5f - (0.05f * Managers.Game.currentStageManager._rail_Speed_Level);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            foreach (BlockMachine _blockmachine in Managers.Game.currentStageManager._blockMachineList)
            {
                _blockmachine._level++;
                _blockmachine._spawnInterval = 6f - 0.5f * _blockmachine._level;
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            foreach (BlockMachine _blockmachine in Managers.Game.currentStageManager._blockMachineList)
            {
                _blockmachine._level--;
                _blockmachine._spawnInterval = 6f - 0.5f * _blockmachine._level;
            }
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Managers.Game.ChangeStage(-1);
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            Managers.Game.ChangeStage(1);
        }


    }

    //#endif



}
