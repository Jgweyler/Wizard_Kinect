using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float speed = 12f;       //Velocidad (Movimiento hacia alante y atrás).
    public float turnSpeed = 180f;  //Velocidad de giro


    private Rigidbody rigidBody;
    private string movementAxisName;
    private string movementTurnAxisName;
    private float movementInputValue;
    private float turnInputValue;
    private Quaternion rotation; // Se usará para indicar a la cámara cuánto tiene que girar.

    //Variables necesarias para la fijación de objetivo y combate.
    private bool hasTarget;
    private GameObject playerTarget;
    private float speedInCombat; //Velocidad de movimiento en combate.
    private MakeTarget makeTargetScript;


    // Use this for initialization
    private void Start () {
        rigidBody = GetComponent< Rigidbody >();
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

    public void combatMove() { //El movimiento del personaje cambia a modo de combate si fija objetivo
        transform.LookAt(playerTarget.transform);
        float move = turnInputValue * speedInCombat;
            transform.Translate(move, 0, 0);

        //Si el jugador quiere retroceder, automáticamente se cancela la fijación de objetivo.
        if (movementInputValue < 0) {
            makeTargetScript.deleteTarget();
        }

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
