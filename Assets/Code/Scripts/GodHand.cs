using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GodHand : MonoBehaviour
{
    [SerializeField] private Animator handAnimator;

    public void OnPackageCollected()
    {
        handAnimator.SetTrigger("PackageTrigger");
    }

    public void OnPlayerApproach()
    {
        handAnimator.SetTrigger("OpenTrigger");
    }
}
