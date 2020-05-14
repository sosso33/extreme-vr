using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using static Scene;
using static PrintType;

interface ISimulContext
{
    bool LoadScene(string file);
    bool RestartScene();
    void SetUserFreeze(bool isFrozen);
}

public class SimulContext : MonoBehaviour, ISimulContext
{
    // Start is called before the first frame update
    public Text notifText;
    public Text inventoryListText;
    public UnityPrint printable;
    public CameraMove camera;
    void Awake()
    {
        //Debug.Log("Awake Method");
        LoadScene("scene01a");
        //Run();
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
        //printable = new UnityPrint();
        //printable.notifText = this.notifText;
        _s.setPrintOutput(printable);
        //s.addTakeObjectUnorderedTask("123");
        StartCoroutine(_s.FirstTry());
        _isRunning = false;
    }

    public void TakeObj(string name)
    {
        if(!_objInventory.Contains(name))
        {
            _s.TakeObject(name);
            _objInventory.Add(name);
            GameObject.Find(name).SetActive(false);
            printable.PrintToUser(name + " pris !",PrintType.WITH_TIMEOUT,3);
            inventoryListText.text += name + "\n";
        }
    }

    public bool LoadScene(string file)
    {
        _s = FileTools.LoadTextFile(file);
        _s.SimulContext = this;
        _objInventory = new List<string>();
        //printable = new UnityPrint();
        //printable.notifText = this.notifText;
        _s.setPrintOutput(printable);
        //s.addTakeObjectUnorderedTask("123");
        StartCoroutine(_s.FirstTry());
        return true;
    }

    public bool RestartScene()
    {
        _objInventory.Clear();
        inventoryListText.text = "";
        return false;
    }

    public Boolean Validate()
    {
        return _s.CheckTasks();
    }

    //DO NOT USE (Use the printable attribute instead)
    public void PrintNotif(string text)
    {
        notifText.text = text;
    }

    public void SetUserFreeze(bool isFrozen)
    {
        camera.IsFrozen = isFrozen;
    }

    public void SignalUserInput()
    {
        _s.SignalUserInput();
    }
}
