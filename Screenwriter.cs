using System.Collections.Generic;
using UnityEngine;

public class Screenwriter : MonoBehaviour
{ 
    [SerializeField] private List<Value> values = new List<Value>();
    [SerializeField] private Script GameScript = new Script();
    private void Update()
    {
        CheckActions();
    }

    [System.Serializable]
    private class Value
    {
        public string name;
        public float value;
    }

    [System.Serializable]
    private class Script
    {

        [SerializeField] private int CurrentIventID;
        public int currentIventID {  get { return CurrentIventID; } }
        [SerializeField] private List <ScriptAction> Actions = new List<ScriptAction>();
        public void CheckActions(GameObject Trigger, Screenwriter screenwriter)
        {
            int ForkID = Actions.Count-1;
            for (int i = 0; i < Actions.Count; i++)
            {
                if (Actions[i].ActionType == ScriptAction.ActionTypes.Fork)
                {
                    ForkID = i; break;
                }
            }
            if (screenwriter.IsConditionsOk(Actions[CurrentIventID].Conditions, Trigger))
            {
                if (CurrentIventID <= ForkID)
                {
                    Actions[CurrentIventID].DoAction(null, Trigger);
                    CurrentIventID += 1;
                    //Активируется дважды при ивенте
                }
                else if (CurrentIventID == ForkID)
                {
                    Actions[CurrentIventID].DoAction(null, Trigger);
                }
            }
        }
/*        public void LoadProgress(int CurrentID)
        {
            for (int i = 0; i < CurrentID; i++)
            {
                Actions[i].DoAction();
            }
        }*/
    }

    [System.Serializable]
    private class ScriptAction
    {
        public enum ActionTypes
        {
            Fork,
            Destroy,
            Instantiate,
            Dialogue,
            DebugLog
        }

        [Header("Essential settings")]
        [SerializeField] private ActionTypes actionType;
        public ActionTypes ActionType { get { return actionType; }}
        [SerializeField] private Condition[] conditions;
        public Condition[] Conditions { get { return conditions; }}
        private bool ConditionsDone;

        [Header("Fork")]
        [SerializeField] private List<Script> ScriptsOfActionsInFork = new List<Script>();

        [Header("Destroy")]
        [SerializeField] private GameObject ToDestroy;

        [Header("Instantiate")]
        [SerializeField] private GameObject ToInstantiate;
        [SerializeField] private Transform Place;

        [Header("Debug Log")]
        [SerializeField] private string Sentance;

        public void DoAction(Screenwriter screenwriter=null, GameObject Trigger=null)
        {
            switch (actionType)
            {
                case ActionTypes.Destroy:
                    Destroy(ToDestroy); break;
                case ActionTypes.Instantiate:
                    Instantiate(ToInstantiate, Place.position, Place.rotation); break;
                case ActionTypes.Fork:
                    foreach (Script script in ScriptsOfActionsInFork)
                    {
                        if(script.currentIventID!=0)
                            script.CheckActions(Trigger, screenwriter);
                    }
                    break;
                case ActionTypes.DebugLog:   
                    Debug.Log(Sentance); break;
            }
        }
    }

    [System.Serializable]
    private class Condition
    {
        public GameObject Trigger;
        public Value FinalValue;
    }

    public void CheckActions(GameObject Trigger=null)
    {  
        GameScript.CheckActions(Trigger, this);
    }

    bool IsConditionsOk(Condition[] conds,GameObject Trigger=null)
    {
        bool IsOk=true;
        foreach (Condition condition in conds)
        {
            if (!IsConditionOk(condition, Trigger))
            {
                IsOk = false;
            }
        }
        return IsOk;
    }

    bool IsConditionOk(Condition cond, GameObject Trigger=null)
    {
        if (cond.Trigger==null || cond.Trigger==Trigger)
        {
            if (cond.FinalValue.value==-1000000f || cond.FinalValue.value==GetValue(cond.FinalValue.name) || cond.FinalValue.name =="")
                return true;
            else
                return false;
        }
        else return false;

    }

/*    public void LoadProgress(int CurrentID)
    {
        int i = 0;
        while (i<=CurrentID)
        {
        }
    }*/

    float GetValue(string name)
    {
        float FinValue=-1000000;
        foreach(Value val in values)
        {
            if (val.name == name)
            {
                FinValue=val.value;
                break;
            }
        }
        return FinValue;
    }

    public void ChangeValue(string name, float NewValue)
    {
        foreach (Value val in values)
        {
            if (val.name == name)
            {
                val.value = NewValue;
            }
        }
    }
}