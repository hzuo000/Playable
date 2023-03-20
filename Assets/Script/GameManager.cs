using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(ControllerManager))]
[RequireComponent(typeof(AnimationManager))]
public class GameManager : MonoBehaviour
{
    private static GameManager _this;
    public static GameManager Inst { get => _this; }
    public static ControllerManager Controller { get; private set; }
    public static AnimationManager Animation { get; private set; }

    private List<GameInterface> _startSequence;

    public Player Player;
    private void Awake()
    {
        _this = this;
        DontDestroyOnLoad(gameObject);
        Controller = GetComponent<ControllerManager>();
        Animation = GetComponent<AnimationManager>();
        _startSequence = new List<GameInterface>
        {
            Controller,
            Animation
        };
    }
    private void Start()
    {
        foreach (var gameManager in _startSequence)
        {
            gameManager.StartUp(); 
        }
        Player.StartUp();
    }
    private void OnDestroy()
    {
        foreach (var gm in _startSequence)
        {
            gm.Close();
        }
        Player.Distory();
    }

    private void FixedUpdate()
    {
        foreach (var mg in _startSequence)
        {
            if (mg.Status == ManagerStatus.Started)
            {
                mg.UpdateData();
                Player.UpdateData();
            }
        }
    }


}
