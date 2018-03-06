using System; //Sort
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;




public class GameManager : MonoBehaviour
{

    //----------------------------------TIPOS PROPIOS--------------------------------------

    //Representa posiciones del tablero (Origen Arriba - Izquierda)
    struct Par
    {
        public int x;
        public int y;
    }

    //Enumerado con las direcciones que puede tomar la ficha
    public enum Direccion { Arriba, Izquierda, Abajo, Derecha, Vacio };

    public class Nodo
    {
        private int[,] _tab; //Configuración
        private Nodo _padre;
        private Direccion _operador; //Operador que se aplicó al nodo padre para generar este nodo hijo
        private int _coste; //Coste de la ruta: Desde la raíz hasta aquí

        public Nodo(int[,] tab, Nodo padre, Direccion operador, int coste)
        {
            _tab = new int[3, 3];
            IgualarTablero(ref _tab, tab);
            _padre = padre;
            _operador = operador;
            _coste = coste;

        }

        //Getters para obtener el tablero, el coste, el padre del nodo y el operador

        public int[,] getTablero()
        {
            return _tab;
        }

        public int getCoste()
        {
            return _coste;
        }

        public Nodo getPadre()
        {
            return _padre;
        }

        public Direccion getOperador()
        {
            return _operador;
        }

    }
    //----------------------------------TIPOS PROPIOS--------------------------------------


    //----------------------------------ATRIBUTOS--------------------------------------

    public Text _text;
    public Text _textoEstadisticas;
    public Button _botonRestablecer;

    public static GameManager instance;

    int[,] _tablero = new int[3, 3];//Lógica del juego
    int[,] _tableroSol = new int[3, 3];//Tablero solución
    int[,] _tableroCopia = new int[3, 3];//Para restablecer

    Par _posVacia;//Posición de la ficha vacía
    const float _distancia = 2.57f;//Distancia entre fichas

    GameObject[] _fichas;
    Stack<Direccion> colaDir;

    bool _puedoMover;//Booleano para detectar si se puede pulsar un botón

    //----------------------------------ATRIBUTOS--------------------------------------

    //----------------------------------CICLO DE VIDA--------------------------------------

    void Start()
    {
        instance = this;

        _puedoMover = true;

        //Buscamos las fichas
        _fichas = GameObject.FindGameObjectsWithTag("Ficha");

        //Ordenamos el array de fichas por Nombre
        IComparer myComparer = new NameComparer();
        Array.Sort(_fichas, myComparer);

        //Inicializamos el tablero y lo actualizamos
        IniciaTablero();

        UpdateTablero();
    }

    //Método de mover fichas después de un tiempo
    void AvanzaUnPaso()
    {
        if (colaDir.Count > 0)
        {
            Direccion dir = colaDir.Pop();
            IntercambioFichas(ref _posVacia, dir, ref _tablero);
            UpdateTablero();

            Invoke("AvanzaUnPaso", 0.5f);

        }
        else
            _puedoMover = true;


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
                _tableroSol[i, j] = _tablero[i, j] = i * 3 + j + 1;


        _tableroSol[2, 2] = _tablero[2, 2] = 0;

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
    static void IgualarTablero(ref int[,] tab, int[,] tab2)
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
            direcciones[1] = true;
            numAdy++;
        }
        else
            direcciones[1] = false;


        //Comprobar derecha
        if ((posVaciaAux.x <= 1))
        {
            direcciones[3] = true;
            numAdy++;
        }
        else
            direcciones[3] = false;

        //Comprobar arriba
        if ((posVaciaAux.y >= 1))
        {
            direcciones[0] = true;
            numAdy++;
        }
        else
            direcciones[0] = false;


        //Comprobar abajo
        if ((posVaciaAux.y <= 1))
        {
            direcciones[2] = true;
            numAdy++;
        }
        else
            direcciones[2] = false;

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
        //Comprobamos si la ficha se mueve
        if (EsMovible(posFicha))
        {
            _tablero[posFicha.y, posFicha.x] = 0;
            _tablero[_posVacia.y, _posVacia.x] = numFicha;

            //La nueva posición vacía es donde estaba la ficha pulsada
            _posVacia = posFicha;

            //Comprobamos si hemos resuelto el puzzle
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

    void BorraTexto()
    {
        _text.text = "";
    }

    void BorraEstadisticas()
    {
        _textoEstadisticas.text = "Estadísticas:";
    }

    //---------------------------BOTONES--------------------------------------------
    //Generamos el algoritmo de búsqueda por anchura al pulsar la tecla de BFS

    struct Estadistica
    {
        public int numNodos;
        public int numMovimientos;
    }
    public void OnClickBFS()
    {

        if (_puedoMover)
        {
            _puedoMover = false;

            Estadistica estadistica = new Estadistica();
            Nodo nodo = Bfs(_tablero, _tableroSol, ref estadistica);
            colaDir = new Stack<Direccion>();

            if (nodo != null)
            {

                while (nodo.getPadre() != null)
                {
                    colaDir.Push(nodo.getOperador());
                    nodo = nodo.getPadre();
                }

                _textoEstadisticas.text = "ESTADÍSTICAS BFS:" + '\n' + "Número de movimientos: "
                    + estadistica.numMovimientos + '\n' + "Número de nodos: " + estadistica.numNodos;

                Invoke("BorraEstadisticas", 10.0f);
                UpdateTablero();

                Invoke("AvanzaUnPaso", 0.5f);
            }

            //DEMASIADOS MOVIMIENTOS
            else
            {
                _puedoMover = true;
                _text.text = "LO SIENTO BFS NO DA PARA TANTO";
                Invoke("BorraTexto", 5.0f);
            }
        }

    }

    //Generamos el algoritmo de búsqueda por profundidad al pulsar la tecla de DFS
    public void OnClickDFS()
    {

        if (_puedoMover)
        {
            _puedoMover = false;

            Estadistica estadistica = new Estadistica();

            Resultado resultado = Dfs(_tablero, _tableroSol, 10, ref estadistica);

            if (resultado.fallo)
            {
                _puedoMover = true;
                _text.text = "Voy a tardar demasiado, no ejecuto, +20000 nodos";
                Invoke("BorraTexto", 5.0f);

            }
            else if (resultado.corte)
            {
                _puedoMover = true;
                _text.text = "CON ESTE NÚMERO DE ITERACIONES NO ALCANZO EL RESULTADO";
                Invoke("BorraTexto", 5.0f);
            }

            else
            {
                Nodo nodo = resultado.getNodo();
                colaDir = new Stack<Direccion>();

                if (nodo != null)
                {

                    while (nodo.getPadre() != null)
                    {
                        colaDir.Push(nodo.getOperador());
                        nodo = nodo.getPadre();
                    }

                    _textoEstadisticas.text = "ESTADÍSTICAS DFS:" + '\n' + "Número de movimientos: "
                    + estadistica.numMovimientos + '\n' + "Número de nodos: " + estadistica.numNodos;

                    Invoke("BorraEstadisticas", 10.0f);

                    UpdateTablero();

                    Invoke("AvanzaUnPaso", 0.5f);
                }

            }
        }
    }

    //Genera un tablero aleatorio al pulsar el botón de aleatorio
    public void OnClickAleatorio()
    {
        if (_puedoMover)
        {
            IniciaTablero();

            //Número de iteraciones que la pieza vacía es movida
            int iteraciones = 20;

            //Mover las fichas aleatoriamente un número de iteraciones
            while (iteraciones != 0)
            {
                Direccion dir = DameDirRandom();

                //Muevo el tablero
                IntercambioFichas(ref _posVacia, dir, ref _tablero);

                iteraciones--;
            }

            IgualarTablero(ref _tableroCopia, _tablero);

            _botonRestablecer.gameObject.SetActive(true);
            UpdateTablero();
        }
    }

    //Reestablece el tablero al pulsar el botón de Reestablecer
    public void OnClickRestablecer()
    {
        if (_puedoMover)
        {

            IgualarTablero(ref _tablero, _tableroCopia);

            _posVacia = EncontrarVacio(_tablero);

            UpdateTablero();
        }
    }
    //---------------------------BOTONES--------------------------------------------

    //----------------------------IA----------------------------------------------

    Queue<Nodo> DameAdyacentes(Nodo nodo)
    {
        Queue<Nodo> colaAdy = new Queue<Nodo>();//Se crea la cola de nodos

        bool[] ady = new bool[4];

        Par posVacia = EncontrarVacio(nodo.getTablero());

        int numAdy;
        ady = DameMovimientosPosibles(nodo.getTablero(), posVacia, out numAdy);

        if (UnityEngine.Random.Range(0, 2) == 0)
        {

            //Buscamos los adyacentes y los procesamos
            for (int i = 0; i < ady.Length; i++)
            {
                Par posVaciaAux;
                posVaciaAux.x = posVacia.x;
                posVaciaAux.y = posVacia.y;

                if (ady[i])
                {
                    int[,] tabAdy = new int[3, 3];
                    Direccion dir = (Direccion)i;
                    IgualarTablero(ref tabAdy, nodo.getTablero());
                    IntercambioFichas(ref posVaciaAux, dir, ref tabAdy);

                    Nodo aux = new Nodo(tabAdy, nodo, dir, nodo.getCoste() + 1);
                    colaAdy.Enqueue(aux);
                }
            }
        }

        else
        {
            //Buscamos los adyacentes y los procesamos
            for (int i = ady.Length - 1; i >= 0; i--)
            {
                Par posVaciaAux;
                posVaciaAux.x = posVacia.x;
                posVaciaAux.y = posVacia.y;

                if (ady[i])
                {
                    int[,] tabAdy = new int[3, 3];
                    Direccion dir = (Direccion)i;
                    IgualarTablero(ref tabAdy, nodo.getTablero());
                    IntercambioFichas(ref posVaciaAux, dir, ref tabAdy);

                    Nodo aux = new Nodo(tabAdy, nodo, dir, nodo.getCoste() + 1);
                    colaAdy.Enqueue(aux);
                }
            }


        }

        return colaAdy;
    }


    Nodo Bfs(int[,] inicio, int[,] fin, ref Estadistica estadistica)
    {
        int[,] actual = new int[3, 3];
        IgualarTablero(ref actual, inicio);

        estadistica.numNodos = 1;

        //Nodo raíz
        Nodo nodo = new Nodo(actual, null, Direccion.Vacio, 0);

        if (TablerosIguales(nodo.getTablero(), fin))
            return nodo;

        Queue<Nodo> cola = new Queue<Nodo>();//Se crea la cola de nodos

        //Se añade el nodo inicial a la cola
        cola.Enqueue(nodo);

        Hashtable visitados = new Hashtable();

        while (nodo.getCoste() <= 7)
        {
            if (cola.Count <= 0)
                return null;

            //Extraemos la cola del nodo
            nodo = cola.Dequeue();

            //La añadimos a visitados
            visitados.Add(nodo.getTablero().GetHashCode(), null);

            //Encontramos todas las configuraciones posibles
            Queue<Nodo> colaAdy = DameAdyacentes(nodo);//Se crea la cola de nodos


            while (colaAdy.Count > 0)
            {
                Nodo nodoAux = colaAdy.Dequeue();

                if (!cola.Contains(nodoAux) && !visitados.Contains(nodoAux.getTablero()))
                {
                    if (TablerosIguales(nodoAux.getTablero(), fin))
                    {
                        estadistica.numMovimientos = nodoAux.getCoste();
                        return nodoAux;
                    }

                    estadistica.numNodos++;
                    cola.Enqueue(nodoAux);
                }
            }
        }

        return null;
    }

    //Clase resultado con la configuracion del problema para el DFS
    class Resultado
    {
        private Nodo _nodo;
        public bool fallo;
        public bool corte;

        public Nodo getNodo()
        {
            return _nodo;
        }

        public Resultado(Nodo nodo, bool f, bool c)
        {
            _nodo = nodo;
            fallo = f;
            corte = c;
        }

    }

    Resultado Dfs(int[,] inicio, int[,] fin, int limite, ref Estadistica estadistica)
    {
        estadistica.numNodos = 1;
        int[,] actual = new int[3, 3];
        IgualarTablero(ref actual, inicio);

        Nodo nodo = new Nodo(actual, null, Direccion.Vacio, 0);
        return RecursividadDFS(nodo, fin, limite, ref estadistica);
    }

    Resultado RecursividadDFS(Nodo nodo, int[,] fin, int limite, ref Estadistica estadistica)
    {
        estadistica.numNodos++;

        if (estadistica.numNodos > 20000)
            return new Resultado(null, true, false);


        if (TablerosIguales(nodo.getTablero(), fin))
        {
            estadistica.numMovimientos = nodo.getCoste();
            Resultado resultado = new Resultado(nodo, false, false);
            return resultado;
        }

        //Devolvemos corte
        else if (limite == 0)
        {
            Resultado corte = new Resultado(null, false, true);
            return corte;
        }

        else
        {
            bool huboCorte = false;

            //Encontramos todas las configuraciones posibles
            Queue<Nodo> colaAdy = DameAdyacentes(nodo);//Se crea la cola de nodos

            while (colaAdy.Count > 0)
            {
                Nodo nodoAux = colaAdy.Dequeue();

                Resultado resultado = RecursividadDFS(nodoAux, fin, limite - 1, ref estadistica);

                //Si resultado es corte
                if (resultado.corte)
                    huboCorte = true;

                else if (!resultado.fallo)
                    return resultado;
                

            }
            if (huboCorte)
            {
                Resultado corte = new Resultado(null, false, true);
                return corte;
            }

            //Devolver fallo
            else
            {
                Resultado fallo = new Resultado(null, true, false);
                return fallo;

            }
        }
    }

    //----------------------------IA----------------------------------------------

}
