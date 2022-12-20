using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;


[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    private void Awake()
    {
        // load components
        cinemachineTargetGroup = GetComponent<CinemachineTargetGroup>();
        
    }

    void Start()
    {
        SetCinemachineTargetGroup();
    }

    private void SetCinemachineTargetGroup()
    {
        // create target group for cinemachine for the cinemachine camera to follow
        CinemachineTargetGroup.Target cinemachineGroupTarget_player = new CinemachineTargetGroup.Target
            { weight = 1f, radius = 1f, target = GameManager.Instance.GetPlayer().transform };

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[]
            { cinemachineGroupTarget_player };
        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
        
        
        
    }
}
