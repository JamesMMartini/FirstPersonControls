using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 20f;
    [SerializeField] float shotTimer;
    [SerializeField] float gravity;
    [SerializeField] float bounceLoss;
    [SerializeField] AudioSource hitSFX;
    [SerializeField] LayerMask projectileMask;

    Vector3 startPos;
    float moveSpeed;
    Rigidbody rb;
    float timer;
    Vector3 gravityVec;
    bool wasHit;
    bool targetHit;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (moveSpeed != 0f)
        {
            Vector3 moveDir = transform.forward * moveSpeed * Time.fixedDeltaTime;

            Vector3 finalDir = new Vector3(moveDir.x, moveDir.y + (gravityVec.y * Time.fixedDeltaTime), moveDir.z);

            RaycastHit hit;
            if (Physics.SphereCast(transform.position, transform.localScale.x / 2f, finalDir, out hit, finalDir.magnitude, projectileMask))
            {
                transform.forward = Vector3.Reflect(finalDir, hit.normal);
                moveSpeed *= bounceLoss;
                gravityVec.y = 0f;

                TargetController target = hit.collider.GetComponent<TargetController>();
                if (target != null && !targetHit)
                {
                    targetHit = true;
                    target.Hit();
                }

                if (wasHit)
                {
                    GameObject.FindObjectOfType<BaseCameraFollow>().StopTargetMode();
                    wasHit = false;
                    hitSFX.Play();
                }

                // bounce so set the timer
                if (timer == 0f)
                {
                    timer = shotTimer;
                }
            }
            else
            {
                gravityVec.y -= gravity;
            }

            rb.MovePosition(transform.position + finalDir);
        }

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                ResetBall();
            }
        }
    }

    public void Fire(Vector3 direction, float speedMod)
    {
        if (speedMod != 0f)
        {
            transform.forward = direction;
            moveSpeed = speed * speedMod;
            gravityVec = Vector3.zero;
            wasHit = true;
            targetHit = false;
        }
    }

    public void SetBall(Vector3 direction, float setSpeed)
    {
        transform.forward = direction;
        moveSpeed = setSpeed;
        gravityVec = Vector3.zero;
    }

    void ResetBall()
    {
        moveSpeed = 0f;
        transform.position = startPos;
        timer = 0f;
        gravityVec = Vector3.zero;
        GetComponent<MeshRenderer>().material.color = Color.white;
        wasHit = false;

        BallSetter setter = GameObject.FindObjectOfType<BallSetter>();
        setter.StartSet();
    }
}
