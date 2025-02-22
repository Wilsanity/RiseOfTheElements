using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;


public class DialogueGraph : EditorWindow
{
    ObjectField sentenceField;
    Sentence startingSentence;
    private DialogueGraphView _graphView;

    private Toolbar toolbar;

    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent(text: "Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolBar();
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(_graphView);   
    }

    void Clear()
    {


        rootVisualElement.Remove(_graphView);


        //Weird work-around for issue with toolbar not being able to be removed...
        rootVisualElement.RemoveAt(0);

        ConstructGraphView();
        GenerateToolBar();
    }

    private void ConstructGraphView()
    {
        _graphView = new DialogueGraphView()
        {
            name = "Dialogue Graph"
        };

        _graphView.StretchToParentSize();
        rootVisualElement.Add(_graphView);

        _graphView.SendToBack();
    }

    private void GenerateToolBar()
    {
        var toolbar = new Toolbar();

        Button btn = new Button(clickEvent: () =>
        {
            DialogueSO dialogue = (DialogueSO)sentenceField.value;
            startingSentence = dialogue.startingSentence;
            if (startingSentence != null)
            {
                rootVisualElement.Remove(_graphView);
                ConstructGraphView();
                _graphView.AutoGenerateNodes(startingSentence);
            }
        })
        {
            text = "Show Dialogue Graph"
        };


        Button saveBtn = new Button(clickEvent: () => { _graphView.SaveConnections(); });
        saveBtn.text = "Save";

        Button clearBtn = new Button(clickEvent: () => { Clear(); });
        clearBtn.text = "Clear";


        sentenceField = new ObjectField();
        sentenceField.objectType = typeof(DialogueSO);

        toolbar.Add(sentenceField); 

        //Add our dialogue add button
        toolbar.Add(btn);

        //Add our dialogue save button
        toolbar.Add(saveBtn);

        //Add our dialogue clear button
        toolbar.Add(clearBtn);

        rootVisualElement.Add(toolbar);
    }
}
