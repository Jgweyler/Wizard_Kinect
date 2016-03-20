using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private GameObject player;
    private Vector3 height;
    float distance = 3f;
    
    void Awake() {
        height = new Vector3(0, 0.8f, 0);
        player = GameObject.FindGameObjectWithTag("Player");
    }

	void Start () {

    }
	
	// Update is called once per frame
	void LateUpdate () {

        transform.rotation = player.transform.rotation;
        transform.position = player.transform.position + distance * -transform.forward + height;
    }

}
