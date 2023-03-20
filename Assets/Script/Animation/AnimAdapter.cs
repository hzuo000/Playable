using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class AnimAdapter : PlayableBehaviour
{
    public AnimBehaviour m_behaviour { get; private set; }

    public void Init(AnimBehaviour animBehaviour)
    {
        m_behaviour = animBehaviour;
    }
    public void Enable()
    {
        m_behaviour?.Enable();
    }
    public void Disable()
    {
        m_behaviour?.Disable();
    }
    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);
        m_behaviour?.Executr(playable, info);
    }
}
