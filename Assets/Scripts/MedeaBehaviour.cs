using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MedeaBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject fireballPrefab;
    [SerializeField]
    private Animator enemyAnimator;
    [SerializeField]
    private AudioSource enemyAudio;
    [SerializeField]
    private AudioClip enemyWalk;
    [SerializeField]
    private AudioClip enemySlash;
    [SerializeField]
    private AudioClip enemyHurt;
    [SerializeField]
    private AudioClip enemyDeath;
    [SerializeField]
    private Slider healthbar;
    [SerializeField]
    private float speed = 5f;

    private bool isDead = false;

    private BossLevelLogic levelLogic;

    private bool isAttacking = false;
    private float timeSinceLastDirectionChange;
    private float changeDirectionTime = 1.5f;
    private float currentDirection = 1f;
    [SerializeField]
    private float enemyHealth = 250;
    private void Start()
    {
        levelLogic = GameObject.Find("LevelLogic").GetComponent<BossLevelLogic>();
    }
    private void FixedUpdate()
    {
        if(!isDead)
        {
            if (!isAttacking)
            {
                Patrol();
                enemyAnimator.SetFloat("speed", speed);
            }
            else
            {
                enemyAnimator.SetFloat("speed", 0);
            }
        }
    }
    private void Patrol()
    {
        timeSinceLastDirectionChange += Time.deltaTime;

        if (timeSinceLastDirectionChange >= changeDirectionTime)
        {
            currentDirection *= -1.0f;
            timeSinceLastDirectionChange = 0.0f;
            changeDirectionTime = Random.RandomRange(0.5f, 1.5f);
            StartCoroutine(ThrowFireballsAnimation());
        }
        if (currentDirection < 0.0f)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector3(
               Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        transform.Translate(Vector2.right * currentDirection * speed * Time.deltaTime);
    }
    IEnumerator ThrowFireballsAnimation()
    {
        enemyAnimator.SetBool("isThrowingFireball", true);
        isAttacking = true;
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        fireball.GetComponent<FireballLogic>().isFriendly = false;
        yield return new WaitForSeconds(1f);
        isAttacking = false;
        enemyAnimator.SetBool("isThrowingFireball", false);
    }
    public void TakeDamage(float damage)
    {
        PlaySound(enemyHurt, true);
        StartCoroutine(TakeHitCourutine());
        enemyHealth -= damage;
        healthbar.value = enemyHealth;
        CheckDeath();
    }
    private void PlaySound(AudioClip enemyClip, bool canInterrupt)
    {
        if (canInterrupt)
        {
            enemyAudio.Stop();
            enemyAudio.clip = enemyClip;
            enemyAudio.Play();
        }
        if (!enemyAudio.isPlaying)
        {
            enemyAudio.clip = enemyClip;
            enemyAudio.Play();
        }
    }
    private void CheckDeath()
    {
        if (enemyHealth <= 0)
        {
            PlaySound(enemyDeath, true);
            StartCoroutine(DeathCourutine());
        }
    }
    private IEnumerator TakeHitCourutine()
    {
        enemyAnimator.SetBool("isHit", true);
        yield return new WaitForSeconds(.3f);
        enemyAnimator.SetBool("isHit", false);
    }
    private IEnumerator DeathCourutine()
    {
        enemyAnimator.SetBool("isDead", true);
        isDead = true;
        yield return new WaitForSeconds(2f);
        levelLogic.MedeaKilledLogic();
    }
}
