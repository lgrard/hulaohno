using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] ParticleSystem p_starsBlue;
    [SerializeField] ParticleSystem p_starsRed;
    [SerializeField] ParticleSystem p_light;

    PlayerAssignement playerAssignement;
    Animator anim;

    private void Start()
    {
        Time.timeScale = 1f;
        anim = gameObject.GetComponent<Animator>();
        playerAssignement = gameObject.GetComponent<PlayerAssignement>();
    }

    private void Update()
    {
        PlayerAssignementMenu();
    }

    private void PlayerAssignementMenu()
    {
        anim.SetBool("p1Paired", playerAssignement.device0Paired);
        anim.SetBool("p2Paired", playerAssignement.device1Paired);

        if (playerAssignement.device0Paired && !p_starsBlue.isPlaying)
            p_starsBlue.Play();

        if (playerAssignement.device1Paired && !p_starsRed.isPlaying)
            p_starsRed.Play();

        if (playerAssignement.device0Paired && playerAssignement.device1Paired && !p_light.isPlaying)
            p_light.Play();
    }
}
