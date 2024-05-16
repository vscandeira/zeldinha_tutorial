using UnityEngine;
public class Walking : State {
    private PlayerController controller;
    public Walking(PlayerController controller) : base("Walking") {
        this.controller = controller;
    }
    public override void Enter() {
        base.Enter();
    }
    public override void Exit() {
        base.Exit();
    }
    public override void Update() {
        base.Update();
        if (controller.AttemptToAttack()) {
            return;
        }
        if(controller.hasJumpInput) {
            controller.stateMachine.ChangeState(controller.jumpState);
            return;
        }
        if(controller.movementVector.IsZero()){
            controller.stateMachine.ChangeState(controller.idleState);
            return;
        }
    }
    public override void LateUpdate() {
        base.LateUpdate();
    }
    public override void FixedUpdate() {
        base.FixedUpdate();
        Vector3 walkVector = controller.CreateWalk(controller.movementVector);
        walkVector = Vector3.ProjectOnPlane(walkVector, controller.slopeNormal);
        walkVector *= controller.speed;
        
        controller.thisRigidbody.AddForce(walkVector, ForceMode.Force);
        controller.RotateBodyToFaceInput();
    }
}