using UnityEngine;

public class DisappearCanvas : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (QueueDelay.IsStarted)
        {
        
            this.gameObject.SetActive(false);
      
        }
    }
}
