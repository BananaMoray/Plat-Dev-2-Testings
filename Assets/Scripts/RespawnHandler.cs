using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;
using System.Diagnostics;
using Unity.VisualScripting;
using static UnityEditor.Experimental.GraphView.GraphView;


public class RespawnHandler : MonoBehaviour
{


    //private Rectangle _bounds = new Rectangle(0, 0, 100, 50);

    [SerializeField]
    GameObject[] _players;
    [SerializeField]
    GameObject[] _toppings;

    [SerializeField]
    Transform[] _respawns;

    [SerializeField] private GameObject _prefab;
    [SerializeField] private float _radius = 5f;
    [SerializeField] private float _spawnDelay = 20;
    [SerializeField] private float _topicSpawnTimer;
    [SerializeField] private int _maxTime;
    [SerializeField] private int _height = 10;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        _players = GameObject.FindGameObjectsWithTag("Player");
        _toppings = GameObject.FindGameObjectsWithTag("Ingredient");

        foreach (GameObject player in _players)
        {
            HitBoundary(player);
        }
        /*foreach (GameObject topp in _toppings)
        {
            HitBoundary(topp);
        }*/

        _spawnDelay -= Time.deltaTime;
        if (_spawnDelay <= 0)
        {
            SpawnAtRandomCirclePosition();
        }
    }

    void HitBoundary(GameObject obj)
    {

        if (obj.transform.position.y <= -25)
        {
            print("below -25");

            if (obj.transform.position.x <= 0)
            {
                //Left Side
                if (obj.transform.position.z <= 0)
                {
                    //Bottom Left
                    Respawn(obj, 0);
                    print("BL");
                }
                if (obj.transform.position.z >= 0)
                {
                    //Top Left
                    Respawn(obj, 1);
                    print("TL");

                }

            }
            if (obj.transform.position.x >= 0)
            {
                //Right Side
                if (obj.transform.position.z <= 0)
                {
                    //Bottom Right
                    Respawn(obj, 2);
                    print("BR");

                }
                if (obj.transform.position.z >= 0)
                {
                    //Top Right
                    Respawn(obj, 3);
                    print("TR");

                }

            }

            obj.transform.position = new Vector3(0, 5, 0);
        }

    }

    void Respawn(GameObject obj, int i)
    {
        print("Respawn triggered");

        CharacterController _char = obj.GetComponent<CharacterController>();

        _char.enabled = false;

        obj.transform.position = _respawns[i].position;

        _char.enabled = true;

        _char.transform.SetParent(null);
        _char.transform.rotation = Quaternion.identity;
        _char.GetComponent<CharacterController>().enabled = true;
        _char.GetComponent<CharacterControl>().IsHit = false;

        print(obj.name);

    }

    void SpawnAtRandomCirclePosition()
    {
        _topicSpawnTimer -= Time.deltaTime;

        float angle = Random.Range(0f, Mathf.PI * 2f);
        Vector3 spawnPosition = new Vector3(Mathf.Cos(angle), _height, Mathf.Sin(angle)) * _radius;

        if (_topicSpawnTimer <= 0)
        {
            Instantiate(_prefab, spawnPosition, Quaternion.identity);
            _topicSpawnTimer = _maxTime;
        }
    }






}
