using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Floating : MonoBehaviour
{

    public Text _valueText;


    public void SetInit(float _value, float _deltaY, float _time)
    {
        if (_valueText == null)
            _valueText = transform.GetComponent<Text>();
        //_valueText = transform.Find("ValueText").GetComponent<Text>();



        transform.localScale = Vector3.one;
        transform.localPosition = new Vector3(50f, 10f, 0);

        if (_value > 0)
        {
            Color color;
            ColorUtility.TryParseHtmlString("#09FF00", out color);
            transform.GetComponent<Text>().color = color;
            _valueText.text = $"+{_value}";
            DOTween.Sequence()
            .Append(transform.DOMoveY(transform.position.y + _deltaY, _time).SetEase(Ease.Linear))
            .OnComplete(() => Managers.Pool.Push(transform.GetComponent<Poolable>()));


        }
        else
        {
            Color color;
            ColorUtility.TryParseHtmlString("#FF4E23", out color);
            transform.GetComponent<Text>().color = color;

            _valueText.text = $"{_value}";
            DOTween.Sequence()
            .Append(transform.DOMoveY(transform.position.y - _deltaY, _time).SetEase(Ease.Linear))
            .OnComplete(() => Managers.Pool.Push(transform.GetComponent<Poolable>()));
        }




    }




}
