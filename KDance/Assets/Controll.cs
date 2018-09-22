using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controll : MonoBehaviour {

    [SerializeField]
    float speed = 0.01f;
    [SerializeField]
    float speed2 = 0.01f;

    CharacterController controller;
    Transform camera;
    bool isPause;

    // Use this for initialization
    void Start () {
        controller = gameObject.GetComponent<CharacterController>();
        camera = gameObject.GetComponentInChildren<Camera>().transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            controller.Move(transform.TransformDirection(Vector3.forward * speed));
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            controller.Move(transform.TransformDirection(Vector3.back * speed));
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            controller.Move(transform.TransformDirection(Vector3.right * speed));
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            controller.Move(transform.TransformDirection(Vector3.left * speed));

        transform.Rotate(Input.GetAxis("Mouse X") * speed2 * Vector3.up);
        camera.transform.Rotate(Input.GetAxis("Mouse Y") * speed2 * Vector3.left);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
            //isPause = !isPause;

            //Cursor.visible = !isPause;
            //Cursor.lockState = isPause ? CursorLockMode.Locked : CursorLockMode.None;
        }

        if (Input.GetKey(KeyCode.C))
        {
            transform.position = new Vector3(-27, 5, -40);
        }
    }
}
