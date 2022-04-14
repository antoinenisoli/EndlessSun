using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField] float Speed = 2.0f;
    public const string DefaultState = "Idle";
    [SerializeField] protected List<ActorState> States;
    [SerializeField] protected List<ActorAction> Actions;
    protected ActorAction m_currentAction;
    protected Vector3? m_destination;
    [HideInInspector] public Vector3 startposition;

    public string CurrentState { get; set; }
    public bool IsMoving {  get { return m_destination.HasValue; } }

    void Awake()
    {
        startposition = transform.position;
    }

    public void SetState(string stateName)
    {
        CurrentState = stateName;
    }

    internal void PlayAction(string actionName)
    {
        m_currentAction = GetAction(actionName);
    }

    public void MoveTo(Vector3 destination)
    {
        m_destination = destination;
        SetState("Move");
    }

    ActorAction GetAction(string name)
    {
        foreach (var item in Actions)
        {
            if (item.AnimationName == name)
                return item;
        }

        Debug.LogError("Can't find action : " + name);
        return null;
    }

    public bool IsPlayingAction(string state)
    {
        return CurrentState == state;
    }

    public virtual void Update()
    {
        if (m_destination != null)
        {
            Vector3 direction = m_destination.Value - transform.position;
            float distance = direction.magnitude;
            float moveStep = Time.deltaTime * Speed;
            direction.Normalize();

            if (distance <= moveStep)
            {
                transform.position = m_destination.Value;
                m_destination = null;
                SetState("Idle");
            }
            else
            {
                transform.position += direction * Time.deltaTime * Speed;
            }
        }
    }
}
