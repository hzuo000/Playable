using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class AnimUnit : AnimBehaviour
{
    private AnimationClipPlayable _anim;
    
    public AnimUnit(PlayableGraph graph,AnimationClip clip,float enterTime = 0f) : base(graph, enterTime)
    {
        _anim = AnimationClipPlayable.Create(graph, clip);
        m_PlayableAdapter.AddInput(_anim, 0);
        Disable();
    }
    public override void Enable()
    {
        base.Enable();
        m_PlayableAdapter.SetTime(0);
        _anim.SetTime(0);
        m_PlayableAdapter.Play();
        _anim.Play();
    }
    public override void Disable()
    {
        base.Disable();
        m_PlayableAdapter.Pause();
        _anim.Pause();
    }
}
public class PlayableQueue : AnimBehaviour
{
    private AnimationMixerPlayable mixer;

    private float timeToNext;
    private int currentClip;

    public PlayableQueue(PlayableGraph graph, AnimationClip[] clips,float enableTime) : base(graph, enableTime)
    {
        if (clips == null || clips.Length <= 0)
        {
            return;
        }

        mixer = AnimationMixerPlayable.Create(graph);
        for (int i = 0; i < clips.Length; ++i)
        {
            mixer.AddInput(AnimationClipPlayable.Create(graph, clips[i]), 0);
        }
        mixer.SetInputWeight(0, 1f);

        //graph.Connect(mixer, 0, m_PlayableAdapter, 0);
        m_PlayableAdapter.AddInput(mixer, 0);
        timeToNext = clips[0].length;
        currentClip = 0;
    }
    public override void Enable()
    {
        base.Enable();
        Restet();
        m_PlayableAdapter.Play();
        mixer.Play();
    }
    public override void Disable()
    {
        base.Disable();
        m_PlayableAdapter.Pause();
        mixer.Pause();
    }
    public void Restet()
    {
        mixer.SetInputWeight(currentClip, 0f);
        currentClip = 0;
        mixer.SetInputWeight(currentClip, 1f);
        mixer.GetInput(currentClip).SetTime(0);
        timeToNext = ((AnimationClipPlayable)mixer.GetInput(0)).GetAnimationClip().length;
        //_owner.Pause();
    }
    public override void Executr(Playable playable, FrameData info)
    {
        base.Executr(playable, info);

        timeToNext -= info.deltaTime;
        if (timeToNext <= 0f && currentClip < mixer.GetInputCount() - 1)
        {
            mixer.SetInputWeight(currentClip, 0);
            mixer.SetInputWeight(currentClip + 1, 1f);
            currentClip++;
            mixer.GetInput(currentClip).SetTime(0);
            timeToNext = ((AnimationClipPlayable)mixer.GetInput(currentClip)).GetAnimationClip().length;
        }
        if (currentClip == mixer.GetInputCount() - 1 && timeToNext <= 0f)
        {
            Restet();
        }
    }
}
public class AnimatorPlayable : AnimBehaviour
{
    AnimationMixerPlayable animationMixer;
    public AnimatorPlayable(PlayableGraph graph, RuntimeAnimatorController RuntimeAnimator, AnimationClip clip,float enableTime) : base(graph, enableTime)
    {
        animationMixer = AnimationMixerPlayable.Create(graph);
        AnimationClipPlayable clipPlayable = AnimationClipPlayable.Create(graph, clip);
        AnimatorControllerPlayable animatorController = AnimatorControllerPlayable.Create(graph, RuntimeAnimator);

        animationMixer.AddInput(clipPlayable,0);
        animationMixer.AddInput(animatorController, 0);
        //graph.Connect(clipPlayable, 0, animationMixer, 0);
        //graph.Connect(animatorController, 0, animationMixer, 1);
        animationMixer.SetInputWeight(1, 1f);
        m_PlayableAdapter.AddInput(animationMixer, 0);
        //Disable();
    }
    public override void Enable()
    {
        base.Enable();
        m_PlayableAdapter.Play();
        animationMixer.Play();
    }
    public override void Disable()
    {
        base.Disable();
        m_PlayableAdapter.Pause();
        animationMixer.Pause();
    }
    public void SetInputWeight(float val)
    {
        animationMixer.SetInputWeight(0, 1f - val);
        animationMixer.SetInputWeight(1, val);
    }
}
public class RandomSelect : AnimBehaviour
{
    AnimationMixerPlayable animationMixer;
    public int currentIndex { get; private set; }
    public int currentCount { get; private set; }

    public RandomSelect(PlayableGraph graph, AnimationClip[] clips) : base(graph)
    {
        currentIndex = 0;
       
        animationMixer = AnimationMixerPlayable.Create(graph);
        for (int i = 0; i < clips.Length; ++i)
        {
            animationMixer.AddInput(AnimationClipPlayable.Create(graph, clips[i]), 0);
        }
        currentCount = clips.Length;

        animationMixer.SetInputWeight(0, 1f);
        m_PlayableAdapter.AddInput(animationMixer, 0);
    }
    public RandomSelect(PlayableGraph graph) : base(graph)
    {
        currentIndex = -1;

        animationMixer = AnimationMixerPlayable.Create(graph);
        
        m_PlayableAdapter.AddInput(animationMixer, 0);
    }
    public override void AddInput(Playable playables)
    {
        animationMixer.AddInput(playables, 0);
        currentCount++;
    }

    public override void Enable()
    {
        base.Enable();
        if (currentIndex < 0 || currentIndex >= currentCount) return;
        AnimUtil.Enable(animationMixer.GetInput(currentIndex));
        animationMixer.SetInputWeight(currentIndex, 1f);
        m_PlayableAdapter.SetTime(0);
        m_PlayableAdapter.Play();
        animationMixer.SetTime(0);
        animationMixer.Play();
    }
    public override void Disable()
    {
        base.Disable();
        if (currentIndex < 0 || currentIndex >= currentCount) return;
        AnimUtil.Disable(animationMixer.GetInput(currentIndex));
        animationMixer.SetInputWeight(currentIndex, 0f);
        m_PlayableAdapter.Pause();
        animationMixer.Pause();
        currentIndex = -1;
    }
    public void SelectAnim(AnimationType animationType)
    {
        if (animationType < AnimationType.IDLE || (int)animationType >= currentCount)
        {
            return;
        }
        currentIndex = (int)animationType;
    }
    public T GetInputBehaviour<T>(int CountIndex) where T : AnimBehaviour
    {
        int allCount = animationMixer.GetInputCount();
        if (CountIndex >= allCount)
        {
            Debug.LogError("获取的behavior超出InputCount");
        }
        //Playable adapter = behaviour.GetAnimAdapterPlayable();
        AnimAdapter adapter = AnimUtil.GetAnimAdapter(animationMixer.GetInput(CountIndex));
        if (adapter != null)
        {
            return (T)adapter.m_behaviour;
        }
        return null;
    }
    
}
public class TranstionMixer : AnimBehaviour
{
    AnimationMixerPlayable animationMixer;
    public int CurrentIndex { get; private set; }
    public int TargetIndex { get; private set; }
    public int CurrentCount { get; private set; }
    public bool IsTranstion { get; private set; }
    private List<int> transtionList;
    private float timeToNext;
    private float currentSpeed;
    private float declineSpeed;
    private float allDeclineWeight;
    
    public TranstionMixer(PlayableGraph graph) : base(graph)
    {
        animationMixer = AnimationMixerPlayable.Create(graph);
        m_PlayableAdapter.AddInput(animationMixer, 0);
        CurrentCount = 0;
        CurrentIndex = 0;
        TargetIndex = 0;
        transtionList = new List<int>();
    }
    public override void AddInput(Playable playables)
    {
        animationMixer.AddInput(playables, 0, CurrentCount == 0 ? 1f : 0f);
        CurrentCount++;
    }
    public override void Enable()
    {
        base.Enable();

        m_PlayableAdapter.SetTime(0);
        animationMixer.SetTime(0);
        m_PlayableAdapter.Play();
        animationMixer.Play();

        CurrentIndex = 0;
        TargetIndex = -1;

        animationMixer.SetInputWeight(0, 1f);
        AnimUtil.Enable(animationMixer);
        AnimUtil.Enable(animationMixer, CurrentIndex);

    }
    public override void Disable()
    {
        base.Disable();

        m_PlayableAdapter.Pause();
        animationMixer.Pause();

        for (int i = 0; i < CurrentCount; ++i) 
        {
            AnimUtil.Disable(animationMixer, i);
            animationMixer.SetInputWeight(i, 0f);
        }
    }
    public override void Executr(Playable playable, FrameData info)
    {
        base.Executr(playable, info);

        if (!IsTranstion || TargetIndex < 0) 
        {
            return;
        }
        if (timeToNext > 0) 
        {
            timeToNext -= info.deltaTime;
            allDeclineWeight = 0f;
            for (int i = transtionList.Count - 1; i >= 0; --i) 
            {
                float weight = ModifyWeight(transtionList[i], -info.deltaTime * declineSpeed);
                if (weight <= 0f) 
                {
                    AnimUtil.Disable(animationMixer, transtionList[i]);
                    transtionList.RemoveAt(i);
                }
                else
                {
                    allDeclineWeight += weight;
                }
            }
            SetWight(TargetIndex, 1 - allDeclineWeight - ModifyWeight(CurrentIndex, -info.deltaTime * currentSpeed));
            return;
        }
        AnimUtil.Disable(animationMixer, CurrentIndex);
        CurrentIndex = TargetIndex;
        TargetIndex = -1;
        IsTranstion = false;
    }
    public void SelectAnim(AnimationType type)
    {
        //if (type < AnimationType.IDLE || type >= AnimationType.FANCY) return;
        int selectIndex = (int)type;
        if (IsTranstion && TargetIndex >= 0) 
        {
            if (selectIndex == TargetIndex)
            {
                return;
            }
            if (CurrentIndex == selectIndex)
            {
                CurrentIndex = TargetIndex;
            }
            else if (GetWight(CurrentIndex) > GetWight(TargetIndex)) 
            {
                transtionList.Add(TargetIndex);
            }
            else
            {
                transtionList.Add(CurrentIndex);
                CurrentIndex = TargetIndex;
            }
        }else if (selectIndex == CurrentIndex)
        {
            return;
        }
        TargetIndex = selectIndex;

        transtionList.Remove(TargetIndex);
        AnimUtil.Enable(animationMixer, TargetIndex);
        timeToNext = GetEnterTime(TargetIndex);
        currentSpeed = GetWight(CurrentIndex) / timeToNext;
        declineSpeed = 2f / timeToNext;
        IsTranstion = true;
    }
    public T GetInputBehaviour<T>(int CountIndex) where T : AnimBehaviour
    {
        int allCount = animationMixer.GetInputCount();
        if (CountIndex >= allCount)
        {
            Debug.LogError("获取的behavior超出InputCount");
        }
        //Playable adapter = behaviour.GetAnimAdapterPlayable();
        AnimAdapter adapter = AnimUtil.GetAnimAdapter(animationMixer.GetInput(CountIndex));
        if (adapter != null)
        {
            return (T)adapter.m_behaviour;
        }
        return null;
    }
    public float GetWight(int index) => (index < 0 || index >= CurrentCount) ? 0f : animationMixer.GetInputWeight(index);
    private void SetWight(int index,float weight)
    {
        if (index < 0 || index >= CurrentCount) return;
        animationMixer.SetInputWeight(index, weight);
    }
    private float GetEnterTime(int index) => GetInputBehaviour<AnimBehaviour>(index).EnterTime;
    private float ModifyWeight(int index, float weightDlt)
    {
        if (index < 0 || index >= CurrentCount) return 0f;
        float value = GetWight(index) + weightDlt;
        SetWight(index, value);
        return value;
    }
}


