using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

    private Canvas enemyCanvas;
    private Slider healthSlider;
    private GameObject player;

    private int maxHP = 100;
    private int currentHealth;


    // Use this for initialization
    void Start() {
        enemyCanvas = gameObject.GetComponentInChildren<Canvas>();
        healthSlider = enemyCanvas.GetComponentInChildren<Slider>();
        player = GameObject.FindGameObjectWithTag("Player");
        currentHealth = maxHP;
        healthSlider.value = currentHealth;
    }

    // Update is called once per frame
    void Update() {
        enemyCanvas.transform.LookAt(player.transform.position);
    }

    public void dealDamage()
    {
        currentHealth = currentHealth - 25;
        healthSlider.value = currentHealth;
        if (currentHealth <= 0)
            Destroy(gameObject);
    }
}
