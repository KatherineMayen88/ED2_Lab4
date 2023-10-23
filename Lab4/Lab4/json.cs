using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net.Mail;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using static System.Collections.Specialized.BitVector32;
using Newtonsoft.Json;
using Lab4;
using System.Numerics;

namespace Lab4
{
    public class json
    {
        public static ArbolAVL<Persona> arbol = new ArbolAVL<Persona>();

        public static List<string> list_compresion = new List<string>();
        public static List<string> list_descompresion = new List<string>();
        public static List<string> list_diccionario = new List<string>();


        public static void leerArchivo()
        {
            string filePath = @"C:/Users/kathe/source/repos/Lab4/input(3).csv";
            string filePathJsonL = @"C:/Users/kathe/source/repos/Lab4/convertidos.txt";
            //"C:\Users\kathe\source\repos\Lab2\convertidos.txt"
            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(';');
                    if (parts.Length == 2)
                    {
                        string action = parts[0].Trim();
                        string dataJson = parts[1].Trim();

                        Persona person = Newtonsoft.Json.JsonConvert.DeserializeObject<Persona>(dataJson);
                        commandReader(action, person, arbol);
                    }
                }
                GuardarArbolEnJsonl(arbol, filePathJsonL);
            }
            catch (Exception)
            {
                Console.WriteLine("ERROR AL CARGAR EL ARCHIVO.");
            }
        }

        public static void commandReader(string action, Persona person, ArbolAVL<Persona> arbol)
        {
            if (action == "INSERT")
            {
                try
                {
                    string name = person.name;
                    string dpi = Convert.ToString(person.DPI);
                    string compresion = string.Join(" ", person.companies);
                    string comprimidos = compresionLZ78(compresion);

                    list_compresion.Add("\n----------DATOS DE LA PERSONA----------\n" + "Nombre: " + name + "\nDPI: " + dpi + "\nCompañias cifradas:\n<" + comprimidos);
                    list_descompresion.Add("\n----------DATOS DE LA PERSONA----------\n" + "Nombre: " + name + "\nDPI: " + dpi + "\nCompañias descifradas:\n" + descompresionLZ78(comprimidos));

                    arbol.Add(person);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR EN INSERT: " + ex);
                    throw;
                }


            }
            else if (action == "DELETE")
            {
                try
                {
                    arbol.Remove(person);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR EN DELETE " + ex);
                    throw;
                }

            }
            else if (action == "PATCH")
            {
                try
                {
                    arbol.Update(person, person.DPI);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR EN PATCH " + ex);
                    throw;
                }
            }
        }

        public static void GuardarArbolEnJsonl(ArbolAVL<Persona> arbol, string filePath)
        {
            try
            {
                List<string> jsonLines = new List<string>();

                List<Persona> elementos = arbol.ObtenerListaOrdenada(); // Cambia esto según el nombre de tu método

                foreach (var persona in elementos)
                {
                    string jsonData = JsonConvert.SerializeObject(persona);
                    jsonLines.Add($"{jsonData}");
                }

                File.WriteAllLines(filePath, jsonLines);
                Console.WriteLine("ÁRBOL GUARDADO, ruta: " + filePath + "\n\n");
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR al guardar el árbol en el archivo JSON: " + ex.Message);
            }
        }

        public static string personaBuscada(long dpiABuscar)
        {
            nodo<Persona> nodoEncontrado = arbol.GetDPI(dpiABuscar);

            if (nodoEncontrado != null)
            {
                Persona personaEncontrada = nodoEncontrado.value;
                //string companies = string.Join(" ", personaEncontrada.companies);
                return ($"\n----------DATOS DE LA PERSONA----------\nNombre: {personaEncontrada.name} \nDPI: {personaEncontrada.DPI} \nFecha de nacimiento: {personaEncontrada.datebirth} \nDirección: {personaEncontrada.address}");
            }
            else
            {
                return ($"No se encontró ninguna persona con el DPI {dpiABuscar}");
            }
        }


        //LABORATORIO 2
        public static string aplicacion_de_compresion(long dpiBuscar)
        {
            string buscarDPI = Convert.ToString(dpiBuscar);
            string encontrado = list_compresion.Find(s => s.Contains(buscarDPI));
            return encontrado;
        }

        public static string compresionLZ78(string compresion)
        {
            string texto = "";
            string comparar_textos = "";
            int index = 0;
            int vretornar = 0;

            texto = compresion;
            compresion = "0 " + texto[0] + ">   \n<";

            list_diccionario.Add(""); //el primer elemento es null
            list_diccionario.Add(texto[0] + ">   <");

            for (int indexText = 1; indexText < texto.Length; indexText++)
            {
                comparar_textos += texto[indexText];

                if (list_diccionario.IndexOf(comparar_textos) != -1)
                {
                    index = list_diccionario.IndexOf(comparar_textos);

                    vretornar = 1; //si se repite la letra, se crea una condicion para que entre en el if la siguiente letra

                    if ((indexText + 1) == texto.Length)
                    {
                        compresion += index + " eof\n"; //end of line
                    }
                }
                else
                {
                    //LETRAS REPETIDAS
                    if (vretornar == 1)
                    {
                        //entra al if y coloca el index de la letra repetida y agrega la letra actual.
                        compresion += index + " " + comparar_textos[comparar_textos.Length - 1] + ">   \n<";
                    }
                    else
                    {
                        //si no se a repetido la letra antes, se coloca 0, letra
                        compresion += "0 " + comparar_textos + ">   \n<";
                    }

                    list_diccionario.Add(comparar_textos); //se agrega la letra al diccionario
                    comparar_textos = ""; //se reinicia el comparador

                    vretornar = 0; //regresa a 0 para no colver a entrar al if de letras repetidas
                }
            }
            return compresion;
        }

        //***
        public static string aplicacion_de_descompresion(long dpiBuscar)
        {
            string buscarDPI = Convert.ToString(dpiBuscar);
            string encontrado = list_descompresion.Find(s => s.Contains(buscarDPI));
            return encontrado;
        }
        //***
        public static string descompresionLZ78(string comprimido)
        {
            string texto = "";
            string next = "";
            int puntero = 0;

            texto = comprimido;
            string[] arregloComprimido = comprimido.Split();

            comprimido = "";

            for (int i = 0; i < texto.Length; i += 2)
            {
                if (arregloComprimido[i].Length == 0)
                {
                    break;
                }

                puntero = int.Parse(arregloComprimido[i]); //obtiene el puntero
                next = arregloComprimido[i + 1]; //obtiene el caracter

                if (next == "")
                {
                    next = "_";
                }

                if (next != "eof")
                {
                    comprimido += list_diccionario[puntero] + next;
                }
                else
                {
                    comprimido += list_diccionario[puntero];
                }

                puntero = 0;
                next = "";
            }

            puntero = 0;
            next = "";
            list_diccionario.Clear();
            return comprimido;
        }

        //este si es
        public static string pruebaDESCOMPRESION(long dpiABuscar)
        {
            nodo<Persona> nodoEncontrado = arbol.GetDPI(dpiABuscar);

            if (nodoEncontrado != null)
            {
                Persona personaEncontrada = nodoEncontrado.value;

                string companies = string.Join("     \n", personaEncontrada.companies);
                return ($"\n----------DATOS DE LA PERSONA----------\nNombre: {personaEncontrada.name} \nDPI: {personaEncontrada.DPI} \nFecha de nacimiento: {personaEncontrada.datebirth} \nDireccion: {personaEncontrada.address} \nCompañias:[\n {companies}");
            }
            else
            {
                return ($"No se encontró ninguna persona con el DPI {dpiABuscar}");
            }
        }



        //LABORATORIO 3
        private static int[] ObtenerIndex(string key)
        {
            int[] indexs = new int[key.Length];
            List<KeyValuePair<int, char>> keyOrdenada = new List<KeyValuePair<int, char>>();

            for (int i = 0; i < key.Length; i++)
            {
                keyOrdenada.Add(new KeyValuePair<int, char>(i, key[i]));
            }

            keyOrdenada.Sort
            (
                delegate (KeyValuePair<int, char> pair1, KeyValuePair<int, char> pair2)
                {
                    return pair1.Value.CompareTo(pair2.Value);
                }
            );

            for (int i = 0; i < key.Length; i++)
            {
                indexs[keyOrdenada[i].Key] = i;
            }
            return indexs;
        }

        public static void archivoBuscadoCifrado(long dpi)
        {
            string carpetaPath = "C:/Users/kathe/source/repos/Lab4/inputs";
            long buscado = dpi;
            string key = "cifrado";

            string[] nombresArchivos = Directory.GetFiles(carpetaPath);

            foreach (string archivo in nombresArchivos)
            {
                string nombreSinExtension = Path.GetFileNameWithoutExtension(archivo);
                string[] dpiNombre = nombreSinExtension.Split('-');
                long dpiConvertidoInt = Convert.ToInt64(dpiNombre[1]);

                if (dpiConvertidoInt == buscado)
                {
                    try
                    {
                        string contenido = File.ReadAllText(archivo);
                        Console.WriteLine("\n\nCarta " + nombreSinExtension + " cifrada: \n");
                        CifradoTransposicion(nombreSinExtension, contenido, key, '-');
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("ERROR en la lectura del archivo " + archivo);
                        throw;
                    }
                }
            }
        }

        public static void archivoBuscadoDescifrado(long dpi)
        {
            string carpetaPath = "C:/Users/kathe/source/repos/Lab4/Cartas cifradas";
            long buscado = dpi;
            string key = "cifrado";

            string[] nombresArchivos = Directory.GetFiles(carpetaPath);

            foreach (string archivo in nombresArchivos)
            {
                string nombreSinExtension = Path.GetFileNameWithoutExtension(archivo);
                string[] dpiNombre = nombreSinExtension.Split('-');
                long dpiConvertidoInt = Convert.ToInt64(dpiNombre[1]);

                if (dpiConvertidoInt == buscado)
                {
                    try
                    {
                        string contenido = File.ReadAllText(archivo);
                        Console.WriteLine("\n\nCarta " + nombreSinExtension + " descifrada: \n");
                        DescifradoTransposicion(contenido, key);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("ERROR en la lectura del archivo " + archivo);
                        throw;
                    }
                }
            }
        }

        public static void CifradoTransposicion(string nombreArchivo, string entrada, string key, char padC)
        {
            entrada = (entrada.Length % key.Length == 0) ? entrada : entrada.PadRight(entrada.Length - (entrada.Length % key.Length) + key.Length, padC);
            StringBuilder outputStringBuilder = new StringBuilder();

            int caracteresTotal = entrada.Length;
            int columnasTotal = key.Length;
            int filasTotal = (int)Math.Ceiling((double)caracteresTotal / columnasTotal);

            char[,] caracteresFila = new char[filasTotal, columnasTotal];
            char[,] caracteresColumna = new char[columnasTotal, filasTotal];
            char[,] columnasOrdenadas = new char[columnasTotal, filasTotal];

            int filaActual = 0;
            int columnaActual = 0;

            int[] mezclaIndexs = ObtenerIndex(key);

            for (int i = 0; i < caracteresTotal; i++)
            {
                filaActual = i / columnasTotal;
                columnaActual = i % columnasTotal;
                caracteresFila[filaActual, columnaActual] = entrada[i];
            }

            for (int i = 0; i < filasTotal; i++)
            {
                for (int j = 0; j < columnasTotal; j++)
                {
                    caracteresColumna[j, i] = caracteresFila[i, j];
                }
            }

            for (int i = 0; i < columnasTotal; i++)
            {
                for (int j = 0; j < filasTotal; j++)
                {
                    columnasOrdenadas[mezclaIndexs[i], j] = caracteresColumna[i, j];
                }
            }

            for (int i = 0; i < caracteresTotal; i++)
            {
                filaActual = i / filasTotal;
                columnaActual = i % filasTotal;
                outputStringBuilder.Append(columnasOrdenadas[filaActual, columnaActual]);
            }

            cifradoESCRITO(nombreArchivo, outputStringBuilder.ToString());
            Console.WriteLine(outputStringBuilder.ToString());
        }

        public static void DescifradoTransposicion(string entrada, string key)
        {
            StringBuilder outputStringBuilder = new StringBuilder();

            int caracteresTotal = entrada.Length;
            int filasTotal = key.Length;
            int columnasTotal = (int)Math.Ceiling((double)caracteresTotal / filasTotal);

            char[,] caracteresFila = new char[filasTotal, columnasTotal];
            char[,] caracteresColumna = new char[columnasTotal, filasTotal];
            char[,] columnasDesordenadas = new char[columnasTotal, filasTotal];

            int filaActual = 0;
            int columnaActual = 0;

            int[] mezclaIndexs = ObtenerIndex(key);

            for (int i = 0; i < caracteresTotal; i++)
            {
                filaActual = i / columnasTotal;
                columnaActual = i % columnasTotal;
                caracteresFila[filaActual, columnaActual] = entrada[i];
            }

            for (int i = 0; i < filasTotal; i++)
            {
                for (int j = 0; j < columnasTotal; j++)
                {
                    caracteresColumna[j, i] = caracteresFila[i, j];
                }
            }

            for (int i = 0; i < columnasTotal; i++)
            {
                for (int j = 0; j < filasTotal; j++)
                {
                    columnasDesordenadas[i, j] = caracteresColumna[i, mezclaIndexs[j]];
                }
            }

            for (int i = 0; i < caracteresTotal; i++)
            {
                filaActual = i / filasTotal;
                columnaActual = i % filasTotal;
                outputStringBuilder.Append(columnasDesordenadas[filaActual, columnaActual]);
            }

            Console.Write(outputStringBuilder.ToString());
        }

        public static void cifradoESCRITO(string archivo, string texto)
        {
            string contenido = texto;
            string pathFile = $"C:/Users/kathe/source/repos/Lab4/Cartas cifradas/{archivo}-cifrado";

            try
            {
                using (StreamWriter writer = new StreamWriter(pathFile))
                {
                    writer.Write(contenido);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("ERROR en la escritura del archivo");
                throw;
            }

        }

        /*POR SI SE ARRUINA
         
        public static string compresionLZ78(string compresion)
        {
            string texto = "";
            string comparar_textos = "";
            int index = 0;
            int vretornar = 0;

            texto = compresion;
            compresion = "0 " + texto[0] + "\n";

            list_diccionario.Add(""); //el primer elemento es null
            list_diccionario.Add(texto[0] + "");

            for (int indexText = 1; indexText < texto.Length; indexText++)
            {
                comparar_textos += texto[indexText];

                if (list_diccionario.IndexOf(comparar_textos) != -1)
                {
                    index = list_diccionario.IndexOf(comparar_textos);

                    vretornar = 1; //si se repite la letra, se crea una condicion para que entre en el if la siguiente letra

                    if ((indexText+1) == texto.Length)
                    {
                        compresion += index + " eof\n"; //end of line
                    }
                }
                else
                {
                    //LETRAS REPETIDAS
                    if (vretornar == 1)
                    {
                        //entra al if y coloca el index de la letra repetida y agrega la letra actual.
                        compresion += index + " " + comparar_textos[comparar_textos.Length - 1] + "\n";
                    }
                    else
                    {
                        //si no se a repetido la letra antes, se coloca 0, letra
                        compresion += "0 " + comparar_textos + "\n";
                    }

                    list_diccionario.Add(comparar_textos); //se agrega la letra al diccionario
                    comparar_textos = ""; //se reinicia el comparador

                    vretornar = 0; //regresa a 0 para no colver a entrar al if de letras repetidas
                }
            }
            return compresion;
        }
         
         
         
         */


        //LAB 4
        public static void ConversacionCifrado(long dpi)
        {
            RSA rsa = new RSA();
            string carpetaPath = @"C:\Users\kathe\source\repos\Lab4\inputs";
            long buscado = dpi;
            string key = "cifrado";
            string CONV = "CONV";
            BigInteger ClavePublica = rsa.RSAA(CONV);

            string[] nombresArchivos = Directory.GetFiles(carpetaPath);

            foreach (string archivo in nombresArchivos)
            {
                string nombreSinExtension = Path.GetFileNameWithoutExtension(archivo);
                string[] dpiNombre = nombreSinExtension.Split('-');
                long dpiConvertidoInt = Convert.ToInt64(dpiNombre[1]);

                if (dpiConvertidoInt == buscado && dpiNombre[0] == CONV) //Se agrega condición CONV debido a que las cartas y las conversaciones están en la misma carpeta
                {
                    try
                    {
                        string contenido = File.ReadAllText(archivo) + "\n" + ClavePublica;
                        Console.WriteLine("\n\nCarta " + nombreSinExtension + " cifrada: \n");
                        CifradoTransposicionC(nombreSinExtension, contenido, key, '-');
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("ERROR en la lectura del archivo " + archivo);
                        throw;
                    }
                }
            }
        }

        public static void ConversacionDescifrado(long dpi)
        {
            string carpetaPath = @"C:\Users\kathe\source\repos\Lab4\Conversaciones Cifradas";
            long buscado = dpi;
            string key = "cifrado";

            string[] nombresArchivos = Directory.GetFiles(carpetaPath);

            foreach (string archivo in nombresArchivos)
            {
                string nombreSinExtension = Path.GetFileNameWithoutExtension(archivo);
                string[] dpiNombre = nombreSinExtension.Split('-');
                long dpiConvertidoInt = Convert.ToInt64(dpiNombre[1]);

                if (dpiConvertidoInt == buscado)
                {
                    try
                    {
                        string contenido = File.ReadAllText(archivo);
                        Console.WriteLine("\n\nCarta " + nombreSinExtension + " descifrada: \n");
                        DescifradoTransposicion(contenido, key);
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("ERROR en la lectura del archivo " + archivo);
                        throw;
                    }
                }
            }
        }

        public static void convCifradoESCRITO(string archivo, string texto)
        {
            string contenido = texto;
            string pathFile = $@"C:\Users\kathe\source\repos\Lab4\Conversaciones Cifradas/{archivo}-cifrado";

            try
            {
                using (StreamWriter writer = new StreamWriter(pathFile))
                {
                    writer.Write(contenido);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("ERROR en la escritura del archivo");
                throw;
            }

        }

        public static void CifradoTransposicionC(string nombreArchivo, string entrada, string key, char padC)
        {
            entrada = (entrada.Length % key.Length == 0) ? entrada : entrada.PadRight(entrada.Length - (entrada.Length % key.Length) + key.Length, padC);
            StringBuilder outputStringBuilder = new StringBuilder();

            int caracteresTotal = entrada.Length;
            int columnasTotal = key.Length;
            int filasTotal = (int)Math.Ceiling((double)caracteresTotal / columnasTotal);

            char[,] caracteresFila = new char[filasTotal, columnasTotal];
            char[,] caracteresColumna = new char[columnasTotal, filasTotal];
            char[,] columnasOrdenadas = new char[columnasTotal, filasTotal];

            int filaActual = 0;
            int columnaActual = 0;

            int[] mezclaIndexs = ObtenerIndex(key);

            for (int i = 0; i < caracteresTotal; i++)
            {
                filaActual = i / columnasTotal;
                columnaActual = i % columnasTotal;
                caracteresFila[filaActual, columnaActual] = entrada[i];
            }

            for (int i = 0; i < filasTotal; i++)
            {
                for (int j = 0; j < columnasTotal; j++)
                {
                    caracteresColumna[j, i] = caracteresFila[i, j];
                }
            }

            for (int i = 0; i < columnasTotal; i++)
            {
                for (int j = 0; j < filasTotal; j++)
                {
                    columnasOrdenadas[mezclaIndexs[i], j] = caracteresColumna[i, j];
                }
            }

            for (int i = 0; i < caracteresTotal; i++)
            {
                filaActual = i / filasTotal;
                columnaActual = i % filasTotal;
                outputStringBuilder.Append(columnasOrdenadas[filaActual, columnaActual]);
            }

            convCifradoESCRITO(nombreArchivo, outputStringBuilder.ToString());
            Console.WriteLine(outputStringBuilder.ToString());
        }

    }
}

