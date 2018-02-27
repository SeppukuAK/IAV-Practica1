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
	int[,] tableroInicial = new int[3, 3];


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
                tablero[i, j] = i*3 + j+1;
			}
		}
        tablero[2, 2] = 0;
        posVacia.x = 2;
        posVacia.y = 2;

		updateTablero();
	
        /*
        for (int i = 0; i < 10; i++)
        {
            int random = UnityEngine.Random.Range(1, 9);

            fichaPulsada(random);
        }
        */

		Nodo inicio;

		inicio.estado = tablero;

		//IA POR ANCHURA
		bfs();

        //contador que mueve las fichas n veces
        int cont = 10;

		Direccion dir= Direccion.Vacio;//Direccion a la que se tiene que mover el vacio
		Direccion dirAnt = Direccion.Vacio; //Direccion cutre y de mierda para controlar que el estado anterior no sea el contrario

		while (cont != 0)
		{



			//Debug.Log("Para contador = " + cont + "El Vacio es: " + posVacia.y + ":" + posVacia.x);
			//Debug.Log("Ficha: " + posVacia.y + ":" + posVacia.x);

			//Se genera el random de mierda
			int random = UnityEngine.Random.Range(0, 4);
            dir = (Direccion)random;

			Debug.Log("Direccion actual: " + dir);

			Debug.Log ("Direccion anterior: " + dirAnt);


            //Si se puede mover la vacia a la izquierda y no he movido antes a la derecha
            //Izquierda
            if (VaciaMovible (posVacia, dir) && dir == Direccion.Izquierda && dirAnt != Direccion.Derecha) {
				Debug.Log ("hola he entrado izquierda");

				dirAnt = Direccion.Izquierda;
				IntercambioFichas (dir, ref tablero);
			}

			//Derecha
			else if (VaciaMovible (posVacia, dir) && dir == Direccion.Derecha && dirAnt != Direccion.Izquierda) {
				Debug.Log ("hola he entrado derecha");

				dirAnt = Direccion.Derecha;
				IntercambioFichas (dir, ref tablero);            }


            //Arriba
            else if (VaciaMovible (posVacia, dir) && dir == Direccion.Arriba && dirAnt != Direccion.Abajo) {
				Debug.Log ("hola he entrado arriba");

				dirAnt = Direccion.Arriba;
				IntercambioFichas (dir, ref tablero);            }

            //Abajo
            else if (VaciaMovible (posVacia, dir) && dir == Direccion.Abajo && dirAnt != Direccion.Arriba) {
				Debug.Log ("hola he entrado abajo");

				dirAnt = Direccion.Abajo;
				IntercambioFichas (dir, ref tablero);
            }

            else if((dir == Direccion.Izquierda && dirAnt == Direccion.Derecha) || (dir == Direccion.Derecha && dirAnt == Direccion.Izquierda) ||
				(dir == Direccion.Arriba && dirAnt == Direccion.Abajo) || (dir == Direccion.Abajo && dirAnt == Direccion.Arriba)) {
				Debug.Log ("No he podido entrar porque la direccion es " + dir + "y la direccion anterior es" + dirAnt);

			}
			cont--;

		}
	
        updateTablero();
		GuardarTableroInicial ();
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

	void MueveVacio(Par fichaAdy, int aux, Direccion d)
	{

		if (d == Direccion.Izquierda)
			colocaFicha(fichaAdy.x, fichaAdy.y - 1, fichas[aux - 1]);	

		else if (d == Direccion.Derecha)
			colocaFicha(fichaAdy.x, fichaAdy.y + 1, fichas[aux - 1]);

		else if (d == Direccion.Arriba)
			colocaFicha(fichaAdy.x - 1, fichaAdy.y, fichas[aux - 1]);

		else if (d == Direccion.Abajo)
			colocaFicha(fichaAdy.x + 1, fichaAdy.y, fichas[aux - 1]);
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


	void updateTableroInicial()
	{

		//Coordenada y
		for (int i = 0; i < 3; i++)
		{
			//Coordenada x
			for (int j = 0; j < 3; j++)
			{
				//En la matriz, el valor 0, significa que no hay ficha
				if (tableroInicial[i, j] != 0)
					colocaFicha(j - 1, i - 1, fichas[tableroInicial[i, j] - 1]);
			}

		}

	}


    //Devuelve true si la ficha puede moverse
	bool esMovible(Par posFicha)
	{
        return (posFicha.x == posVacia.x && (Mathf.Abs(posFicha.y - posVacia.y) == 1) //Arriba y abajo
            || posFicha.y == posVacia.y && (Mathf.Abs(posFicha.x - posVacia.x) == 1)); //Izquierda y derecha
		
	}

	public void OnClick(){
		GenerarTableroInicial ();
		updateTableroInicial ();
	}


	void IntercambioFichas(Direccion dir, ref int[,] tab){
		int aux;//Auxiliar para intercambiar las posiciones del vacio y la ficha

		Par fichaAdy; //ficha adyacente
		fichaAdy.x = 0;
		fichaAdy.y = 0;

		switch (dir) {
		//Izquierda
		case 0:
			//Colocar las fichas, sin mover
			fichaAdy.x = posVacia.x - 1;
			fichaAdy.y = posVacia.y;

			posVacia.x--;
			break;

		//Derecha
		case 1:
			fichaAdy.x = posVacia.x + 1;
			fichaAdy.y = posVacia.y;

			posVacia.x++;
			break;

		//Arriba
		case 2:
			fichaAdy.x = posVacia.x;
			fichaAdy.y = posVacia.y - 1;

			posVacia.y--;
			break;

		//Abajo
		case 3:
			fichaAdy.x = posVacia.x;
			fichaAdy.y = posVacia.y +1;

			posVacia.y++;
			break;

		}

		aux = tablero [fichaAdy.y, fichaAdy.x]; //ficha de la izquierda

		tablero [posVacia.y, posVacia.x] = aux;//Guardo en la vacia el contenido de la adyacente
		tablero [fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0
	}

	void Adyacentes(){
			//Comprobar izquierda
			if (posVacia.x >= 1)   //Compruebo si puedo mover la vacía  
				return true;

			//Comprobar derecha
			else if (posVacia.x <= 1)
				return true;

			//Comprobar arriba
			else if (posVacia.y >= 1)
				return true;

			//Comprobar abajo
			else if (posVacia.y <= 1)
				return true;

			return false;
		}

	}

	/*
	public static List<Nodo> bfs(Nodo inicio, Nodo fin){
		Queue<Nodo> colaNodos = new Queue<Nodo> ();//Se crea la cola de nodos
		List<Nodo> visitados = new List<Nodo> ();//Se crea la lista de marcados (vector(?))
		bool solucion = false; 
		int [,] int tabAux = tablero;
		
		//Se añade el nodo inicial a la cola
		colaNodos.Enqueue(inicio);

		//Se marca como visitado el nodo inicial
		visitados.Add(inicio);

		inicio.history = new List<Nodo>();

		//while (!solucion && colaNodos.Count > 0) {
			//Extraemos de la cola el nodo
			Nodo actual = colaNodos.Dequeue ();

			//Comprobamos si hemos llegado a la configuración final
			if (actual == fin) {
				solucion = true;
			} 

				//si no esta marcado y es movible a la Derecha
				//si (VaciaMovible(fichaVacia,Direccion.Derecha)   //Si se puede mover la vacia a la derecha

				//Ahora creo el tablero con la logica de la vacia movida a la derecha

				//!visitados.Contains[tableroConLaVaciaMovidaALaDerecha])//Paso el tablero con el vacío movido

				//si no esta marcado y es movible a la Izquierda
				//si no esta marcado y es movible a la Arriba
				//si no esta marcado y es movible a la Abajo
			
			return visitados;
				//Se ha encontrado 123456780


	}


	public class Nodo{

		int [,] estado;

		public Nodo(int[,] estado){
			estado = estado;
		}
	}

*/

}
