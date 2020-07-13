using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [Header("Player Index")]
    public int playerIndex = 0;

    [Header("Values")]
    [SerializeField] int maxHp = 5;
    public int HP;
    [SerializeField] float jumpHeight = 10f;
    [SerializeField] float speed = 10f;
    [SerializeField] LayerMask groundLayer;
    private Vector2 input;
    private float blinkingTime = 0.05f;

    [Header("Attack Values")]
    [SerializeField] int attackDepth;
    [SerializeField] int attackDepthMax = 3;
    [SerializeField] float attackRadius;
    [SerializeField] LayerMask enemyLayers;
    [SerializeField] Transform punchPoint;
    [SerializeField] float attackTimerMax = 0.15f;
    [SerializeField] float attackTimeStamp = 0f;

    [Header("Lists of attacks per damage")]
    [SerializeField] int punchL = 2;
    [SerializeField] int punchR = 2;
    [SerializeField] int uppercutR = 3;
    [SerializeField] int kick = 4;
    [SerializeField] int spin = 4;

    int[] attackList;
    [SerializeField] Attack[] attackArray;

    [Header("Object")]
    [SerializeField] GameObject mesh;
    [SerializeField] EffectManager effectManager;
    [SerializeField] InputActionAsset inputAction;
    [SerializeField] Material whiteMat;
    private Material[] defMats;
    private SkinnedMeshRenderer renderer;
    private Transform camContainer;
    private AudioSource audioSource;
    private GameManager gameManager;

    [Header("States")]
    public bool isGrounded;
    public bool isAttacking;
    [SerializeField] bool rumbleActive;
    bool isRumbling = false;

    PlayerInput playerInput;
    Animator meshAnim;
    Rigidbody rb;
    float groundCheckDistance = 0.5f;
    float rotationSmoothingAmount = 0.75f;


    //Initialize
    void Start()
    {
        renderer = GetComponentInChildren<SkinnedMeshRenderer>();
        defMats = new Material[] {renderer.materials[0], renderer.materials[1], renderer.materials[2] };
        playerInput = GetComponent<PlayerInput>();
        playerIndex = playerInput.playerIndex;
        if(playerInput.actions == null)
            playerInput.actions = inputAction;

        HP = maxHp;
        rb = gameObject.GetComponent<Rigidbody>();
        groundLayer = LayerMask.GetMask("Ground");
        meshAnim = mesh.GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        camContainer = gameManager.camContainer;

        if (playerIndex == 0)
            gameManager.player0 = this;
        else
            gameManager.player1 = this;

        attackList = new int[] { punchL, punchR, uppercutR, kick, spin };
    }

    //Unity Cycles
    void Update()
    {
        HP = Mathf.Clamp(HP, 0, maxHp);

        HandleMovement();
        GroundCheck();
        AttackState();
    }

    private void OnMovement(InputValue value)
    {
        input = value.Get<Vector2>();
    }

    //Movement method
    private void HandleMovement()
    {
        if (isGrounded && !isAttacking)
        {
            #region variables
            Vector3 camForward = camContainer.transform.forward;
            Vector3 camRight = camContainer.transform.right;

            Vector3 DesiredPosition = camForward * input.y + camRight * input.x;
            #endregion

            //Move player's RigidBody
            rb.velocity = new Vector3(DesiredPosition.x * speed, rb.velocity.y, DesiredPosition.z * speed);

            //Rotate player's Mesh
            if (DesiredPosition != Vector3.zero)
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
        if(isGrounded && !isAttacking)
        {
            rb.AddForce(0, jumpHeight, 0);
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
        Gizmos.DrawWireSphere(punchPoint.position, attackRadius);
    }

    public void TakeDamage(int damageTaken)
    {
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

    public void GainHP(int hpGained) => HP += hpGained;


    private void Die()
    {
        meshAnim.SetTrigger("Dies");
        effectManager.p_die.Play();
        //gameObject.GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 0.5f);
    }



    //Attack method
    private void OnPunch()
    {
        var progressState = meshAnim.GetCurrentAnimatorStateInfo(0).normalizedTime;

        if (isGrounded)
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
                    rb.velocity = mesh.transform.forward * 5;
                }

                meshAnim.SetTrigger("Punch");

                if (Physics.CheckSphere(punchPoint.position, attackRadius, enemyLayers))
                {
                    effectManager.p_impact.Play();
                    AudioSinglePlay(audioSource.clip, 0.05f);
                    StartCoroutine(camContainer.GetComponent<CameraEffects>().Hitstop(0.07f));
                    StartCoroutine(camContainer.GetComponent<CameraEffects>().Shake(0.1f, 0.03f));
                    
                    if (rumbleActive)
                        StartCoroutine(Rumble(0.5f, 1, 0.1f));
                }

                Collider[] hitEnemies = Physics.OverlapSphere(punchPoint.position, attackRadius, enemyLayers);

                foreach (Collider hit in hitEnemies)
                    hit.GetComponent<Enemy>().TakeDamage(attackList[attackDepth - 1]);
            }
        }
    }

    //Special method
    private void OnSpecial()
    {
        if (isGrounded)
        {
            //StartCoroutine(Attack("Spin"));
            isAttacking = true;
            attackTimeStamp = attackTimerMax;
        }
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
        renderer.materials[0] = whiteMat;
        renderer.materials[1] = whiteMat;
        renderer.materials[2] = whiteMat;

        yield return new WaitForSeconds(blinkingTime);
        renderer.materials[0] = defMats[0];
        renderer.materials[1] = defMats[1];
        renderer.materials[2] = defMats[2];

        yield return new WaitForSeconds(blinkingTime);
        renderer.materials[0] = whiteMat;
        renderer.materials[1] = whiteMat;
        renderer.materials[2] = whiteMat;

        yield return new WaitForSeconds(blinkingTime);
        renderer.materials[0] = defMats[0];
        renderer.materials[1] = defMats[1];
        renderer.materials[2] = defMats[2];

        yield return new WaitForSeconds(blinkingTime);
    }
}
