using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public ParticleSystem p_spawn;
    public ParticleSystem p_land;
    public ParticleSystem p_jump;
    public ParticleSystem p_run;
    public ParticleSystem p_hit;
    public ParticleSystem p_die;
    public ParticleSystem p_impact;
    public TrailRenderer t_dashTrail;
    public ParticleSystem p_dash;
    public ParticleSystem p_spinCharge;
    public ParticleSystem p_spinCharged;
    public AudioClip audio_dash;
    public AudioClip audio_punch;

    int groundLayer;

    //Initialize
    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }

    private void OnTriggerEnter(Collider other)
    {
        p_land.Play();
    }
}
