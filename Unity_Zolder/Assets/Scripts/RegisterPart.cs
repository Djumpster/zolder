using Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Densetsu.App
{
    public class RegisterPart : MonoBehaviour
    {
        [SerializeField, ConstantTag(typeof(string), typeof(PartIdentifiers))] private string partID;

        protected void Start()
        {

        }
    }
}
