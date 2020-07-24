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

    [Header("Attack Values")]
    [SerializeField] LayerMask enemyLayers;
    private int attackDepth;
    [SerializeField] int attackDepthMax = 3;
    [SerializeField] float attackTimerMax = 0.15f;
    private float attackTimeStamp = 0f;
    [SerializeField] float attackStepAmount = 5f;

    [SerializeField] GameObject attackManager;
    private Attack[] attackArray;

    [Header("Object")]
    [SerializeField] GameObject mesh;
    [SerializeField] EffectManager effectManager;
    [SerializeField] InputActionAsset inputAction;
    [SerializeField] Material whiteMat;
    private Material defMat;
    private SkinnedMeshRenderer renderer;
    private Transform camContainer;
    private AudioSource audioSource;
    private GameManager gameManager;
    private List <GameObject> targets = new List<GameObject>();
    private GameObject currenttarget;

    [Header("States")]
    public bool isGrounded;
    public bool isAttacking;
    [SerializeField] bool rumbleActive = false;
    bool isRumbling = false;
    bool isDashing = false;
    bool canDash = true;
    bool isInvincible = false;

    PlayerInput playerInput;
    Animator meshAnim;
    Rigidbody rb;
    float groundCheckDistance = 0.5f;
    float rotationSmoothingAmount = 0.75f;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (playerIndex == 0)
            gameManager.player0 = this;
        else
            gameManager.player1 = this;
    }

    //Initialize
    void Start()
    {
        airControlAmount = Mathf.Clamp(airControlAmount, 0f, 1f);

        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        defMat = renderer.material;
        playerInput = GetComponent<PlayerInput>();
        playerIndex = playerInput.playerIndex;
        if(playerInput.actions == null)
            playerInput.actions = inputAction;

        HP = maxHp;
        rb = gameObject.GetComponent<Rigidbody>();
        groundLayer = LayerMask.GetMask("Ground");
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
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnMovement(InputValue value) => input = value.Get<Vector2>();

    //Movement method
    private void HandleMovement()
    {
        if (!isAttacking && !isDashing)
        {            
            //rb.velocity = Vector3.ClampMagnitude(rb.velocity, speed);
            
            #region variables
            Vector3 camForward = camContainer.transform.forward;
            Vector3 camRight = camContainer.transform.right;

            Vector3 DesiredPosition = camForward * input.y + camRight * input.x;
            #endregion

            //Move player's RigidBody
            if(isGrounded)
                rb.velocity = new Vector3(DesiredPosition.x * speed, rb.velocity.y, DesiredPosition.z * speed);

            else
                rb.velocity = new Vector3(DesiredPosition.x * speed * airControlAmount, rb.velocity.y, DesiredPosition.z * speed * airControlAmount);


            //Rotate player's Mesh
            if (DesiredPosition != Vector3.zero && isGrounded)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(DesiredPosition.x, 0, DesiredPosition.z));
                mesh.transform.rotation = Quaternion.Slerp(desiredRotation, mesh.transform.rotation, rotationSmoothingAmount);
            }

            //Set player's Animator speed
            meshAnim.SetFloat("Speed", rb.velocity.magnitude);
        }

        //Set running particles
        if (isGrounded && rb.velocity.magnitude > 2 && !effectManager.p_run.isPlaying)
            effectManager.p_run.Play();

        if(!isGrounded | rb.velocity.magnitude <= 2 && effectManager.p_run.isPlaying)
            effectManager.p_run.Stop();
;    }

    //Jump Method
    private void OnJump()
    {
        if(isGrounded && !isAttacking && !isDashing)
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
        isGrounded = Physics.Raycast(new Ray(gameObject.transform.position, gameObject.transform.up * -1),groundCheckDistance,groundLayer);

        meshAnim.SetBool("Grounded", isGrounded);
    }

    //Draw various things on Gizmo
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(gameObject.transform.position, new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y - groundCheckDistance, gameObject.transform.position.z));
    }

    public void TakeDamage(int damageTaken)
    {
        if (!isInvincible)
        {
            isInvincible = true;
            StartCoroutine(Invincibility());

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
        meshAnim.SetTrigger("Dies");
        effectManager.p_die.Play();
        //gameObject.GetComponent<Collider>().enabled = false;
        gameObject.SetActive(false);
    }

    private IEnumerator Invincibility()
    {
        yield return new WaitForSeconds(invincibilityTime);
        isInvincible = false;
    }

    //Dash input method
    private void OnDash()
    {
        if(canDash)
            StartCoroutine(HandleDash());
    }
    
    //Dash handling method
    private IEnumerator HandleDash()
    {
        canDash = false;

        isDashing = true;
        meshAnim.SetBool("Dash", true);

        effectManager.t_dashTrail.enabled = true;
        effectManager.p_dash.Play();
        dashStamp = 0;

        while (isDashing && dashStamp < dashDuration)
        {
            Vector3 direction = mesh.transform.forward;
            rb.velocity = direction * dashAmount;
                
            dashStamp += Time.deltaTime;

            yield return new WaitForEndOfFrame();
        }

        effectManager.t_dashTrail.enabled = false;
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
                    StartCoroutine(Targetting());
                }

                meshAnim.SetTrigger("Punch");

                StartCoroutine(Attack(attackArray[attackDepth - 1]));
            }
        }
    }

    //Special method
    private void OnSpecial()
    {
        if (isGrounded)
        {
            meshAnim.SetTrigger("Spin");
            isAttacking = true;
            attackTimeStamp = attackTimerMax;

            StartCoroutine(Attack(attackArray[4]));
        }
    }

    //Reference attack method
    private IEnumerator Attack(Attack attack)
    {
        yield return new WaitForSeconds(attack.delay);

        if (Physics.CheckSphere(attack.attackPosition.position, attack.attackRadius, enemyLayers))
        {
            AudioSinglePlay(audioSource.clip, 0.05f);
            StartCoroutine(camContainer.GetComponent<CameraEffects>().Hitstop(0.07f));
            StartCoroutine(camContainer.GetComponent<CameraEffects>().Shake(0.1f, 0.03f));

            if (rumbleActive)
                StartCoroutine(Rumble(0.5f, 1, 0.1f));
        }

        Collider[] hitEnemies = Physics.OverlapSphere(attack.attackPosition.position, attack.attackRadius, enemyLayers);

        foreach (Collider hit in hitEnemies)
        {
            hit.GetComponent<Enemy>().TakeDamage(attack.attackDamage,playerIndex);
            effectManager.p_impact.transform.position = hit.transform.position;
            effectManager.p_impact.Play();
        }
    }

    //Set up effect on charge state
    private void OnSpecialCharge()
    {
        if(isGrounded)
            effectManager.p_spinCharge.Play();
    }

    //Set up the attack states
    private void AttackState()
    {
        meshAnim.SetBool("Attacking", isAttacking);
        meshAnim.SetInteger("Depth", attackDepth);

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
        if (!isRumbling)
        {
            InputDevice playerDevice = playerInput.devices[playerIndex];

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
        renderer.material = whiteMat;

        yield return new WaitForSeconds(blinkingTime);
        renderer.material = defMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = whiteMat;
        yield return new WaitForSeconds(blinkingTime);
        renderer.material = defMat;
        yield return new WaitForSeconds(blinkingTime);
    }

    //Targetting system
    private void OnTriggerEnter(Collider other)
    {
        if (isDashing)
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();

            if (enemy != null)
                StartCoroutine(enemy.KnockBack(dashPush));
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
                    if (Vector3.Distance(target.transform.position, transform.position) < Vector3.Distance(currenttarget.transform.position, transform.position) && currenttarget != null)
                        currenttarget = target;

                    yield return null;
                }

                Quaternion desiredRotation = Quaternion.LookRotation(new Vector3(currenttarget.transform.position.x, 0, currenttarget.transform.position.z));
                mesh.transform.LookAt(currenttarget.transform);
            }
        }
    }
}
