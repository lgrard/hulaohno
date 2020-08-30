using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    [Header("Player Index")]
    public int playerIndex = 0;

    [Header("Values")]
    public int maxHp = 5;
    public int HP;
    [SerializeField] float jumpHeight = 10f;
    [SerializeField] float speed = 10f;
    [SerializeField] float normalSpeed = 10f;
    [SerializeField] float whileChargingSpeed = 5f;
    [Tooltip("Controls the amount of air control (value between 0 and 1)")]
    [SerializeField] float airControlAmount = 0.5f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float dashAmount = 30;
    [SerializeField] float dashPush = 50;
    [SerializeField] float dashDuration = 0.2f;
    [SerializeField] float dashCooldown = 0.5f;
    private float dashStamp;
    private Vector2 input;
    private float blinkingTime = 0.05f;
    private float invincibilityTime = 1f; 
    [SerializeField] float gravityAmount = 1.5f;

    [Header("Attack Values")]
    [SerializeField] LayerMask enemyLayers;
    private int attackDepth;
    [SerializeField] int attackDepthMax = 3;
    [SerializeField] float attackTimerMax = 0.15f;
    private float attackTimeStamp = 0f;
    [SerializeField] float attackStepAmount = 5f;
    private float specialStamp = 0;
    [SerializeField] float specialStampMax = 1f;
    public float specialStampRatio;

    [SerializeField] GameObject attackManager;
    private Attack[] attackArray;

    [Header("Object")]
    public Transform playerSpawnPoint;
    [SerializeField] GameObject mesh;
    [SerializeField] EffectManager effectManager;
    [SerializeField] InputActionAsset inputAction;
    [SerializeField] Material whiteMat;

    [Header("Audio")]
    [SerializeField] AudioSource audioStep;
    [SerializeField] AudioClip woodStep;
    [SerializeField] AudioClip rockStep;
    [SerializeField] AudioClip sandStep;
    [SerializeField] AudioClip audio_specialCharge;
    [SerializeField] AudioClip audio_special;

    private PhysicMaterial noFrictionMat;
    private Material[] defMat;
    private SkinnedMeshRenderer renderer;
    private Transform camContainer;
    private AudioSource audioSource;
    private GameManager gameManager;
    private List <GameObject> targets = new List<GameObject>();
    private GameObject currenttarget;
    private SphereCollider dashCollider;
    private CapsuleCollider playerCollider;

    [Header("States")]
    public bool isGrounded;
    public bool isAttacking;
    [SerializeField] bool rumbleActive = false;
    bool isRumbling = false;
    public bool isDashing = false;
    bool canDash = true;
    bool isInvincible = false;
    bool isChargingSpecial = false;

    [Header("Item and item states")]
    public GameObject i_shield;
    public bool shieldActive = false;

    public PlayerInput playerInput;
    Animator meshAnim;
    Rigidbody rb;
    float groundCheckDistance = 0.5f;
    float rotationSmoothingAmount = 0.75f;
    [SerializeField] bool somethingForward;


    //Initialize
    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (playerIndex == 0)
            gameManager.player0 = this;
        else
            gameManager.player1 = this;
    }
    private void OnEnable()
    {
        Spawn();
    }
    void Start()
    {
        noFrictionMat = new PhysicMaterial("noFrictionMat");
        noFrictionMat.dynamicFriction = 0;
        noFrictionMat.staticFriction = 0;
        noFrictionMat.bounciness = 0;
        noFrictionMat.frictionCombine = PhysicMaterialCombine.Minimum;

        airControlAmount = Mathf.Clamp(airControlAmount, 0f, 1f);

        dashCollider = gameObject.GetComponent<SphereCollider>();
        playerCollider = gameObject.GetComponent<CapsuleCollider>();
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        defMat = renderer.materials;
        playerInput = GetComponent<PlayerInput>();

        if (playerInput.actions == null)
            playerInput.actions = inputAction;

        HP = maxHp;
        rb = gameObject.GetComponent<Rigidbody>();
        meshAnim = mesh.GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();

        camContainer = gameManager.camContainer;

        attackArray = attackManager.GetComponents<Attack>();
    }

    //Unity Cycles
    void Update()
    {
        HP = Mathf.Clamp(HP, 0, maxHp);

        GroundCheck();
        AttackState();

        if (isChargingSpecial)
            specialStampRatio = (specialStamp / specialStampMax);
        else
            specialStampRatio = 1;

        if (!isGrounded && isChargingSpecial)
        {
            isAttacking = true;
            attackTimeStamp = attackTimerMax;
            effectManager.p_spinCharge.Stop();
            effectManager.p_spinCharged.Stop();
            isChargingSpecial = false;
        }
    }
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnMovement(InputValue value) => input = value.Get<Vector2>();

    //Movement method
    private void HandleMovement()
    {
        somethingForward = Physics.Raycast(new Ray(gameObject.transform.position + new Vector3(0, 0.5f, 0), mesh.transform.forward), 3, groundLayer);

        if (!isAttacking && !isDashing)
        {            
            #region variables
            Vector3 camForward = camContainer.transform.forward;
            Vector3 camRight = camContainer.transform.right;

            Vector3 DesiredPosition = camForward * input.y + camRight * input.x;
            #endregion

            //Move player's RigidBody
            if (isGrounded && !somethingForward)
            {
                playerCollider.material = null;
                rb.velocity = new Vector3(DesiredPosition.x * speed, rb.velocity.y, DesiredPosition.z * speed);
            }

            else if (!isGrounded || isGrounded && somethingForward)
            {
                playerCollider.material = noFrictionMat;

                float gravity = 0f;
                gravity = rb.velocity.y - Time.deltaTime * gravityAmount;

                rb.velocity = new Vector3(DesiredPosition.x * speed * airControlAmount, gravity, DesiredPosition.z * speed * airControlAmount);
            }

            //Rotate player's Mesh
            if (DesiredPosition != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(DesiredPosition.x, 0, DesiredPosition.z));
                mesh.transform.rotation = Quaternion.Slerp(desiredRotation, mesh.transform.rotation, rotationSmoothingAmount);
            }

            //Set player's Animator speed
            meshAnim.SetFloat("Speed", rb.velocity.magnitude);
        }

        else
            playerCollider.material = null;

        //Set running particles
        if (isGrounded && rb.velocity.magnitude > 2 && !effectManager.p_run.isPlaying)
            effectManager.p_run.Play();

        if(!isGrounded | rb.velocity.magnitude <= 2 && effectManager.p_run.isPlaying)
            effectManager.p_run.Stop();

        //Set step sounds
        if (!isDashing && isGrounded && !audioStep.isPlaying && rb.velocity.magnitude > 2)
        {
            audioStep.Play();
            audioStep.pitch = (rb.velocity.magnitude / speed)*1.5f;
        }

        if (isDashing | !isGrounded | rb.velocity.magnitude <= 2 && audioStep.isPlaying)
            audioStep.Stop();
    }

    //Jump Method
    private void OnJump()
    {
        if(isGrounded && !isAttacking && !isDashing && !gameManager.isPaused && !gameManager.isEnded)
        {
            Vector3 upDir = new Vector3(0, jumpHeight, 0);
            rb.AddForce(upDir);
            effectManager.p_jump.Play();
            meshAnim.SetTrigger("Jump");
        }
    }

    //Check if the player is Grounded
    private void GroundCheck()
    {
        Ray groundRay = new Ray(gameObject.transform.position, gameObject.transform.up * -1);
        isGrounded = Physics.Raycast(groundRay, out RaycastHit hitInfo, groundCheckDistance, groundLayer);

        if (isGrounded)
        {
            if (hitInfo.collider.tag.Equals("wood") && audioStep.clip != woodStep)
            {
                audioStep.volume = 0.4f;
                audioStep.clip = woodStep;
            }

            else if (hitInfo.collider.tag.Equals("rock") && audioStep.clip != rockStep)
            {
                audioStep.volume = 0.4f;
                audioStep.clip = rockStep;
            }

            else if (hitInfo.collider.tag.Equals("sand") && audioStep.clip != sandStep)
            {
                audioStep.volume = 1;
                audioStep.clip = sandStep;
            }
        }

        meshAnim.SetBool("Grounded", isGrounded);
    }

    public void TakeDamage(int damageTaken)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            StartCoroutine(Invincibility());

            if (shieldActive)
            {
                i_shield.SetActive(false);
                shieldActive = false;
            }

            else
            {
                if (playerIndex == 0)
                    gameManager.TakeDamage1();
                else
                    gameManager.TakeDamage2();

                StartCoroutine(camContainer.GetComponent<CameraEffects>().Shake(0.1f, 0.06f));
                if(rumbleActive)
                    StartCoroutine(Rumble(2, 2, 0.1f));

                HP -= damageTaken;

                if (HP <= 0)
                    Die();

                else
                {
                    meshAnim.SetTrigger("GetHit");
                    effectManager.p_hit.Play();
                    StartCoroutine(Blink());
                }
            }
        }
    }

    public void GainHP(int hpGained)
    {
        if (playerIndex == 0)
            gameManager.GainHP1();
        else
            gameManager.GainHP2();

        HP += hpGained;
    }

    //Death method
    private void Die()
    {
        if (playerIndex == 0)
            gameManager.Respawn1();

        else
            gameManager.Respawn2();

        meshAnim.SetTrigger("Dies");
        effectManager.p_die.Play();
        gameObject.SetActive(false);
    }

    private IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(invincibilityTime);
        playerCollider.enabled = false;
        playerCollider.enabled = true;
        isInvincible = false;
    }

    //Dash input method
    private void OnDash()
    {
        if(canDash && !gameManager.isPaused && !gameManager.isEnded)
            StartCoroutine(HandleDash());

        if(gameManager.isPaused)
            gameManager.PauseGame();
    }
    
    //Dash handling method
    private IEnumerator HandleDash()
    {
        canDash = false;

        if (isChargingSpecial)
        {
            isChargingSpecial = false;
            effectManager.p_spinCharge.Stop();
            effectManager.p_spinCharged.Stop();
        }

        isDashing = true;
        isInvincible = true;
        meshAnim.SetBool("Dash", true);

        dashCollider.enabled = false;
        dashCollider.enabled = true;

        effectManager.t_dashTrail.enabled = true;
        effectManager.p_dash.Play();
        AudioSinglePlay(effectManager.audio_dash, 0.05f);
        dashStamp = 0;

        while (isDashing && dashStamp < dashDuration)
        {
            Vector3 direction = mesh.transform.forward;
            rb.velocity = direction * dashAmount;
                
            dashStamp += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        effectManager.t_dashTrail.enabled = false;
        isInvincible = false;
        isDashing = false;
        meshAnim.SetBool("Dash", false);

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    //Attack method
    private void OnPunch()
    {
        var progressState = meshAnim.GetCurrentAnimatorStateInfo(1).normalizedTime;

        if (isGrounded && !isDashing)
        {
            attackTimeStamp = attackTimerMax;

            if (isAttacking && progressState > 0.8 || !isAttacking)
            {
                if (attackDepth <= attackDepthMax)
                    attackDepth += 1;

                else
                    attackDepth = 1;


                if (!isAttacking)
                {
                    isAttacking = true;
                    rb.velocity = mesh.transform.forward * attackStepAmount;
                    //StartCoroutine(Targetting());
                }

                meshAnim.SetTrigger("Punch");

                StartCoroutine(Attack(attackArray[attackDepth - 1]));
            }
        }
    }

    //Special method
    private void OnSpecial()
    {
        if (isGrounded && specialStamp <= 0 && isChargingSpecial)
        {
            meshAnim.SetTrigger("Spin");
            isAttacking = true;
            attackTimeStamp = attackTimerMax;
            AudioSinglePlay(audio_special, 0.05f);
            StartCoroutine(Attack(attackArray[4]));
        }
        effectManager.p_spinCharge.Stop();
        effectManager.p_spinCharged.Stop();
        isChargingSpecial = false;
    }

    private void OnPause() => gameManager.PauseGame();

    //Reference attack method
    private IEnumerator Attack(Attack attack)
    {
        yield return new WaitForSeconds(attack.delay);

        if (Physics.CheckSphere(attack.attackPosition.position, attack.attackRadius, enemyLayers))
        {
            AudioSinglePlay(effectManager.audio_punch, 0.05f);
            StartCoroutine(camContainer.GetComponent<CameraEffects>().Hitstop(0.03f));
            StartCoroutine(camContainer.GetComponent<CameraEffects>().Shake(0.1f, 0.02f));

            if (rumbleActive)
                StartCoroutine(Rumble(0.5f, 1, 0.1f));
        }

        Collider[] hitEnemies = Physics.OverlapSphere(attack.attackPosition.position, attack.attackRadius, enemyLayers);

        foreach (Collider hit in hitEnemies)
        {
            if (hit.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(attack.attackDamage, playerIndex);
                if (attack == attackArray[4])
                    StartCoroutine(enemy.KnockBack(dashPush,transform));
            }

            if (hit.TryGetComponent(out Destructible destructible))
                destructible.OpenChest(attack.attackDamage);

            ParticleSystem hitImpact = Instantiate(effectManager.p_impact);
            hitImpact.transform.position = new Vector3(hit.transform.position.x, effectManager.p_impact.transform.position.y, hit.transform.position.z);
            hitImpact.Play();
            Destroy(hitImpact.gameObject, hitImpact.main.duration);

            //Old way to play particles
            //effectManager.p_impact.transform.position = new Vector3(hit.transform.position.x, effectManager.p_impact.transform.position.y,hit.transform.position.z);
            //effectManager.p_impact.Play();
        }
    }

    //Set up effect on charge state
    private void OnSpecialCharge()
    {
        if (isGrounded)
        {
            AudioSinglePlay(audio_specialCharge, 0.05f);
            effectManager.p_spinCharge.Play();
            specialStamp = specialStampMax;
            isChargingSpecial = true;
        }
    }

    //Set up the attack states
    private void AttackState()
    {
        meshAnim.SetBool("Attacking", isAttacking);
        meshAnim.SetInteger("Depth", attackDepth);

        if (isGrounded && specialStamp <= 0 && isChargingSpecial)
        {
            speed = whileChargingSpeed;
            effectManager.p_spinCharge.Stop();
            effectManager.p_spinCharged.Play();
            speed = whileChargingSpeed;
        }

        else
            speed = normalSpeed;

        if (isChargingSpecial && specialStamp > 0)
            specialStamp -= Time.deltaTime;

        if (attackTimeStamp > 0f && isAttacking)
            attackTimeStamp -= Time.deltaTime;

        else
        {
            isAttacking = false;
            attackDepth = 0;
        }
    }

    //Audio play method
    private void AudioSinglePlay(AudioClip clipToPlay, float pitchVariation)
    {
        audioSource.clip = clipToPlay;
        audioSource.pitch = Random.Range(1 - pitchVariation, 1 + pitchVariation);
        audioSource.Play();
    }

    //Rumble management
    private IEnumerator Rumble(float low, float high, float duration)
    {
        InputDevice playerDevice = playerInput.devices[playerIndex];
        
        if (!isRumbling)
        {
            isRumbling = true;
            var gamePad = (Gamepad)InputSystem.GetDeviceById(playerDevice.deviceId);
            gamePad.SetMotorSpeeds(low, high);
            yield return new WaitForSeconds(duration);
            gamePad.SetMotorSpeeds(0, 0);
            isRumbling = false;

        }
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


        //Old blinking method
        /*renderer.material = whiteMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = defMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = whiteMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = defMat;
        yield return new WaitForSeconds(blinkingTime);*/
    }

    //Targetting system
    private void OnTriggerEnter(Collider other)
    {
        if (isDashing)
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy != null)
                StartCoroutine(enemy.KnockBack(dashPush,transform));
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            targets.Add(other.gameObject);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            targets.Remove(other.gameObject);
    }

    private IEnumerator Targetting()
    {
        if (targets.Count > 0)
        {
            currenttarget = targets[0];
            
            if (currenttarget != null)
            {
                foreach(GameObject target in targets)
                {
                    if (currenttarget != null && Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(currenttarget.transform.position, transform.position))
                        currenttarget = target;

                    yield return null;
                }

                Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(currenttarget.transform.position.x, 0, currenttarget.transform.position.z));
                mesh.transform.LookAt(currenttarget.transform);
            }
        }
    }

    public void Spawn()
    {
        gameObject.transform.SetParent(null);
        isDashing = false;
        canDash = true;
        isInvincible = false;
        shieldActive = false;
        isAttacking = false;
        attackTimeStamp = 0f;

        input = Vector2.zero;
        effectManager.p_spawn.Play();
        HP = maxHp;

        if (playerIndex == 0)
        {
            if(gameManager.player1 == null || !gameManager.player1.gameObject.activeSelf)
                transform.position = gameManager.currentProgressionCp;
            
            else if(gameManager.player1 != null || gameManager.player1.gameObject.activeSelf)
                transform.position = gameManager.player1.playerSpawnPoint.position;
        }

        else if (playerIndex == 1)
        {
            if (gameManager.player0 == null || !gameManager.player0.gameObject.activeSelf)
                transform.position = gameManager.currentProgressionCp;

            else if (gameManager.player0 != null || gameManager.player0.gameObject.activeSelf)
                transform.position = gameManager.player0.playerSpawnPoint.position;
        }
    }


    //Draw various things on Gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(gameObject.transform.position, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y - groundCheckDistance, gameObject.transform.position.z));
    }
}
