using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float slideLength;
    [SerializeField] private float JumpLength;
    [SerializeField] private float JumpHeight;
    [SerializeField] private float LandSpeed;
    [SerializeField] private float Speed;

    private Rigidbody rb;
    private int currentLane = 1;
    private Vector3 verticalTargetPosition;
    private bool jumping = false;
    private float JumpStart;
    private Animator animator;
    private bool isSliding = false;
    private BoxCollider boxCollider;
    private Vector3 boxColliderSize;
    private float slideStart;
    public float speed = 5.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        boxColliderSize = boxCollider.size;
        animator.Play("runStart");
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (horizontalInput > 0)
        { //nếu bấm phải thì sẽ lớn hơn không nên sẽ thỏa điều kiện này
            transform.Rotate(0, 90, 0); //xoay nhân vật tới 90 độ theo trục Y
        }
        if (horizontalInput < 0)
        { //nếu bấm trái thì sẽ bé hơn không nên sẽ thỏa điều kiện này
            transform.Rotate(0, -90, 0); //xoay nhân vật lùi 90 độ theo trục Y
        }
        transform.Translate(Vector3.forward * speed * Time.deltaTime);




























        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {

            ChangeLane(-1);

        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ChangeLane(1);

        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Jump();
        }
        else if(Input.GetKeyDown(KeyCode.DownArrow))
        {
            slide();
        }
        if (jumping)
        {
            float ratio = (transform.position.z - JumpStart) / JumpLength;
            if (ratio >= 1f)
            {
                jumping = false;
                animator.SetBool("Jumping", false);
            }
            else
            {
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * JumpHeight;
            }
        }
        else
        {
            verticalTargetPosition.y = Mathf.MoveTowards(verticalTargetPosition.y, 0, 5 * Time.deltaTime);

        }
        if (isSliding)
        {
            float ratio = (transform.position.z - slideStart) / slideLength;
            if (ratio >= 1f)
            {
                isSliding = false;
                animator.SetBool("Sliding", false);
                boxCollider.size = boxColliderSize;
            }
        }

        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, LandSpeed * Time.deltaTime);
    }
    void FixedUpdate()
    {
        rb.velocity = Vector3.forward * Speed;
    }
    private void ChangeLane(int direction)
    {
        int targetLane = currentLane + direction;
        if (targetLane < 0 || targetLane > 2) return;
        currentLane = targetLane;
        verticalTargetPosition = new Vector3((currentLane - 1), 0, 0);
    }
    private void Jump()
    {
        if(jumping == false)
        {
            JumpStart = transform.position.z;
            animator.SetFloat("JumpSpeed", Speed / JumpLength);
            animator.SetBool("Jumping", true);
            jumping = true;
        }
    }
    void slide()
    {
        if (!jumping && !isSliding)
        {
            slideStart = transform.position.z;
            animator.SetFloat("JumpSpeed", Speed / slideLength);
            animator.SetBool("Sliding", true);
            Vector3 newSize = boxCollider.size;
            newSize.y = newSize.y / 2;
            boxCollider.size = newSize;
            isSliding = true;
        }

    }
}
