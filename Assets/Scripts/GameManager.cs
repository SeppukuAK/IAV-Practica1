using System; //Sort
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    struct Par
    {
        public int x;
        public int y;
    }

    enum Direccion { Izquierda, Derecha, Arriba, Abajo, Vacio };

    public static GameManager instance;

    int[,] tablero = new int[3, 3];
    int[,] tableroIni = new int[3, 3];


    Par posVacia;
    GameObject[] fichas;
    const float distancia = 2.57f;

    //Ordena una lista de GameObjects por su nombre
    public class NameComparer : IComparer
    {
        // Calls CaseInsensitiveComparer.Compare on the monster name string.
        int IComparer.Compare(System.Object x, System.Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(((GameObject)x).name, ((GameObject)y).name));
        }
    }

    void Start()
    {
        instance = this;
        //Buscamos las fichas
        fichas = GameObject.FindGameObjectsWithTag("Ficha");

        //Ordenamos el array de fichas por Nombre
        IComparer myComparer = new NameComparer();
        Array.Sort(fichas, myComparer);

        //Tablero por defecto {1,2,3,4,5,6,7,8,0} 
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                tableroIni[i, j] = tablero[i,j] = i * 3 + j + 1;
            }
        }
		tableroIni[2, 2] = tablero[2, 2] =  0;

        posVacia.x = 2;
        posVacia.y = 2;

        //updateTablero(ref tableroIni);

        /*
        for (int i = 0; i < 10; i++)
        {
            int random = UnityEngine.Random.Range(1, 9);

            fichaPulsada(random);
        }
        */
        //contador que mueve las fichas n veces
        int cont = 1;

        Direccion dir = Direccion.Vacio;//Direccion a la que se tiene que mover el vacio
        Direccion dirAnt = Direccion.Vacio; //Direccion cutre y de mierda para controlar que el estado anterior no sea el contrario

        while (cont != 0)
        {
            //Debug.Log("Para contador = " + cont + "El Vacio es: " + posVacia.y + ":" + posVacia.x);
            //Debug.Log("Ficha: " + posVacia.y + ":" + posVacia.x);

            //Se genera el random de mierda
            int random = UnityEngine.Random.Range(0, 4);
            dir = (Direccion)random;

         //   Debug.Log("Direccion actual: " + dir);

          //  Debug.Log("Direccion anterior: " + dirAnt);


            //Si se puede mover la vacia a la izquierda y no he movido antes a la derecha
            //Izquierda
            if (VaciaMovible(posVacia, dir) && dir == Direccion.Izquierda && dirAnt != Direccion.Derecha) {
                Debug.Log("hola he entrado izquierda");

                dirAnt = Direccion.Izquierda;
                IntercambioFichas(dir, ref tablero);
            }

            //Derecha
            else if (VaciaMovible(posVacia, dir) && dir == Direccion.Derecha && dirAnt != Direccion.Izquierda) {
                Debug.Log("hola he entrado derecha");

                dirAnt = Direccion.Derecha;
                IntercambioFichas(dir, ref tablero); 
			}


            //Arriba
            else if (VaciaMovible(posVacia, dir) && dir == Direccion.Arriba && dirAnt != Direccion.Abajo) {
                Debug.Log("hola he entrado arriba");

                dirAnt = Direccion.Arriba;
                IntercambioFichas(dir, ref tablero);
			}

            //Abajo
            else if (VaciaMovible(posVacia, dir) && dir == Direccion.Abajo && dirAnt != Direccion.Arriba) {
                Debug.Log("hola he entrado abajo");

                dirAnt = Direccion.Abajo;
                IntercambioFichas(dir, ref tablero);
            }

            else if ((dir == Direccion.Izquierda && dirAnt == Direccion.Derecha) || (dir == Direccion.Derecha && dirAnt == Direccion.Izquierda) ||
                (dir == Direccion.Arriba && dirAnt == Direccion.Abajo) || (dir == Direccion.Abajo && dirAnt == Direccion.Arriba)) {
                Debug.Log("No he podido entrar porque la direccion es " + dir + "y la direccion anterior es" + dirAnt);

            }
            cont--;

        }




        updateTablero();

        //IA POR ANCHURA
        tablero = Bfs(tablero, tableroIni);

        updateTablero();

    }




    //Devuelve si la ficha vacia es movible en la direccion d
    bool VaciaMovible(Par posVacia, Direccion d)
    {
        //Comprobar izquierda
        if (d == Direccion.Izquierda && posVacia.x >= 1)   //Compruebo si puedo mover la vacía  
            return true;


        //Comprobar derecha
        else if (d == Direccion.Derecha && posVacia.x <= 1)
            return true;


        //Comprobar arriba
        else if (d == Direccion.Arriba && posVacia.y >= 1)
            return true;


        //Comprobar abajo
        else if (d == Direccion.Abajo && posVacia.y <= 1)
            return true;


        return false;
    }


    //Updatea la posición de los gameObjects a los correspondientes a la lógica del tablero
    void updateTablero()
    {

        //Coordenada y
        for (int i = 0; i < 3; i++)
        {
            //Coordenada x
            for (int j = 0; j < 3; j++)
            {
                //En la matriz, el valor 0, significa que no hay ficha
                if (tablero[i, j] != 0)
                    colocaFicha(j - 1, i - 1, fichas[tablero[i, j] - 1]);
            }

        }

    }


    //Recibe unas coordenadas del tablero y un gameObject y coloca el gameObject en la posición correspondiente al tablero
    void colocaFicha(int x, int y, GameObject ficha)
    {
        ficha.transform.position = new Vector3(x * distancia, -y * distancia, 0);

    }

    //Comprueba si el puzzle se ha resuelto
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

        return (contador == 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (comprobarVictoria())
            Debug.Log("HAS GANADO");
    }

    //Se le llama cuando se pulsa a una ficha, mueve la ficha si es adyacente al hueco vacío
    public void fichaPulsada(int numFicha)
    {
        Par posFicha;
        posFicha.x = posFicha.y = 0;

        int i = 0;
        int j = 0;
        bool encontradaFicha = false;

        //Encontrar la ficha 
        while (!encontradaFicha && i < 3)
        {
            j = 0;
            while (!encontradaFicha && j < 3)
            {

                if (tablero[i, j] == numFicha)
                {
                    encontradaFicha = true;
                    posFicha.y = i;
                    posFicha.x = j;
                }
                j++;
            }
            i++;
        }

        if (esMovible(posFicha))
        {
            tablero[posFicha.y, posFicha.x] = 0;
            tablero[posVacia.y, posVacia.x] = numFicha;

            //La nueva posición vacía es donde estaba la ficha pulsada
            posVacia = posFicha;

        }

        updateTablero();

    }


	void IgualarTablero(int [,] tab, int [,] tab2)
	{

		//Coordenada y
		for (int i = 0; i < 3; i++)
		{
			//Coordenada x
			for (int j = 0; j < 3; j++)
			{
				tab [i, j] = tab2 [i, j];
			}

		}

	}
    

    //Devuelve true si la ficha puede moverse
    bool esMovible(Par posFicha)
    {
        return (posFicha.x == posVacia.x && (Mathf.Abs(posFicha.y - posVacia.y) == 1) //Arriba y abajo
            || posFicha.y == posVacia.y && (Mathf.Abs(posFicha.x - posVacia.x) == 1)); //Izquierda y derecha

    }

    public void OnClick() {
        //GenerarTableroInicial ();
        //updateTableroInicial ();
    }


    void IntercambioFichas(Direccion dir, ref int[,] tab) {
        int aux;//Auxiliar para intercambiar las posiciones del vacio y la ficha

        Par fichaAdy; //ficha adyacente
        fichaAdy.x = 0;
        fichaAdy.y = 0;

        switch (dir) {
            //Izquierda
            case Direccion.Izquierda:
                //Colocar las fichas, sin mover
                fichaAdy.x = posVacia.x - 1;
                fichaAdy.y = posVacia.y;
                aux = tab[fichaAdy.y, fichaAdy.x]; //ficha de la izquierda

                tab[posVacia.y, posVacia.x] = aux;//Guardo en la vacia el contenido de la adyacente
                tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0
                posVacia.x--;
                break;

            //Derecha
            case Direccion.Derecha:

                fichaAdy.x = posVacia.x + 1;
                fichaAdy.y = posVacia.y;
                aux = tab[fichaAdy.y, fichaAdy.x]; //ficha de la izquierda

                tab[posVacia.y, posVacia.x] = aux;//Guardo en la vacia el contenido de la adyacente
                tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0
                posVacia.x++;
                break;

            //Arriba
            case Direccion.Arriba:

                fichaAdy.x = posVacia.x;
                fichaAdy.y = posVacia.y - 1;
                aux = tab[fichaAdy.y, fichaAdy.x]; //ficha de la izquierda

                tab[posVacia.y, posVacia.x] = aux;//Guardo en la vacia el contenido de la adyacente
                tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0
                posVacia.y--;
                break;

            //Abajo
            case Direccion.Abajo:

                fichaAdy.x = posVacia.x;
                fichaAdy.y = posVacia.y + 1;
                aux = tab[fichaAdy.y, fichaAdy.x]; //ficha de la izquierda

                tab[posVacia.y, posVacia.x] = aux;//Guardo en la vacia el contenido de la adyacente
                tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0
                posVacia.y++;
                break;

        }


    }

	void IntercambioFichasVirtual(Direccion dir, ref int[,] tab, Par posVaciaAux) {
		int aux;//Auxiliar para intercambiar las posiciones del vacio y la ficha

		Par fichaAdy; //ficha adyacente
		fichaAdy.x = 0;
		fichaAdy.y = 0;

		switch (dir) {
		//Izquierda
		case Direccion.Izquierda:
			//Colocar las fichas, sin mover
			fichaAdy.x = posVaciaAux.x - 1;
			fichaAdy.y = posVaciaAux.y;
			aux = tab[fichaAdy.y, fichaAdy.x]; //ficha de la izquierda

			tab[posVaciaAux.y, posVaciaAux.x] = aux;//Guardo en la vacia el contenido de la adyacente
			tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0
			break;

			//Derecha
		case Direccion.Derecha:

			fichaAdy.x = posVaciaAux.x + 1;
			fichaAdy.y = posVaciaAux.y;
			aux = tab[fichaAdy.y, fichaAdy.x]; //ficha de la izquierda

			tab[posVaciaAux.y, posVaciaAux.x] = aux;//Guardo en la vacia el contenido de la adyacente
			tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0
			break;

			//Arriba
		case Direccion.Arriba:

			fichaAdy.x = posVaciaAux.x;
			fichaAdy.y = posVaciaAux.y - 1;
			aux = tab[fichaAdy.y, fichaAdy.x]; //ficha de la izquierda

			tab[posVaciaAux.y, posVaciaAux.x] = aux;//Guardo en la vacia el contenido de la adyacente
			tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0
			break;

			//Abajo
		case Direccion.Abajo:

			fichaAdy.x = posVaciaAux.x;
			fichaAdy.y = posVaciaAux.y + 1;
			aux = tab[fichaAdy.y, fichaAdy.x]; //ficha de la izquierda

			tab[posVaciaAux.y, posVaciaAux.x] = aux;//Guardo en la vacia el contenido de la adyacente
			tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0
			break;

		}


	}

	Par EncontrarVacio(int[,] tab){
		Par posVaciaAux;
		posVaciaAux.x = posVaciaAux.y = 0;

		bool encontrado = false;
		int i = 0;
		while (!encontrado && i < 3) {
			int j = 0;
			while (!encontrado && j < 3) 
			{
				if (tab [i, j] == 0) {
					encontrado = true;
					posVaciaAux.x = j;
					posVaciaAux.y = i;
				}
				j++;
			}
			i++;
		}

		return posVaciaAux;
	}

	bool[] Adyacentes(int[,] tab, Par posVaciaAux) {
        bool[] direcciones = new bool[4];


        //Comprobar izquierda
		direcciones[0] = (posVaciaAux.x >= 1);

        //Comprobar derecha
		direcciones[1] = (posVaciaAux.x <= 1);

        //Comprobar arriba
		direcciones[2] = (posVaciaAux.y >= 1);

        //Comprobar abajo
		direcciones[3] = (posVaciaAux.y <= 1);

        return direcciones;
    }
		

	bool tablerosIguales(int[,] tab1, int[,] tab2)
	{
		int i = 0;
		bool iguales = true;
		while (iguales && i < 3) 
		{
			int j = 0;
			while (iguales && j < 3)
			{
				if (tab1 [i, j] != tab2 [i, j])
					iguales = false;
				j++;
			}
				
			i++;
		}

		return iguales;
	}

    int[,] Bfs(int[,] inicio, int[,] fin)
    {
        Queue<int[,]> colaTab = new Queue<int[,]>();//Se crea la cola de nodos
        List<int[,]> visitado = new List<int[,]>();//Se crea la lista de marcados (vector(?))

        bool[] ady = new bool[4];
        bool solucion = false;

        //inicio.history = new List<Nodo>();
		int[,] actual = new int[3,3];
		IgualarTablero (actual, inicio);

		//Se añade el nodo inicial a la cola
		colaTab.Enqueue(actual);

		//Se marca como visitado el nodo inicial
		visitado.Add(actual);

        while (!solucion && colaTab.Count > 0) {
            Queue<int[,]> colaAdy = new Queue<int[,]>();//Se crea la cola de nodos

            //Extraemos de la cola el nodo
            actual = colaTab.Dequeue();

            //Comprobamos si hemos llegado a la configuración final
            //Se ha encontrado 123456780

			if (tablerosIguales (actual,fin))
                solucion = true;

            else
            {
				Par posVaciaAux = EncontrarVacio (actual);

				ady = Adyacentes(actual,posVaciaAux);

                //Buscamos los adyacentes y los procesamos
                for (int i = 0; i < ady.Length; i++)
                {
                    if (ady[i])
                    {
						int[,] tabAdy= new int[3,3];
						IgualarTablero (tabAdy, actual);
						IntercambioFichasVirtual((Direccion)i, ref tabAdy, posVaciaAux);

                        colaAdy.Enqueue(tabAdy);
                    }
                }

                while (colaAdy.Count > 0)
                {
                    int[,] tabAux = colaAdy.Dequeue();

                    if (!(visitado.Contains(tabAux)))
                    {
                        visitado.Add(tabAux);
                        colaTab.Enqueue(tabAux);
                    }
                }
            }
    	}

        return actual;
	}

}
