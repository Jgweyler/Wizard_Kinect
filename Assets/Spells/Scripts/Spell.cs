using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour
{

    private int elementType = 1; //Por defecto vamos a dejarlo como "fuego". Cuando inicialice el juego este parametro ira cambiando.

    public float maxTimeLife = 2f; //Máximo tiempo de vida en el que el hechizo está dentro del juego. Si lo sobrepasa, se elimina del juego.

    void Start()
    {
        elementType = SpellManager.currentElement;
        Destroy(gameObject, maxTimeLife); //Indicamos que si el objeto sobrepasa el tiempo de vida, este se destruya.
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Debug.Log("Te piléee!");
            int enemyElement = other.gameObject.GetComponent<EnemyBehavior>().getElement();
            int counterElement = SpellManager.getCounterElement(enemyElement);
            Debug.Log("Player: " + elementType + " Enemigo= " + enemyElement + " Counter: " + counterElement);
            if (elementType == counterElement)
                other.gameObject.GetComponent<EnemyHealth>().dealDamage();
        }

		if (MakeTarget.target == other.gameObject) {
			MakeTarget.updateEnemySlider(other.gameObject.GetComponent<EnemyHealth>().getCurrentHealth());
		}
        //Comprobar si el hechizo colisiona con algo P.E, un enemigo, y realizar las operaciones oportunas.
    }

   
   public int getElementType()
   {
       return elementType;
   }

    public void setElementType(int type)
    {
        elementType = type;
    }

}
