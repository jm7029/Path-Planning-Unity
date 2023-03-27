using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    Ray ray;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.DrawRay(this.transform.position, Vector3.forward * 15f, Color.red);
        Debug.DrawRay(this.transform.position, Vector3.right * 15f, Color.red);
        Debug.DrawRay(this.transform.position, Vector3.left * 15f, Color.red);
        Debug.DrawRay(this.transform.position, Vector3.back * 15f, Color.red);
    }
}
