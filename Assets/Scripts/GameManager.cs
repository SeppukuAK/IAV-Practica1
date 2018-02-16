using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    int [,] tablero = new int[3,3];
    GameObject[] fichas;
    const float distancia= 2.57f;
	// Use this for initialization
	void Start () {
        fichas = GameObject.FindGameObjectsWithTag("Ficha");

        for(int i = 0; i < 8; i++)
             Debug.Log("Ficha " + i + ": "+ fichas[i]);
        //Generar un tablero aleatorio AHORA MISMO NO ES ALEATORIO
        tablero[0, 0] = 2;
        int contador = 8;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                tablero[i, j] = contador;
                contador--;
            }
        }

        fichas[0].ToString()


	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
