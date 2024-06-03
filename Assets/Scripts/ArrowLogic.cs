using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowLogic : MonoBehaviour
{
    public float damage;
    private Transform playerTransform;
    private Vector3 targetPosition;
    private Vector2 direction;
    private Rigidbody2D rb;
    public float timeToLive = 10f;
    public float speed = 6f;
    //Inicijalizacija pocetnih promenljivih
    private void Start()
    {
        //Pronalazenja reference igraca jer ce strela putovati u smeru gde se on nalazi
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        targetPosition = playerTransform.position;
        rb = GetComponent<Rigidbody2D>();
        //Potrebno je unistiti objekat strele nakon odredjenog vremenskog perioda
        Invoke("DestroySelf", timeToLive);
        direction = targetPosition - transform.position;
    }
    private void FixedUpdate()
    {
        if (playerTransform != null)
        {
            //Kalkulisanje ugla prema kome treba strela da bude okrenuta (u smeru kretanja)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg; 
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            direction.Normalize();

            rb.velocity = direction * speed;
        }
    }
    //Kada strela dodje u dodir sa igracem, potrebno je da mu skine odredjeni damage i da se unisti objekat
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerCombat>().TakeDamage(damage, 0, null, 0);
            DestroySelf();
        }
    }
    //Funkcija koja je pozvana nakon nekog vremena kako bi se unistila strela koja je zalutala i vise se
    //ne prikazuje na ekranu
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
