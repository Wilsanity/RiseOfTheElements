using AmplifyShaderEditor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;


public class DialogueGraphView : GraphView
{
    private readonly Vector2 entryPosition = new Vector2(200, 200);

    private readonly Vector2 defaultNodeSize = new Vector2(200, 200);

    public DialogueGraphView()
    {
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);



        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());




        //AddElement(GenerateEntryPointNode());

        style.backgroundColor = new Color(0.16f, 0.16f, 0.16f);
    }

    private Port GeneratePort(DialogueNode node, Direction portDirection, Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }

    private DialogueNode GenerateEntryPointNode(Sentence sentence)
    {
        DialogueNode node = new DialogueNode
        {
            title = sentence.from.Value,
            Layer = 1,
            Index = 1,
            sentence = sentence,
            GUID = Guid.NewGuid().ToString(),
            conversationText = "Start Text",
            isEntryPoint = true
        };

        //Single Capacity since entry port
        var generatedPort = GeneratePort(node, Direction.Output);

        generatedPort.portName = "Next";

        node.outputPorts.Add(generatedPort);
        node.outputContainer.Add(generatedPort);


        TextField text = new TextField();
        text.SetValueWithoutNotify(sentence.text);
        text.multiline = true;
        text.style.maxWidth = 200;
        text.style.flexBasis = 150f;
        text.style.flexDirection = FlexDirection.Column;
        text.style.flexWrap = Wrap.Wrap;
        text.RegisterValueChangedCallback((evt) => sentence.text = evt.newValue);
        node.titleContainer.Add(text);


        node.RefreshExpandedState();
        node.RefreshPorts();


        node.SetPosition(new Rect(entryPosition, defaultNodeSize));
        return node;
    }

    public void RenderNode(DialogueNode node)
    {
        AddElement(node);
    }

    public DialogueNode CreateDialogueNode(string nodeName)
    {
        DialogueNode dialogueNode = new DialogueNode
        {
            title = nodeName,
            GUID = System.Guid.NewGuid().ToString(),
            conversationText = "txt..."
        };

        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        dialogueNode.inputContainer.Add(inputPort);


        Button button = new Button(clickEvent: () => { AddChoicePort(dialogueNode); });
        button.text = "New Choice";
        dialogueNode.titleContainer.Add(button);


        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(position: Vector2.zero, defaultNodeSize));
        return dialogueNode;
    }

    private void AddChoicePort(DialogueNode dialogueNode)
    {
        var generatePort = GeneratePort(dialogueNode, Direction.Output);

        var outputPortCount = dialogueNode.outputContainer.Query(name: "connector").ToList().Count;
        generatePort.portName = $"Choice {outputPortCount}";

        dialogueNode.outputContainer.Add(generatePort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();

    }

    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        Debug.Log("Compat ports?");
        var compatiblePorts = new List<Port>();
        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node)
            {
                compatiblePorts.Add(port);
            }
        });
        return compatiblePorts;
    }


    public void AutoGenerateNodes(Sentence startingSentence)
    {
        DialogueNode entryNode = GenerateEntryPointNode(startingSentence);
        RenderNode(entryNode);
        DialogueNode node = null;
        if (startingSentence.nextSentence != null)
        {
            node = CreateFollowingNode(startingSentence.nextSentence, 2, 1, entryNode, 0);
            RenderNode(node);
        }

        Recursion(startingSentence.nextSentence, 2, node);

    }


    public DialogueNode CreateFollowingNode(Sentence sentence, int layer, int index, DialogueNode parentNode, int parentPortIndex)
    {
        Debug.Log($"Layer{layer}, Index{index}, {sentence.name}");
        Debug.Log($"Parent: {parentNode.Layer},{parentNode.Index}");


        DialogueNode newDialogueNode = new DialogueNode
        {
            title = sentence.from.Value,
            Layer = layer,
            sentence = sentence,
            Index = index,
            GUID = System.Guid.NewGuid().ToString(),
            conversationText = "txt..."
        };


        var inputPort = GeneratePort(newDialogueNode, Direction.Input, Port.Capacity.Multi);
        newDialogueNode.inputPort = inputPort;
        inputPort.portName = "Previous";
        Edge edge = inputPort.ConnectTo(parentNode.outputPorts[parentPortIndex]);
        Add(edge);

        newDialogueNode.inputContainer.Add(inputPort);

        newDialogueNode.SetPosition(new Rect(new Vector2(300 * layer - 100, 250 * index - 50), defaultNodeSize));

        if(sentence.HasOptions())
        {
            for (int i = 0; i < sentence.options.Count; i++)
            {
                var port = GeneratePort(newDialogueNode, Direction.Output);
                var t = new TextField();
                t.SetValueWithoutNotify(sentence.options[i].text);
                //t.RegisterValueChangedCallback((evt) => sentence.options[0].text = evt.newValue);

                switch (i)
                {
                    case 0:
                        t.RegisterValueChangedCallback((evt) => sentence.options[0].text = evt.newValue);
                        break;

                    case 1:
                        t.RegisterValueChangedCallback((evt) => sentence.options[1].text = evt.newValue);
                        break;

                    case 2:
                        t.RegisterValueChangedCallback((evt) => sentence.options[2].text = evt.newValue);
                        break;
                    default:
                        break;
                }


                t.multiline = true;
                t.style.flexBasis = 100f;

                port.contentContainer.Add(t);
                port.portName = (i+1).ToString();
                newDialogueNode.outputPorts.Add(port);
                newDialogueNode.outputContainer.Add(port);
            }
        }
        else
        {

            TextField text = new TextField();
            text.SetValueWithoutNotify(sentence.text);
            text.multiline = true;
            text.style.flexBasis = 150f;

            text.RegisterValueChangedCallback((evt) => sentence.text = evt.newValue);
            newDialogueNode.titleContainer.Add(text);


            var port = GeneratePort(newDialogueNode, Direction.Output);
            port.portName = "Next";
            newDialogueNode.outputPorts.Add(port);
            newDialogueNode.outputContainer.Add(port);


        }

        newDialogueNode.RefreshExpandedState();
        newDialogueNode.RefreshPorts();

        return newDialogueNode;

    }

    int[] layers = new int[100];
    List<Sentence> repeatedSentence = new List<Sentence>();
    List<DialogueNode> repeatedNodes = new List<DialogueNode>();

    bool CheckRepeat(Sentence sentence)
    {
        foreach (var node in repeatedNodes)
        {
            if (node.sentence == sentence)
            {
                return true;
            }
        }
        return false;
    }

    DialogueNode GetRepeatedNode(Sentence sentence)
    {
        foreach (var node in repeatedNodes)
        {
            if (node.sentence == sentence)
            {
                return node;
            }
        }
        return null;
    }


    void Recursion(Sentence sentence, int layer, DialogueNode parentNode)
    {
        Sentence currentSentence = sentence;

        while (currentSentence != null)
        {

            if (!currentSentence.HasOptions())
            {

                currentSentence = currentSentence.nextSentence;
                if (currentSentence != null)
                {
                    bool repeated = CheckRepeat(currentSentence);

                    if (!repeated)
                    {
                        RenderNormally(currentSentence, layer, parentNode);
                    }
                    else
                    {
                        Debug.Log($"{currentSentence.name} repeated");
                        Debug.Log($"connected to {parentNode.sentence.name}'s {1} output port ");

                        ConnectRepeated(currentSentence, layer, parentNode);
                    }
                }
                return;
            }
            else
            {
                List<Choice> options = currentSentence.options;

                for (int i = 0; i < options.Count; i++)
                {
                    currentSentence = options[i].nextSentence;
                    if(currentSentence != null)
                    {
                        bool repeated = CheckRepeat(currentSentence);
                        if (!repeated)
                        {
                            RenderNormally(currentSentence, layer, parentNode, i);
                        }
                        else
                        {
                            Debug.Log($"{currentSentence.name} repeated");
                            Debug.Log($"connected to {parentNode.sentence.name}'s {i + 1} output port ");

                            ConnectRepeated(currentSentence, layer, parentNode, i);
                        }
                    }
                }
                return;
            }

        }
    }


    void ConnectRepeated(Sentence currentSentence_, int layer_, DialogueNode parentNode_, int i = 0)
    {
        DialogueNode inputNode = GetRepeatedNode(currentSentence_);
        if (inputNode != null)
        {
            Port inputPort = inputNode.inputPort;
            Port outputPort = parentNode_.outputPorts[i];

            Edge edge = inputPort.ConnectTo(outputPort);
            Add(edge);
        }
    }

    void RenderNormally(Sentence currentSentence_, int layer_, DialogueNode parentNode_, int i = 0)
    {
        layers[layer_ + 1]++;
        DialogueNode node;
        node = CreateFollowingNode(currentSentence_, layer_ + 1, layers[layer_ + 1], parentNode_, i);
        RenderNode(node);
        //repeatedSentence.Add(currentSentence);
        repeatedNodes.Add(node);
        Recursion(currentSentence_, layer_ + 1, node);
    }




    public void SaveConnections()
    {
        ResetSentenceLinks();

        ports.ForEach(funcCall: port =>
        {

            //Debug.Log("port");
            if (port.direction == Direction.Output)
            {
                DialogueNode outputNode = (DialogueNode)port.node;
                var edgeList = port.connections.ToList();
                if (edgeList.Count != 0)
                {
                    DialogueNode inputNode = (DialogueNode)edgeList[0].input.node;
                    Debug.Log($"{outputNode.sentence.name} is connected to {inputNode.sentence.name}");
                    if (outputNode.sentence.HasOptions())
                    {
                        if (port.portName != null)
                        {
                            int index = int.Parse(port.portName) - 1;
                            outputNode.sentence.options[index].nextSentence = inputNode.sentence;
                        }
                    }
                    else
                    {
                        outputNode.sentence.nextSentence = inputNode.sentence;
                    }
                }
            }
        });
    }


    void ResetSentenceLinks()
    {
        ports.ForEach(funcCall: port =>
        {
            DialogueNode outputNode = (DialogueNode)port.node;
            if (outputNode.sentence.HasOptions())
            {
                foreach (var option in outputNode.sentence.options)
                {
                    option.nextSentence = null;
                }
            }
            else
            {
                outputNode.sentence.nextSentence = null;
            }
        });

    }
}
