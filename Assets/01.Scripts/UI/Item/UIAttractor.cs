using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Coffee.UIExtensions;

public class UIAttractor : UI_Base
{
    enum GameObjects
    {
        Particle,
        TargetImage,
    }
    enum TextMeshs
    {
        TargetCountText,
    }
    private void Awake()
    {
        Bind<TMPro.TextMeshProUGUI>(typeof(TextMeshs));
        Bind<GameObject>(typeof(GameObjects));

        Init();
    }
    public override void Init()
    {
        uIParticleAttractor = GetObject(GameObjects.TargetImage).GetComponent<UIParticleAttractor>();
        uIParticleAttractor.m_OnAttracted.AddListener(TargetImageSize);
        dOTweenAnimation = GetObject(GameObjects.TargetImage).GetComponent<DOTweenAnimation>();
        int.TryParse(GetTextMesh(TextMeshs.TargetCountText).text, out oriCount);
        ps = GetObject(GameObjects.Particle).GetComponent<ParticleSystem>();
        ps.Stop();
    }
    UIParticleAttractor uIParticleAttractor;
    ParticleSystem ps;
    DOTweenAnimation dOTweenAnimation;
    int oriCount = 0;
    int count = 0;

    public void PlayWithoutText()
    {
        ps.Play();
    }
    public void PlayAddCount(int addCount, float changeTime = 1f)
    {
        count = oriCount + addCount;
        ps.Play();
        GetTextMesh(TextMeshs.TargetCountText).DOCounter(oriCount, count, 0.5f, false).SetEase(Ease.Linear).SetDelay(changeTime);
        oriCount = count;
    }
    public void PlaySetCount(int count, float changeTime = 1f)
    {
        this.count = count;
        ps.Play();
        GetTextMesh(TextMeshs.TargetCountText).DOCounter(oriCount, count, 0.5f, false).SetEase(Ease.Linear).SetDelay(changeTime);
    }

    private void TargetImageSize()
    {
        dOTweenAnimation.DORestart();
    }
}
