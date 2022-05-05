using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Feedbacks
{
    public static void FreezeGame(float timeValue = 0.3f, float duration = 1f)
    {
        Time.timeScale = timeValue;
        DOVirtual.Float(0, 1, duration, Freeze).SetUpdate(true);
    }

    public static void SlowMotion(float timeValue = 0.3f, float duration = 1f)
    {
        DOVirtual.Float(timeValue, 1, duration, Slow).SetUpdate(true);
    }

    static void Freeze(float f)
    {
        if (f > 0.9f)
            Time.timeScale = 1;
    }

    static void Slow(float f)
    {
        Time.timeScale = f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}
