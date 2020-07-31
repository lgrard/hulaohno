using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    private enum ChestType
    {
        destructible,
        collectibleChest,
        ItemChest,
        SpecificChest,
    }

    [SerializeField] ParticleSystem p_opens;
    [SerializeField] float timeToDespawn = 2f;

    [Header("HP values and dispertion rate")]
    [SerializeField] int maxHp;
    [SerializeField] float dispersionRate = 2f;
    private int HP;

    [Header("Current chest type")]
    [SerializeField] ChestType currentType;
    [SerializeField] bool getsDestroyed = true;

    [Header("Colectible options")]
    [SerializeField] int collectibleAmount;

    [Header("Specific options")]
    [SerializeField] GameObject[] itemToAdd;

    private GameManager gameManager;
    private Animator anim;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        anim = gameObject.GetComponent<Animator>();
        HP = maxHp;
    }

    public void OpenChest(int damage)
    {
        if (HP <= 0)
        {
            switch (currentType)
            {
                case ChestType.destructible:
                    DestructibleItem();
                    break;

                case ChestType.collectibleChest:
                    CollectibleChest();
                    break;

                case ChestType.ItemChest:
                    ItemChest();
                    break;

                case ChestType.SpecificChest:
                    SpecificChest();
                    break;
            }

            if (getsDestroyed)
            {
                Destroy(gameObject, timeToDespawn);
            }
            
            gameObject.GetComponent<BoxCollider>().enabled = false;

            audioSource.Play();
            anim.SetTrigger("Opens");
            p_opens.Play();
        }

        else
        {
            HP -= damage;
            audioSource.Play();
            anim.SetTrigger("TakeDamage");
        }
    }

    private void CollectibleChest()
    {
        StartCoroutine(gameManager.AddCollectible(collectibleAmount, transform, dispersionRate));
    }

    private void ItemChest()
    {
        gameManager.AddItems(transform,1, dispersionRate);
    }

    private void SpecificChest()
    {
        StartCoroutine(gameManager.AddSpecificItems(transform, itemToAdd, dispersionRate));
    }

    private void DestructibleItem()
    {
        //put destructible code here
    }
}
