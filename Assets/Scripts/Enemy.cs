using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [Header("Mesh and attack point")]
    [SerializeField] GameObject mesh;

    [Header("Components")]
    public EnemySpawner spawner;
    private Renderer renderer;
    private Material defMat;
    private Animator anim;
    private NavMeshAgent agent;
    private GameManager gameManager;
    private Attack attackScript;

    [Header("Values")]
    [SerializeField] int maxHp = 10;
    private int HP;
    [SerializeField] int scoreAmount = 10;
    private float blinkingTime = 0.05f;
    private float attackStamp = 0f;
    [SerializeField] float attackStampMax = 3f;
    [SerializeField] int collectibleAmount = 3;
    [SerializeField] float itemDropRate = 0.03f; 
    [SerializeField] float stoppingDistance;
    private Vector3 knockBackDir;
    private Transform target;
    private GameObject[] targetList;
    private int lastHitIndex;

    [SerializeField] LayerMask enemyLayers;

    [Header("Effects")]
    [SerializeField] ParticleSystem p_hit;
    [SerializeField] ParticleSystem p_die;
    [SerializeField] Material whiteMat;

    [Header("States")]
    private bool isKnockedBack;

    [Header("Enemy type")]
    public bool isMelee;
    public bool isLinearCaster;
    public bool isRadialCaster;

    [Header("Caster")]
    [SerializeField] int gustNumber = 2;
    [SerializeField] int radialProjectilesNumber = 4;
    [SerializeField] float radius = 1f;
    [SerializeField] float casterRange = 30f;
    [SerializeField] float gustSpacing = 0.5f;
    [SerializeField] GameObject projectiles;
    [SerializeField] GameObject projectilesPoint;

    // Start is called before the first frame update
    void Start()
    {

        if (isMelee)
            attackScript = gameObject.GetComponent<Attack>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
        anim = gameObject.GetComponentInChildren<Animator>();
        renderer = mesh.GetComponent<Renderer>();
        defMat = renderer.material;
        HP = maxHp;

        Targetting();
    }

    //Unity Cycles
    #region Cycle
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
    #endregion

    //When the ennemy takes damage
    public void TakeDamage(int damageTaken, int index)
    {
        lastHitIndex = index;

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
        {
            spawner.enemyRemaining -= 1;

            if(gameManager.currentEvent != null)
            {
                if(gameManager.currentEvent.currentType == Events.EventsType.killGlobal)
                    gameManager.currentEvent.amountLeft += 1;

                else if (gameManager.currentEvent.currentType == Events.EventsType.killMelee && isMelee)
                    gameManager.currentEvent.amountLeft += 1;

                else if (gameManager.currentEvent.currentType == Events.EventsType.killCasterLinear && isLinearCaster)
                    gameManager.currentEvent.amountLeft += 1;

                else if (gameManager.currentEvent.currentType == Events.EventsType.killCasterRadial && isRadialCaster)
                    gameManager.currentEvent.amountLeft += 1;
            }
        }

        StartCoroutine(Blink());
        StartCoroutine(gameManager.AddCollectible(collectibleAmount,transform));
        gameManager.AddItems(transform, itemDropRate);

        if (lastHitIndex == 0)
            gameManager.Scoring1(scoreAmount);
        else
            gameManager.Scoring2(scoreAmount);

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

        if (attackStamp >= attackStampMax)
        {
            if (isLinearCaster && distanceFromTarget <= casterRange && distanceFromTarget <= agent.stoppingDistance)
                StartCoroutine(LinearCasterAttack());

            else if (isRadialCaster && distanceFromTarget <= casterRange && distanceFromTarget <= agent.stoppingDistance)
                StartCoroutine(RadialCasterAttack());

            else if (isMelee && distanceFromTarget <= agent.stoppingDistance && attackStamp >= attackStampMax)
                StartCoroutine(Hit());
        }

        else if (attackStamp < attackStampMax && distanceFromTarget <= agent.stoppingDistance)
            attackStamp += Time.deltaTime;

        else if (attackStamp < attackStampMax && distanceFromTarget >= agent.stoppingDistance)
            attackStamp = 0;
    }

    private IEnumerator LinearCasterAttack()
    {
        attackStamp = 0f;

        for (int i = 0; i < gustNumber; i++)
        {
            GameObject projectileInstance = Instantiate(projectiles, projectilesPoint.transform.position, Quaternion.identity);
            projectileInstance.GetComponent<Projectiles>().direction = new Vector3(transform.forward.x,0,transform.forward.z);
            yield return new WaitForSeconds(gustSpacing);
        }  
    }

    private IEnumerator RadialCasterAttack()
    {
        attackStamp = 0f;

        float angleStep = 360 / radialProjectilesNumber;
        float angle = 0;

        for (int i = 0; i < gustNumber; i++)
        {
            for (int u = 0; u <= radialProjectilesNumber - 1; u++)
            {
                float projectileDirXposition = projectilesPoint.transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180 * radius);
                float projectileDirZposition = projectilesPoint.transform.position.z + Mathf.Cos((angle * Mathf.PI) / 180 * radius);
                Vector3 projectileDir = new Vector3(projectileDirXposition, projectilesPoint.transform.position.y, projectileDirZposition);
                Vector3 projectileMoveDirection = (projectileDir - projectilesPoint.transform.position).normalized; 
                GameObject projectileInstance = Instantiate(projectiles, projectileDir, Quaternion.identity);
                projectileInstance.GetComponent<Projectiles>().direction = projectileMoveDirection;
                angle += angleStep;

                yield return null;
            }

            yield return new WaitForSeconds(gustSpacing);
        }
    }

    //Hit Method
    private IEnumerator Hit()
    {
        attackStamp = 0f;

        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackScript.delay);

        Collider[] hitEnemies = Physics.OverlapSphere(attackScript.attackPosition.position, attackScript.attackRadius, enemyLayers);

        foreach (Collider hit in hitEnemies)
            hit.GetComponent<PlayerController>().TakeDamage(attackScript.attackDamage);
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
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}
