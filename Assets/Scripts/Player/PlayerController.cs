using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Start is called before the first frame update
    [HideInInspector] public StateMachine stateMachine;
    [HideInInspector] public Idle idleState;
    [HideInInspector] public Walking walkingState;
    [HideInInspector] public Jump jumpState;
    [HideInInspector] public Dead deadState;
    [HideInInspector] public Collider thisCollider;
    [HideInInspector] public Vector3 movementVector;
    [HideInInspector] public Rigidbody thisRigidbody;
    [HideInInspector] public Animator thisAnimator;
    [HideInInspector] public bool hasJumpInput;
    [HideInInspector] public bool isGrounded;
    public float speed = 10f;
    public float jumpPower = 8f;
    public float jumpMovementFactor = 0.5f;

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
        stateMachine.ChangeState(idleState);
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

        float velocity = thisRigidbody.velocity.magnitude/speed;
        thisAnimator.SetFloat("fVelocity",velocity);

        DetectGround();

        stateMachine.Update();
    }
    void FixedUpdate() {
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

    public Quaternion GetFoward() {
        Camera camera = Camera.main;
        float eulerY = camera.transform.eulerAngles.y;
        return Quaternion.Euler(0,eulerY,0);
    }

    public void RotateBodyToFaceInput() {
        if(movementVector.IsZero()) return;
        
        Camera camera = Camera.main;
        Quaternion q1 = Quaternion.LookRotation(movementVector, Vector3.up);
        Quaternion q2 = Quaternion.Euler(0,camera.transform.eulerAngles.y,0);
        Quaternion newRotation = Quaternion.LerpUnclamped(transform.rotation, q1*q2, 0.5f);
        
        thisRigidbody.MoveRotation(newRotation);
    }

    public void DetectGround() {
        // reset flag
        isGrounded = false;

        // detect ground
        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        Bounds bounds = thisCollider.bounds;
        float radius = bounds.size.y * 0.33f;
        float maxDistance = bounds.size.y * 0.25f;
        if(Physics.SphereCast(origin, radius, direction, out var hitInfo, maxDistance)) {
            GameObject hitObject = hitInfo.transform.gameObject;
            if(hitObject.CompareTag("Plataform") | hitObject.CompareTag("Water")){
                isGrounded = true;
            }
        }
    }

    void OnDrawGizmos() {
        if(!thisCollider) return;

        Vector3 origin = transform.position;
        Vector3 direction = Vector3.down;
        Bounds bounds = thisCollider.bounds;
        float radius = bounds.size.x * 0.33f;
        float maxDistance = bounds.size.y * 0.25f;

        // Draw ray
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(new Ray(origin, direction * maxDistance));

        // Draw origin
        Gizmos.color = Color.gray;
        Gizmos.DrawSphere(origin, 0.1f);

        //Draw sphere
        Vector3 spherePosition = direction * maxDistance + origin;
        Gizmos.color = isGrounded ? Color.green : Color.red;
        Gizmos.DrawSphere(spherePosition, radius);
    }
}
