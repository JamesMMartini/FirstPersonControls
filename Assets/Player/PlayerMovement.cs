using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float airDrag;
    [SerializeField] float maxRotAmount;

    [SerializeField] float gravity, jumpHeight, ballTrackSpeed;

    [SerializeField] Transform cameraTransform, lookDirection;

    [SerializeField] Animator anim;

    [SerializeField] BaseCameraFollow camFollow;

    //MouseLook mouseLook;

    float jumpVelocity;
    bool jumpTriggered;
    bool lockMovement;

    CharacterController characterController;

    float moveX;
    float moveY;

    Vector3 velocity, velocityInput, velocityGravity, velocitySpecial;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        //mouseLook = GetComponent<MouseLook>();

        Cursor.lockState = CursorLockMode.Locked;

        jumpVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
    }

    // Update is called once per frame
    void Update()
    {
        if (jumpTriggered && characterController.isGrounded)
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Vector3 viewDirection = transform.position - new Vector3(cameraTransform.position.x, transform.position.y, cameraTransform.position.z);
        lookDirection.forward = viewDirection.normalized;

        //transform.forward = Vector3.Slerp(transform.forward, lookDirection.forward, 10 * Time.deltaTime);

        velocityInput = lookDirection.forward * moveY + lookDirection.right * moveX;
        velocityInput.Normalize();
        velocityInput *= speed;

        if (velocityInput != Vector3.zero && !anim.GetBool("Running"))
        {
            anim.SetBool("Running", true);
        }
        else if (velocityInput == Vector3.zero && anim.GetBool("Running"))
        {
            anim.SetBool("Running", false);
        }

        velocityGravity.y -= gravity * Time.fixedDeltaTime;

        if (velocitySpecial.x > 0)
            velocitySpecial.x -= airDrag * Time.deltaTime;
        else if (velocitySpecial.x < 0)
            velocitySpecial.x += airDrag * Time.deltaTime;

        if (velocitySpecial.z > 0)
            velocitySpecial.z -= airDrag * Time.deltaTime;
        else if (velocitySpecial.z < 0)
            velocitySpecial.z += airDrag * Time.deltaTime;

        if (Mathf.Abs(velocitySpecial.x) < 0.1f)
            velocitySpecial.x = 0f;

        if (Mathf.Abs(velocitySpecial.z) < 0.1f)
            velocitySpecial.z = 0f;

        //if (characterController.isGrounded && mouseLook.lookAtBall && velocityGravity.y < -0.1f)
        //    mouseLook.lookAtBall = false;

        if (characterController.isGrounded && velocityGravity.y < 0)
            velocityGravity.y = -0.1f;

        if (characterController.isGrounded && velocitySpecial != Vector3.zero && velocityGravity.y < -0.1f)
            velocitySpecial = Vector3.zero;

        if (!lockMovement && characterController.isGrounded)
        {
            velocity = velocityInput + velocityGravity + velocitySpecial;
            transform.forward = Vector3.RotateTowards(transform.forward, velocityInput.normalized, Mathf.Deg2Rad * maxRotAmount, 0f);
        }
        else
        {
            velocity = Vector3.zero + velocityGravity + velocitySpecial;

            if (anim.GetBool("Running"))
            {
                anim.SetBool("Running", false);
            }
        }

        characterController.Move(velocity * Time.fixedDeltaTime);
    }

    void Jump()
    {
        Transform ball = GameObject.FindGameObjectWithTag("Ball").GetComponent<Transform>();
        Vector3 finalLookTarget = ball.position;

        Vector3 toBall = ball.position - transform.position;
        toBall.y = 0f;
        transform.forward = toBall.normalized;

        //if (Vector3.SignedAngle(ball.forward, ball.position - mouseLook.cameraHolder.position, Vector3.up) < 0)
        //{
        //    finalLookTarget = ball.position - (ball.right * 0.5f);
        //}
        //else
        //{
        //    finalLookTarget = ball.position + (ball.right * 0.5f);
        //}

        //mouseLook.lookAtBall = true;
        //StartCoroutine(LookAtBall(finalLookTarget, ball));
        //mouseLook.LookAtObject(ball.position, ball);

        velocitySpecial = (ball.position - transform.position).normalized * jumpVelocity * 0.5f;
        velocitySpecial.y = 0f;

        velocityGravity.y = jumpVelocity;
        jumpTriggered = false;
    }

    //IEnumerator LookAtBall(Vector3 lookTarget, Transform objTarget)
    //{
    //    Vector3 startLookPos = mouseLook.cameraHolder.position + (mouseLook.cameraHolder.forward * (lookTarget - mouseLook.cameraHolder.position).magnitude);
    //    mouseLook.ball = null;

    //    float t = 0f;
    //    while (t < 1)
    //    {
    //        Vector3 newLookPos = Vector3.Lerp(startLookPos, lookTarget, t);

    //        Quaternion newLookQuat = Quaternion.LookRotation((newLookPos - mouseLook.cameraHolder.position).normalized);
    //        Vector3 newLook = newLookQuat.eulerAngles;

    //        // Make the player and camera rotate correctly
    //        mouseLook.LookAtDir(newLook);

    //        t += Time.deltaTime / ballTrackSpeed;

    //        yield return new WaitForEndOfFrame();
    //    }

    //    mouseLook.LookAtObject(objTarget.position, objTarget);
    //}

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 movementInput = context.ReadValue<Vector2>();
            moveX = movementInput.x;
            moveY = movementInput.y;
        }
        else if (context.canceled)
        {
            moveX = 0f;
            moveY = 0f;
        }
    }

    public void JumpInput(InputAction.CallbackContext context)
    {
        if (context.performed && characterController.isGrounded)
        {
            //jumpTriggered = true;
            lockMovement = true;
        }
        else if (context.canceled && characterController.isGrounded)
        {
            jumpTriggered = true;
            lockMovement = false;
        }
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 lookInput = context.ReadValue<Vector2>();

            camFollow.lookX = lookInput.x;
            camFollow.lookY = lookInput.y;
        }
        else if (context.canceled)
        {
            camFollow.lookX = 0f;
            camFollow.lookY = 0f;
        }
    }

    public void Restart(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void Quit(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Application.Quit();
        }
    }
}
