using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public class Player : MonoBehaviour
{
    [Range(0,1)]
    public float weight;
    private PlayableGraph Graph;
    private AnimationPlayableOutput Output;
    private Transform playerTransform;
    private Animator animator;
    //private Dictionary<AnimationType, AnimBehaviour> ClipDic;
    //private Dictionary<AnimationQueueType, AnimBehaviour> BehaviourDic;
    private TranstionMixer SelectBehaviour { get;  set; }

    private bool Up  => GameManager.Controller.Up;
    private bool Down => GameManager.Controller.Down;
    private bool Left => GameManager.Controller.Left;
    private bool Right=> GameManager.Controller.Right;
    private bool Shift => GameManager.Controller.Shift;
    private bool Space => GameManager.Controller.Space;




    //private AnimatorPlayable ControllerMix;
    private AnimationType _curType;
    public float moveSpeedMax = 3f;
    public float DashSpeedMax = 6f;
    private float MoveMax;
    public float targetRotateAngel;
    private float curYSpeed;
    private float curXSpeed;
    public float rotateSpeed;
    private int rotateDir;
    private AnimationType CurType
    {
        get => _curType;
        set
        {
            if (value != _curType)
            {
                _curType = value;
                PlayAnimation(_curType);
            }
        }
    }
    public void StartUp()
    {
        playerTransform = GetComponent<Transform>();
        animator = GetComponent<Animator>();

        CreatAnimation();
        
        _curType = AnimationType.NULL;
        SelectBehaviour.Enable();
        //CurType = AnimationType.IDLE;

        //Graph.Play();

        AnimUtil.Start(Graph);
        
    }

    public void UpdateData()
    {
        UpdateSpeed();
        
        if (Up || Left || Right || Down)
        {
            UpdateRotateSpeed();
            if (Shift)
            {
                MoveMax = DashSpeedMax;
                CurType = AnimationType.DASH;
            }
            else if (Space)
            {
                MoveMax = moveSpeedMax;
                CurType = AnimationType.CONTROLLER;
            }
            else
            {
                MoveMax = moveSpeedMax;
                CurType = AnimationType.MOVE;
            }

        }
        else
        {
            CurType = AnimationType.IDLE;
        }
        UpdateRotate();
        UpdatePostation();
        
        //if (Up)
        //{
        //    CurType = AnimationType.IDLE;
        //}
        //else if (Left)
        //{
        //    CurType = AnimationType.MOVE;
        //}
        //else if (Right)
        //{
        //    CurType = AnimationType.DASH;
        //}else if (GameManager.Controller.Space)
        //{
        //    CurType = AnimationType.FANCY;
        //    //PlayAnimation(AnimationQueueType.FANCY);
        //}
        //else if(GameManager.Controller.C)
        //{
        //    CurType = AnimationType.CONTROLLER;
        //    //PlayController();
        //}
        //if (CurType == AnimationType.CONTROLLER)
        //{
        //    //weight = Mathf.Clamp01(weight);
        //    SelectBehaviour.GetInputBehaviour<AnimatorPlayable>((int)AnimationType.CONTROLLER).SetInputWeight(weight);
        //    //SelectBehaviour.GetAnimAdapterPlayable().GetInput<AnimatorPlayable>((int)AnimationType.CONTROLLER).SetInputWeight(weight);
        //    //ControllerMix.SetInputWeight(1f - weight);
        //    //ControllerMix.SetInputWeight(weight);
        //}
    }
    private void UpdatePostation()
    {
        playerTransform.localPosition = new Vector3(playerTransform.localPosition.x + curXSpeed * Time.deltaTime, 0, playerTransform.localPosition.z + curYSpeed * Time.deltaTime);
    }
    private void UpdateRotate()
    {
        float curAngele = playerTransform.transform.localEulerAngles.y ;
        //float curAngele1 = curAngele > 180 ? curAngele - 360 : curAngele;
        float tar = targetRotateAngel < 0 ? 360 + targetRotateAngel : targetRotateAngel;
        float angel = rotateSpeed * Time.deltaTime;
        if (Mathf.Abs(tar - curAngele) <= Mathf.Abs(angel))
        {
            angel = Mathf.Abs(tar - curAngele) * rotateDir;
            rotateSpeed = 0;
        }
        playerTransform.transform.RotateAround(playerTransform.localPosition, Vector3.up, angel);
    }
    private void UpdateSpeed()
    {
        float targetYSpeed;
        float targetXSpeed;
        if (Up || Down)
        {
            targetYSpeed = Up ? MoveMax : -MoveMax;
        }
        else
        {
            targetYSpeed = 0f;
        }
        float detlSpeed = MoveMax / GameManager.Animation.enableTime;
        if (targetYSpeed > curYSpeed)
        {
            curYSpeed += detlSpeed;
            curYSpeed = Mathf.Min(curYSpeed, targetYSpeed);
        }
        else
        {
            curYSpeed -= detlSpeed;
            curYSpeed = Mathf.Max(curYSpeed, targetYSpeed);
        }

        if (Left || Right)
        {
            targetXSpeed = Right ? MoveMax : -MoveMax;
        }
        else
        {
            targetXSpeed = 0f;
        }
        if (targetXSpeed > curXSpeed)
        {
            curXSpeed += detlSpeed;
            curXSpeed = Mathf.Min(curXSpeed, targetXSpeed);
        }
        else
        {
            curXSpeed -= detlSpeed;
            curXSpeed = Mathf.Max(curXSpeed, targetXSpeed);
        }
    }
    public void UpdateRotateSpeed()
    {
        targetRotateAngel = GameManager.Controller.GetAngle();
        //if (targetRotateAngel <= -998)
        //{
        //    rotateSpeed = 0f;
        //    return;
        //}

        float curAngele = playerTransform.transform.localEulerAngles.y;

        float tar = targetRotateAngel < 0 ? 360 + targetRotateAngel : targetRotateAngel;
        //float cur = curAngele > 180 ? curAngele - 360 : curAngele;

        float dtlAngel =  tar - curAngele;
        if (tar >= curAngele && Mathf.Abs(dtlAngel) <= 180f)
        {
            rotateDir = 1;
        }else if (tar >= curAngele && Mathf.Abs(dtlAngel) > 180f)
        {
            rotateDir = -1;
        }else if (tar < curAngele && Mathf.Abs(dtlAngel) <= 180f)
        {
            rotateDir = -1;
        }else if (tar < curAngele && Mathf.Abs(dtlAngel) > 180f)
        {
            rotateDir = 1;
        }

        rotateSpeed = MoveMax / GameManager.Animation.enableTime * 50f * rotateDir;

    }
    public void Distory()
    {
        Graph.Destroy();
    }


    private void CreatAnimation()
    {
        //ClipDic = new Dictionary<AnimationType, AnimBehaviour>();
        //BehaviourDic = new Dictionary<AnimationQueueType, AnimBehaviour>();

        GameManager.Animation.CreatPlayable(out Graph, animator);
        //RootBehaviour = new AnimRoot(Graph);
        SelectBehaviour = GameManager.Animation.SetSourceClip(Graph,animator);
        //AnimUtil.SetOutput(Graph,animator, new AnimRoot(Graph));
        for (int i = (int)AnimationType.IDLE; i < (int)AnimationType.MAX; ++i)
        {
            SelectBehaviour.AddInput(GameManager.Animation.CreatAnimUnit(Graph,(AnimationType)i));
        }

        //for (int i = (int)AnimationQueueType.FANCY; i < (int)AnimationQueueType.MAX; ++i)
        //{
        //    BehaviourDic.Add((AnimationQueueType)i, GameManager.Animation.QueueInit(Graph, (AnimationQueueType)i));
        //}
        //ControllerMix = GameManager.Animation.CreatController(Graph);


    }


    private void PlayAnimation(AnimationType animationType)
    {
        //foreach (var kv in ClipDic)
        //{
        //    if (kv.Key == animationType)
        //    {

        //       // GameManager.Animation.SetSourceClip(Output, kv.Value.GetAnimAdapterPlayable());
        //        kv.Value.Enable();
        //    }
        //    else
        //    {
        //        kv.Value.Disable();
        //    }
        //}
        //SelectBehaviour.Disable();

        SelectBehaviour.SelectAnim(animationType);
    
    }

    private void PlayController()
    {
        //GameManager.Animation.SetSourceClip(Output, ControllerMix);
        //Graph.Play();
        //ControllerMix.Enable();
    }

}
public class Test : MonoBehaviour
{
    public void Init(AnimationClip clip)
    {
        //创建PlayableGraph
        PlayableGraph graph = PlayableGraph.Create();
        //创建AnimationPlayableOutput
        AnimationPlayableOutput output = AnimationPlayableOutput.Create(graph, "Anim", GetComponent<Animator>());
        //通过AnimationClip创建AnimationClipPlayable
        AnimationClipPlayable animationClip = AnimationClipPlayable.Create(graph, clip);
        //将animationClip设为输出
        output.SetSourcePlayable(animationClip);
    }
}
