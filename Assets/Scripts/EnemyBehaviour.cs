using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    [SerializeField]
    AudioSource enemyAudio;
    [SerializeField]
    private AudioClip enemyWalk;
    [SerializeField]
    private AudioClip enemySlash;
    [SerializeField]
    private AudioClip enemyHurt;
    [SerializeField]
    private AudioClip enemyDeath;
    [SerializeField]
    private Slider enemyHealthBar;
    [SerializeField]
    private float speed = 3.5f;
    [SerializeField]
    private float changeDirectionTime = 2f; // Interval promene smera patrole
    [SerializeField]
    private float sightRange = 8;
    [SerializeField]
    private float damage = 15;
    [SerializeField]
    private float attackCooldown = 3;
    [SerializeField]
    private float enemymaxHealth = 50;
    //Svaki neprijatelj ima razlicitu vrednost za coinDrop, i u svakom nivou je menjano koliko 
    //novcica ispusta
    [SerializeField]
    private int coinDrop = 13;
    public float attackRange = 1.5f;

    private BaseLevelLogic levelLogic;

    private EnemyAnimation enemyAnimation;

    private float distanceToPlayer;
    
    
    private float lastAttackTime = 0;
    
    private float currentDirection = 1; // Smer kretanja patrole (1 - desno, -1 - levo)
    private float timeSinceLastDirectionChange = 0;
    private Transform playerTransform;
    
    private float enemyHealth;
    
    private float maxHearableDistance = 10.0f;

    private Camera mainCamera;

    //Inicijalizacija pocetnih promenljivih
    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        enemyAnimation = GetComponent<EnemyAnimation>();
        levelLogic = GameObject.Find("LevelLogic").GetComponent<BaseLevelLogic>();
        enemyHealth = enemymaxHealth;
        //Uzima se referenca na kameru i smesta se odmah u jednu promenljivu jer bi pozivanje Camera.main
        //mnogo oduzimalo racunarskih resursa
        mainCamera = Camera.main;
    }


    void Update()
    {
        //Funkcija za pozicioniranje healthbara neprijatelja
        PositionHealthBar();
        //Logika za jacinu zvuka hodanja u odnosu na daljinu od igraca (sto je neprijatelj blizi igracu,
        //to se jace cuje zvuk)
        float volume = Mathf.Clamp01(1.0f - distanceToPlayer / maxHearableDistance);
        enemyAudio.volume = Mathf.Max(volume, 0.1f);

        //Ukoliko igrac vise ne postoji (verovatno znaci da je ubijen) onda nece izvrsavati logiku pomeranja
        if (playerTransform == null) return;

        //U suprotnom, kalkulisace se distanca od igraca i onda ce se na osnovu toga neprijatelj animirati tako da
        //stoji pored igraca i samo ga udara, u suprotnom neprijatelj ce biti animiran da hoda i da patrolira ili
        //da prati igraca
        distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if(distanceToPlayer <  attackRange)
        {
            enemyAnimation.UpdateSpeed(0);
        }
        else
        {
            enemyAnimation.UpdateSpeed(speed);
        }
        
        
        //Ukoliko je igrac u attack range-u neprijatelja, onda ce neprijatelj da ga napada u intervalima
        if ((Time.time - lastAttackTime >= attackCooldown) && (attackRange > distanceToPlayer))
        {
            AttackPlayer();
            lastAttackTime = Time.time;
        }
        //Potrebno je da se pusta zvuk hodanja dokle god juri igraca ili patrolira
        if (attackRange < distanceToPlayer && enemyHealth > 0)
        {
            PlaySound(enemyWalk, false);
            //Pomocna promenljiva sightRange proverava da li neprijatelj vidi igraca, i ukoliko ga vidi
            //onda treba da ga prati, a ukoliko ga ne vidi onda ce samo da nastavi da patrolira odredjenom
            //putanjom
            if (distanceToPlayer < sightRange)
            {
                ChasePlayer();
            }
            else
            {
                Patrol();
            }
        }

    }
    //Funkcija koja pusta zvuk i ima parametar canInterrupt sto omogucava nekim zvukovima da budu prioritetniji,
    //i da se time prekine pustanje jednog zvuka, da bi se pustio drugi zvuk.
    private void PlaySound(AudioClip enemyClip, bool canInterrupt)
    {
        if(canInterrupt)
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
    //Funkcija koja se poziva kada neprijatelju treba da se skine neki helt. Ova funkcija izvrsava logiku
    //animiranja, zvuka i azuriranja healthbar-a, a nakon svega toga proverava se da li neprijatelj treba
    //da bude mrtav
    public void TakeDamage(float damage)
    {
        PlaySound(enemyHurt, true);
        enemyAnimation.PlayTakeHit();
        enemyHealth -= damage;
        enemyHealthBar.value = enemyHealth;
        lastAttackTime = Time.time;
        CheckDeath();
    }
    //Funkcija koja proverava da li je neprijatelj mrtav (ima <= 0 helta) i ako jeste izvrsava se odredjena
    //logika
    private void CheckDeath()
    {
        if (enemyHealth <=0)
        {
            PlaySound(enemyDeath, true);
            enemyAnimation.PlayDeath();
            levelLogic.EnemyKilled();
            PlayerData.playerCoins += coinDrop;
        }
    }

    //Funkcija koja pozicionira healthbar neprijatelja tako da bude tacno iznad njega i da ga uvek prati,
    //ima mali offset po Y osi za 1.2 kako bi mu bio iznad glave.
    private void PositionHealthBar()
    {
        Vector2 targetPos = transform.position;
        targetPos.y += 1.2f;
        if(enemyHealthBar != null)
        {
            enemyHealthBar.transform.position = mainCamera.WorldToScreenPoint(targetPos);
        }
        
    }
    //Funkcija koja se poziva kada neprijatelj treba da juri igraca
    void ChasePlayer()
    {
        //Proverava u kom smeru treba da bude okrenut da bi krenuo da juri ka igracu
        Vector2 direction = playerTransform.position - transform.position;
        direction.Normalize();
        if (direction.x < 0.0f)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector3(
               Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        //Izvrsava se kretanje u odredjenom smeru po odredjenoj brzini
        transform.Translate(direction * speed * Time.deltaTime);
        //Ukoliko neprijatelj stigne igraca, onda ce da krene da ga udara
        if (distanceToPlayer <= attackRange)
        {
            PlaySound(enemySlash, true);
            AttackPlayer();
        }
    }
    //Funkcija koja se poziva kada neprijatelj patrolira
    private void Patrol()
    {
        //Promenljiva koja odredjuje kada neprijatelj treba da promeni smer kretanja
        timeSinceLastDirectionChange += Time.deltaTime;

        if (timeSinceLastDirectionChange >= changeDirectionTime)
        {
            //Obrce smer kretanja kada prodje odredjeni vremenski period
            currentDirection *= -1.0f;
            timeSinceLastDirectionChange = 0.0f;
        }
        //Logika za okretanje igraca u odnosu na smer kretanja
        if(currentDirection < 0.0f)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else
        {
            transform.localScale = new Vector3(
               Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        //Logika za pomeranje igraca u smeru u kome je okrenut
        transform.Translate(Vector2.right * currentDirection * speed * Time.deltaTime);
    }
    //Funkcija koja se poziva kada neprijatelj treba da napadne igraca
    private void AttackPlayer()
    {
        //Ukoliko postoji referenca na igraca (ukoliko nije mrtav) onda se neprijatelj animira i pusta se zvuk
        if (playerTransform != null)
        {
            PlaySound(enemySlash, true);
            enemyAnimation.PlayAttack(damage);
        }
    }
}
