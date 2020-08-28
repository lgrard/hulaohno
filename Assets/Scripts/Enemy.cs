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
    [SerializeField] AudioClip audio_hit;

    [Header("Components")]
    public EnemySpawner spawner;
    private Renderer renderer;
    private Material[] defMat;
    private Animator anim;
    private NavMeshAgent agent;
    private GameManager gameManager;
    private Attack attackScript;
    private AudioSource audio_attack;
    private AudioSource audioSource;

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
    [SerializeField] float despawnTime = 0.3f;
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
    private bool isTargeting = false;

    //public bool isMelee;
    //public bool isLinearCaster;
    //public bool isRadialCaster;
    [Header("Enemy type")]
    public EnemyType currentType;
    public enum EnemyType
    {
        melee,
        linearCaster,
        radialCaster,
    }
    public SpecialType currentSpecialType;
    public enum SpecialType
    {
        none,
        duet,
    }


    [Header("Caster")]
    [SerializeField] int gustNumber = 2;
    [SerializeField] int radialProjectilesNumber = 4;
    [SerializeField] float radius = 1f;
    [SerializeField] float casterRange = 30f;
    [SerializeField] float gustSpacing = 0.5f;
    [SerializeField] GameObject projectiles;
    [SerializeField] GameObject projectilesPoint;
    [SerializeField] float rotationSpeed = 1;

    private void Awake()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        PlaySingle(audioSource.clip);
    }

    // Start is called before the first frame update
    void Start()
    {
        if (currentType == EnemyType.melee)
        {
            attackScript = gameObject.GetComponent<Attack>();
            audio_attack = attackScript.attackPosition.GetComponent<AudioSource>();
        }

        if(currentType == EnemyType.linearCaster ||currentType == EnemyType.radialCaster)
            audio_attack = projectilesPoint.GetComponent<AudioSource>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.stoppingDistance = stoppingDistance;
        anim = gameObject.GetComponentInChildren<Animator>();
        renderer = mesh.GetComponent<Renderer>();
        //defMat = renderer.material;
        defMat = renderer.materials;
        HP = maxHp;

        StartCoroutine(Targeting());
    }

    //Unity Cycles
    #region Cycle
    private void Update()
    {
        if (target == null && !isTargeting || target != null && !target.gameObject.activeSelf && !isTargeting)
        {
            StartCoroutine(Targeting());
        }

        if(target != null)
        {
            if(!isKnockedBack)
            {
                agent.SetDestination(target.position);
                Attack();
            }

            anim.SetFloat("Speed", agent.velocity.magnitude / agent.speed);

            if(agent.remainingDistance < agent.stoppingDistance);
            {
                Vector3 direction = (target.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0,lookRotation.eulerAngles.y,0), Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isKnockedBack)
            agent.velocity = knockBackDir;
    }
    #endregion

    //When the ennemy takes damage
    public void TakeDamage(int damageTaken, int index)
    {
        lastHitIndex = index;

        HP -= damageTaken;
        attackStamp = 0f;

        PlaySingle(audio_hit);

        if (HP <= 0)
            Die();

        else
        {
            anim.SetTrigger("GetHit");
            p_hit.Play();
            StartCoroutine(Blink());
            
            if(!isTargeting)
                StartCoroutine(Targeting());
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

                else if (gameManager.currentEvent.currentType == Events.EventsType.killMelee && currentType == EnemyType.melee)
                    gameManager.currentEvent.amountLeft += 1;

                else if (gameManager.currentEvent.currentType == Events.EventsType.killCasterLinear && currentType == EnemyType.linearCaster)
                    gameManager.currentEvent.amountLeft += 1;

                else if (gameManager.currentEvent.currentType == Events.EventsType.killCasterRadial && currentType == EnemyType.radialCaster)
                    gameManager.currentEvent.amountLeft += 1;
            }
        }

        StartCoroutine(Blink());
        StartCoroutine(gameManager.AddCollectible(collectibleAmount,transform, 2f));
        gameManager.AddItems(transform, itemDropRate,2f);

        if (lastHitIndex == 0)
            gameManager.Scoring1(scoreAmount, true);
        else
            gameManager.Scoring2(scoreAmount, true);

        anim.SetTrigger("Dies");
        p_die.Play();
        gameObject.GetComponent<Collider>().enabled = false;
        Destroy(gameObject, despawnTime);
        this.enabled = false;
    }

    //Blinking while damage method
    private IEnumerator Blink()
    {
        Material[] matList = renderer.materials;

        for (int i = 0; i < matList.Length; i++)
        {
            matList[i] = whiteMat;
            yield return null;
        }
        renderer.materials = matList;
        yield return new WaitForSeconds(blinkingTime);

        for (int i = 0; i < matList.Length; i++)
        {
            matList[i] = defMat[i];
            yield return null;
        }
        renderer.materials = matList;
        yield return new WaitForSeconds(blinkingTime);

        for (int i = 0; i < matList.Length; i++)
        {
            matList[i] = whiteMat;
            yield return null;
        }
        renderer.materials = matList;
        yield return new WaitForSeconds(blinkingTime);

        for (int i = 0; i < matList.Length; i++)
        {
            matList[i] = defMat[i];
            yield return null;
        }
        renderer.materials = matList;
        yield return new WaitForSeconds(blinkingTime);

        //Old blinking
        /*renderer.material = whiteMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = defMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = whiteMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = defMat;
        yield return new WaitForSeconds(blinkingTime);*/
    }

    //Attack method
    private void Attack()
    {
        float distanceFromTarget = Vector3.Distance(target.position, gameObject.transform.position);

        if (attackStamp >= attackStampMax)
        {
            if (currentType == EnemyType.linearCaster && distanceFromTarget <= casterRange && distanceFromTarget <= agent.stoppingDistance)
                StartCoroutine(LinearCasterAttack());

            else if (currentType == EnemyType.radialCaster && distanceFromTarget <= casterRange && distanceFromTarget <= agent.stoppingDistance)
                StartCoroutine(RadialCasterAttack());

            else if (currentType == EnemyType.melee && distanceFromTarget <= agent.stoppingDistance && attackStamp >= attackStampMax)
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
            audio_attack.Play();
            anim.SetTrigger("Attack");
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
            audio_attack.Play();
            anim.SetTrigger("Attack");

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
        audio_attack.Play();

        anim.SetTrigger("Attack");
        yield return new WaitForSeconds(attackScript.delay);

        Collider[] hitEnemies = Physics.OverlapSphere(attackScript.attackPosition.position, attackScript.attackRadius, enemyLayers);

        foreach (Collider hit in hitEnemies)
            hit.GetComponent<PlayerController>().TakeDamage(attackScript.attackDamage);
    }

    //Targetting method
    private IEnumerator Targeting()
    {
        isTargeting = true;

        if (gameManager.player0 != null && gameManager.player1 != null)
        {
            targetList = new GameObject[] { gameManager.player0.gameObject, gameManager.player1.gameObject };

            GameObject currentTarget = targetList[0];

            foreach (GameObject singleTarget in targetList)
            {
                float distanceFromTarget = Vector3.Distance(singleTarget.transform.position, transform.position);

                if (distanceFromTarget < Vector3.Distance(currentTarget.transform.position, transform.position) && singleTarget.activeSelf)
                    currentTarget = singleTarget;

                yield return null;
            }

            target = currentTarget.transform;
            yield return new WaitForEndOfFrame();
        }

        else if (gameManager.player0 != null && gameManager.player1 == null)
            target = gameManager.player0.transform;

        else if (gameManager.player1 != null && gameManager.player0 == null)
            target = gameManager.player1.transform;

        isTargeting = false;
    }

    public IEnumerator KnockBack(float force, Transform origin)
    {
        if (!isKnockedBack)
        {
            float currentAgentSpeed = agent.speed;
            float currentAgentAngular = agent.angularSpeed;

            knockBackDir = (mesh.transform.position - origin.transform.position) * force;
            agent.angularSpeed = 0;
            agent.speed = 0;
            attackStamp = 0f;
            isKnockedBack = true;
        
            yield return new WaitForSeconds(0.25f);

            agent.speed = currentAgentSpeed;
            agent.angularSpeed = currentAgentAngular;
            isKnockedBack = false;
            agent.velocity = Vector3.zero;
        }
    }

    private void PlaySingle(AudioClip clip)
    {
        audioSource.pitch = Random.Range(0.9f, 1.1f);
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, stoppingDistance);
    }
}
