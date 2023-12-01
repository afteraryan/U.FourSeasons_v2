using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MobileDebugger : GenericMonoSingleton<MobileDebugger>
{
    [SerializeField] private Canvas canvasDebugger;
    [SerializeField] private TMP_Text textDebug;
    
    public void SetText(string text)
    {
        //gameObject.SetActive(true);
        textDebug.text = text;
    }
}
