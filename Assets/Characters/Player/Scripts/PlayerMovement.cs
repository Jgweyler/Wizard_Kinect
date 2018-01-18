using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float speed = 12f;             //Speed (For moves front and back).
    public float turnSpeed = 180f;        // Turn Speed
	public float moveSpeed_Kinect = 0.5f; // Turn speed with kinect
	private float turnSpeedKinect = 45f;  // Velocidad de giro usando Kinect.
	private const int LEFT = 0;
	private const int RIGHT = 1;

    private Rigidbody rigidBody;
    private string movementAxisName;
    private string movementTurnAxisName;
    private float movementInputValue;
    private float turnInputValue;
    private Quaternion rotation; // Se usará para indicar a la cámara cuánto tiene que girar.

    //Variables necesarias para la fijación de objetivo y combate.
    private bool hasTarget;
    private GameObject playerTarget;
	private Animator playerAnim;
    private float speedInCombat; // Speen while the player is in combat..
    private MakeTarget makeTargetScript;

	private KinectManager kinectManagerScript;
	private bool isDetectingUserWithKinect; // Checks if kinect sensor is available.


    // Use this for initialization
    private void Start () {
        rigidBody = GetComponent< Rigidbody >();
		playerAnim = GetComponent<Animator> ();
        makeTargetScript = GetComponent<MakeTarget>();
		kinectManagerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<KinectManager> ();
		isDetectingUserWithKinect = kinectManagerScript.IsUserDetected();
        //Set input axis name.
        movementAxisName = "Vertical";
        movementTurnAxisName = "Horizontal";
        //Set default Input values from start (0).
        movementInputValue = 0f;
        turnInputValue = 0f;
        speedInCombat = 0.50f;

        hasTarget = false;

    }
	
	// Update is called once per frame
	private void Update () {
        movementInputValue = Input.GetAxis(movementAxisName);
        turnInputValue = Input.GetAxis(movementTurnAxisName);

		isDetectingUserWithKinect = kinectManagerScript.IsUserDetected ();

		if (!isDetectingUserWithKinect) { //If kinect does not detect an user, set keyboard control.
			if (isMovingForward ()) {
				playerAnim.SetBool ("isMoving", true);
			} else if (isMovingBackwards ()) {
				playerAnim.SetBool ("goBack", true);
			} else {
				playerAnim.SetBool ("isMoving", false);
				playerAnim.SetBool ("goBack", false);
			}

			if (turnInputValue != 0) {
				playerAnim.SetBool ("isTurning", true);
			} else {
				playerAnim.SetBool ("isTurning", false);
			}
		}
	}

	private bool isMovingForward(){
		if (movementInputValue > 0)
			return true;
		else
			return false;
	}

	private bool isMovingBackwards(){
		if(movementInputValue < 0)
			return true;
		else
			return false;
	}
    private void FixedUpdate() {

        if (!hasTarget){
            move();
            turn();
        }
        else{
            combatMove();
        }
    }

    private void move(){
       Vector3 movement = transform.forward * movementInputValue * speed * Time.deltaTime;
       rigidBody.MovePosition(rigidBody.position + movement);
	   
    }

    private void turn()
    {
        // Calculate the turn value
        float turn = turnInputValue * turnSpeed * Time.deltaTime;
        // Make this into a rotation in the y axis.
        rotation = Quaternion.Euler(0f, turn, 0f);

        // Apply rotation to rigidbody
        rigidBody.MoveRotation(rigidBody.rotation * rotation);
    }

	//-------------------------------------------------------------------------KINECT FUNCTIONS!!!!------------------------------------------------------------
	public void goBack_Kinect(){

		if(!playerAnim.GetBool("goBack")){ 
			playerAnim.SetBool("goBack", true);
		}
		if(playerTarget != null){ //Remove the target if the player wants to go back.
			makeTargetScript.deleteTarget();
		}
		Vector3 movement = transform.forward * (-moveSpeed_Kinect) * speed * Time.deltaTime;
		rigidBody.MovePosition(rigidBody.position + movement);
	}

	public void moveForward_Kinect(){
		if(!playerAnim.GetBool("isMoving")){ 
			playerAnim.SetBool("isMoving", true);
			Debug.Log("Correr pasa a true!");
		}
		Vector3 movement = transform.forward * moveSpeed_Kinect * speed * Time.deltaTime;
		rigidBody.MovePosition(rigidBody.position + movement);
	}
	public void turnRight_Kinect(){
		if (!playerAnim.GetBool ("isMoving"))
			playerAnim.SetBool ("isTurning", true);
		float turn = turnSpeedKinect * Time.deltaTime;
		rotation = Quaternion.Euler (0f, turn, 0f);
		rigidBody.MoveRotation (rigidBody.rotation * rotation);
	}

	public void turnLeft_Kinect(){
		if (!playerAnim.GetBool ("isMoving"))
			playerAnim.SetBool ("isTurning", true);
		float turn = -1.0f * (turnSpeedKinect * Time.deltaTime);
		rotation = Quaternion.Euler (0f, turn, 0f);
		rigidBody.MoveRotation (rigidBody.rotation * rotation);
	}

	public void combatMoveKinect(int side){ //0 left, 1 right
		if (playerTarget == null)
		{
			makeTargetScript.deleteTarget();
			return;
		}
		Vector3 newPosition = new Vector3 (playerTarget.transform.position.x - transform.position.x, transform.position.y, playerTarget.transform.position.z - transform.position.z);
		//Quaternion targetRotation = Quaternion.LookRotation(playerTarget.transform.position - transform.position);
		Quaternion targetRotation = Quaternion.LookRotation(newPosition);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);

		float move;
		if (side == LEFT) {
			move = -speedInCombat;
		} else {
			move = speedInCombat;
		}
		transform.Translate(move, 0, 0);
	}
	//----------------------------------------------------------------------END KINECT FUNCTIONS-----------------------------------------------------------------

    public void combatMove() { //The player movement changes when he's in combat.

        //If the player wants to go back -> delete player target if exists
        if (playerTarget == null || movementInputValue < 0)
        {
            makeTargetScript.deleteTarget();
            return;
        }

		Vector3 newPosition = new Vector3 (playerTarget.transform.position.x - transform.position.x, transform.position.y, playerTarget.transform.position.z - transform.position.z);
		Quaternion targetRotation = Quaternion.LookRotation(newPosition);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);
        float move = turnInputValue * speedInCombat;
		if (move != 0f) {
			playerAnim.SetBool ("isMovingInCombat", true);
		} else {
			playerAnim.SetBool ("isMovingInCombat", false);
		}
            transform.Translate(move, 0, 0);

    }

    public void setHasTarget(bool hasTarg)
    {
        hasTarget = hasTarg;
    }

    public void setTarget(GameObject tg)
    {
        playerTarget = tg;
    }

}
