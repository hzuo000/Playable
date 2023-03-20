using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class PlayAnimationSimple : MonoBehaviour
{
    public AnimationClip clip;
    private PlayableGraph graph;

    AnimationClipPlayable clipPlayable;

    private void Start()
    {
        graph = PlayableGraph.Create();
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        clipPlayable = AnimationClipPlayable.Create(graph, clip);

        AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Anim", GetComponent<Animator>());

        output.SetSourcePlayable(clipPlayable);

        graph.Play();

        //AnimationPlayableUtilities.PlayClip(GetComponent<Animator>(), clip, out graph);
    }
    private void Update()
    {
        if (Input.GetKeyDown( KeyCode.Space))
        {
            if (clipPlayable.GetPlayState() == PlayState.Playing)
            {
                clipPlayable.Pause();
            }
            else
            {
                clipPlayable.Play();
                clipPlayable.SetTime(0f);
            }
        }
    }
    private void OnDestroy()
    {
        graph.Destroy();
    }
}
