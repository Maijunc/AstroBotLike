using UnityEngine;

public class LocomotionState : BaseState 
{
    public LocomotionState(PlayerController player, Animator animator) : base(player, animator) 
    {

    }

    public override void OnEnter() 
    {
        // Debug.Log("LocomotionState OnEnter");
        animator.CrossFade(LocomotionHash, crossFadeDuration);
    }

    public override void FixedUpdate()
    {
        // call Player's move logic 
        player.HandleMovement();
    }
}

