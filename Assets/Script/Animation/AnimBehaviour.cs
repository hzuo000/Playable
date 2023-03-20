using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public abstract class AnimBehaviour 
{
    public bool enable { get; protected set; }
    protected Playable m_PlayableAdapter;
    public float EnterTime { get; protected set; }
    public AnimBehaviour(PlayableGraph graph,float enterTime = 0f)
    {
        m_PlayableAdapter = ScriptPlayable<AnimAdapter>.Create(graph);
        ((ScriptPlayable<AnimAdapter>)m_PlayableAdapter).GetBehaviour().Init(this);
        EnterTime = enterTime;
    }
    public virtual void Enable()
    {
        enable = true;
    }
    public virtual void Disable()
    {
        enable = false;
    }
    public virtual void Executr(Playable playable, FrameData info)
    {
        if (!enable) return;
    }
    public Playable GetAnimAdapterPlayable()
    {
        return m_PlayableAdapter;
    }
   
    public virtual void AddInput(Playable playables)
    {

    }
    public virtual void AddInput(AnimBehaviour behaviour)
    {
        AddInput(behaviour.GetAnimAdapterPlayable());
    }
}
