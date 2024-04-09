using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    [SerializeField] public Transform cameraHolder;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float maxShotLookOffset;
    [SerializeField] GameObject dropShadow;
    [SerializeField] LayerMask dropShadowLayerMask;
    [SerializeField] float maxHitDistance;
    [SerializeField] float hitMaxForceRange;
    [SerializeField] AudioSource hitSFX;
    [SerializeField] PlayerInput inputActions;

    public bool lookAtBall;

    CharacterController characterController;

    public Transform ball;
    Material ballMat;
    Vector3 lookTarget;

    float lookX;
    float lookY;
    float xRotation;
    float jumpStartYRot;

    float storedModdedXRot;
    float storedModdedYRot;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        float inputHorizontalThisFrame = lookX * mouseSensitivity * Time.deltaTime;
        float inputVerticalThisFrame = lookY * mouseSensitivity * Time.deltaTime;

        if (!lookAtBall)
        {
            if (storedModdedXRot != 0f)
                storedModdedXRot = 0f;

            if (storedModdedYRot != 0f)
                storedModdedYRot = 0f;

            if (dropShadow.activeInHierarchy)
                dropShadow.SetActive(false);

            // x rotation: up/down
            xRotation -= inputVerticalThisFrame;

            //xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            cameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);

            // y rotation: left/right
            transform.Rotate(Vector3.up, inputHorizontalThisFrame);
        }
        else if (ball != null)
        {
            if (!dropShadow.activeInHierarchy)
                dropShadow.SetActive(true);

            //LookAtObject(lookTarget, ball);

            float moddedXRot = xRotation - lookY * maxShotLookOffset;

            if (inputActions.currentControlScheme != "Keyboard&Mouse")
            {
                cameraHolder.localRotation = Quaternion.Euler(moddedXRot, 0, 0);
            }
            else
            {
                storedModdedXRot += moddedXRot;
                cameraHolder.localRotation = Quaternion.Euler(moddedXRot, 0, 0);
            }

            // x rotation: up/down
            //xRotation -= (inputVerticalThisFrame * 50f);

            //xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            //cameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);

            // y rotation: left/right
            if (inputActions.currentControlScheme != "Keyboard&Mouse")
            {
                Vector3 newRot = new Vector3(0f, jumpStartYRot + (lookX * maxShotLookOffset), 0f);
                transform.rotation = Quaternion.Euler(newRot);
            }
            else
            {
                storedModdedYRot += (lookX * maxShotLookOffset);
                Vector3 newRot = new Vector3(0f, jumpStartYRot + storedModdedYRot, 0f);
                transform.rotation = Quaternion.Euler(newRot);
            }

            //transform.Rotate(Vector3.up, (inputHorizontalThisFrame * 50f));

            // Place the drop shadow as necessary
            RaycastHit hit;
            if (Physics.Raycast(cameraHolder.position, cameraHolder.forward, out hit, Mathf.Infinity, dropShadowLayerMask))
            {
                dropShadow.transform.position = hit.point;
            }

            // update ball color
            float distToBall = (ball.position - cameraHolder.position).magnitude;
            if (distToBall < hitMaxForceRange)
            {
                ballMat.color = new Color(1f, 0f, 0f);
            }
            else if (distToBall > maxHitDistance)
            {
                ballMat.color = new Color(0.25f, 0.25f, 0.25f);
            }
            else
            {
                ballMat.color = new Color(1 - (distToBall /maxHitDistance), 0f, 0f);
            }

            //transform.Rotate(Vector3.up, lookY);

            //Vector3 newRot = transform.rotation.eulerAngles;
            //newRot.y += lookY * maxShotLookOffset;
            //transform.rotation = Quaternion.Euler(newRot);
        }
    }

    public void HitBall()
    {
        if (ball != null)
        {
            // calculate the force of the hit
            float distToBall = (ball.position - cameraHolder.position).magnitude;
            float hitForce = 0f;
            if (distToBall < hitMaxForceRange)
            {
                hitForce = 1f;
            }
            else if (distToBall > maxHitDistance)
            {
                hitForce = 0f;
            }
            else
            {
                hitForce = 1 - (distToBall / maxHitDistance);
            }

            Vector3 direction = (dropShadow.transform.position - ball.position).normalized;

            hitSFX.Play();
            ball.GetComponent<Projectile>().Fire(direction, hitForce);
        }
    }

    public void LookAtDir(Vector3 lookDir)
    {
        //Quaternion lookRot = Quaternion.LookRotation(lookDir);
        //Vector3 lookVec = lookRot.eulerAngles;

        xRotation = lookDir.x;
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.rotation = Quaternion.Euler(new Vector3(0f, lookDir.y, 0f));
    }

    public void LookAtObject(Vector3 target, Transform obj)
    {
        lookTarget = target;
        ball = obj;
        
        if (ballMat == null)
            ballMat = ball.GetComponent<MeshRenderer>().material;

        Vector3 lookDir = target - cameraHolder.position;

        Quaternion lookRot = Quaternion.LookRotation(lookDir);
        Vector3 lookVec = lookRot.eulerAngles;

        xRotation = lookVec.x;

        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0, 0);

        transform.rotation = Quaternion.Euler(new Vector3(0f, lookVec.y, 0f));
    }

    public void Look(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector2 lookInput = context.ReadValue<Vector2>();

            lookX = lookInput.x;
            lookY = lookInput.y;
        }
        else if (context.canceled)
        {
            lookX = 0f;
            lookY = 0f;
        }
    }

    public void SetJumpStartRot()
    {
        jumpStartYRot = transform.rotation.eulerAngles.y;
    }
}
