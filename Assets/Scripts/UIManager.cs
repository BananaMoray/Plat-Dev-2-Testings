using UnityEngine;
using UnityEngine.Device;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private GameObject _screen;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseScreen(GameObject screen)
    {
        screen.SetActive(false);
    }

    public void OpenScreen(GameObject screen)
    {
        screen.SetActive(true);
        
    }

    public void OpenPauseScreen()
    {
        
        _screen.SetActive(true);
        if (_screen)
        {
            _gameManager.PauseGame();
        }
    }




}
