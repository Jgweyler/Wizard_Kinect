using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private GameObject player;
    float distance = 3f;

    void Awake() {
        player = GameObject.FindGameObjectWithTag("Player");
    }

	void Start () {

    }
	
	// Update is called once per frame
	void LateUpdate () {

        transform.rotation = player.transform.rotation;
        transform.position = player.transform.position + distance * -transform.forward;
    }

}
