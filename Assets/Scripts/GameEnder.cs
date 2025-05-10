using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class GameEnder : MonoBehaviour
{
    [SerializeField]
    private GameObject _gameManagerObject;

    [SerializeField]
    private GameObject _endScreenUI;

    [SerializeField]
    private GameObject[] _textOfScores;

    [SerializeField]
    private GameObject[] _panelsOfPlayers;

    private Vector3[] _panelPositions = new Vector3[4];

    [SerializeField]
    private GameObject _characters;

    private GameManager _gameManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _endScreenUI.SetActive(false);
        _gameManager = _gameManagerObject.GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //pause game when ended
        //redirect to the vizualize winner

        if (_gameManager._timer <= 0)
        {
            Time.timeScale = 0;
            _endScreenUI.SetActive(true);

            int[] scores = new int[4];
            for (int i = 0; i < 4; i++)
            {
                int score = PizzaScoreZone.GetPlayerScore(i);

                _textOfScores[i].GetComponent<TextMeshProUGUI>().text = $"{score}";

                //add scores to scores array
                scores[i] = score;

                //add all positions to the list
                _panelPositions[i] = _panelsOfPlayers[i].transform.position;
                int j = i;
            }
            int[] copyOfScore = new int[4];
            copyOfScore = scores;
            copyOfScore =  copyOfScore.OrderByDescending(x => x).ToArray();
            bool[] isPositionsOccupied = new bool[4];
            //sort the scores
            for (int i = 0; i < 4; i++)
            {
                if (scores[i] == copyOfScore[0] && isPositionsOccupied[0])
                {
                    isPositionsOccupied[0] = true;
                    _panelsOfPlayers[i].transform.position = _panelPositions[0];
                }
                else if (scores[i] == copyOfScore[1] && isPositionsOccupied[1])
                {
                    isPositionsOccupied[1] = true;
                    _panelsOfPlayers[i].transform.position = _panelPositions[1];
                }
                else if (scores[i] == copyOfScore[2] && isPositionsOccupied[2])
                {
                    isPositionsOccupied[2] = true;
                    _panelsOfPlayers[i].transform.position = _panelPositions[2];
                }
                else if (scores[i] == copyOfScore[3] && isPositionsOccupied[3])
                {
                    isPositionsOccupied[3] = true;
                    _panelsOfPlayers[i].transform.position = _panelPositions[3];
                }
            }


        }


    }
}
