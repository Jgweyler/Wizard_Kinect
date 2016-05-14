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

	private KinectManager kinectManagerScript;
	private bool isDetectingUserWithKinect; // Determina si el sensor de kinect está disponible.


    // Use this for initialization
    private void Start () {
        rigidBody = GetComponent< Rigidbody >();
		playerAnim = GetComponent<Animator> ();
        makeTargetScript = GetComponent<MakeTarget>();
		kinectManagerScript = GameObject.FindGameObjectWithTag ("GameController").GetComponent<KinectManager> ();
		isDetectingUserWithKinect = kinectManagerScript.IsUserDetected();
        //Inicializamos los gestores del input y los referenciamos con un nombre.
        movementAxisName = "Vertical";
        movementTurnAxisName = "Horizontal";
        //El valor de giro y de movimiento deberán estar a cero.
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

		if (!isDetectingUserWithKinect) { //Si kinect no detecta a un usuario, habilitamos las animaciones por teclado
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
        // Determina el numero de grados que gira basado en la entrada, velocidad y tiempo entre frames.
        float turn = turnInputValue * turnSpeed * Time.deltaTime;
        // Make this into a rotation in the y axis.
        rotation = Quaternion.Euler(0f, turn, 0f);

        // Aplicamos la rotación al rigidbody.
        rigidBody.MoveRotation(rigidBody.rotation * rotation);
    }

	//Funciones de Kinect
	public void goBack_Kinect(){

		if(!playerAnim.GetBool("goBack")){ //Comprobamos que se detenga correctamente la animación de correr.
			playerAnim.SetBool("goBack", true);
		}
		if(playerTarget != null){ //Si tiene objetivo asignado, hay que quitarlo.
			makeTargetScript.deleteTarget();
		}
		Vector3 movement = transform.forward * (-moveSpeed_Kinect) * speed * Time.deltaTime;
		rigidBody.MovePosition(rigidBody.position + movement);
	}

	public void moveForward_Kinect(){
		if(!playerAnim.GetBool("isMoving")){ //Si no se esta ejecutando la animacion de correr
			playerAnim.SetBool("isMoving", true); // Hacemos que corra.
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
	//Fin de las funciones con Kinect.

    public void combatMove() { //El movimiento del personaje cambia a modo de combate si fija objetivo

        //Si el jugador quiere retroceder o destruye a su objetivo, automáticamente se cancela la fijación de objetivo.
        if (playerTarget == null || movementInputValue < 0)
        {
            makeTargetScript.deleteTarget();
            return;
        }
		/*
		//Primero necesitas las coordenadas del enemigo.
		float enemy_x = playerTarget.transform.position.x;
		float enemy_z = playerTarget.transform.position.z;

		float radius = Vector3.Distance (transform.position, playerTarget.transform.position);
		radius = Mathf.Abs (radius);

		float speedScale = (float)(0.001*2*Mathf.PI)/speedInCombat;
		float angle = turnInputValue * speedScale;
		transform.Translate (new Vector3 (enemy_x + Mathf.Sin(angle)*radius, transform.position.y, enemy_z + Mathf.Cos(angle)*radius));
		*/
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
