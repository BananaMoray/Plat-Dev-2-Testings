using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem;
using System.Diagnostics;
using Unity.VisualScripting;


public class RespawnHandler : MonoBehaviour
{


    //private Rectangle _bounds = new Rectangle(0, 0, 100, 50);

    [SerializeField]
    GameObject[] _players;
    [SerializeField]
    GameObject[] _toppings;

    [SerializeField]
    Transform[] _respawns;


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

        print(obj.name);

    }








}
