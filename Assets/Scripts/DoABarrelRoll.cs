using System;
using UnityEngine;
using DG.Tweening;

public class DoABarrelRoll : MonoBehaviour
{
    private Vector3 rotation = new Vector3(0, 0, 350f);
    public bool isRotating { get; private set; }
    public float duration, upwardMotion, threshold;
    public Transform[] rotateTargets, liftTargets;
    private bool tweenKilled;
    private Tween tween, tween2;

    public void BarrelRoll(Transform target, int dir)
    {
        if (!isRotating)
        {
            foreach (var t in rotateTargets)
            {
                tween = t.DORotate(rotation * dir, duration, RotateMode.LocalAxisAdd).SetEase(Ease.OutSine);
                tween.OnStart(OnStartTween).OnKill(OnTweenKill);
            }

            foreach (var t in liftTargets)
            {
                tween2 = t.DOLocalMoveY(upwardMotion, duration / 2);
                tween2.OnKill(() => t.DOLocalMoveY(0f, duration / 2));
            }
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