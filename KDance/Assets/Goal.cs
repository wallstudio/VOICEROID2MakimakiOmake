using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : MonoBehaviour {

    [SerializeField]
    GameObject player;
    [SerializeField]
    AudioClip fanfare;

    bool end = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(!end  && (player.transform.position - transform.position).sqrMagnitude < 0.3f)
        {
            end = true;
            gameObject.GetComponent<AudioSource>().PlayOneShot(fanfare);
            StartCoroutine(Reset());
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
	}

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene(0);
    }
}
