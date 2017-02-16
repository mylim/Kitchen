﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InterferenceDialog : MonoBehaviour {
    public GameObject dialog;
    private string answer;
    private bool interfering;

    void Start()
    {
        interfering = false;
    }

    public void ShowDialog()
    {
        dialog.SetActive(true);        
    }

    public void ShowAnswer()
    {
        answer = dialog.GetComponent<InputField>().text.ToString();
        Debug.Log("Answer " + answer);
    }

    public void CloseDialog()
    {
        ResetInterference();
        dialog.SetActive(false);
    }

    public void SetInterference()
    {
        interfering = true;
    }

    public bool GetInterference()
    {
        return interfering;
    }

    public void ResetInterference()
    {
        interfering = false;
    }

}