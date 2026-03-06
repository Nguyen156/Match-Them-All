using System;
using UnityEngine;

public class Vacuum : Powerup
{
    public static Action OnStarted;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void TriggerPowerupStart()
    {
        OnStarted?.Invoke();
    }

    public void PlayAnim() => anim.Play("Activate");
}
