using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoulderMotor : HingeMotor
{
    [SerializeField]
    private GameObject topArmBeam;
    [SerializeField]
    private GameObject frontLinkageBeam;
    [SerializeField]
    private GameObject bottomFrontNode;
    [SerializeField]
    private GameObject topFrontNode;
    [SerializeField]
    private GameObject topBackNode;
    [SerializeField]
    private GameObject bottomBackNode;
    [SerializeField]
    private GameObject elbowMotor;
    [SerializeField]
    private float shoulderAngle = 90f;
    private float width;
    private float height;
    protected override void Start()
    {
        base.Start();
        width = Vector3.Distance(bottomFrontNode.transform.position,
            bottomBackNode.transform.position);
        Debug.Log(width);
        height = Vector3.Distance(bottomFrontNode.transform.position,
            topBackNode.transform.position);
        Debug.Log(height);
    }
    protected override void Render()
    {
        base.Render();
        topArmBeam.transform.localPosition = new Vector3(topArmBeam.transform.localPosition.x + (frontLinkageBeam.transform.localScale.z * Mathf.Sin(0)),
                                                    topArmBeam.transform.localPosition.y,
                                                    topArmBeam.transform.localPosition.z);

        bottomBackNode.transform.localPosition = new Vector3(bottomBackNode.transform.localPosition.x,
            bottomFrontNode.transform.localPosition.y + (width * Mathf.Sin((transform.localRotation.x + 90) * Mathf.Deg2Rad)),
            bottomFrontNode.transform.localPosition.z + (width * Mathf.Cos((transform.localRotation.x + 90) * Mathf.Deg2Rad)));
        Debug.Log(elbowMotor.transform.localEulerAngles.x);
        topFrontNode.transform.localPosition = new Vector3(topFrontNode.transform.localPosition.x,
            height * Mathf.Cos((elbowMotor.transform.localEulerAngles.x + 90) * Mathf.Deg2Rad), height * Mathf.Sin((elbowMotor.transform.localEulerAngles.x + 90) * Mathf.Deg2Rad));

        topBackNode.transform.localPosition = new Vector3(topBackNode.transform.localPosition.x,
            topFrontNode.transform.localPosition.y + (width * Mathf.Cos(transform.localRotation.x * Mathf.Deg2Rad)),
            topFrontNode.transform.localPosition.z + (width * Mathf.Sin(transform.localRotation.x * Mathf.Deg2Rad)));
    }
}   
