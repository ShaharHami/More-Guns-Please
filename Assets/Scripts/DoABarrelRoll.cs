using System;
using UnityEngine;
using DG.Tweening;

public class DoABarrelRoll : MonoBehaviour
{
    private Vector3 rotation = new Vector3(0, 0,350f);
    public bool isRotating { get; private set; }
    public float duration, upwardMotion, threshold;
    private bool tweenKilled;
    private Tween tween, tween2;
    public void BarrelRoll(Transform target, int dir)
    {
        if (!isRotating)
        {
            tween = target.DORotate(rotation * dir, duration, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine);
            tween2 = target.DOLocalMoveY(upwardMotion, duration/2);
            tween2.OnKill(() => target.DOLocalMoveY(0f, duration / 2));
            tween.OnStart(OnStartTween).OnKill(OnTweenKill);
        }
    }

    private void OnStartTween()
    {
        isRotating = true;
    }
    private void OnTweenKill()
    {
        isRotating = false;
    }
}
