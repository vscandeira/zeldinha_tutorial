using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Internals
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public Idle idleState;
    [HideInInspector] public Walking walkingState;
    [HideInInspector] public Jump jumpState;
    [HideInInspector] public Dead deadState;
    [HideInInspector] public Attack attackState;
    [HideInInspector] public Defend defendState;
    [HideInInspector] public Collider thisCollider;
    [HideInInspector] public Rigidbody thisRigidbody;
    [HideInInspector] public Animator thisAnimator;

    [Header("Movement")]
    public float speed = 10f;
    public float maxSpeed = 10f;
    public float movSmooth = 0.3f;
    [HideInInspector] public Vector3 movementVector;

    [Header("Jump")]
    public float jumpPower = 8f;
    public float jumpMovementFactor = 0.5f;
    [HideInInspector] public bool hasJumpInput;
   
    [Header("Slope")]
    public float maxSlopeAngle = 60f;
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public bool isOnSlope;
    [HideInInspector] public Vector3 slopeNormal;

    [Header("Attack")]
    public int attackStages;
    public List<float> attackStageDurations;
    public List<float> attackStageMaxIntervals;
    public List<float> attackStageImpulses;
    public GameObject swordHitbox;
    public float swordKnockBackImpulse = 10;
    [Header("Defend")]
    public GameObject shieldHitbox;
    public float shieldKnockBackImpulse = 10;
    [HideInInspector] public bool hasDefenseInput;
    void Awake() {
        thisRigidbody = GetComponent<Rigidbody>();
        thisAnimator = GetComponent<Animator>();
        thisCollider = GetComponent<Collider>();
    }
    void Start() {
        stateMachine = new StateMachine();
        idleState = new Idle(this);
        walkingState = new Walking(this);
        jumpState = new Jump(this);
        deadState = new Dead(this);
        attackState = new Attack(this);
        defendState = new Defend(this);
        stateMachine.ChangeState(idleState);
        swordHitbox.SetActive(false);
        shieldHitbox.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        if(GameManager.Instance.isGameOver){
            if(stateMachine.currentStateName != deadState.name){
                stateMachine.ChangeState(deadState);
            }
            return;
        }

        bool isUp = Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.UpArrow);
        bool isDown = Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.DownArrow);
        bool isLeft = Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.LeftArrow);
        bool isRight = Input.GetKey(KeyCode.D) | Input.GetKey(KeyCode.RightArrow);
        float inputZ = isUp ? 1 : isDown ? -1 : 0;
        float inputX = isRight ? 1 : isLeft ? -1 : 0;
        movementVector = new Vector3(inputX, 0, inputZ);
        hasJumpInput = Input.GetKey(KeyCode.Space);
        hasDefenseInput = Input.GetMouseButton(1);

        float velocity = thisRigidbody.velocity.magnitude/maxSpeed;
        thisAnimator.SetFloat("fVelocity",velocity);

        DetectGround();
        DetectSlope();

        stateMachine.Update();
    }
    void FixedUpdate() {
        Vector3 gravityForce = Physics.gravity * (isOnSlope ? 0.1f : 1f);
        thisRigidbody.AddForce(gravityForce, ForceMode.Acceleration);
        LimitSpeed();
        stateMachine.FixedUpdate();
    }
    void LateUpdate() {
        stateMachine.LateUpdate();
    }

    public Vector3 CreateWalk(Vector3 movVector) {
        Vector3 ret = movVector * speed;
        ret = GetFoward() * ret;
        return ret;
    }

    public void OnSwordCollisionEnter(Collider other){
        var otherObject = other.gameObject;
        var otherRigidBody = otherObject.GetComponent<Rigidbody>();
        var isTarget = otherObject.layer == LayerMask.NameToLayer("Target");
        if (isTarget && otherRigidBody != null) {
            var positionDiff = otherObject.transform.position - gameObject.transform.position;
            var impulseVector = new Vector3(positionDiff.normalized.x, 0, positionDiff.normalized.z);
            impulseVector *= swordKnockBackImpulse;
            otherRigidBody.AddForce(impulseVector, ForceMode.Impulse);
        }
    }

    public void OnShieldCollisionEnter(Collider other){
        var otherObject = other.gameObject;
        var otherRigidBody = otherObject.GetComponent<Rigidbody>();
        var isTarget = true;
        if (isTarget && otherRigidBody != null) {
            var positionDiff = otherObject.transform.position - gameObject.transform.position;
            var impulseVector = new Vector3(positionDiff.normalized.x, 0, positionDiff.normalized.z);
            impulseVector *= shieldKnockBackImpulse;
            otherRigidBody.AddForce(impulseVector, ForceMode.Impulse);
        }
    }
    public Quaternion GetFoward() {
        Camera camera = Camera.main;
        float eulerY = camera.transform.eulerAngles.y;
        return Quaternion.Euler(0,eulerY,0);
    }

    public void RotateBodyToFaceInput(float alpha = 0f) {
        if(movementVector.IsZero()) return;
        
        float beta = alpha==0f ? movSmooth : alpha;
        Camera camera = Camera.main;
        Quaternion q1 = Quaternion.LookRotation(movementVector, Vector3.up);
        Quaternion q2 = Quaternion.Euler(0,camera.transform.eulerAngles.y,0);
        Quaternion newRotation = Quaternion.LerpUnclamped(transform.rotation, q1*q2, beta);
        
        thisRigidbody.MoveRotation(newRotation);
    }

    public bool AttemptToAttack() {
        if (Input.GetMouseButtonDown(0)) {
            var isAttacking = stateMachine.currentStateName == attackState.name;
            var canAttack = !isAttacking || attackState.CanSwitchStages();
            if (canAttack) {
                var attackStage = isAttacking ? (attackState.stage + 1) : 0;
                attackState.stage = attackStage;
                stateMachine.ChangeState(attackState);
                return true;
            }
        }
        return false;
    }

    public void DetectGround() {
        // reset flag
        isGrounded = false;

        // detect ground
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        float maxDistance = 0.1f;
        LayerMask groundLayer = GameManager.Instance.groundLayer;
        if(Physics.Raycast(origin, direction, maxDistance, groundLayer)) {
            isGrounded = true;
        }
    }

    public void DetectSlope(){
        isOnSlope = false;
        slopeNormal = Vector3.zero;

        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        float maxDistance = 0.2f;
        if (Physics.Raycast(origin, direction, out var slopeHitInfo, maxDistance)){
            float angle = Vector3.Angle(Vector3.up, slopeHitInfo.normal);
            if (angle != 0) Debug.Log(angle);
            isOnSlope = angle < maxSlopeAngle && angle != 0;
            slopeNormal = isOnSlope ? slopeHitInfo.normal : Vector3.zero;
        }
    }

    private void LimitSpeed() {
        Vector3 flatVelocity = new Vector3(thisRigidbody.velocity.x, 0, thisRigidbody.velocity.z);
        if (flatVelocity.magnitude > maxSpeed) {
            Vector3 limitedVelocity = flatVelocity.normalized * maxSpeed;
            thisRigidbody.velocity = new Vector3(limitedVelocity.x, thisRigidbody.velocity.y, limitedVelocity.z);
        }

    }
// /*
    void OnGUI() {
        string s= stateMachine.currentStateName + " - " + isOnSlope + " - " + slopeNormal;
        GUI.Label(new Rect(5,5,400,100), s);
    }
// */

}
