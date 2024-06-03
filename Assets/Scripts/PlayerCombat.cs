using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField]
    AudioSource playerAudio;
    [SerializeField]
    private AudioClip playerSlash;
    [SerializeField]
    private AudioClip playerHurt;
    [SerializeField]
    private AudioClip playerDeath;
    [SerializeField]
    private GameObject fireballPrefab;
    [SerializeField]
    private LayerMask enemyLayer;
    [SerializeField]
    private float attackRadius = 0.5f;
    [SerializeField]
    private float attackHeight = 1f;
    [SerializeField]
    private float attackOffset = 0.5f;

    private float fireballCooldown = 2f;
    private float fireballTimer = 0f;

    private Slider healthBar;
    [SerializeField]
    private Image flashImage;

    private BaseLevelLogic levelLogic;

    private Animator playerAnimator;
    //Kada resava zagonetku ne sme da se krece ili da udara
    public bool disabledPlayer = false;

    private float maxHealth=100;
    [HideInInspector]
    public float playerHealth;
    private float attackDamage = 20;
    private float attackCooldown = 0.5f;
    private float lastAttackTime = 0;
    private bool isDead = false;
    private bool isJumping = false;

    //Funkcija u kojoj se ucitavaju razne promenljive
    private void Awake()
    {
        healthBar = GameObject.Find("MainCanvas").transform.GetChild(0).GetChild(1).GetComponent<Slider>();
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        //Flash image se u sedmom nivou ne nalazi u hijerarhiji na istom mestu kao i u ostalim nivoima
        if(sceneName!= "Level7")
        {
            flashImage = GameObject.Find("MainCanvas").transform.GetChild(3).GetComponent<Image>();
        }
        
        playerAnimator = GetComponent<Animator>();
        levelLogic = GameObject.Find("LevelLogic").GetComponent<BaseLevelLogic>();
        //Ucitava helte i azurira healthbar
        healthBar.value = PlayerData.remainingHealth;
        playerHealth = PlayerData.remainingHealth;
        Invoke("LoadPlayerData", 0.1f);
    }

    private void Update()
    {
        if (disabledPlayer) return;

        //Ukoliko igrac stisne levi klik misa, onda se pokrece logika za udaranje 
        if (Input.GetMouseButtonDown(0))
        {
            //Ukoliko igrac ne skace, i zavrsio se cooldown onda se izvrsava logika
            if (Time.time - lastAttackTime >= attackCooldown && !isJumping)
            {
                PlaySound(playerSlash, true);
                
                AttackEnemies();
                lastAttackTime = Time.time;
            }
        }

        //Ovaj deo koda je znacajan za sedmi nivo, jer se bacanje vatrenih lopti moze izvrsiti samo u sedmom
        //nivou zato sto fireballPrefab nije ucitan na igracu sve do poslednjeg nivoa
        fireballTimer += Time.deltaTime;
        if(fireballTimer >= fireballCooldown)
        {
            if (Input.GetMouseButtonDown(1) && fireballPrefab != null)
            {
                //Kada se lopta instancira, potrebno je promeniti isFriendly promenljivu da bi ona znala kome
                //treba da nanese damage
                GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
                fireball.GetComponent<FireballLogic>().isFriendly = true;
                fireballTimer = 0;
            }
        }
    }
    //Funkcija koja se poziva sa malim zakasnjenjem kako bi se lepo ucitao trenutni health i attack damage
    //zbog mogucnosti menjanja helta i damage-a u krcmi.
    private void LoadPlayerData()
    {
        maxHealth = PlayerData.maxHealth;
        healthBar.maxValue = maxHealth;
        attackDamage = PlayerData.maxPlayerDamage;

        //Ukoliko se nalazi u prvom nivou, treba da resetuje helte, a ukoliko je poslednji nivo, takodje
        //treba da resetuje helte kako bi igracu malo olaksali igricu
        Scene currentScene = SceneManager.GetActiveScene();
        string sceneName = currentScene.name;
        if (sceneName == "Level1" || sceneName == "Level7")
        { 
            healthBar.value = maxHealth;
            playerHealth = maxHealth;
        }
    }
    //Funkcija koja se poziva kada treba da se pusti neki zvuk, takodje ima promenljivu canInterrupt koja
    //dozvoljava nekim zvukovima da prekinu trenutni zvuk, kako bi se oni pustili
    private void PlaySound(AudioClip playerClip, bool canInterrupt)
    {
        if (canInterrupt)
        {
            playerAudio.Stop();
            playerAudio.clip = playerClip;
            playerAudio.Play();
        }
        if (!playerAudio.isPlaying)
        {
            playerAudio.clip = playerClip;
            playerAudio.Play();
        }
    }
    public void TakeDamage(float damage, float delayTime, Transform enemyTransform, float enemyAttackRange)
    {
        if(!isDead)
        {
            StartCoroutine(TakeHitCourutine(damage, delayTime, enemyTransform, enemyAttackRange));
            CheckDeath();
        }
    }
    public void SetJumping(bool _isJumping)
    {
        isJumping = _isJumping;
    }
    private IEnumerator TakeHitCourutine(float damage, float delayTime, Transform enemyTransform, float enemyAttackRange)
    {
        //Pomocna promenljiva da bi se znalo da li igrac ipak treba da primi damage ili ne
        bool keepRunning = true;
        //Pauzira se sa izvrsavanjem koda dok se ne odradi animacija neprijatelja za udaranje
        yield return new WaitForSeconds(delayTime);
        //Ukoliko je damage nanesen iz blizine onda se proverava distanca izmedju igraca i neprijatelja, ukoliko
        //je distanca veca od attack range-a neprijatelja, onda igrac ne treba da primi damage (keepRunning = false)
        if(enemyTransform != null)
        {
            if (Mathf.Abs((Mathf.Abs(enemyTransform.position.x) - Mathf.Abs(transform.position.x))) > enemyAttackRange)
            {
                keepRunning = false;
            }
        }
        //Ukoliko distanca igraca od neprijatelja nije bila dovoljno velika, onda ce se izvrsiti logika
        //udaranja igraca
        if(keepRunning)
        {
            PlaySound(playerHurt, true);
            StartCoroutine(FlashAndFade());
            playerAnimator.SetBool("isHit", true);
            playerHealth -= damage;
            healthBar.value = playerHealth;
            yield return new WaitForSeconds(0.3f);
            playerAnimator.SetBool("isHit", false);
            CheckDeath();
        }
    }
    //Korutina koja prvo prikaze crvenu sliku i onda je sakriva tako sto pravi efekat fade-out
    private IEnumerator FlashAndFade()
    {
        flashImage.color = new Color(255, 0, 0, .75f);
        yield return new WaitForSeconds(0.3f);

        float time = 0f;
        while (time < 0.3f)
        {
            float t = time / 0.3f;
            float alphaValue = 1 - (t+.2f);
            flashImage.color = new Color(255, 0, 0, alphaValue);
            time += Time.deltaTime;
            yield return null;
        }
        flashImage.color = new Color(255, 0, 0, 0);
    }
    //Ukoliko je igrac umro onda se prisilno zavrsavaju sve korutine (sve druge animacije se otkazuju)
    //i izvrsava se logika posle umiranja
    private void CheckDeath()
    {
        if (playerHealth <= 0)
        {
            StopAllCoroutines();
            GetComponent<PlayerMovement>().PlayerDead();
            isDead = true;
            StartCoroutine(DeathCourutine());
        }
    }
    //Korutina koja pokrece animaciju umiranja i onda posle 2 sekunde se pojavljuje panel gde igrac moze
    //da ide na meni, u krcmu ili da ponovo pokrene nivo, a zatim se unistava objekat igraca
    private IEnumerator DeathCourutine()
    {
        playerAnimator.SetBool("isDead", true);
        PlaySound(playerDeath, true);
        yield return new WaitForSeconds(2f);
        levelLogic.DisplayDeathMessage();
        Destroy(gameObject);
    }

    private void AttackEnemies()
    {
        //Udara u pravcu u kome je okrenut igrac
        Vector2 attackDirection = transform.right;
        //Logika za okretanje smera igraca
        if(transform.localScale.x < 1)
        {
            attackDirection = -transform.right;
        }

        //Igrac kada udari, on proverava da li je dosao u dodir sa nekim objektima u obliku kapsule
        Vector2 center = (Vector2)transform.position + (attackDirection * attackOffset);
        Vector2 capsuleSize = new Vector2(attackRadius * 2f, attackHeight);

        Collider2D[] hitEnemies = Physics2D.OverlapCapsuleAll(center, capsuleSize, CapsuleDirection2D.Vertical, 0f, enemyLayer);

        //Proverava se svaki objekat koji je zahvacen tom kapsulom
        foreach (Collider2D enemy in hitEnemies)
        {
            //Ukoliko je obican neprijatelj, onda on prima damage
            if(enemy.transform.CompareTag("Enemy"))
            {
                enemy.transform.GetComponent<EnemyBehaviour>().TakeDamage(attackDamage);
            }
            //Ukoliko je neprijatelj koji puca onda on takodje prima damage ali preko druge skripte
            else if(enemy.transform.CompareTag("EnemyRanged"))
            {
                enemy.transform.GetComponent<RangedEnemy>().TakeDamage(attackDamage);
            }
            //Ukoliko je igrac udario strelu, onda treba da se unisti ta strela
            else if(enemy.transform.CompareTag("Arrow"))
            {
                Destroy(enemy.transform.gameObject);
            }
        }
    }
}
