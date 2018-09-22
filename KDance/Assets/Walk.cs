using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walk : MonoBehaviour {

    [SerializeField]
    float sideLimit = 1;
    [SerializeField]
    float speed = 0.01f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(Vector3.forward * speed, Space.Self);
        if (transform.position.x < -sideLimit) transform.position = new Vector3(sideLimit, transform.position.y, transform.position.z);
	}
}
