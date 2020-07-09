using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Densetsu.App
{
    public class PartsCounter : MonoBehaviour
    {
        private static readonly Dictionary<string, List<GameObject>> register = new Dictionary<string, List<GameObject>>();

        public static void RegisterPart (string partID, GameObject registree)
        {
            if (!register.ContainsKey(partID))
            {
                register.Add(partID, new List<GameObject>());
            }
            if (!register[partID].Contains(registree))
            {
                register[partID].Add(registree);
            }
            PrintRegister();
        }

        private static void PrintRegister()
        {
            foreach(var kvp in register)
            {
                Debug.Log(kvp.Key + " : " + kvp.Value.Count);
            }
        }
    }
}
