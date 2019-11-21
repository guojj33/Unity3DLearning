using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IMGUIHealthBar : MonoBehaviour
{
    public float health = 0f;
    private float resultHealth;
    private Rect healthBar;
    public Rect healthUp;
    public Rect healthDown;
    public Slider slider;
    

    // Start is called before the first frame update
    void Start()
    {    
        healthBar = new Rect(50, 50, 200, 30);
        healthUp = new Rect(100, 80, 40, 30);
        healthDown = new Rect(150, 80, 40, 30);
        resultHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        if(GUI.Button(healthUp, "加血"))
        {
            resultHealth = resultHealth + 0.1f > 1f ? 1f : resultHealth + 0.1f;
            Debug.Log("加血");
        }
        if(GUI.Button(healthDown, "减血"))
        {
            resultHealth = resultHealth - 0.1f < 0 ? 0 : resultHealth - 0.1f;
            Debug.Log("减血");
        }

        health = Mathf.Lerp(health, resultHealth, 0.05f);
        slider.value = health;
        GUI.HorizontalScrollbar(healthBar, 0f, health, 0f, 1f);
    }
}
