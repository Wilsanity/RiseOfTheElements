using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this line allows us to write multiple separator attributes in a row. Regularily we cannot
[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = true)]
public class SeparatorAttribute : PropertyAttribute
{
    public readonly float strokeWeight;
    public readonly float lineSpacing;


    public SeparatorAttribute(float _strokeWeight = 1, float _lineSpacing = 10)
    {
        strokeWeight = _strokeWeight; //default value of 1
        lineSpacing = _lineSpacing; //default value of 10
    }
}
