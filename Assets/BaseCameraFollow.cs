using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseCameraFollow : MonoBehaviour
{
    [Header("Base Values")]
    [SerializeField] Transform PointOne;
    [SerializeField] Transform MidPoint;
    [SerializeField] Transform PointTwo;
    [SerializeField] float lookBetweenRatio;

    [Header("Targeting Values")]
    [SerializeField] float targetSpeed;
    [SerializeField] Transform Targeting_1;
    [SerializeField] Transform Targeting_2;
    [SerializeField] float mouseSensitivity;
    [SerializeField] float maxShotLookOffset;
    [SerializeField] GameObject dropShadow;
    [SerializeField] LayerMask dropShadowLayerMask;
    [SerializeField] float maxHitDistance;
    [SerializeField] float hitMaxForceRange;
    [SerializeField] AudioSource hitSFX;
    List<Vector3> storedLookTargets;

    [Header("General Objects")]
    [SerializeField] Transform CameraTarget;
    [SerializeField] Transform Ball;
    [SerializeField] GameObject reticle;
    Material ballMat;

    public bool targeting;

    bool zooming;
    PlayerMovement player;

    public float lookX;
    public float lookY;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindObjectOfType<PlayerMovement>();
        ballMat = Ball.GetComponent<MeshRenderer>().material;
        storedLookTargets = new List<Vector3>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!zooming)
        {
            if (!targeting)
            {
                if (reticle.activeInHierarchy)
                    reticle.SetActive(false);

                // Find where between the two points the player is on the x axis
                float playerProgress = (player.transform.position.x - PointOne.position.x) / (PointTwo.position.x - PointOne.position.x);

                playerProgress = Mathf.Clamp(playerProgress, 0f, 1f);

                // Get the bezier position based on the player's progress
                Vector3 newPosition = MidPoint.position + (Mathf.Pow(1 - playerProgress, 2) * (PointOne.position - MidPoint.position)) +
                    (Mathf.Pow(playerProgress, 2) * (PointTwo.position - MidPoint.position));

                transform.position = newPosition;

                Vector3 lookPos = CameraTarget.position + (Ball.position - CameraTarget.position) * lookBetweenRatio;

                transform.LookAt(lookPos);
            }
            else
            {
                if (!reticle.activeInHierarchy)
                    reticle.SetActive(true);

                // set the camera's position
                float finalProgress = 1 - ((Ball.position.x - Targeting_1.position.x) / (Targeting_2.position.x - Targeting_1.position.x));
                finalProgress = Mathf.Clamp(finalProgress, 0f, 1f);

                Vector3 finalCameraPos = Targeting_1.position + (finalProgress * (Targeting_2.position - Targeting_1.position));
                transform.position = finalCameraPos;

                if (!dropShadow.activeInHierarchy)
                    dropShadow.SetActive(true);

                Vector3 lookPos = CameraTarget.position + (Ball.position - CameraTarget.position) * lookBetweenRatio;
                transform.LookAt(lookPos);

                float moddedXRot = transform.rotation.eulerAngles.x - (lookY * maxShotLookOffset);
                float moddedYRot = transform.rotation.eulerAngles.y + (lookX * maxShotLookOffset);
                transform.localRotation = Quaternion.Euler(moddedXRot, moddedYRot, transform.rotation.eulerAngles.z);

                storedLookTargets.Add(transform.forward);
                if (storedLookTargets.Count > 10)
                    storedLookTargets.RemoveAt(0);

                Vector3 averageLookTarget = Vector3.zero;
                foreach (Vector3 storedDir in storedLookTargets)
                    averageLookTarget += storedDir;

                averageLookTarget = averageLookTarget / storedLookTargets.Count;

                transform.forward = averageLookTarget;

                // Place the drop shadow as necessary
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, dropShadowLayerMask))
                {
                    dropShadow.transform.position = hit.point;
                }

                // update ball color
                float distToBall = (Ball.position - CameraTarget.position).magnitude;
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
                    ballMat.color = new Color(1 - (distToBall / maxHitDistance), 0f, 0f);
                }
            }
        }
    }

    public void StartTargetMode()
    {
        targeting = true;
        zooming = true;

        StartCoroutine(StartTargeting());
    }

    IEnumerator StartTargeting()
    {
        Vector3 startingPos = transform.position;

        float t = 0f;
        while (t <= 1)
        {
            // set the camera's position
            float ballProgress = 1 - ((Ball.position.x - Targeting_1.position.x) / (Targeting_2.position.x - Targeting_1.position.x));
            ballProgress = Mathf.Clamp(ballProgress, 0f, 1f);

            Vector3 targetCameraPos = Targeting_1.position + (ballProgress * (Targeting_2.position - Targeting_1.position));

            transform.position = Vector3.Lerp(startingPos, targetCameraPos, t);

            Vector3 lookPos = CameraTarget.position + (Ball.position - CameraTarget.position) * lookBetweenRatio;
            transform.LookAt(lookPos);

            t += Time.unscaledDeltaTime / targetSpeed;
            yield return new WaitForEndOfFrame();
        }

        // set the camera's position
        float finalProgress = 1 - ((Ball.position.x - Targeting_1.position.x) / (Targeting_2.position.x - Targeting_1.position.x));
        finalProgress = Mathf.Clamp(finalProgress, 0f, 1f);

        Vector3 finalCameraPos = Targeting_1.position + (finalProgress * (Targeting_2.position - Targeting_1.position));
        transform.position = finalCameraPos;

        Vector3 finalLook = CameraTarget.position + (Ball.position - CameraTarget.position) * lookBetweenRatio;
        transform.LookAt(finalLook);

        player.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;

        zooming = false;
    }

    public void StopTargetMode()
    {
        targeting = false;
        zooming = true;

        StartCoroutine(StopTargeting());
    }

    IEnumerator StopTargeting()
    {
        player.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;

        Vector3 startingPos = transform.position;

        float t = 0f;
        while (t <= 1)
        {
            // Find where between the two points the player is on the x axis
            float playerProgress = (player.transform.position.x - PointOne.position.x) / (PointTwo.position.x - PointOne.position.x);

            playerProgress = Mathf.Clamp(playerProgress, 0f, 1f);

            // Get the bezier position based on the player's progress
            Vector3 newPosition = MidPoint.position + (Mathf.Pow(1 - playerProgress, 2) * (PointOne.position - MidPoint.position)) +
                (Mathf.Pow(playerProgress, 2) * (PointTwo.position - MidPoint.position));

            transform.position = Vector3.Lerp(startingPos, newPosition, t);

            Vector3 lookPos = CameraTarget.position + (Ball.position - CameraTarget.position) * lookBetweenRatio;
            transform.LookAt(lookPos);

            t += Time.unscaledDeltaTime / targetSpeed;
            yield return new WaitForEndOfFrame();
        }

        // Find where between the two points the player is on the x axis
        float finalProgress = (player.transform.position.x - PointOne.position.x) / (PointTwo.position.x - PointOne.position.x);

        finalProgress = Mathf.Clamp(finalProgress, 0f, 1f);

        // Get the bezier position based on the player's progress
        Vector3 finalCameraPos = MidPoint.position + (Mathf.Pow(1 - finalProgress, 2) * (PointOne.position - MidPoint.position)) +
            (Mathf.Pow(finalProgress, 2) * (PointTwo.position - MidPoint.position));
        transform.position = finalCameraPos;

        Vector3 finalLook = CameraTarget.position + (Ball.position - CameraTarget.position) * lookBetweenRatio;
        transform.LookAt(finalLook);

        zooming = false;
    }

    public void HitBall()
    {
        if (Ball != null)
        {
            // calculate the force of the hit
            float distToBall = (Ball.position - CameraTarget.position).magnitude;
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

            Vector3 direction = (dropShadow.transform.position - Ball.position).normalized;

            hitSFX.Play();
            Ball.GetComponent<Projectile>().Fire(direction, hitForce);

            zooming = true;
        }
    }
}
