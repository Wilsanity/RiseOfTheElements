using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(TitleAttribute))]
public class TitleDrawer : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        //get a reference to the attribute
        TitleAttribute titleAttribute = attribute as TitleAttribute;

        //Apply Styles to the Text
        GUIStyle headStyle = new GUIStyle();
        headStyle.fontSize = titleAttribute.titleSize;
        headStyle.fontStyle = FontStyle.Bold;
        headStyle.normal.textColor = GetColour(titleAttribute);
        headStyle.alignment = GetAlignment(titleAttribute);

        //Drawing Text
        GUI.Label(new Rect(position.xMin, position.yMin, position.width, position.height),
                        titleAttribute.titleText,
                        headStyle);

       
    }
    
    private TextAnchor GetAlignment(TitleAttribute titleAttribute)
    {
        switch(titleAttribute.titleAlignment)
        {
            case TextAlignment.Left:
                return TextAnchor.MiddleLeft;
            case TextAlignment.Right:
                return TextAnchor.MiddleRight;
            case TextAlignment.Center:
                return TextAnchor.MiddleCenter;
            default:
                return TextAnchor.MiddleCenter;
        }
    }
    private Color GetColour(TitleAttribute titleAttribute)
    {
        switch(titleAttribute.titleColour)
        {
            case TextColour.White:
                return Color.white;
            case TextColour.Black:
                return Color.black;
            case TextColour.Red:
                return Color.red;
            case TextColour.Blue:
                return Color.blue;
            case TextColour.Yellow:
                return Color.yellow;
            case TextColour.Green:
                return Color.green;
            case TextColour.Orange:
                return new Color(255, 127, 80);
            case TextColour.Purple:
                return new Color(128, 0, 128);
            case TextColour.Brown:
                return new Color(222, 184, 135);
            case TextColour.Aqua:
                return new Color(0, 191, 255);
            case TextColour.Pink:
                return new Color(255,182,193);
            default:
                return Color.white;
        }
    }
    public override float GetHeight()
    {
        //get a reference to the attribute
        TitleAttribute titleAttribute = attribute as TitleAttribute;

        return titleAttribute.titleSize * 1.5f;
    }

}
