using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private GameObject enemyHealthBar;
    private Animator enemyAnimator;
    private Transform playerTransform;
    private EnemyBehaviour enemyBehaviour;

    private void Start()
    {
        //Posto Enemy i RangedEnemy dele ovu skriptu, potrebno je da se osigura da referenca na EnemyBehaviour
        //ima samo odgovarajuci neprijatelj
        if(CompareTag("Enemy"))
            enemyBehaviour = GetComponent<EnemyBehaviour>();
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        enemyAnimator = GetComponent<Animator>();
        enemyHealthBar = transform.GetChild(0).gameObject;
    }

    //Funkcija za azuriranja parametra za brzinu zbog animacije
    public void UpdateSpeed(float speed)
    {
        enemyAnimator.SetFloat("speed", speed);
    }
    //Funkcija koju poziva druga skripta kako bi se pokrenula korutina za animaciju primljenog udarca neprijatelja
    public void PlayTakeHit()
    {
        StartCoroutine(TakeHitCourutine());
    }
    //Funkcija koju poziva druga skripta kako bi se pokrenula korutina za animaciju smrti neprijatelja
    public void PlayDeath()
    {
        StartCoroutine(DeathCourutine());
    }
    //Funkcija koju poziva druga skripta kako bi se pokrenula animacija za napadanje igraca
    public void PlayAttack(float damage)
    {
        StartCoroutine(AttackCourutine());
        if(enemyBehaviour != null)
            playerTransform.GetComponent<PlayerCombat>().TakeDamage(damage, 0.4f, transform, enemyBehaviour.attackRange);
        else
            playerTransform.GetComponent<PlayerCombat>().TakeDamage(damage, 0.4f, transform, 0);
    }
    //Funkcija koju poziva druga skripta za pustanje animacije za napad neprijatelja koji baca projektile
    public void PlayRangedAttack()
    {
        StartCoroutine(AttackCourutine());
    }
    //Korutina za animiranje napada neprijatelja
    private IEnumerator AttackCourutine()
    {
        enemyAnimator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.6f);       
        enemyAnimator.SetBool("isAttacking", false);
    }
    //Korutina za primanje udarca za neprijatelja
    private IEnumerator TakeHitCourutine()
    {
        enemyAnimator.SetBool("isHit", true);
        yield return new WaitForSeconds(.3f);
        enemyAnimator.SetBool("isHit", false);
    }
    //Korutina za smrt neprijatelja
    private IEnumerator DeathCourutine()
    {
        Destroy(enemyHealthBar);
        enemyAnimator.SetBool("isDead", true);
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
