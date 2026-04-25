using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DgData", menuName = "Dialogue")]
public class DgData : ScriptableObject
{
    [Serializable]
    public class Choice
    {
        // 选择文本
        public string text;
        // -1 表示对话结束，>=0 表示跳转到该节点索引
        public int targetNodeIndex = -1; 
    }

    [Serializable]
    public class Node
    {
        [TextArea] 
        public string content;
        public string speaker;
        public bool isChoice = false;
        public List<Choice> choices = new List<Choice>();
    }

    public List<Node> nodes = new List<Node>();

    // 起始节点索引
    public int startIndex = 0;
}

