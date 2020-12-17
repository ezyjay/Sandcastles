using UnityEngine;
using System;

[AttributeUsage
    (
        AttributeTargets.Field,
        AllowMultiple = false,
        Inherited = true
    )
]
public class ShowIfAttribute : PropertyAttribute
{
    public string EnumField;
    public object EnumValue;
    public object EnumValue2;

    public ShowIfAttribute (string enumField, object enumValue)
    {
        EnumField = enumField;
        EnumValue = enumValue;
    }

    public ShowIfAttribute(string enumField, object enumValue, object enumValue2) {
        EnumField = enumField;
        EnumValue = enumValue;
        EnumValue2 = enumValue2;
    }
}