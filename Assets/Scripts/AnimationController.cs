using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void RunAnimation(string animationName)
    {
        animator.Play(animationName);
    }
     public void Idle()
    {
        RunAnimation("Idle");
    }
    public void Run()
    {
        RunAnimation("Run"); // Replace "run" with your actual animation clip name
    }

    public void Jump()
    {
        RunAnimation("jump"); // Replace "jump" with your actual animation clip name
    }

    public void Attack()
    {
        RunAnimation("attack"); // Replace "attack" with your actual animation clip name
    }

    public void Hurt()
    {
        RunAnimation("hurt"); // Replace "hurt" with your actual animation clip name
    }

    public void Death()
    {
        RunAnimation("death"); // Replace "death" with your actual animation clip name
    }
}
