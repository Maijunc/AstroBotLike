using UnityEngine;

public class ChargeState : BaseState
{
    public ChargeState(PlayerController player, Animator animator) : base(player, animator)
    {
    }

    public override void OnEnter()
    {
        animator.CrossFadeInFixedTime(ChargeHash, 0.1f, layer: 1);
    }

    public override void FixedUpdate()
    {
        player.HandleCharge();
        player.HandleMovement();
    }

    public override void OnExit() 
    {
    }

}
