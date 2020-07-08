using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Densetsu.App
{
    public class PartsCounter : MonoBehaviour
    {
        private static readonly Dictionary<string, int> register = new Dictionary<string, int>();

        public static void RegisterPart (string partID)
        {
            if (!register.ContainsKey(partID))
            {
                register.Add(partID, 0);
            }

            register[partID]++;


            PrintRegister();
        }

        private static void PrintRegister()
        {
            foreach(var kvp in register)
            {
                Debug.Log(kvp.Key + " : " + kvp.Value);
            }
        }
    }
}
