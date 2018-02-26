using System; //Sort
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{

	//Para las coordenadas de las fichas

	public static GameManager instance;

	int[,] tablero = new int[3, 3];
	GameObject[] fichas;
	const float distancia = 2.57f;
	enum Direccion { Izquierda, Derecha, Arriba, Abajo, Vacio };
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

	void Start()
	{
		instance = this;
		//Buscamos las fichas
		fichas = GameObject.FindGameObjectsWithTag("Ficha");

		//Ordenamos el array de fichas por Nombre
		IComparer myComparer = new NameComparer();
		Array.Sort(fichas, myComparer);

		// for (int i = 0; i < 8; i++)
		//Debug.Log("Ficha " + (i+1) + ": " + fichas[i]);


		//Lista de numeros randoms
		List<int> randoms = new List<int>();

		//int cont = 2; contador que mueve las fichas n veces


		//Tablero por defecto {1,2,3,4,5,6,7,8,0} FATAL HECHO SEGURO
		for (int i = 0; i < 3; i++)
		{
			for (int j = 0; j < 3; j++)
			{
				if (i == 1 && j == 2)
				{
					tablero[i, j] = 0;
					//Debug.Log (tablero [i, j]);
				}
				else if((i == 2 && j== 0) || (i == 2 && j== 1) || (i == 2 && j== 2))
				{
						tablero[i, j] = (i * 3 + j) ;
					//Debug.Log (tablero [i, j]);

				}
				else
				{
					tablero[i, j] = (i * 3 + j) + 1;
					//Debug.Log (tablero [i, j]);

				}
			}
		}
		updateTablero();

		int cont = 1;

		Direccion dir= Direccion.Vacio;//Direccion a la que se tiene que mover el vacio
		Direccion dirAnt = Direccion.Vacio; //Direccion cutre y de mierda para controlar que el estado anterior no sea el contrario

		//AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA
		while (cont != 0)
		{
			Par fichaVacia, fichaAdy; //ficha Vacia y adyacente
			fichaAdy.x = 0;
			fichaAdy.y = 0;

			fichaVacia = devuelveVacio(); //Hallamos la posicion de la ficha Vacia

			//Debug.Log("Para contador = " + cont + "El Vacio es: " + fichaVacia.y + ":" + fichaVacia.x);
			//Debug.Log("Ficha: " + fichaVacia.y + ":" + fichaVacia.x);

			int aux;//Auxiliar para intercambiar las posiciones del vacio y la ficha


			//Se genera el random de mierda
			int random = UnityEngine.Random.Range(0, 4);

			//Debug.Log("El random es: " + random);

			EligeDir(3, ref dir); //Se elige la dirección según el random VAMOS A PROBAR A MOVER EL VACIO A ALGUN LUGAR DONDE PUEDA BAJAAR AAAAAAAAAAAAAAAAAAA

			Debug.Log("Direccion actual: " + dir);

			Debug.Log ("Direccion anterior: " + dirAnt);

			//Izquierda
			if (VaciaMovible (fichaVacia, dir) && dir == Direccion.Izquierda && dirAnt != Direccion.Derecha) {
				Debug.Log ("hola he entrado izquierda");

				dirAnt = Direccion.Izquierda;

				//Colocar las fichas, sin mover
				fichaAdy.x = fichaVacia.x - 1;
				fichaAdy.y = fichaVacia.y;

				//Debug.Log ("la ficha Adyacente esta en el: " + fichaAdy.y + ":" + fichaAdy.x);
				//Debug.Log ("La ficha Vacia es: " + fichaVacia.y + ":" + fichaVacia.x + "y su contenido es" + tablero [fichaVacia.y, fichaVacia.x]);

				aux = tablero [fichaVacia.y, fichaVacia.x - 1]; //ficha de la izquierda
				//Debug.Log ("El aux tiene" + aux);

				tablero [fichaVacia.y, fichaVacia.x] = aux;//Guardo en la vacia el contenido de la adyacente
				tablero [fichaVacia.y, fichaVacia.x - 1] = 0;//Guardo en la adyacente el valor 0

				MueveVacio (fichaAdy, aux, dir);

			}

			//Derecha
			else if (VaciaMovible (fichaVacia, dir) && dir == Direccion.Derecha && dirAnt != Direccion.Izquierda) {
				Debug.Log ("hola he entrado derecha");

				dirAnt = Direccion.Derecha;

				fichaAdy.x = fichaVacia.x + 1;
				fichaAdy.y = fichaVacia.y;

				//Debug.Log ("la ficha Adyacente esta en el: " + fichaAdy.y + ":" + fichaAdy.x);
				//Debug.Log ("La ficha Vacia es: " + fichaVacia.y + ":" + fichaVacia.x + "y su contenido es" + tablero [fichaVacia.y, fichaVacia.x]);

				//Colocar las fichas, sin mover
				aux = tablero [fichaVacia.y, fichaVacia.x + 1]; //ficha de la derecha
				//Debug.Log ("El aux tiene" + aux);

				tablero [fichaVacia.y, fichaVacia.x] = aux;
				tablero [fichaVacia.y, fichaVacia.x + 1] = 0;

				MueveVacio (fichaAdy, aux, dir);
			}


			//Arriba
			else if (VaciaMovible (fichaVacia, dir) && dir == Direccion.Arriba && dirAnt != Direccion.Abajo) {
				Debug.Log ("hola he entrado arriba");

				dirAnt = Direccion.Arriba;

				fichaAdy.x = fichaVacia.x;
				fichaAdy.y = fichaVacia.y - 1;

				//Debug.Log ("la ficha Adyacente esta en el: " + fichaAdy.y + ":" + fichaAdy.x);
				//Debug.Log ("La ficha Vacia es: " + fichaVacia.y + ":" + fichaVacia.x + "y su contenido es" + tablero [fichaVacia.y, fichaVacia.x]);

				aux = tablero [fichaVacia.y - 1, fichaVacia.x]; //ficha de encima del hueco
				//Debug.Log ("El aux tiene" + aux);

				tablero [fichaVacia.y, fichaVacia.x] = aux;
				tablero [fichaVacia.y - 1, fichaVacia.x] = 0;


				MueveVacio (fichaAdy, aux, dir);
			}

			//Abajo
			else if (VaciaMovible (fichaVacia, dir) && dir == Direccion.Abajo && dirAnt != Direccion.Arriba) {
				Debug.Log ("hola he entrado abajo");

				dirAnt = Direccion.Abajo;

				fichaAdy.x = fichaVacia.x;
				fichaAdy.y = fichaVacia.y + 1;
				//Debug.Log ("la ficha Adyacente esta en el: " + fichaAdy.y + ":" + fichaAdy.x);
				//Debug.Log ("La ficha Vacia es: " + fichaVacia.y + ":" + fichaVacia.x + "y su contenido es" + tablero [fichaVacia.y, fichaVacia.x]);

				aux = tablero [fichaVacia.y + 1, fichaVacia.x];
				//Debug.Log ("El aux tiene" + aux);

				tablero [fichaVacia.y, fichaVacia.x] = aux;
				tablero [fichaVacia.y + 1, fichaVacia.x] = 0;

				MueveVacio (fichaAdy, aux, dir);


			} 

			else if((dir == Direccion.Izquierda && dirAnt == Direccion.Derecha) || (dir == Direccion.Derecha && dirAnt == Direccion.Izquierda) ||
				(dir == Direccion.Arriba && dirAnt == Direccion.Abajo) || (dir == Direccion.Abajo && dirAnt == Direccion.Arriba)) {
				Debug.Log ("No he podido entrar porque la direccion es " + dir + "y la direccion anterior es" + dirAnt);

			}
			cont--;

		}
		//updateTablero();
	}

	//Método que establece una dirección según el random dado
	void EligeDir(int random, ref Direccion dir)
	{
		switch (random)
		{
		//Izquierda
		case 0:
			dir = Direccion.Izquierda;
			break;

			//Derecha
		case 1:
			dir = Direccion.Derecha;
			break;

			//Arrriba
		case 2:
			dir = Direccion.Arriba;
			break;

			//Abajo
		case 3:
			dir = Direccion.Abajo;
			break;

		}
	}

	bool VaciaMovible(Par fichaVacia, Direccion d)
	{
		//Comprobar izquierda
		if (d == Direccion.Izquierda && fichaVacia.x >= 1)   //Compruebo si puedo mover la vacía  
			return true;


		//Comprobar derecha
		else if (d == Direccion.Derecha && fichaVacia.x <= 1)
			return true;


		//Comprobar arriba
		else if (d == Direccion.Arriba && fichaVacia.y >= 1)
			return true;


		//Comprobar abajo
		else if (d == Direccion.Abajo && fichaVacia.y <= 1)
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
		//Dar a los gameObjects la posición aleatoria del tablero

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


		return (contador == 0);

	}

	// Update is called once per frame
	void Update()
	{
		if (comprobarVictoria())
			Debug.Log("HAS GANADO");
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
		while ((!encontradaFicha || !encontradoVacio) && i < 3)
		{
			j = 0;
			while ((!encontradaFicha || !encontradoVacio) && j < 3)
			{
				//Debug.Log(tablero[i, j]);

				if (tablero[i, j] == numFicha)
				{
					encontradaFicha = true;
					posFicha.y = i;
					posFicha.x = j;
				}
				else if (tablero[i, j] == 0)
				{
					encontradoVacio = true;
					posVacia.y = i;
					posVacia.x = j;
				}
				j++;
			}
			i++;
		}

		if (esMovible(posFicha))
		{
			tablero[posFicha.y, posFicha.x] = 0;
			tablero[posVacia.y, posVacia.x] = numFicha;

		}

		updateTablero();

	}


	bool esMovible(Par posFicha)
	{
		//Comprobar izquierda
		if (posFicha.x >= 1 && tablero[posFicha.y, posFicha.x - 1] == 0)
		{
			return true;
		}

		//Comprobar derecha
		else if (posFicha.x <= 1 && tablero[posFicha.y, posFicha.x + 1] == 0)
		{
			return true;
		}

		//Comprobar arriba
		else if (posFicha.y >= 1 && tablero[posFicha.y - 1, posFicha.x] == 0)
		{
			return true;
		}

		//Comprobar abajo
		else if (posFicha.y <= 1 && tablero[posFicha.y + 1, posFicha.x] == 0)
		{
			return true;
		}
		return false;
	}

	Par devuelveVacio()
	{
		Par posVacia;
		bool encontradoVacio = false;
		posVacia.x = posVacia.y = 0;

		int i = 0, j = 0;

		//Encontrar el hueco vacio
		while (!encontradoVacio && i < 3)
		{
			j = 0;
			while (!encontradoVacio && j < 3)
			{
				//Debug.Log(tablero[i, j]);

				//Si la ficha es vacía
				if (tablero[i, j] == 0)
				{
					encontradoVacio = true;
					posVacia.y = i;
					posVacia.x = j;
				}
				j++;
			}
			i++;
		}
		return posVacia;
	}


}
