using UnityEngine;
public class Jump: State {
    private PlayerController controller;
    private bool hasJumped=false;
    private static readonly float initialCooldown = 0.5f;
    private float cooldown = 0.5f;
    public Jump(PlayerController controller) : base("Jump") {
        this.controller = controller;
    }

    public override void Enter() {
        base.Enter();
        hasJumped = false;
        cooldown = initialCooldown;
        controller.thisAnimator.SetBool("bJumping", true);
    }
    public override void Exit() {
        base.Exit();
        controller.thisAnimator.SetBool("bJumping", false);
    }
    public override void Update() {
        base.Update();

        // Update cooldown
        cooldown -= Time.deltaTime;

        // Switch to Idle
        if(hasJumped && controller.isGrounded && cooldown <=0) {
            controller.stateMachine.ChangeState(controller.idleState);
            return;
        }
    }
    public override void LateUpdate() {
        base.LateUpdate();
    }
    public override void FixedUpdate() {
        base.FixedUpdate();
        //Jump
        if(!hasJumped) {
            hasJumped = true;
            ApplyImpulse();
        }
        Vector3 walkVector = controller.CreateWalk(controller.movementVector);
        walkVector *= controller.jumpMovementFactor;
        controller.thisRigidbody.AddForce(walkVector, ForceMode.Force);
        controller.RotateBodyToFaceInput();
    }

    private void ApplyImpulse(){
        //pula
        Vector3 forceVector = Vector3.up * controller.jumpPower;
        controller.thisRigidbody.AddForce(forceVector, ForceMode.Impulse);
    }
}