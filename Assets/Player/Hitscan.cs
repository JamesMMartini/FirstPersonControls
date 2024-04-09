using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Hitscan : MonoBehaviour
{
    [SerializeField] Transform cameraHolder;
    [SerializeField] LayerMask hitscanLayerMask;
    [SerializeField] float aimAssistLevel = 2f;
    [SerializeField] GameObject hitscanLineViz;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HitscanInput(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            GameObject line = Instantiate(hitscanLineViz, cameraHolder.position + (cameraHolder.forward * 150) - (cameraHolder.up * 0.5f), cameraHolder.rotation);
            line.transform.up = cameraHolder.forward;

            //List<GameObject> objectsHit = new List<GameObject>();
            if (!HitscanRaycast(cameraHolder.position, cameraHolder.forward))
            {
                // aim assist
                HitscanSpherecast(cameraHolder.position, cameraHolder.forward);
            }
        }
    }

    bool HitscanSpherecast(Vector3 origin, Vector3 direction)
    {
        if (direction == null) direction = cameraHolder.forward;

        RaycastHit hit;
        if (Physics.SphereCast(origin, aimAssistLevel, direction, out hit, Mathf.Infinity, hitscanLayerMask))
        {
            return EvaluateObjectHit(hit);
        }

        return false;
    }

    bool HitscanRaycast(Vector3 origin, Vector3 direction)
    {
        if (direction == null) direction = cameraHolder.forward;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, hitscanLayerMask))
        {
            return EvaluateObjectHit(hit);
        }

        return false;
    }

    bool EvaluateObjectHit(RaycastHit hit)
    {
        if (hit.collider.gameObject.tag == "Target")
        {
            Debug.Log(hit.collider.name);
            return true;
        }

        return false;
    }
}
