using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Animations;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private CharacterController2D controller;
    [SerializeField]
    private PlayerCombat playerCombat;
    [SerializeField]
    private Animator playerAnimator;
    [SerializeField]
    private Transform groundCheck;
    [SerializeField]
    AudioSource playerAudio;

    [SerializeField]
    private AudioClip playerWalk;
    [SerializeField]
    private AudioClip playerJump;

    //Kada igrac resava zagonetku, ne sme da se krece ili da udara
    public bool disabledPlayer = false;
    

    [SerializeField]
    private float runSpeed = 40f;

    private bool isMoving = true;
    private float horizontalMove = 0f;

    private bool controllerJump = false;
    private bool animationJump = false;

    private void Start()
    {
        playerCombat = GetComponent<PlayerCombat>();
    }

    private void Update()    
    {   //Ukoliko je igrac onemogucen da se krece, ova funkcija se nece izvrsiti
        if (disabledPlayer) return;

        //Uzima smer i brzinu kretanja igraca
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;

        //Kada igrac skoci, pusti zvuk skakanja, animaciju i uradi logiku skakanja iz character controller
        if (Input.GetButtonDown("Jump") && isMoving)
        {
            PlaySound(playerJump, true);
            controllerJump = true;
            playerCombat.SetJumping(true);
        }
        //Kada igrac pritisne levi klik misa onda se izvrsava animacija udaranja i kretanje mu je onemoguceno
        if(Input.GetMouseButtonDown(0) && isMoving)
        {
            isMoving = false;
            StartCoroutine(PlayAttackingAnimation());
        }
        //Pusta zvuk trcanja ukoliko igrac pokusava da se krece i ukoliko mu je dozvoljeno kretanje
        if(horizontalMove != 0 && isMoving)
        {
            PlaySound(playerWalk, false);
        }
        //Podesavaju se promenljive za animaciju skakanja i trcanja
        playerAnimator.SetBool("isJumping", animationJump);
        playerAnimator.SetFloat("speed", Mathf.Abs(horizontalMove));
    }
    IEnumerator PlayAttackingAnimation()
    {
        playerAnimator.SetBool("isAttacking", true);
        yield return new WaitForSeconds(0.5f);
        playerAnimator.SetBool("isAttacking", false);
        isMoving = true;
    }
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
    private void FixedUpdate()
    {
        if (disabledPlayer) return;

        //Izvrsava se logika kretanja za igraca preko character controllera
        if(isMoving)
        {
            controller.Move(horizontalMove * Time.fixedDeltaTime, false, controllerJump);
        }
        //Ukoliko je kretanje onemoguceno, svakako prosledi nulte vrednosti za kretanje
        else
        {
            controller.Move(0, false, false);
        }
        controllerJump = false;

        //Proverava da li je igrac u dodiru sa tlom, i treba da smesti u niz sve objekte sa kojima je
        //u dodiru.
        Collider2D[] grounds = Physics2D.OverlapCircleAll(groundCheck.position, 0.5f);

        //Ukoliko igrac se ne nalazi na tlu onda znaci da je skocio i da se pusta animacija za skakanje
        if (grounds.Length == 1)
        {
            playerCombat.SetJumping(true);
            animationJump = true;
        }
        //Ukoliko je igrac u dodiru sa vise od 1 objekta onda znaci da je na tlu i onda se zavrsava animacija
        //skakanja.
        else
        {
            playerCombat.SetJumping(false);
            animationJump = false;
        }
            
    }
    //Funkcija se poziva kada igrac ima <= 0 helta i onda treba da mu se onemoguci kretanje
    public void PlayerDead()
    {
        isMoving = false;
    }
     
}
