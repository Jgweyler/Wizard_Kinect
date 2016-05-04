using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float speed = 12f;       //Velocidad (Movimiento hacia alante y atrás).
    public float turnSpeed = 180f;  //Velocidad de giro
	public float moveSpeed_Kinect = 0.5f;
	private float turnSpeedKinect = 45f; // Velocidad de giro usando Kinect.
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
    private float speedInCombat; //Velocidad de movimiento en combate.
    private MakeTarget makeTargetScript;


    // Use this for initialization
    private void Start () {
        rigidBody = GetComponent< Rigidbody >();
		playerAnim = GetComponent<Animator> ();
        makeTargetScript = GetComponent<MakeTarget>();
        //Inicializamos los gestores del input y los referenciamos con un nombre.
        movementAxisName = "Vertical";
        movementTurnAxisName = "Horizontal";
        //El valor de giro y de movimiento deberán estar a cero.
        movementInputValue = 0f;
        turnInputValue = 0f;
        speedInCombat = 0.10f;

        hasTarget = false;

    }
	
	// Update is called once per frame
	private void Update () {
        movementInputValue = Input.GetAxis(movementAxisName);
        turnInputValue = Input.GetAxis(movementTurnAxisName);

		if (movementInputValue > 0) {
			playerAnim.SetBool ("isMoving", true);
		} else {
			playerAnim.SetBool ("isMoving", false);
		}

		if (turnInputValue != 0) {
			playerAnim.SetBool ("isTurning", true);
		} else {
			playerAnim.SetBool ("isTurning", false);
		}
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
        // Determina el numero de grados que gira basado en la entrada, velocidad y tiempo entre frames.
        float turn = turnInputValue * turnSpeed * Time.deltaTime;
        // Make this into a rotation in the y axis.
        rotation = Quaternion.Euler(0f, turn, 0f);

        // Aplicamos la rotación al rigidbody.
        rigidBody.MoveRotation(rigidBody.rotation * rotation);
    }

	//Funciones de Kinect
	public void goBack_Kinect(){
		if(playerTarget != null){ //Si tiene objetivo asignado, hay que quitarlo.
			makeTargetScript.deleteTarget();
		}
		Vector3 movement = transform.forward * (-moveSpeed_Kinect) * speed * Time.deltaTime;
		rigidBody.MovePosition(rigidBody.position + movement);
	}

	public void moveForward_Kinect(){
		Vector3 movement = transform.forward * moveSpeed_Kinect * speed * Time.deltaTime;
		rigidBody.MovePosition(rigidBody.position + movement);
	}
	public void turnRight_Kinect(){
		float turn = turnSpeedKinect * Time.deltaTime;
		rotation = Quaternion.Euler (0f, turn, 0f);
		rigidBody.MoveRotation (rigidBody.rotation * rotation);
	}

	public void turnLeft_Kinect(){
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
		Quaternion targetRotation = Quaternion.LookRotation(playerTarget.transform.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, speed * Time.deltaTime);

		float move;
		if (side == LEFT) {
			move = -speedInCombat;
		} else {
			move = speedInCombat;
		}
		transform.Translate(move, 0, 0);
	}
	//Fin de las funciones con Kinect.

    public void combatMove() { //El movimiento del personaje cambia a modo de combate si fija objetivo

        //Si el jugador quiere retroceder o destruye a su objetivo, automáticamente se cancela la fijación de objetivo.
        if (playerTarget == null || movementInputValue < 0)
        {
            makeTargetScript.deleteTarget();
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(playerTarget.transform.position - transform.position);
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
