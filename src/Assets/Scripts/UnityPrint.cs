using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using static IPrintable;


public class UnityPrint : MonoBehaviour, IPrintable
{
    public Text notifText;
    public void PrintToUser(String text, int type, double time = -1)
    {
        if((type & PrintType.WITH_CONFIRMATION) == PrintType.WITH_CONFIRMATION)
        {
            Debug.Log(text + "\nPress Enter to continue");
            if(notifText != null) notifText.text = text + "\nPress Enter to continue";
            else Debug.Log("NotifText = null !");
            //Console.ReadKey();
            Debug.Log("Confirmation");
        }
        else if((type & PrintType.WITH_TIMEOUT) == PrintType.WITH_TIMEOUT)
        {
            Debug.Log(text);
            if(notifText != null) notifText.text = text;
            else Debug.Log("NotifText = null !");
            //Thread.Sleep((int)time * 1000);
            Debug.Log("Wait");
        }
        else Debug.Log("PrintToUser : Unknown type");
    }

    public List<int> CheckboxToUser(string message,List<string> choices)
    {/*
        string input;
        int parseResult;
        List<int> chList = new List<int>();
        Debug.Log(message);
        Debug.Log("Write your choices separated with \',\'");
        for(int i = 0;i<choices.Count;i++)
        {
            Debug.Log(i + ") " + choices[i]);
        }
        //input = Console.ReadLine();
        //input = input.Trim();
        string []splitInput = input.Split(',');
        foreach(string ch in splitInput)
        {
            if( int.TryParse(ch, out parseResult) )
            {
                chList.Add(parseResult);
            }
        }
        return chList;
    */
    return null;
    }
    
}