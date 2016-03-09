using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float speed = 12f;       //Velocidad (Movimiento hacia alante y atrás).
    public float turnSpeed = 180f;  //Velocidad de giro


    private Rigidbody rigidBody;
    private string movementAxisName;
    private string movementTurnAxisName;
    private string markTargetName;
    private float movementInputValue;
    private float turnInputValue;
    private Quaternion rotation; // Se usará para indicar a la cámara cuánto tiene que girar.

    private bool hasTarget;        //Si el jugador ha fijado un objetivo.
    private GameObject target;     //Referencia al enemigo que ha marcado el jugador.


	// Use this for initialization
	private void Start () {
        rigidBody = GetComponent< Rigidbody >();
        //Inicializamos los gestores del input y los referenciamos con un nombre.
        movementAxisName = "Vertical";
        movementTurnAxisName = "Horizontal";
        markTargetName = "Fire2";
        //El valor de giro y de movimiento deberán estar a cero.
        movementInputValue = 0f;
        turnInputValue = 0f;
	}
	
	// Update is called once per frame
	private void Update () {
        movementInputValue = Input.GetAxis(movementAxisName);
        turnInputValue = Input.GetAxis(movementTurnAxisName);
	}

    private void FixedUpdate() {
        move();
        turn();
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

    private void pointTarget(){ //Fija el objetivo
        //Puedes usar la funcion de área de daño de los tanques, pero con los enemigos. Solo podrías seleccionar aquellos que sean enemigos.
        //Si se vuelve a pulsar el boton (o a realizar el evento de marcado), se debería cambiar de objetivo siempre y cuando lo haya.
        Input.GetMouseButton(1); //Devuelve si se pulso el click derecho.
    }
}
