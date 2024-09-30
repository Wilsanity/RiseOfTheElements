using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SeparatorAttribute))]
public class SeparatorDrawer : DecoratorDrawer
{
    //the position in the parameter below is the position where we are declaring the attribute in the code (and in the inspector)
    public override void OnGUI(Rect position)
    {
        //get a reference to the attribute
        SeparatorAttribute separatorAttribute = attribute as SeparatorAttribute;

        //define the line to draw
        //position.xMin = the position all the way to the left, lowest value it can be
        //position.width = the full width across
        //can get access to values inside our separator attribute
        Rect separatorRect = new Rect(position.xMin, position.yMin + separatorAttribute.lineSpacing, position.width - 15, separatorAttribute.strokeWeight);

        //draw it
        EditorGUI.DrawRect(separatorRect, Color.white);


        
    }

    //Problem: If we need anything that requires more than 1 space in the inspector, the contents overlap. We want everything else to be pushed down
    //By default the GetHeight returns as 1 line, but that may not be enough for us
    //We need to calculate the height that we need
    public override float GetHeight()
    {
        //get a reference to the attribute
        SeparatorAttribute separatorAttribute = attribute as SeparatorAttribute;

        float totalSpacing = separatorAttribute.strokeWeight + (separatorAttribute.lineSpacing * 2);

        return totalSpacing;
    }
}
