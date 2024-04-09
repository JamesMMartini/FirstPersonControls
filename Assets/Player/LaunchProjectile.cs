using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaunchProjectile : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float slowdown;
    MouseLook mouseLook;

    float baseFixedDeltaTime;
    bool projectileTriggered;

    [SerializeField] Transform cameraHolder;

    // Start is called before the first frame update
    void Start()
    {
        baseFixedDeltaTime = Time.fixedDeltaTime;
        mouseLook = GetComponent<MouseLook>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Launch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Time.timeScale = slowdown;
            Time.fixedDeltaTime = baseFixedDeltaTime * slowdown;

            if (mouseLook.ball != null)
            {
                mouseLook.LookAtObject(mouseLook.ball.position, mouseLook.ball);
                mouseLook.SetJumpStartRot();
            }

            //projectileTriggered = true;

            //Instantiate(projectilePrefab, cameraHolder.position + cameraHolder.forward * 2, cameraHolder.rotation);
        }
        else if (context.canceled)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = baseFixedDeltaTime;

            mouseLook.HitBall();

            mouseLook.ball = null;
        }
    }
}
