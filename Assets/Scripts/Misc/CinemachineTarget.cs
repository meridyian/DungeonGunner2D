using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using Cinemachine;
using UnityEditor.Rendering;
using UnityEditorInternal;
using UnityEngine;


[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTarget : MonoBehaviour
{
    private CinemachineTargetGroup cinemachineTargetGroup;

    #region Tooltip

    [Tooltip("Populate with the CursorTarget gameobject")]

    #endregion Tooltip

    [SerializeField] private Transform cursorTarget;
    
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
        
        // create cursor
        CinemachineTargetGroup.Target cinemachineGroupTarget_cursor = new CinemachineTargetGroup.Target
                { weight = 1f, radius = 1f, target = cursorTarget};

        CinemachineTargetGroup.Target[] cinemachineTargetArray = new CinemachineTargetGroup.Target[]
            { cinemachineGroupTarget_player, cinemachineGroupTarget_cursor };
        
        cinemachineTargetGroup.m_Targets = cinemachineTargetArray;
    }

    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }
}
