using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

    private Canvas enemyCanvas;
    private GameObject player;
	private MakeTarget playerTargetScript;

    private int maxHP = 100;
    private int currentHealth;


    // Use this for initialization
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHP;
    }

    // Update is called once per frame
    void Update() {
    }

    public void dealDamage()
    {
        currentHealth = currentHealth - 25;
        if (currentHealth <= 0)
            Destroy(gameObject);
    }

	public float getCurrentHealth(){
		return currentHealth;
	}
}
