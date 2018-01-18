using UnityEngine;
using System.Collections;

public class Spell : MonoBehaviour
{

    private int elementType = 1; //By default, de element type is 1 -> FIRE.

    public float maxTimeLife = 2f; //Max LifeTime that a spell can be in game.

    void Start()
    {
        elementType = SpellManager.currentElement;
        Destroy(gameObject, maxTimeLife); //If spell's lifetime is > maxLifetime  -> Destroy...
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            Debug.Log("Hit!!");
            int enemyElement = other.gameObject.GetComponent<EnemyBehavior>().getElement();
            int counterElement = SpellManager.getCounterElement(enemyElement);
            Debug.Log("Player: " + elementType + " Enemigo= " + enemyElement + " Counter: " + counterElement);
            if (elementType == counterElement)
                other.gameObject.GetComponent<EnemyHealth>().dealDamage();
        }

		if (MakeTarget.target == other.gameObject) {
			MakeTarget.updateEnemySlider(other.gameObject.GetComponent<EnemyHealth>().getCurrentHealth());
		}
        //Check if spells collides with an enemy
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
