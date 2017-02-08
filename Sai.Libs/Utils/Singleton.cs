using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Utils
{
    public static class Singleton<T> where T : class, new()
    {
        // Fields
        private static T _Instance;

        // Methods
        static Singleton()
        {
            Singleton<T>._Instance = default(T);
        }

        public static T GetInstance()
        {
            if (Singleton<T>._Instance == null)
            {
                Interlocked.CompareExchange<T>(ref Singleton<T>._Instance, Activator.CreateInstance<T>(), default(T));
            }
            return Singleton<T>._Instance;
        }
    }



}
