using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartBomb : MonoBehaviour
{
    [Header("Values")]
    [SerializeField] float delay = 2f;
    [SerializeField] float duration = 3f; 
    [SerializeField] float startRadius = 3f;
    [SerializeField] float endRadius = 15f;
    [SerializeField] int numberOfTicks = 10;
    [SerializeField] int damagePerTick = 1;
    [SerializeField] float hitStopDuration = 0.01f;
    [SerializeField] LayerMask enemyLayers;

    [Header("Objects")]
    [SerializeField] GameObject mesh;
    [SerializeField] Material whiteMat;
    [SerializeField] ParticleSystem p_take;

    private int playerIndex;
    private float currentRadius;
    private CameraEffects cameraEffects;
    private MeshRenderer renderer;
    private Material defMat;



    private void Start()
    {
        cameraEffects = GameObject.Find("GameManager").GetComponent<GameManager>().camContainer.GetComponent<CameraEffects>();
        renderer = mesh.GetComponent<MeshRenderer>();
        defMat = renderer.material;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out PlayerController playerController) && other.GetType() == typeof(CapsuleCollider))
        {
            GetComponent<BoxCollider>().enabled = false;
            playerIndex = playerController.playerIndex;
            p_take.Play();
            gameObject.GetComponent<AudioSource>().Play();
            gameObject.GetComponent<Animator>().SetTrigger("Take");
            StartCoroutine(Bomb());
            StartCoroutine(MeshSetting());
            Destroy(gameObject, duration + delay);
        }
    }

    private IEnumerator Bomb()
    {
        yield return new WaitForSeconds(delay);

        mesh.GetComponent<AudioSource>().Play();
        StartCoroutine(cameraEffects.Shake(duration,0.04f));

        float tickTimeSpacing = duration / numberOfTicks;
        float tickRadiusSpacing = (endRadius-startRadius)/ numberOfTicks;

        currentRadius = startRadius;

        for (int i = 0; i < numberOfTicks; i++)
        {
            if (Physics.CheckSphere(transform.position, currentRadius, enemyLayers))
            {
                StartCoroutine(cameraEffects.Hitstop(hitStopDuration));
                StartCoroutine(Blink());
            }

            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, currentRadius, enemyLayers);
            foreach(Collider hit in hitEnemies)
            {
                if (hit.TryGetComponent(out Enemy enemy))
                    enemy.TakeDamage(damagePerTick, playerIndex);

                if (hit.TryGetComponent(out Destructible destructible))
                    destructible.OpenChest(damagePerTick);

                yield return null;
            }

            currentRadius += tickRadiusSpacing;
            yield return new WaitForSeconds(tickTimeSpacing);
        }
    }
    private IEnumerator MeshSetting()
    {
        yield return new WaitForSeconds(delay);

        float meshRadiusTick = (endRadius*Time.fixedDeltaTime)/duration;
        float radius = 0;

        while(currentRadius < endRadius)
        {
            mesh.transform.localScale = Vector3.one * radius * 2;
            radius += meshRadiusTick;

            if (currentRadius > endRadius * 0.7)
                renderer.material.SetFloat("alphaMultiplier", Mathf.Clamp(renderer.material.GetFloat("alphaMultiplier") - Time.fixedDeltaTime*2,0,1));

            yield return new WaitForFixedUpdate();
        }
    }
    private IEnumerator Blink()
    {
        renderer.material = whiteMat;
        yield return new WaitForSeconds(0.05f);
        renderer.material = defMat;
        yield return new WaitForSeconds(0.05f);
        renderer.material = whiteMat;
        yield return new WaitForSeconds(0.05f);
        renderer.material = defMat;
        yield return new WaitForSeconds(0.05f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, endRadius);
        Gizmos.DrawWireSphere(transform.position, startRadius);
    }
}
