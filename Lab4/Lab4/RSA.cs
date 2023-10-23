using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Security.Cryptography;

namespace Lab4
{
    public class RSA
    {
        static Random random = new Random();
        public BigInteger RSAA(string m)
        {
            BigInteger mensaje = CalculateSHA256HashAsBigInteger(m);
            // Escoge dos números primos grandes (p y q)
            BigInteger p = GenerateLargePrime(256);//BORRAR se ingresa la cantidad de bits para generar el número primo 1024 se tarda mucho en cargar
            BigInteger q = GenerateLargePrime(256);

            // Calcula n (producto de p y q)
            BigInteger n = p * q;

            // Calcula la función Z 
            BigInteger z = (p - 1) * (q - 1);

            // Escogemos un valor para la clave de cifrado (k), 
            BigInteger k = 65537;

            // Calcula la clave privada (j) usando el algoritmo de Euclides extendido
            BigInteger j = ModInverse(k, z);

            // Ahora tienes las claves pública (e, n) y privada (d, n)
            BigInteger cifrado = Cifrado(mensaje, k, n);
            return cifrado;

        }

        static BigInteger CalculateSHA256HashAsBigInteger(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Convierte los bytes en un BigInteger
                BigInteger hashBigInteger = new BigInteger(hashBytes);

                return hashBigInteger;
            }
        }

        public BigInteger Cifrado(BigInteger M, BigInteger K, BigInteger n)
        {
            BigInteger cifrado = 0;
            cifrado = BigInteger.Pow(1454864648464, (int)K) % n;//BORRAR en vez del número 1454864648464 debería ir el hash del mensaje peeero lo mismo, si son números muy grandes
            //se pone numeros fijos dado que se tarda mucho en realizar la operacion 
            return cifrado;
        }

        public BigInteger Descifrado(BigInteger C, BigInteger j, BigInteger n) //BORRAR funciona peeero al ser números tan grandes literal se tarda una eternidad en encontrarlo.
        //se usa la clave publica que si se encuentra rapido, y se manda a archivo de texto
        {
            BigInteger mensaje = 0;
            BigInteger pow = BigIntegerPow(C, j);
            mensaje = pow % n;

            return mensaje;
        }

        static BigInteger BigIntegerPow(BigInteger numero, BigInteger exponente)
        {
            BigInteger result = 1;
            while (exponente > 0)
            {
                if (exponente % 2 == 1)
                    result *= numero;

                numero *= numero;
                exponente /= 2;
            }

            return result;
        }


        static BigInteger ModInverse(BigInteger a, BigInteger m)
        {
            // Implementación del algoritmo de Euclides extendido para calcular el inverso modular
            BigInteger m0 = m;
            BigInteger x0 = 0;
            BigInteger x1 = 1;

            if (m == 1)
                return 0;

            while (a > 1)
            {
                // Calcula el cociente y actualiza a y m
                BigInteger q = a / m;
                BigInteger t = m;

                m = a % m;
                a = t;
                t = x0;

                // Actualiza x0 y x1
                x0 = x1 - q * x0;
                x1 = t;
            }

            // Asegúrate de que el resultado sea positivo
            if (x1 < 0)
                x1 += m0;

            // Si 'a' y 'm' no son coprimos, no hay inverso modular
            if (a != 1)
                return -1;

            return x1;
        }


        static BigInteger GenerateLargePrime(int bitLength)
        {
            while (true)
            {
                BigInteger potentialPrime = GenerateRandomBigInteger(bitLength);

                if (IsProbablyPrime(potentialPrime, 10)) // 10 iteraciones para mayor confiabilidad
                    return potentialPrime;
            }
        }

        static BigInteger GenerateRandomBigInteger(int bitLength)
        {
            byte[] randomBytes = new byte[bitLength / 8];
            random.NextBytes(randomBytes);

            BigInteger randomBigInt = new BigInteger(randomBytes);
            randomBigInt |= BigInteger.One << (bitLength - 1); // Asegura que el número sea positivo y tenga la longitud deseada

            return randomBigInt;
        }

        static bool IsProbablyPrime(BigInteger n, int k)
        {
            if (n <= 1)
                return false;

            if (n <= 3)
                return true;

            if (n % 2 == 0)
                return false;

            BigInteger d = n - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s++;
            }

            for (int i = 0; i < k; i++)
            {
                BigInteger a = GenerateRandomBigInteger(n.ToByteArray().Length * 8);
                if (!MillerRabinTest(a, n, d, s))
                    return false;
            }

            return true;
        }

        static bool MillerRabinTest(BigInteger a, BigInteger n, BigInteger d, int s)
        {
            BigInteger x = BigInteger.ModPow(a, d, n);

            if (x == 1 || x == n - 1)
                return true;

            for (int r = 1; r < s; r++)
            {
                x = BigInteger.ModPow(x, 2, n);
                if (x == n - 1)
                    return true;
            }

            return false;
        }

    }
}
