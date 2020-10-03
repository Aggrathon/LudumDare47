using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CommandStreamCharacter : MonoBehaviour
{

    public enum Action
    {
        LeftDown,
        LeftUp,
        RightDown,
        RightUp,
        Jump,
        Slide,
        Attack,
        Disable
    }

    public struct Event
    {
        public float time;
        public Action action;

        public Event(float time, Action action)
        {
            this.time = time;
            this.action = action;
        }
    }

    public bool activePlayer = false;

    [Header("Horisontal Movement")]
    public float runSpeed = 10f;
    public float smoothTime = 0.3f;

    [Header("Vertical Movement")]
    public bool canJump = true;
    public float jumpSpeed = 500f;
    [SerializeField] Vector3 groundCheckPoint = Vector2.zero;
    [SerializeField] LayerMask groundCheckMask;
    [SerializeField] float groundCheckRadius = 0.05f;
    public bool canDoubleJump = true;

    [Header("Sliding")]
    public bool canSlide = true;
    public float slideSpeed = 18f;
    public float slideCooldown = 0.5f;

    [Header("Attacking")]
    public bool canAttack = true;

    // Privates
    private float startTime;
    private Vector3 startPos;
    private List<Event> stream;
    private int index;
    private Rigidbody2D rb;
    private float horisontalMovement = 0;
    private Vector2 velocity;
    private float slideTime;
    private bool hasDoubleJumped;
    private bool isGrounded;

    private void Start()
    {
        if (stream == null)
            stream = new List<Event>();
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
        Reset();
        if (GameManager.instance != null)
            GameManager.instance.RegisterCharacter(this);
    }

    void Update()
    {
        if (activePlayer)
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                RecordAction(Action.LeftDown);
            }
            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                RecordAction(Action.LeftUp);
            }
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                RecordAction(Action.RightDown);
            }
            if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                RecordAction(Action.RightUp);
            }
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (canJump)
                    RecordAction(Action.Jump);
            }
            if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (canSlide && Time.time - slideCooldown > slideTime)
                {
                    slideTime = Time.time;
                    RecordAction(Action.Slide);
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            {
                if (canAttack)
                    RecordAction(Action.Attack);
            }
        }
        else
        {
            if (stream.Count > index && Time.time - startTime >= stream[index].time)
            {
                TakeAction(stream[index].action);
                index++;
            }
        }
    }

    void TakeAction(Action action)
    {
        switch (action)
        {
            case Action.LeftDown:
                horisontalMovement--;
                MaybeFlip();
                break;
            case Action.LeftUp:
                horisontalMovement++;
                MaybeFlip();
                break;
            case Action.RightDown:
                horisontalMovement++;
                MaybeFlip();
                break;
            case Action.RightUp:
                horisontalMovement--;
                MaybeFlip();
                break;
            case Action.Jump:
                if (isGrounded || (canDoubleJump && !hasDoubleJumped))
                {
                    rb.AddForce(Vector2.up * jumpSpeed);
                    hasDoubleJumped = true;
                    // TODO: FX (dust cloud)
                }
                break;
            case Action.Slide:
                var vel = rb.velocity;
                vel.x += horisontalMovement * slideSpeed;
                rb.velocity = vel;
                velocity = Vector2.zero;
                // TODO: FX (dust cloud)
                break;
            case Action.Attack:
                Debug.LogWarning("Attacking not implemented");
                break;
            case Action.Disable:
                gameObject.SetActive(false);
                break;
            default:
                Debug.LogError("Action not implemented");
                break;
        }
    }

    void MaybeFlip()
    {
        if (horisontalMovement < 0)
        {
            var scale = transform.localScale;
            scale.x = -1f;
            transform.localScale = scale;
        }
        else if (horisontalMovement > 0)
        {
            var scale = transform.localScale;
            scale.x = 1f;
            transform.localScale = scale;
        }
    }

    void RecordAction(Action action)
    {
        stream.Add(new Event(Time.time - startTime, action));
        TakeAction(action);
    }

    public void AddActionToStreamNow(Action action)
    {
        stream.Add(new Event(Time.time - startTime, action));
    }

    private void FixedUpdate()
    {
        var vel = rb.velocity;
        vel.x = horisontalMovement * runSpeed;
        rb.velocity = Vector2.SmoothDamp(rb.velocity, vel, ref velocity, smoothTime);
        isGrounded = Physics2D.OverlapCircle(transform.position + groundCheckPoint, groundCheckRadius, groundCheckMask);
        if (isGrounded)
        {
            hasDoubleJumped = false;
        }
    }

    public void Reset()
    {
        transform.position = startPos;
        startTime = Time.time;
        index = 0;
        rb.velocity = Vector2.zero;
        velocity = Vector2.zero;
        rb.WakeUp();
        slideTime = 0f;
        hasDoubleJumped = false;
        isGrounded = true;
        gameObject.SetActive(true);
        if (activePlayer)
        {
            stream.Clear();
            if (horisontalMovement > 0)
                AddActionToStreamNow(Action.RightDown);
            else if (horisontalMovement < 0)
                AddActionToStreamNow(Action.LeftDown);
        }
        else
        {
            horisontalMovement = 0f;
        }
    }

    public void SetStream(List<Event> stream)
    {
        this.stream = stream;
    }

    public List<Event> GetStream()
    {
        return stream;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + groundCheckPoint, groundCheckRadius);
    }
}
