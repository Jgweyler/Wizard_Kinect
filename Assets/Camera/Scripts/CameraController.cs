using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    private GameObject player;
    private Vector3 height;
    float distance = 30f;
    
    void Awake() {
		//transform.position = new Vector3(-0.4f, 17.2f, -23.86f);
        height = new Vector3(0, 17.2f, 0);
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
