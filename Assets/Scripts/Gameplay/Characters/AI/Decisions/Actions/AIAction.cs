using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AIAction
{
    protected bool m_isStarted;

    public enum ActionState
    {
        InProgress,
        Finished,
    }

    public virtual void Start(Actor actor)
    {

    }

    public ActionState Update(Actor actor)
    {
        if (m_isStarted)
        {
            m_isStarted = true;
            Start(actor);
        }

        return UpdateInterval(actor);
    }

    public void Reset()
    {
        m_isStarted = false;
    }

    protected abstract ActionState UpdateInterval(Actor actor);
}
