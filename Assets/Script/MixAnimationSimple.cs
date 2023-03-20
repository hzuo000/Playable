using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class MixAnimationSimple : MonoBehaviour
{

    public AnimationClip clip1,clip2;
    private PlayableGraph graph;
    private AnimationMixerPlayable mixer;

    [Range(0f, 1f)]
    public float weight;
    // Start is called before the first frame update
    void Start()
    {
        graph = PlayableGraph.Create();
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);

        mixer = AnimationMixerPlayable.Create(graph, 2);
        

        AnimationClipPlayable clipPlayable1 = AnimationClipPlayable.Create(graph, clip1);
        AnimationClipPlayable clipPlayable2 = AnimationClipPlayable.Create(graph, clip2);

        //graph.Connect(clipPlayable1, 0, mixer, 0);
        //graph.Connect(clipPlayable2, 0, mixer, 1);
        //mixer.SetInputWeight(0, 1f);

        mixer.AddInput(clipPlayable1, 0, 1);
        mixer.AddInput(clipPlayable2, 0, 0);

        AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Anim", GetComponent<Animator>());
        output.SetSourcePlayable(mixer);

        graph.Play();
    }

    // Update is called once per frame
    void Update()
    {
        mixer.SetInputWeight(0, 1 - weight);
        mixer.SetInputWeight(1, weight);
    }
    private void OnDestroy()
    {
        graph.Destroy();
    }
}
