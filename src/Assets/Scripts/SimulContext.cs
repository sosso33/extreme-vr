using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Scene;

interface ISimulContext
{
    bool LoadScene(string file);
    bool RestartScene();
}

public class SimulContext : MonoBehaviour, ISimulContext
{
    // Start is called before the first frame update
    public Text notifText;
    public Text inventoryListText;
    void Awake()
    {
        //Debug.Log("Awake Method");
        LoadScene("scene01a");
        Run();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Scene _s = null;
    private bool _isRunning = false;
    private List<string> _objInventory;
    public void Run()
    {
        if(_s == null)
        {
            Debug.Log("Please load a scene before calling the Run() method");
            return;
        }
        _isRunning = true;
        _objInventory = new List<string>();
        UnityPrint up = new UnityPrint();
        up.notifText = this.notifText;
        _s.setPrintOutput(up);
        //s.addTakeObjectUnorderedTask("123");
        _s.FirstTry();
        _isRunning = false;
    }

    public void TakeObj(string name)
    {
        if(!_objInventory.Contains(name))
        {
            _s.TakeObject(name);
            _objInventory.Add(name);
            GameObject.Find(name).SetActive(false);
            PrintNotif(name + " pris !");
            inventoryListText.text += name + "\n";
        }
    }

    public bool LoadScene(string file)
    {
        _s = FileTools.LoadTextFile(file);
        _s.SimulContext = this;
        if(_isRunning) 
        {
            _s.setPrintOutput(new UnityPrint());
            _s.FirstTry();
        }
        return true;
    }

    public bool RestartScene()
    {
        return false;
    }

    public Boolean Validate()
    {
        return _s.CheckTasks();
    }

    public void PrintNotif(string text)
    {
        notifText.text = text;
    }
}
