using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField] Material whiteMat;
    private SkinnedMeshRenderer renderer;
    private Material defMat;
    private float blinkingTime = 0.05f;
    private Animator anim;

    private void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        renderer = gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
        defMat = renderer.material;
    }

    public void GetHit() => anim.SetTrigger("GetHit");

    //Blinking while damage method
    private IEnumerator Blink()
    {
        renderer.material = whiteMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = defMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = whiteMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = defMat;
        yield return new WaitForSeconds(blinkingTime);
    }
}
