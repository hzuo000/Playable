using System.Collections;
using System.Collections.Generic;

using UnityEngine.Playables;
using UnityEngine.Animations;

public class AnimRoot : AnimBehaviour
{
    public AnimRoot(PlayableGraph graph) : base(graph)
    {

    }
    public override void AddInput(Playable playables)
    {
        m_PlayableAdapter.AddInput(playables, 0, 1f);
    }
    public override void Enable()
    {
        base.Enable();
        for (int i = 0; i < m_PlayableAdapter.GetInputCount(); ++i) 
        {
            AnimUtil.Enable(m_PlayableAdapter.GetInput(i));
        }
    }
    public override void Disable()
    {
        base.Disable();
        for (int i = 0; i < m_PlayableAdapter.GetInputCount(); ++i)
        {
            AnimUtil.Enable(m_PlayableAdapter.GetInput(i));
        }
    }
}
