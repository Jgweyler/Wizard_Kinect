using UnityEngine;
using System.Collections;

public class KinectRecorder : MonoBehaviour {

	private KinectManager kinectManager;

	// Use this for initialization
	void Start () {
		kinectManager = KinectManager.Instance;
	}
	
	// Update is called once per frame
	void Update () {
		//kinectManager.Get
	}
}
