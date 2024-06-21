using UnityEngine;
public class Defend: State {
    private PlayerController controller;
    public Defend(PlayerController controller) : base("Defend") {
        this.controller = controller;
    }

    public override void Enter() {
        base.Enter();
        controller.thisAnimator.SetBool("bDefend", true);
        controller.shieldHitbox.SetActive(true);
    }
    public override void Exit() {
        base.Exit();
        controller.thisAnimator.SetBool("bDefend", false);
        controller.shieldHitbox.SetActive(false);
    }
    public override void Update() {
        base.Update();
        if(!controller.hasDefenseInput){
            controller.stateMachine.ChangeState(controller.idleState);
            return;
        }
    }
    public override void LateUpdate() {
        base.LateUpdate();
    }
    public override void FixedUpdate() {
        base.FixedUpdate();
        controller.RotateBodyToFaceInput();
    }
}