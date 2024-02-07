using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using MondayOFF;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager _instance;

    //public Vector3[] _imgPoss;
    //public Vector2[] _imgSizes;
    public GameObject[] _cams;


    public bool isFirst = true;
    //public GameObject _maskImg;
    public int _tutorial_Level = 0;

    public bool isComplete = false;


    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {

        isFirst = ES3.Load<bool>("isFirst", true);

        //_maskImg = Managers._gameUi.MaskImg;

        if (isFirst)
        {

            EventTracker.LogEvent("Village", new Dictionary<string, object> { { "Village",
                $"{((GameManager.ABType)Managers.Game.isA).ToString()}_VillageTry-{Managers.Game.currentStageLevel}"}});


            Tutorial_Img();
        }



    }

    [Button]
    public void Tutorial_Img()
    {
        if (isComplete == false)
        {

            //Debug.Log("Tutorial Img :" + _tutorial_Level);

            //Managers._gameUi.ChangePanel(0);

            for (int i = 0; i < Managers._gameUi.Mask_Panel.transform.childCount; i++)
            {
                Managers._gameUi.Mask_Panel.transform.GetChild(i).gameObject.SetActive(false);
            }

            Managers._gameUi.Mask_Panel.transform.GetChild(_tutorial_Level).gameObject.SetActive(true);




            _cams[_tutorial_Level].SetActive(true);

        }
    }

    [Button]
    public void Tutorial_Complete()
    {
        if (isComplete == false)
        {
            //Debug.Log("Tutorial Complete : +" + _tutorial_Level);
            ES3.Save<bool>("isFirst", false);
            //_maskImg.SetActive(false);
            for (int i = 0; i < Managers._gameUi.Mask_Panel.transform.childCount; i++)
            {
                Managers._gameUi.Mask_Panel.transform.GetChild(i).gameObject.SetActive(false);
            }
            _cams[_tutorial_Level].SetActive(false);
            _tutorial_Level++;
        }
    }






}
