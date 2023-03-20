using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public enum AnimationType
{
    NULL = -1,

    IDLE,
    MOVE,
    DASH,
    FANCY,
    CONTROLLER,

    MAX
}



public class AnimationManager : GameInterface
{
    public AnimationClip idle;
    public AnimationClip move;
    public AnimationClip dash;
    private List<AnimationClip> clipList;
    public AnimationClip[] fancyClips;
    public RuntimeAnimatorController RuntimeAnimator;
    public float enableTime = .5f;
    public override void StartUp()
    {
        clipList = new List<AnimationClip>()
        {
            idle,
            move,
            dash
        };
        
        base.StartUp();
    }
    public override void UpdateData()
    {
        base.UpdateData();
    }
    public override void Close()
    {
        base.Close();
    }
    public void CreatPlayable(out PlayableGraph graph, Animator animator)
    {
        graph = PlayableGraph.Create();
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        //output = AnimationPlayableOutput.Create(graph, "Anim", animator);
    }
    public AnimationClipPlayable AddClip(AnimationType type, PlayableGraph graph)
    {
        AnimationClip clip = clipList[(int)type];
        AnimationClipPlayable animationClip = AnimationClipPlayable.Create(graph, clip);
        

        return animationClip;
    }
    public AnimBehaviour CreatAnimUnit(PlayableGraph graph, AnimationType type)
    {
        AnimBehaviour anim = null;

        switch (type)
        {
            case AnimationType.NULL:
                break;
            case AnimationType.IDLE:
                anim = new AnimUnit(graph, idle, enableTime);
                break;
            case AnimationType.MOVE:
                anim = new AnimUnit(graph, move, enableTime);
                break;
            case AnimationType.DASH:
                anim = new AnimUnit(graph, dash, enableTime);
                break;
            case AnimationType.FANCY:
                anim = new PlayableQueue(graph, fancyClips, enableTime);
                break;
            case AnimationType.CONTROLLER:
                anim = new AnimatorPlayable(graph, RuntimeAnimator, idle, enableTime);
                break;
            case AnimationType.MAX:
                break;
        }
        return anim;
    }
    public AnimatorPlayable CreatController(PlayableGraph graph)
    {
        //AnimationMixerPlayable animationMixer = AnimationMixerPlayable.Create(graph, 2);
        //AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph, idle);
        //AnimatorControllerPlayable animatorController = AnimatorControllerPlayable.Create(graph, RuntimeAnimator);
        ////animationMixer.AddInput(clipPlayable,0,1f);
        ////animationMixer.AddInput(animatorController, 0,1f);
        //graph.Connect(clipPlayable, 0, animationMixer, 0);
        //graph.Connect(animatorController, 0, animationMixer, 1);
        //return animationMixer;

        return new AnimatorPlayable(graph, RuntimeAnimator, idle, enableTime);


    }
    public TranstionMixer SetSourceClip(PlayableGraph graph, Animator animator)
    {
        //output.SetSourcePlayable(behaviour.GetAnimAdapterPlayable()); ;
        TranstionMixer behaviour = new TranstionMixer(graph);
        
        AnimUtil.SetOutput(graph, animator, behaviour);

        return behaviour;
    }
    //public PlayableQueue QueueInit(PlayableGraph graph, AnimationQueueType type)
    //{
    //    PlayableQueue anim = null;
    //    AnimationClip[] clips = null ;
    //    switch (type)
    //    {
    //        case AnimationQueueType.FANCY:
    //            clips = fancyClips;
    //            anim = new PlayableQueue(graph, clips);
    //            break;
    //    }
    //    return anim;
    //}

}
