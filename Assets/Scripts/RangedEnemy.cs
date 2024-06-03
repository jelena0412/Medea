using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField]
    AudioSource enemyAudio;

    [SerializeField]
    private AudioClip enemySlash;
    [SerializeField]
    private AudioClip enemyHurt;
    [SerializeField]
    private AudioClip enemyDeath;
    [SerializeField]
    private Slider enemyHealthBar;

    [SerializeField]
    private float damage = 15;
    [SerializeField]
    private float attackCooldown = 3;
    [SerializeField]
    private float enemymaxHealth = 50;
    [SerializeField]
    private float arrowSpeed;
    [SerializeField]
    private int coinDrop = 17;
    [SerializeField]
    private GameObject arrowPrefab;

    private BaseLevelLogic levelLogic;

    private EnemyAnimation enemyAnimation;

    private Transform playerTransform;

    private float enemyHealth;
    private float distanceToPlayer;
    private float lastAttackTime;
    

    private float maxHearableDistance = 10.0f;

    private Camera mainCamera;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        enemyAnimation = GetComponent<EnemyAnimation>();
        levelLogic = GameObject.Find("LevelLogic").GetComponent<BaseLevelLogic>();
        enemyHealth = enemymaxHealth;
        mainCamera = Camera.main;
    }

    //Slicna logika kao i u EnemyBehaviour
    void Update()
    {
        PositionHealthBar();
        //Kalkulisanje rastojanja neprijatelja i igraca, jer sto je blizi neprijatelj igracu, to ce jacina
        //zvuka koji ispusta neprijatelj biti jaca
        float volume = Mathf.Clamp01(1.0f - distanceToPlayer / maxHearableDistance);
        enemyAudio.volume = Mathf.Max(volume, 0.1f);

        if (playerTransform == null) return;

        //Racunanje distance zbog jacine zvuka
        distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        //Napada neprijatelja svaki cas u odredjenim vremenskim intervalima
        if (Time.time - lastAttackTime >= attackCooldown)
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }

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
    //Kao i kod EnemyBehaviour je i ovde ista logika za primanje damage-a i primene animacije i zvuka
    public void TakeDamage(float damage)
    {
        PlaySound(enemyHurt, true);
        enemyAnimation.PlayTakeHit();
        enemyHealth -= damage;
        enemyHealthBar.value = enemyHealth;
        lastAttackTime = Time.time;
        CheckDeath();
    }
    //Ukoliko je neprijatelj mrtav, izvrsava se odredjena logika
    private void CheckDeath()
    {
        if (enemyHealth <= 0)
        {
            PlaySound(enemyDeath, true);
            enemyAnimation.PlayDeath();
            levelLogic.EnemyKilled();
            PlayerData.playerCoins += coinDrop;
        }
    }
    //Funkcija koja se osigurava da healthbar bude uvek iznad neprijatelja kao i u EnemyBehaviour
    private void PositionHealthBar()
    {
        Vector2 targetPos = transform.position;
        targetPos.y += 1.2f;
        if (enemyHealthBar != null)
        {
            enemyHealthBar.transform.position = mainCamera.WorldToScreenPoint(targetPos);
        }

    }
    //Ova funkcija je drugacija nego EnemyBehaviour jer se ovde instancira strela kojoj se dodeljuje
    //odredjeni damage i brzina kako bi svaki ranged neprijatelj mogao da ima odredjenu tezinu da se ubije
    private void AttackPlayer()
    {
        if (playerTransform != null)
        {
            GameObject arrow = Instantiate(arrowPrefab, transform.position, Quaternion.identity);
            arrow.GetComponent<ArrowLogic>().damage = damage;
            arrow.GetComponent<ArrowLogic>().speed = arrowSpeed;
            enemyAnimation.PlayRangedAttack();
        }
    }
}
