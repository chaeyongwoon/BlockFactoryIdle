using UnityEngine;

[System.Serializable]
public class DataManager
{
    ///<summary>Manager생산할때 만들어짐</summary>
    public void Init()
    {
        _useHaptic = ES3.Load<bool>("Haptic", true);
        _useSound = ES3.Load<bool>("Sound", true);
    }


    public bool UseHaptic
    {
        get => _useHaptic;
        set
        {
            _useHaptic = value;
            SaveData("Haptic", value);
        }
    }
    [SerializeField]
    private bool _useHaptic;

    public bool UseSound
    {
        get => _useSound;
        set
        {
            _useSound = value;
            SaveData("Sound", value);
            Managers.Sound.BgmOnOff(value);
        }
    }
    [SerializeField]
    private bool _useSound;

    public void SaveData<T>(string key, T data)
    {
        ES3.Save(key, data);
    }

    public T GetData<T>(string key, T _default = default(T))
    {
        return ES3.Load<T>(key, _default);
    }
}