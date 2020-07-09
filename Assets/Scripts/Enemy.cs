using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Mesh and attack point")]
    [SerializeField] GameObject mesh;
    [SerializeField] Transform attackPoint;

    [Header("Components")]
    public EnemySpawner spawner;
    private Renderer renderer;
    private Material defMat;
    private Animator anim;
    private NavMeshAgent agent;
    private GameManager gameManager;

    [Header("Values")]
    [SerializeField] int maxHp = 10;
    [SerializeField] Transform target;
    private GameObject[] targetList;
    [SerializeField] int scoreAmount = 10;
    [SerializeField] int attackDamage = 1;
    private float blinkingTime = 0.05f;
    private int HP;
    private float attackStamp = 0f;
    [SerializeField] float attackStampMax = 3f;
    private float hitStamp = 0f;
    [SerializeField] float shitStampMax = 3f;

    [SerializeField] float attackRadius = 2f;
    [SerializeField] LayerMask enemyLayers;

    [Header("Effects")]
    [SerializeField] ParticleSystem p_hit;
    [SerializeField] ParticleSystem p_die;
    [SerializeField] Material whiteMat;


    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        renderer = mesh.GetComponentInChildren<Renderer>();
        defMat = renderer.material;
        HP = maxHp;

        Targetting();
    }

    private void Update()
    {
        if (target == null)
            Targetting();

        else
        {
            agent.SetDestination(target.position);
            Attack();
        }
    }

    private void LateUpdate()
    {
        if(target != null)
            gameObject.transform.LookAt(target);
    }

    //When the ennemy takes damage
    public void TakeDamage(int damageTaken)
    {
        HP -= damageTaken;
        attackStamp = 0f;

        if (HP <= 0)
            Die();

        else
        {
            anim.SetTrigger("GetHit");
            p_hit.Play();
            StartCoroutine(Blink());
        }
    }

    //When the enemy dies
    private void Die()
    {
        if (spawner != null)
            spawner.enemyRemaining -= 1;

        StartCoroutine(Blink());
        gameManager.Scoring(scoreAmount);
        anim.SetTrigger("Dies");
        p_die.Play();
        gameObject.GetComponent<Collider>().enabled = false;
        Destroy(gameObject,0.3f);
        this.enabled = false;
    }

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

    //Attack method
    private void Attack()
    {
        float distanceFromTarget = Vector3.Distance(target.position, gameObject.transform.position);

        if (distanceFromTarget <= agent.stoppingDistance && attackStamp >= attackStampMax)
        {
            attackStamp = 0f;

            anim.SetTrigger("Attack");
        }

        else if (attackStamp < attackStampMax)
            attackStamp += Time.deltaTime;
    }

    //Hit Method
    private void Hit()
    {
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRadius, enemyLayers);

        foreach (Collider hit in hitEnemies)
            hit.GetComponent<PlayerController>().TakeDamage(attackDamage);
    }

    //Targetting method
    private void Targetting()
    {
        if (gameManager.player0 != null && gameManager.player1 != null)
        {
            targetList = new GameObject[] { gameManager.player0.gameObject, gameManager.player1.gameObject };

            if (targetList.Length > 0)
                target = targetList[Random.Range(0, targetList.Length - 1)].transform;
        }

        else if (gameManager.player0 != null && gameManager.player1 == null)
            target = gameManager.player0.transform;


        else if (gameManager.player1 != null && gameManager.player0 == null)
            target = gameManager.player1.transform;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
