﻿using Talespin.Core.Foundation.Attributes;
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
