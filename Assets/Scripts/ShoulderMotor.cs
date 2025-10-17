using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShoulderMotor : HingeMotor
{
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

    // Elbow motor script that we read angle off of and limit angle
    [SerializeField]
    private GameObject elbowMotor;

    // Linkage Visual Objects
    [SerializeField]
    private GameObject frontLinkage;
    [SerializeField]
    private GameObject backLinkage;
    [SerializeField]
    private GameObject bottomLinkage;
    private float width = 0.205f;
    private float height = 0.609f;
    protected override void Start()
    {
        base.Start();
        // width = Vector3.Distance(bottomFrontNode.transform.position,
        //     bottomBackNode.transform.position);
        Debug.Log(width);
        // height = Vector3.Distance(bottomFrontNode.transform.position,
        //     topBackNode.transform.position);
        Debug.Log(height);
    }
    protected override void Render()
    {
        base.Render();

        topFrontNode.transform.localPosition = new Vector3(topFrontNode.transform.localPosition.x,
            height * Mathf.Cos(elbowMotor.transform.localEulerAngles.x * Mathf.Deg2Rad),
            height * Mathf.Sin(elbowMotor.transform.localEulerAngles.x * Mathf.Deg2Rad));

        bottomBackNode.transform.localPosition = new Vector3(bottomBackNode.transform.localPosition.x,
            bottomFrontNode.transform.localPosition.y + (width * Mathf.Sin(transform.localEulerAngles.x * Mathf.Deg2Rad)),
            bottomFrontNode.transform.localPosition.z - (width * Mathf.Cos(transform.localEulerAngles.x * Mathf.Deg2Rad)));

        topBackNode.transform.localPosition = new Vector3(topBackNode.transform.localPosition.x,
            topFrontNode.transform.localPosition.y + Mathf.Abs(width * Mathf.Sin(transform.localEulerAngles.x * Mathf.Deg2Rad)),
            topFrontNode.transform.localPosition.z - (width * Mathf.Cos(transform.localEulerAngles.x * Mathf.Deg2Rad)));

        // Update the forearm position
        Vector3 topFrontToTopBack = topFrontNode.transform.localPosition - topBackNode.transform.localPosition;
        topFrontToTopBack = topFrontToTopBack / 2;
        frontLinkageBeam.transform.localPosition = topFrontNode.transform.localPosition - topFrontToTopBack;
        frontLinkageBeam.transform.localEulerAngles = new Vector3(
            -Mathf.Abs(Mathf.Atan(topFrontToTopBack.z / topFrontToTopBack.y) * Mathf.Rad2Deg) + 90,
            frontLinkageBeam.transform.localEulerAngles.y,
            frontLinkageBeam.transform.localEulerAngles.z);


        // Update the upper arm posiiton
        Vector3 botFrontToTopFront = topFrontNode.transform.localPosition - bottomFrontNode.transform.localPosition;
        botFrontToTopFront = botFrontToTopFront / 2;
        frontLinkage.transform.localPosition = botFrontToTopFront + bottomFrontNode.transform.localPosition;
        frontLinkage.transform.localEulerAngles = new Vector3(
            elbowMotor.transform.localEulerAngles.x + 90,
            frontLinkageBeam.transform.localEulerAngles.y,
            frontLinkageBeam.transform.localEulerAngles.z);

        Vector3 botBackToTopBack = topBackNode.transform.localPosition - bottomBackNode.transform.localPosition;
        botBackToTopBack = botBackToTopBack / 2;
        backLinkage.transform.localPosition = botBackToTopBack + bottomBackNode.transform.localPosition;
        backLinkage.transform.localEulerAngles = new Vector3(
            elbowMotor.transform.localEulerAngles.x + 90,
            frontLinkageBeam.transform.localEulerAngles.y,
            frontLinkageBeam.transform.localEulerAngles.z);

        // Update bottom bracket
        bottomLinkage.transform.localPosition = new Vector3(bottomBackNode.transform.localPosition.x,
            bottomFrontNode.transform.localPosition.y + ((width / 2) * Mathf.Sin(transform.localEulerAngles.x * Mathf.Deg2Rad)),
            bottomFrontNode.transform.localPosition.z - ((width / 2) * Mathf.Cos(transform.localEulerAngles.x * Mathf.Deg2Rad)));
        bottomLinkage.transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            bottomLinkage.transform.localEulerAngles.y,
            bottomLinkage.transform.localEulerAngles.z);
    }
}   
