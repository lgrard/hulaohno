using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPItem : MonoBehaviour
{
    [SerializeField] int amount;
    [SerializeField] ParticleSystem p_take;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController) && other.GetType() == typeof(CapsuleCollider))
        {
            GetComponent<BoxCollider>().enabled = false;
            playerController.GainHP(amount);
            p_take.Play();
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<Animator>().SetTrigger("Take");
            Destroy(gameObject,0.5f);
        }
    }
}
