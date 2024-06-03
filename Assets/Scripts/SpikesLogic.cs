using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesLogic : MonoBehaviour
{
    [SerializeField]
    private bool isDeadly = false;
    [SerializeField]
    private float spikeDamage = 10f;

    private float damageCooldown = 1f;
    private float timer = 1;
    private bool takingDamage = false;
    private PlayerCombat playerCombat;

    //Ukoliko igrac treba da prima damage onda postoji brojac koji ce da poziva funkciju TakeDamage
    //na svaku sekundu
    private void Update()
    {
        if (takingDamage && playerCombat != null)
        {     
            timer += Time.deltaTime;
            if(timer >= damageCooldown)
            {
                playerCombat.TakeDamage(spikeDamage, 0, null, 0);
                timer = 0f;
            }
        }
    }

    //Ukoliko siljci dodju u dodir sa igracem ili neprijateljem, treba da im skida damage
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            //Ukoliko su siljci postavljeni da budu smrtonosni (postavljeni u rupi) onda treba da skine
            //maksimalan damage igracu kako bi ga odmah ubio, u suprotnom samo ce postaviti obicnu
            //promenljivu na true kako bi mogao da prima mali damage u odredjenim vremenskim intervalima
            playerCombat = collision.GetComponent<PlayerCombat>();
            if (isDeadly)
            {
                playerCombat.TakeDamage(200, 0, null, 0);
            }
            else
            {
                takingDamage = true;
            }
        }
        //Ukoliko neprijatelj upadne slucajno u rupu sa siljcima, potrebno je da ga odmah ubiju, jer 
        //igrac ne bi mogao onda nikako da predje nivo, a ukoliko stanu na neke druge siljke, nista
        //se ne desava
        if(collision.CompareTag("Enemy"))
        {
            if(isDeadly)
            {
                collision.GetComponent<EnemyBehaviour>().TakeDamage(200);
            }
        }
    }
    //Kada igrac izadje iz tih siljaka, onda se resetuje tajmer i ne treba da prima vise damage
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            timer = 1f;
            takingDamage = false;
            playerCombat = null;
        }
    }
}
