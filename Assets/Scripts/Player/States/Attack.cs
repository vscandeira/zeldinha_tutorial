using UnityEngine;
public class Attack: State {
    private PlayerController controller;
    public int stage = 0;
    private float stateTime;
    private bool firstFixedUpdate;
    public Attack(PlayerController controller) : base("Attack") {
        this.controller = controller;
    }

    public override void Enter() {
        base.Enter();
        
        if (stage < 0 || stage > (controller.attackStages-1)) {
            controller.stateMachine.ChangeState(controller.idleState);
            return;
        }

        stateTime = 0;
        firstFixedUpdate = true;
        controller.thisAnimator.SetTrigger("tAttack"+stage);

    }
    public override void Exit() {
        base.Exit();
        //stage = stage>(controller.attackStages-1) ? 0 : stage;
    }
    public override void Update() {
        base.Update();

        if (controller.AttemptToAttack()) {
            return;
        }
        stateTime += Time.deltaTime;
        if (IsStageExpired()) {
            controller.stateMachine.ChangeState(controller.idleState);
            return;
        }
    }
    public override void LateUpdate() {
        base.LateUpdate();
    }
    public override void FixedUpdate() {
        base.FixedUpdate();
        if (firstFixedUpdate) {
            firstFixedUpdate = false;
            controller.RotateBodyToFaceInput(1);

            var impulseValue = controller.attackStageImpulses[stage];
            var impulseVector = controller.thisRigidbody.rotation * Vector3.forward;
            impulseVector *= impulseValue;
            controller.thisRigidbody.AddForce(impulseVector,ForceMode.Impulse);
        }
    }

    public bool CanSwitchStages() {
        var isLastState = stage==(controller.attackStages-1);
        var stageDuration = controller.attackStageDurations[stage];
        var stageMaxInterval = isLastState ? 0 : controller.attackStageMaxIntervals[stage];
        var maxStageDuration = stageDuration + stageMaxInterval;

        return !isLastState && stateTime >= stageDuration && stateTime <= maxStageDuration;
    }

    public bool IsStageExpired() {
        var isLastState = stage==(controller.attackStages-1);
        var stageDuration = controller.attackStageDurations[stage];
        var stageMaxInterval = isLastState ? 0 : controller.attackStageMaxIntervals[stage];
        var maxStageDuration = stageDuration + stageMaxInterval;

        return stateTime > maxStageDuration;
    }
}