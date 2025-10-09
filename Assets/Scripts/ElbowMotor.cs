using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElbowMotor : HingeMotor
{
    [SerializeField]
    private GameObject backLinkageBeam;
    [SerializeField]
    private GameObject frontLinkageMountJoint;
    private float initAngle = 0;
    private float backLinkageCenterToPivot;
    private float initHeight;

    protected override void Start()
    {
        base.Start();
        initAngle = transform.localRotation.x;
        backLinkageCenterToPivot = Mathf.Abs(Mathf.Sqrt(Mathf.Pow(backLinkageBeam.transform.localPosition.y, 2) +
            Mathf.Pow(backLinkageBeam.transform.localPosition.z, 2)));
        initHeight = backLinkageBeam.transform.localScale.z;
    }
    protected override void Render()
    {
        base.Render();
        backLinkageBeam.transform.localPosition =
            new Vector3(backLinkageBeam.transform.localPosition.x,
                backLinkageBeam.transform.localPosition.y,
                initHeight + Mathf.Abs(backLinkageCenterToPivot * Mathf.Sin(Mathf.Deg2Rad * transform.localRotation.x)));

        frontLinkageMountJoint.transform.localRotation = new Quaternion(transform.localRotation.x,
            frontLinkageMountJoint.transform.localRotation.y,
            frontLinkageMountJoint.transform.localRotation.z,
            frontLinkageMountJoint.transform.localRotation.w);
    }
}
