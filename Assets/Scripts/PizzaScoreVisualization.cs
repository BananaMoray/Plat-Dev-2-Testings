using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PizzaScoreVisualization : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = $"P{i + 1}: {PizzaScoreZone.GetPlayerScore(i)}";
        }
        //Debug.Log(_pizzaScoreZone.GetPlayerScore(0));
    }
    
}
