using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ficha : MonoBehaviour {

    public GameObject gameManager;          //GameManager prefab to instantiate.
    int numFicha;

	// Use this for initialization
	void Start () {
        numFicha = Convert.ToInt32(this.name);
    }
  
    // Update is called once per frame
    void Update () {
	}

    private void OnMouseDown()
    {
        GameManager.instance.fichaPulsada(numFicha);

    }   
}
