using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
    public GameObject player;

    float distance = 3f;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void LateUpdate () {

        transform.rotation = player.transform.rotation;
        transform.position = player.transform.position + distance * -transform.forward;
    }

}
