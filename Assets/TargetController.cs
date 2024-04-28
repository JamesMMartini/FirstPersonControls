using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField] int pointValue;
    [SerializeField] SetManager setManager;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Ball")
    //    {
    //        Debug.Log("ENTERED");

    //        setManager.AddPoints(pointValue);
    //    }
    //}

    public void Hit()
    {
        if (pointValue > 0)
        {
            setManager.AddPoints(pointValue);
        }
    }
}
