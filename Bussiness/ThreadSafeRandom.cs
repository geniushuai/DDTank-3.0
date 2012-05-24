using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bussiness
{
    public class ThreadSafeRandom
    {
        #region static

        private static Random randomStatic = new Random();

        public static int NextStatic()
        {

            lock (randomStatic)
            {
                return randomStatic.Next();
            }

        }

        public static int NextStatic(int maxValue)
        {

            lock (randomStatic)
            {
                return randomStatic.Next(maxValue);
            }
        }

        public static int NextStatic(int minValue, int maxValue)
        {

            lock (randomStatic)
            {
                return randomStatic.Next(minValue, maxValue);
            }
        }

        public static void NextStatic(byte[] keys)
        {
            lock (randomStatic)
            {
                randomStatic.NextBytes(keys);
            }
        }
        #endregion

        private Random random = new Random();

        public int Next()
        {
            lock (random)
            {
                return random.Next();
            }
        }

        public int Next(int maxValue)
        {

            lock (random)
            {
                return random.Next(maxValue);
            }
        }

        public int Next(int minValue,int maxValue)
        {

            lock (random)
            {
                return random.Next(minValue,maxValue);
            }
        }



    }

}
