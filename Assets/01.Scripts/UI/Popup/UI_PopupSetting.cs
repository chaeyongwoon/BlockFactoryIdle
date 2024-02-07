using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PopupSetting : UI_Popup
{
    enum Buttons
    {
        CloseButton,
    }
    enum GameObjects
    {
        BackImage,
        Panel,
        SoundToggle,
        HapticToggle,
    }

    Toggle _soundToggle;
    Toggle _hapticToggle;

    public override void Init()
    {
        base.Init();
        Bind<UnityEngine.UI.Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));

        OpenPop(GetObject(GameObjects.Panel).transform);

        GetObject(GameObjects.BackImage).BindEvent(() => ClosePop(GetObject(GameObjects.Panel).transform));
        GetButton(Buttons.CloseButton).AddButtonEvent(() => ClosePop(GetObject(GameObjects.Panel).transform));

        //Time Sclae 을 0 으로 하고 싶다면 위 2개 라인을 주석 처리하고 아래 3개 라인 주석풀기
        // Time.timeScale = 0;
        // GetObject(GameObjects.BackImage).BindEvent(() => ClosePop(GetObject(GameObjects.Panel).transform, () => Time.timeScale = 1));
        // GetButton(Buttons.CloseButton).AddButtonEvent(() => ClosePop(GetObject(GameObjects.Panel).transform, () => Time.timeScale = 1));


        _soundToggle = GetObject(GameObjects.SoundToggle).GetComponent<Toggle>();
        _hapticToggle = GetObject(GameObjects.HapticToggle).GetComponent<Toggle>();

        _soundToggle.isOn = Managers.Data.UseSound;
        _hapticToggle.isOn = Managers.Data.UseHaptic;

        _soundToggle.onValueChanged.AddListener((isOn) => Managers.Data.UseSound = isOn);
        _hapticToggle.onValueChanged.AddListener((isOn) => Managers.Data.UseHaptic = isOn);
    }
}
