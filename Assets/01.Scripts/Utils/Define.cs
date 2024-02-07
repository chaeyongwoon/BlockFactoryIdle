using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum Scene
    {
        Unknown,
        Loading,
        Game,
    }

    public enum Sound
    {
        Bgm,
        Effect,
        MaxCount,
    }

    public enum UIEvent
    {
        Click,
        Drag,
        Up,
        Down,
    }

    public enum Language
    {
        Kor,
        Eng,
    }
    public enum ObjectType
    {
        NotAssigned,
        GameObject,
        TextMesh,
        Image,
        Button,
        Text,
    }

    public static float BOUND = 0.5f;
}
