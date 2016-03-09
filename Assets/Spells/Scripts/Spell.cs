using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour
{

    public float maxTimeLife = 2f; //Máximo tiempo de vida en el que el hechizo está dentro del juego. Si lo sobrepasa, se elimina del juego.
                                   // Use this for initialization
    void Start()
    {
        Destroy(gameObject, maxTimeLife); //Indicamos que si el objeto sobrepasa el tiempo de vida, este se destruya.
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //Comprobar si el hechizo colisiona con algo P.E, un enemigo, y realizar las operaciones oportunas.
    }

}
