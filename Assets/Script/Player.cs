using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] private float speed;
    [SerializeField] private float laneSpeed;


    [Header("Jump")]
    [SerializeField] private float jumpLength;
    [SerializeField] private float jumpHeight;


    [Header("Slide")]
    [SerializeField] private float slideLength;


    [Header("Life")]
    [SerializeField] private int maxLife;
    [SerializeField] private float minSpeed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float invincibleTime;
    [SerializeField] private GameObject model;


    [HideInInspector] public float score;
    [HideInInspector] public int coins;


    private Rigidbody rb;
    private BoxCollider boxCollider;
    private Vector3 boxColliderSize;


    private Animator anim;
    private int currentLane = 1;
    private bool isJumping = false;
    private Vector3 verticalTargetPosition;
    private float jumpStart;
    private bool isSliding = false;


    private float slideStart;


    //life
    private int currentlife;
    private bool invincicle = false;
    private static int blinkingValue;




    // luot bang tay
    private bool isSwipping = false;
    private Vector2 startingTouch;




    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        boxCollider = GetComponent<BoxCollider>();
        boxColliderSize = boxCollider.size;
        anim.Play("runStart");
        
        speed = minSpeed;
        blinkingValue = Shader.PropertyToID("_BlinkingValue");
        currentlife = maxLife;
    }


    // Update is called once per frame
    void Update()
    {
        score += Time.deltaTime * speed;
        UImanager.instance.UpdateScore((int)score);
        // luot bang nut
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
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Slide();
        }
        // luot bang ngon tay
        #region control by hand
        if (Input.touchCount == 1)
        {
            if (isSwipping)
            {
                Vector2 diff = Input.GetTouch(0).position - startingTouch;
                diff = new Vector2(diff.x / Screen.width, diff.y / Screen.width);
                if (diff.magnitude > 0.01f)
                {
                    if (Mathf.Abs(diff.y) > Mathf.Abs(diff.x))
                    {
                        if (diff.y < 0)
                        {
                            Slide();
                        }
                        else
                        {
                            Jump();
                        }
                    }
                    else
                    {
                        if (diff.x < 0)
                        {
                            ChangeLane(-1);
                        }
                        else
                        {
                            ChangeLane(1);
                        }
                    }


                    isSwipping = false;
                }
            }
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                startingTouch = Input.GetTouch(0).position;
                isSwipping = true;
            }
            else if (Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                isSwipping = false;
            }
        }
        #endregion

        if (isJumping)
        {
            float ratio = (transform.position.z - jumpStart) / jumpLength;
            if (ratio >= 1f)
            {
                isJumping = false;
                anim.SetBool("Jumping", false);
            }
            else
            {
                verticalTargetPosition.y = Mathf.Sin(ratio * Mathf.PI) * jumpHeight;
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
                anim.SetBool("Sliding", false);
                boxCollider.size = boxColliderSize;
            }
        }


        Vector3 targetPosition = new Vector3(verticalTargetPosition.x, verticalTargetPosition.y, transform.position.z);
        this.transform.position = Vector3.MoveTowards(transform.position, targetPosition, laneSpeed * Time.deltaTime);
    }


    private void FixedUpdate()
    {
        rb.velocity = Vector3.forward * speed;
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
        if (!isJumping)
        {
            jumpStart = transform.position.z;
            anim.SetFloat("JumpSpeed", speed / jumpLength);
            anim.SetBool("Jumping", true);
            isJumping = true;
        }
    }


    private void Slide()
    {
        if (!isJumping && !isSliding)
        {
            slideStart = transform.position.z;
            anim.SetFloat("JumpSpeed", speed / slideLength);
            anim.SetBool("Sliding", true);
            Vector3 newSize = boxCollider.size;
            newSize.y = newSize.y / 2;
            boxCollider.size = newSize;
            isSliding = true;
        }
    }


    //Collision
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coins++;
            UImanager.instance.UpdateCoins(coins);
            other.transform.gameObject.SetActive(false);
        }

        if (invincicle) return;
        if (other.CompareTag("Obstacle"))
        {
            currentlife--;
            UImanager.instance.UpdateLives(currentlife);
            anim.SetTrigger("Hit");
            speed = 0;
            if (currentlife <= 0)
            {
                speed = 0;
                anim.SetBool("Dead", true);
                UImanager.instance.ShowGameOverPannel();
                Invoke("CallMenu", 2f);
                //gameover
            }
            else
            {
                StartCoroutine(Blinking(invincibleTime));
            }
        }
    }


    IEnumerator Blinking(float time)
    {
        invincicle = true;
        float timer = 0;
        float currentBlink = 1f;
        float lastBlink = 0;
        float blinkPeriod = 0.1f;
        bool enabled = false;
        yield return new WaitForSeconds(1f);
        speed = minSpeed;
        while (timer < time && invincicle)
        {
            model.SetActive(enabled);
            //Shader.SetGlobalFloat(blinkingValue, currentBlink);
            yield return null;
            timer += Time.deltaTime;
            lastBlink += Time.deltaTime;
            if (blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1f - currentBlink;
                enabled = !enabled;
            }
        }
        model.SetActive(true);
        //Shader.SetGlobalFloat(blinkingValue, 0);
        invincicle = false;
    }


    void CallMenu()
    {
        //GameManager.instance.EndRun();
    }


    public void IncreaseSpeed()
    {
        speed *= 1.15f;
        if (speed >= maxSpeed)
        {
            speed = maxSpeed;
        }
    }


}

    


