using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CloseScreen()
    {
        gameObject.SetActive(false);
    }

    public void OpenScreen(GameObject screen)
    {
        screen.SetActive(true);
        if(screen.active)
        {
            _gameManager.PauseGame();
        }
    }




}
