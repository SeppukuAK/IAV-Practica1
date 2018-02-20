using System; //Sort
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour {

    //Para las coordenadas de las fichas
  
    public static GameManager instance;

    int[,] tablero = new int[3,3];
    GameObject[] fichas;
    const float distancia= 2.57f;
    // Use this for initialization

    //Ordena una lista de GameObjects por su nombre
    public class NameComparer : IComparer
    {
        // Calls CaseInsensitiveComparer.Compare on the monster name string.
        int IComparer.Compare(System.Object x, System.Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(((GameObject)x).name, ((GameObject)y).name));
        }
    }

    void Start () {
        instance = this;
        fichas = GameObject.FindGameObjectsWithTag("Ficha");
     
        //Ordenamos el array de fichas por Nombre
        IComparer myComparer = new NameComparer();
        Array.Sort(fichas, myComparer);

        //for (int i = 0; i < 8; i++)
        //Debug.Log("Ficha " + (i+1) + ": " + fichas[i]);

        //Generar un tablero aleatorio AHORA MISMO NO ES ALEATORIO
        
        List<int> randoms = new List<int>();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                int Rand = UnityEngine.Random.Range(0, 9);

                //Si la lista contiene en random, ese número ya había sido asignado
                while (randoms.Contains(Rand))
                {
                    Rand = UnityEngine.Random.Range(0, 9);
                }

                randoms.Add(Rand);
                //Debug.Log("Rand" + Rand);
                tablero[i, j] = Rand;
            }
        }
        
        /*
        int contador = 8;
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                tablero[i, j] = contador;
                Debug.Log("Tablero " + i + " " + j + ": " + contador);

                contador--;
            }
        }
        */


        updateTablero();

    }

    //Updatea la posición de los gameObjects a los correspondientes a la lógica del tablero
    void updateTablero()
    {
        //Dar a los gameObjects la posición aleatoria del tablero
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                //En la matriz, el valor 0, significa que no hay ficha
                if (tablero[i, j] != 0)
                    colocaFicha(j - 1, i - 1, fichas[tablero[i, j] - 1]);
            }

        }

    }

    void colocaFicha(int x, int y, GameObject ficha)
    {
        ficha.transform.position = new Vector3(x * distancia, -y * distancia, 0);

    }


    bool comprobarVictoria()
    {
        int i = 0;
        int contador = 8;
        while (contador > 0 && i < 3)
        {
            int j = 0;
            while (contador > 0 && j < 3)
            {
                if (tablero[i, j] == i * 3 + j + 1)
                    contador--;
                
                else
                    contador = -1;
                j++;
            }
            i++;
        }

        if (contador == 0)
            Debug.Log("HAS GANADO");

        return (contador == 0);

    }

	// Update is called once per frame
	void Update () {

    }

    struct Par
    {
        public int x;
        public int y;
    }

    //Se le llama cuando se pulsa a una ficha
    public void fichaPulsada(int numFicha)
    {

        Par posFicha, posVacia;
        posFicha.x = posFicha.y = posVacia.x = posVacia.y = 0;

        int i = 0;
        int j = 0;
        bool encontradaFicha = false;
        bool encontradoVacio = false;

        //Encontrar la ficha y el hueco vacio
        while (!encontradaFicha && !encontradoVacio && i < 3)
        {
            j = 0;
            while (!encontradaFicha && !encontradoVacio &&j < 3)
            {
                if (tablero[i, j] == numFicha)
                {
                    encontradaFicha = true;
                    posFicha.y = j;
                    posFicha.x = i;
                }
                else if (tablero[i, j] == numFicha)
                {
                    encontradoVacio = true;
                    posVacia.y = j;
                    posVacia.x = i;
                }
                j++;
            }
            i++;
        }

        Debug.Log("TABLERO ANTES");

        for (int a = 0; a < 3; a++)
        {
            for (int b = 0; b < 3; b++)
            {
                Debug.Log(tablero[a, b]);
            }
        }


        if (esMovible(posFicha))
        {
            tablero[posFicha.x, posFicha.y] = 0;
            tablero[posVacia.x, posVacia.y] = numFicha;
        
        }

        Debug.Log("TABLERO DESPUES");


        for (int a = 0; a < 3; a++)
        {
            for (int b = 0; b < 3; b++)
            {
                Debug.Log(tablero[a, b]);
            }
        }

        updateTablero();

    }

    bool esMovible(Par posFicha)
    {
        //Comprobar izquierda
        if (posFicha.x >= 1 && tablero[posFicha.x - 1, posFicha.y] == 0)
            return true;

        //Comprobar derecha
        else if (posFicha.x <= 1 && tablero[posFicha.x + 1, posFicha.y] == 0)
            return true;

        //Comprobar arriba
        else if (posFicha.y >= 1 && tablero[posFicha.x, posFicha.y - 1] == 0)
            return true;

        //Comprobar abajo
        else if (posFicha.y <= 1 && tablero[posFicha.x, posFicha.y + 1] == 0)
            return true;

        return false;
    }
}
