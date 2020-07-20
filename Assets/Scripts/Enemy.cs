using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
    [SerializeField] int collectibleAmount = 3;
    private Vector3 knockBackDir;

    [SerializeField] float attackRadius = 2f;
    [SerializeField] LayerMask enemyLayers;

    [Header("Effects")]
    [SerializeField] ParticleSystem p_hit;
    [SerializeField] ParticleSystem p_die;
    [SerializeField] Material whiteMat;

    [Header("States")]
    private bool isKnockedBack;

    [Header("Caster")]
    public bool isCaster;
    [SerializeField] float projectilesSpeed = 1f;
    [SerializeField] float casterAttackStampMax = 8f;
    [SerializeField] float casterRange = 30f;
    [SerializeField] GameObject projectiles;
    [SerializeField] GameObject projectilesPoint;

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

        else if(!isKnockedBack)
        {
            agent.SetDestination(target.position);
            Attack();
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack)
            agent.velocity = knockBackDir;
    }

    private void LateUpdate()
    {
        if(target != null && !isKnockedBack)
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
        StartCoroutine(gameManager.AddCollectible(collectibleAmount,transform));

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

        if (distanceFromTarget <= casterRange && distanceFromTarget >= agent.stoppingDistance && attackStamp >= casterAttackStampMax)
        {
            attackStamp = 0f;
            casterAttack();

            //anim.SetTrigger("Attack");
        }

        else if (distanceFromTarget <= agent.stoppingDistance && attackStamp >= attackStampMax)
        {
            attackStamp = 0f;

            anim.SetTrigger("Attack");
        }

        else if (attackStamp < attackStampMax)
            attackStamp += Time.deltaTime;
    }

    void casterAttack()
    {
        GameObject Projectiles = Instantiate(projectiles, projectilesPoint.transform.position, Quaternion.identity);
        Rigidbody rbProjectiles = Projectiles.GetComponent<Rigidbody>();
        
        rbProjectiles.velocity = transform.forward * Time.deltaTime * projectilesSpeed;
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

    public IEnumerator KnockBack(float force)
    {
        knockBackDir = -mesh.transform.forward * force;
        agent.angularSpeed = 0;
        agent.speed = 0;
        attackStamp = 0f;
        isKnockedBack = true;

        yield return new WaitForSeconds(0.25f);

        isKnockedBack = false;
        agent.velocity = Vector3.zero;
        agent.speed = 1.5f;
        agent.angularSpeed = 120;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }

}
