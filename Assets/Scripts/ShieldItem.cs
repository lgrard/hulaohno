using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldItem : MonoBehaviour
{
    public float dropRate;
    [SerializeField] ParticleSystem p_take;
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController) && other.GetType() == typeof(CapsuleCollider))
        {
            GetComponent<BoxCollider>().enabled = false;
            playerController.i_shield.SetActive(true);
            playerController.shieldActive = true;
            p_take.Play();
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<Animator>().SetTrigger("Take");
            Destroy(gameObject, 0.5f);
        }
    }
}
