using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

class Scene
{
    class Task
    {
        public const int TASK_TAKE = 1;
        public const int TASK_WAIT_UNORDERED = 2;
        public const int TASK_CHECK = 3;
        private int type;
        private List<String> args;
        private int order;
        private bool isDone;

        public Task(int type, String []args, int order = -1)
        {
            this.type = type;
            this.args = new List<String>();
            foreach (String s in args)
            {
                this.args.Add(s);
            }
            this.order = order;
            isDone = false;
        }

        public Task(int type, String arg, int order = -1)
        {
            this.args = new List<String>();
            this.type = type;
            this.args.Add(arg);
            this.order = order;
            isDone = false;
        }

        public int getType() { return type; }
        public bool isTaskDone() { return isDone; }
        public void setTaskDone(bool b) { isDone = b; }
        public List<String> getArgs() { return args; }

        public override bool Equals(object obj)
        {
            if(this.type != ((Task)obj).type) return false;
            return args.Equals(((Task)obj).args);
        }
    }
    private List<Task> _unorderedTasks; //<Task,isDone>
    private List<Task> _orderedTasks;
    private List<Task> _watchedAction;
    private List<Instruction> _successInst;
    private List<Instruction> _failInst;
    private List<Instruction> _firstTryInst;
    private List<Instruction> _atStartInst;
    private IPrintable _printOutput;
    private List<String> _objInventory;
    private bool _waitForValidation;
    public bool WaitForValidation {get {return _waitForValidation;} set {_waitForValidation = value;}}
    public IPrintable PrintOutput { get { return _printOutput; } }
    private ISimulContext _simulContext;
    public ISimulContext SimulContext { get { return _simulContext; } set { _simulContext = value; }}

    public Scene()
    {
        _unorderedTasks = new List<Task>();
        _orderedTasks = new List<Task>();
        _successInst = new List<Instruction>();
        _failInst = new List<Instruction>();
        _firstTryInst = new List<Instruction>();
        _atStartInst = new List<Instruction>();
        _objInventory = new List<String>();
        WaitForValidation = false;
    }
    
    public void setPrintOutput(IPrintable p)
    {
        _printOutput = p;
    }

    public void AddInstructionToFirstTry(Instruction i)
    {
        if(i != null) _firstTryInst.Add(i);
    }

    public void FirstTry()
    {
        foreach (Instruction i in _firstTryInst) i.Execute(this);
    }

    public void AtStart()
    {
        foreach(Instruction i in _atStartInst) i.Execute(this);
    }

    public void Restart()
    {
        foreach(Task t in _orderedTasks )
        {
            

        }
    }

    public void addTakeObjectUnorderedTask(String objID)
    {
        //Debug.Log("Add " + objID);
        Task tmp = new Task(Task.TASK_TAKE,objID);
        _unorderedTasks.Add(tmp);
    }

    public void addTakeObjectOrderedTask(String objID)
    {
        Task tmp = new Task(Task.TASK_TAKE,objID,_orderedTasks.Count + 1);
        _orderedTasks.Add(tmp);
    }
    
    public Boolean TakeObject(String objID) //Return true if object is in tasks list, else return false
    {
        Debug.Log("objID = " + objID);
        objID = objID.Split('.')[0];
        bool returnValue = false;
        Task tmp = _unorderedTasks.Find(t => t.getType() == Task.TASK_TAKE && t.getArgs()[0] == objID);
        if(tmp != null)
        {
            tmp.setTaskDone(true);
            returnValue = true;
        }

        int tmpIndex = _orderedTasks.FindIndex(t => t.getType() == Task.TASK_TAKE && t.getArgs()[0] == objID);
        if(tmpIndex >= 0)
        {
            if(_orderedTasks[tmpIndex > 0 ? tmpIndex - 1 : 0].isTaskDone() || tmpIndex == 0)
            {
                tmp.setTaskDone(true);
                returnValue = true;
            }
        }
        _objInventory.Add(objID);

        if(!WaitForValidation) CheckTasks();
        //Enlever l'objet de l'environnement
        return returnValue;
    }

    public Boolean DropObject(String objID) //Return true if object is in tasks list, else return false
    {
        bool returnValue = false;
        Task tmp = _unorderedTasks.Find(t => t.getType() == Task.TASK_TAKE && t.getArgs()[0] == objID);
        if(tmp != null)
        {
            tmp.setTaskDone(false);
            returnValue = true;
        }

        tmp = _orderedTasks.Find(t => t.getType() == Task.TASK_TAKE && t.getArgs()[0] == objID);
        if(tmp != null)
        {
            tmp.setTaskDone(false);
            returnValue = true;
        }
        //if(_objInventory.Contains(objID))
        if(!WaitForValidation) CheckTasks();
        return returnValue;
    }

    public Boolean CheckTasks()
    {
        foreach(Task t in _unorderedTasks)
        {
            if(!t.isTaskDone())
            {
                fail();
                return false;
            }
        }

        foreach(Task t in _orderedTasks)
        {
            if(!t.isTaskDone())
            {
                fail();
                return false;
            }
        }

        success();
        return true;
    }

    private void success()
    {
        //_printOutput.PrintToUser("Success !",PrintType.WITH_TIMEOUT,3);
        foreach (Instruction i in _successInst)
        {
            i.Execute(this);
        }
    }
    
    private void fail()
    {
        //_printOutput.PrintToUser("Fail !",PrintType.WITH_CONFIRMATION);
        foreach (Instruction i in _failInst)
        {
            i.Execute(this);
        }
    }

    public void AddInstructionToSuccess(Instruction i)
    {
        _successInst.Add(i);
    }

    public void AddInstructionToFail(Instruction i)
    {
        _failInst.Add(i);
    }

    public bool ChangeScene(string file)
    {
        return SimulContext.LoadScene(file);
    }

    public void CheckBox(string message, List<string> options, Dictionary<List<int>,List<Instruction>> inst, bool isStrict = false)
    {
        List<int> input;
        Debug.Log("CHKBOX");
        input = PrintOutput.CheckboxToUser(message, options);
        Console.Write("INPUT => ");
        foreach(int i in input) Console.Write(i+",");
        Console.Write("\n");
        foreach (KeyValuePair<List<int>,List<Instruction>> kv in inst)
        {
            Debug.Log("FOREACH/ ISSTRICT = " + isStrict);
            if(isStrict)
            {
                Console.Write("KEY ");
                foreach(int k in kv.Key) Console.Write(k + ",");
                Console.Write(" VALUE ");
                foreach(Instruction v in kv.Value) Debug.Log("foreach " + v==null);//Console.Write(v.Type + ",");
                Console.Write("\n");
                Debug.Log("input == null ? " + input==null + ";kv == null ?" + kv==null);
                if(Enumerable.SequenceEqual(input.OrderBy(e => e), kv.Key.OrderBy(e => e)))
                {
                    Debug.Log("EQUAL");
                    foreach (Instruction i in kv.Value)
                    {
                        Debug.Log("execute");
                        i.Execute(this);
                    }
                }
            }
            else
            {
                if(kv.Key.Intersect(input).ToList().Count == input.Count)
                {
                    foreach (Instruction i in kv.Value)
                    {
                        i.Execute(this);
                    }
                }
            }
        }
    }
}
