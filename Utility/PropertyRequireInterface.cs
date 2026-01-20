using System;
using UnityEngine;

public class PropertyRequireInterface : PropertyAttribute
{
    public PropertyRequireInterface(Type interfaceType)
    {
        InterfaceType = interfaceType;
    }
    public Type InterfaceType { get; private set; }
}