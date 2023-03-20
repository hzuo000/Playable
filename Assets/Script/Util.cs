using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class AnimUtil
{
    public static void Enable(Playable playable)
    {
        AnimAdapter adapter = GetAnimAdapter(playable);
        if (adapter != null)
        {
            adapter.Enable();
        }
    }
    public static void Disable(Playable playable)
    {
        AnimAdapter adapter = GetAnimAdapter(playable);
        if (adapter != null)
        {
            adapter.Disable();
        }
    }
    public static void Disable(Playable playable,int index)
    {
        Disable(playable.GetInput(index));
    }
    public static void Enable(Playable playable, int index)
    {
        Enable(playable.GetInput(index));
    }
    public static AnimAdapter GetAnimAdapter(Playable playable)
    {
        if (typeof(AnimAdapter).IsAssignableFrom(playable.GetPlayableType()))
        {
            return ((ScriptPlayable<AnimAdapter>)playable).GetBehaviour();
        }
        return null;
    }
    public static void SetOutput(PlayableGraph graph, Animator animator, AnimBehaviour behaviour)
    {
        var root = new AnimRoot(graph);
        root.AddInput(behaviour);
        AnimationPlayableOutput.Create(graph, "Anim", animator).SetSourcePlayable(root.GetAnimAdapterPlayable());
    }
    public static void Start(PlayableGraph graph, AnimBehaviour behaviour)
    {
        graph.Play();
        behaviour.Enable();
    }
    public static void Start(PlayableGraph graph)
    {
        graph.Play();
        GetAnimAdapter(graph.GetOutputByType<AnimationPlayableOutput>(0).GetSourcePlayable()).Enable();
    }
   
}
