using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballLogic : MonoBehaviour
{
    public bool isFriendly;
    [SerializeField]
    private float damage = 40;
    [SerializeField]
    private float medeaSpeed;
    [SerializeField]
    private float playerSpeed;
    [SerializeField]
    private Rigidbody2D rb;
    private Transform playerTransform;
    private Vector3 mousePosition;
    private Vector2 direction;
    private Vector3 targetPosition;
    private float speed;

    private void Start()
    {
        playerTransform = GameObject.Find("Player").GetComponent<Transform>();
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        
        //Ukoliko je isFriendly true onda lopta bude brza i prati smer misa u pocetnom trenutku
        if(isFriendly)
        {
            speed = playerSpeed;
            targetPosition = mousePosition;
        }
        //Ukoliko nije, onda je lopta malo sporija i krece se ka igracu kada je bio u tom trenutku kada se
        //instancirala lopta
        else
        {
            speed = medeaSpeed;
            targetPosition = playerTransform.position;
        }
        //Logika za izracunavanje smera kuda treba da se krece lopta
        direction = targetPosition - transform.position;
        //Unistava samu sebe nakon 10 sekundi
        Invoke("DestroySelf", 10f);
    }

    private void FixedUpdate()
    {
        if (playerTransform != null)
        {
            //Kalkulisanje ugla prema kome treba lopta da bude okrenuta (u smeru kretanja)
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

            direction.Normalize();

            //Stavlja se brzina lopte da bude u smeru kretanja
            rb.velocity = direction * speed;
        }
    }
    //Ako dodje u dodir sa nekim objektom
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Ako dodirne igraca, onda se proverava da li je lopta medeina, ako jeste onda ce igracu da skine
        //damage, ako nije onda se nece desiti nista
        if(collision.CompareTag("Player"))
        {
            if(!isFriendly)
            {
                collision.GetComponent<PlayerCombat>().TakeDamage(damage / 3, 0, null, 0);
            }
        }
        //Ako dodirne Medeu, proverava da li je lopta od igraca, ako jeste onda medei skine pun damage
        if(collision.gameObject.name == "Medea")
        {
            if (isFriendly)
            {
                collision.GetComponent<MedeaBehaviour>().TakeDamage(damage);
            }
        }
    }
    //Unistava samu sebe da se ne bi gomilali objekti u sceni, ako lopta omasi igraca ili medeu, da ne bi
    //lutali u nedogled
    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
