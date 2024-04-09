using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSetter : MonoBehaviour
{
    [SerializeField] Projectile ball;
    [SerializeField] float maxSetAngle;
    [SerializeField] float maxSetTimer;
    [SerializeField] float setSpeed;

    float setTimer;

    private void Start()
    {
        setTimer = maxSetTimer * 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (setTimer > 0)
        {
            setTimer -= Time.deltaTime;

            if (setTimer <= 0)
            {
                // Launch the ball
                float randomAngle = Random.Range(-maxSetAngle, maxSetAngle);

                Vector3 setDir = transform.up;
                setDir = Quaternion.AngleAxis(randomAngle, transform.forward) * setDir;

                ball.SetBall(setDir, setSpeed);
            }
        }
    }

    public void StartSet()
    {
        setTimer = maxSetTimer;
    }
}
