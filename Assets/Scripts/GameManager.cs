using System; //Sort
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //----------------------------------TIPOS PROPIOS--------------------------------------

    //Representa posiciones del tablero (Origen Arriba - Izquierda)
    struct Par
    {
        public int x;
        public int y;
    }

    enum Direccion { Izquierda, Derecha, Arriba, Abajo, Vacio };

    //----------------------------------TIPOS PROPIOS--------------------------------------


    //----------------------------------ATRIBUTOS--------------------------------------

    public static GameManager instance;

    int[,] _tablero = new int[3, 3];//Lógica del juego
    int[,] _tableroSol = new int[3, 3];//Tablero solución
    Par _posVacia;//Posición de la ficha vacía
	const float _distancia = 2.57f;

    GameObject[] _fichas;

    //----------------------------------ATRIBUTOS--------------------------------------

    //----------------------------------CICLO DE VIDA--------------------------------------

    void Start()
    {
        instance = this;

        //Buscamos las fichas
        _fichas = GameObject.FindGameObjectsWithTag("Ficha");

        //Ordenamos el array de fichas por Nombre
        IComparer myComparer = new NameComparer();
        Array.Sort(_fichas, myComparer);

        IniciaTablero();

        //Número de iteraciones que la pieza vacía es movida
        int iteraciones = 10;

        //Mover las fichas aleatoriamente un número de iteraciones
        while (iteraciones != 0)
        {
            Direccion dir = DameDirRandom();

            //Muevo el tablero
            IntercambioFichas(ref _posVacia, dir, ref _tablero);

            iteraciones--;
        }

        UpdateTablero();

        //TO DO: COSAS DE IA
        //IA POR ANCHURA
        // tablero = Bfs(tablero, tableroSol);

        //updateTablero();

        _tablero = Bfs(_tablero,_tableroSol);

        UpdateTablero();

    }

    // Update is called once per frame
    void Update()
    {
 
    }

    //----------------------------------CICLO DE VIDA--------------------------------------

    //----------------------------------FUNCIONES AUXILIARES--------------------------------------

    //Ordena una lista de GameObjects por su nombre
    public class NameComparer : IComparer
    {
        // Calls CaseInsensitiveComparer.Compare on the monster name string.
        int IComparer.Compare(System.Object x, System.Object y)
        {
            return ((new CaseInsensitiveComparer()).Compare(((GameObject)x).name, ((GameObject)y).name));
        }
    }

	//Método que inicializa el tablero a la posición resultado
	void IniciaTablero()
	{
		//Tablero por defecto {1,2,3,4,5,6,7,8,0} 
		for (int i = 0; i < 3; i++)
			for (int j = 0; j < 3; j++)
				_tableroSol[i, j] = _tablero[i,j] = i * 3 + j + 1;


		_tableroSol[2, 2] = _tablero[2, 2] =  0;

		_posVacia.x = 2;
		_posVacia.y = 2;
	}

    //Devuelve una dirección random y correcta de movimiento de la ficha vacia
    Direccion DameDirRandom()
    {
        Direccion dir = Direccion.Vacio;//Direccion a la que se tiene que mover el vacio

        //Primero miramos cuantos movimientos son posibles
        int numMovPosibles = 0;
        bool[] ady = DameMovimientosPosibles(_tablero, _posVacia, out numMovPosibles);

        //Generamos un random entre los posibles
        int random = UnityEngine.Random.Range(0, numMovPosibles);

        //Encontramos el random
        int i = 0; int aux = 0;
        bool encontrado = false;
        while (!encontrado)
        {
            //Si la dirección es posible
            if (ady[i])
            {
                //Si es el random que toca
                if (aux == random)
                {
                    encontrado = true;
                    dir = (Direccion)i;

                }

                //Si todavía no es el random que toca
                else
                    aux++;
            }

            i++;
        }

        return dir;

    }

    //Intercambia el vacío con la ficha que apunte la direccion. El ultimo parámetro indica si modifica el tablero de juego
    void IntercambioFichas(ref Par posVacia, Direccion dir, ref int[,] tab)
    {
        Par fichaAdy; //ficha adyacente

        switch (dir)
        {
            //Izquierda
            case Direccion.Izquierda:
                //Calculamos posición de adyacente
                fichaAdy.x = posVacia.x - 1;
                fichaAdy.y = posVacia.y;

                //Intercambiamos las posiciones
                tab[posVacia.y, posVacia.x] = tab[fichaAdy.y, fichaAdy.x];//Guardo en la vacia el contenido de la adyacente
                tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0

                posVacia.x--;

                break;

            //Derecha
            case Direccion.Derecha:
                //Calculamos posición de adyacente
                fichaAdy.x = posVacia.x + 1;
                fichaAdy.y = posVacia.y;

                //Intercambiamos las posiciones
                tab[posVacia.y, posVacia.x] = tab[fichaAdy.y, fichaAdy.x];//Guardo en la vacia el contenido de la adyacente
                tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0

                posVacia.x++;

                break;

            //Arriba
            case Direccion.Arriba:
                //Calculamos posición de adyacente
                fichaAdy.x = posVacia.x;
                fichaAdy.y = posVacia.y - 1;

                //Intercambiamos las posiciones
                tab[posVacia.y, posVacia.x] = tab[fichaAdy.y, fichaAdy.x];//Guardo en la vacia el contenido de la adyacente
                tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0

                posVacia.y--;

                break;

            //Abajo
            case Direccion.Abajo:
                //Calculamos posición de adyacente
                fichaAdy.x = posVacia.x;
                fichaAdy.y = posVacia.y + 1;

                //Intercambiamos las posiciones
                tab[posVacia.y, posVacia.x] = tab[fichaAdy.y, fichaAdy.x];//Guardo en la vacia el contenido de la adyacente
                tab[fichaAdy.y, fichaAdy.x] = 0;//Guardo en la adyacente el valor 0

                posVacia.y++;

                break;

        }


    }

    //Hace una copia de un tablero en otro
    void IgualarTablero(int[,] tab, int[,] tab2)
    {

        //Coordenada y
        for (int i = 0; i < 3; i++)
        {
            //Coordenada x
            for (int j = 0; j < 3; j++)
            {
                tab[i, j] = tab2[i, j];
            }

        }

    }

    //Método que devuelve si dos mátrices son iguales
    bool TablerosIguales(int[,] tab1, int[,] tab2)
    {
        int i = 0;
        bool iguales = true;
        while (iguales && i < 3)
        {
            int j = 0;
            while (iguales && j < 3)
            {
                if (tab1[i, j] != tab2[i, j])
                    iguales = false;
                j++;
            }

            i++;
        }

        return iguales;
    }

    //Devuelve las coordenadas de la ficha vacía
    Par EncontrarVacio(int[,] tab)
    {
        Par posVaciaAux;
        posVaciaAux.x = posVaciaAux.y = 0;

        bool encontrado = false;
        int i = 0;
        while (!encontrado && i < 3)
        {
            int j = 0;
            while (!encontrado && j < 3)
            {
                if (tab[i, j] == 0)
                {
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

    //Devuelve todos los movimientos que puede hacer una posVacia y el número de posibles en un parámetro out
    bool[] DameMovimientosPosibles(int[,] tab, Par posVaciaAux, out int numAdy)
    {
        bool[] direcciones = new bool[4];
        numAdy = 0;

        //Comprobar izquierda
        if ((posVaciaAux.x >= 1))
        {
            direcciones[0] = true;
            numAdy++;
        }
        else
            direcciones[0] = false;


        //Comprobar derecha
        if ((posVaciaAux.x <= 1))
        {
            direcciones[1] = true;
            numAdy++;
        }
        else
            direcciones[1] = false;

        //Comprobar arriba
        if ((posVaciaAux.y >= 1))
        {
            direcciones[2] = true;
            numAdy++;
        }
        else
            direcciones[2] = false;


        //Comprobar abajo
        if ((posVaciaAux.y <= 1))
        {
            direcciones[3] = true;
            numAdy++;
        }
        else
            direcciones[3] = false;

        return direcciones;
    }


    //----------------------------------FUNCIONES AUXILIARES--------------------------------------


    //---------------------------REPRESENTACIÓN GAMEOBJECTS----------------------------------------

    //Updatea la posición de los gameObjects a los correspondientes a la lógica del tablero
    void UpdateTablero()
    {

        //Coordenada y
        for (int i = 0; i < 3; i++)
        {
            //Coordenada x
            for (int j = 0; j < 3; j++)
            {
                //En la matriz, el valor 0, significa que no hay ficha
                if (_tablero[i, j] != 0)
                    ColocaFicha(j - 1, i - 1, _fichas[_tablero[i, j] - 1]);
            }

        }

    }

    //Recibe unas coordenadas del tablero y un gameObject y coloca el gameObject en la posición correspondiente al tablero
    void ColocaFicha(int x, int y, GameObject ficha)
    {
        ficha.transform.position = new Vector3(x * _distancia, -y * _distancia, 0);

    }

    //---------------------------REPRESENTACIÓN GAMEOBJECTS----------------------------------------


    //---------------------------MOVER FICHA----------------------------------------

    //Se le llama cuando se pulsa a una ficha, mueve la ficha si es adyacente al hueco vacío
    public void FichaPulsada(int numFicha)
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

                if (_tablero[i, j] == numFicha)
                {
                    encontradaFicha = true;
                    posFicha.y = i;
                    posFicha.x = j;
                }
                j++;
            }
            i++;
        }

        if (EsMovible(posFicha))
        {
            _tablero[posFicha.y, posFicha.x] = 0;
            _tablero[_posVacia.y, _posVacia.x] = numFicha;

            //La nueva posición vacía es donde estaba la ficha pulsada
            _posVacia = posFicha;

            //TO DO: Pasa algo si hay victoria
            ComprobarVictoria();

        }

        UpdateTablero();

    }

    //Devuelve true si la ficha puede moverse
    bool EsMovible(Par posFicha)
    {
        return (posFicha.x == _posVacia.x && (Mathf.Abs(posFicha.y - _posVacia.y) == 1) //Arriba y abajo
            || posFicha.y == _posVacia.y && (Mathf.Abs(posFicha.x - _posVacia.x) == 1)); //Izquierda y derecha

    }

    //Comprueba si el puzzle se ha resuelto
    bool ComprobarVictoria()
    {
        int i = 0;
        int contador = 8;
        while (contador > 0 && i < 3)
        {
            int j = 0;
            while (contador > 0 && j < 3)
            {
                if (_tablero[i, j] == i * 3 + j + 1)
                    contador--;

                else
                    contador = -1;
                j++;
            }
            i++;
        }

        return (contador == 0);
    }


    //---------------------------MOVER FICHA----------------------------------------

    //---------------------------BOTONES--------------------------------------------
    public void OnClick() {
        //GenerarTableroInicial ();
        //updateTableroInicial ();
    }
    //---------------------------BOTONES--------------------------------------------

    int[,] Bfs(int[,] inicio, int[,] fin)
    {
        Queue<int[,]> colaTab = new Queue<int[,]>();//Se crea la cola de nodos
        List<int[,]> visitado = new List<int[,]>();//Se crea la lista de marcados (vector(?))

        bool[] ady = new bool[4];
        bool solucion = false;

        //inicio.history = new List<Nodo>();
        int[,] actual = new int[3, 3];
        IgualarTablero(actual, inicio);

        //Se añade el nodo inicial a la cola
        colaTab.Enqueue(actual);

        //Se marca como visitado el nodo inicial
        visitado.Add(actual);

        while (!solucion && colaTab.Count > 0)
        {
            Queue<int[,]> colaAdy = new Queue<int[,]>();//Se crea la cola de nodos

            //Extraemos de la cola el nodo
            actual = colaTab.Dequeue();

            //Comprobamos si hemos llegado a la configuración final
            //Se ha encontrado 123456780

            if (TablerosIguales(actual, fin))
                solucion = true;

            else
            {
                Par posVacia = EncontrarVacio(actual);
                int numAdy;
                ady = DameMovimientosPosibles(actual, posVacia, out numAdy);

                //Buscamos los adyacentes y los procesamos
                for (int i = 0; i < ady.Length; i++)
                {
                    Par posVaciaAux;
                    posVaciaAux.x = posVacia.x;
                    posVaciaAux.y = posVacia.y;

                    if (ady[i])
                    {
                        int[,] tabAdy = new int[3, 3];
                        IgualarTablero(tabAdy, actual);
                        IntercambioFichas(ref posVaciaAux, (Direccion)i, ref tabAdy);

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
