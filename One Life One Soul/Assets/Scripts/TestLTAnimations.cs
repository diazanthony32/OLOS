using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLTAnimations : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Shake();
    }

    void Shake()
    {
        LeanTween.moveLocalX(this.gameObject, 0.005f, 0.03f).setLoopOnce().setDelay(0.03f).setOnComplete(() =>
        {
            LeanTween.moveLocalX(this.gameObject, -0.005f, 0.03f).setLoopOnce().setDelay(0.03f).setOnComplete(() =>
            {
                Shake();
            }
        ); ;
        }
        );
    }
}
