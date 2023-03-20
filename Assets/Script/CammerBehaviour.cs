using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CammerBehaviour : MonoBehaviour
{
    public Transform player;
    Transform cTransform;
    // Start is called before the first frame update
    void Start()
    {
        cTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        cTransform.LookAt(player);
    }
}
