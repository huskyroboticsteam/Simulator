using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
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
    [SerializeField]
    private float width = 0.205f;
    [SerializeField]
    private float height = 0.609f;

    private float prevElbowAngle = 0;
    private float prevShoulderAngle = 0; 
    protected override void Start()
    {
        base.Start();
        // Update mesh scale with inputed values
        frontLinkage.transform.localScale = new Vector3(frontLinkage.transform.localScale.x,
                                                            frontLinkage.transform.localScale.y,
                                                            height);
        backLinkage.transform.localScale = new Vector3(backLinkage.transform.localScale.x,
                                                            backLinkage.transform.localScale.y,
                                                            height);
        bottomLinkage.transform.localScale = new Vector3(bottomLinkage.transform.localScale.x,
                                                            bottomLinkage.transform.localScale.y,
                                                            width);
    }
    protected override void Render()
    {
        base.Render();

        // No state change then don't update anything
        if (prevElbowAngle == elbowMotor.transform.localEulerAngles.x &&
                prevShoulderAngle == transform.localEulerAngles.x)
        {
            return;
        }

        // calculate the nodes of each of the joints of the linkage
        topFrontNode.transform.localPosition = new Vector3(topFrontNode.transform.localPosition.x,
            height * Mathf.Cos(elbowMotor.transform.localEulerAngles.x * Mathf.Deg2Rad),
            height * Mathf.Sin(elbowMotor.transform.localEulerAngles.x * Mathf.Deg2Rad));

        bottomBackNode.transform.localPosition = new Vector3(bottomBackNode.transform.localPosition.x,
            bottomFrontNode.transform.localPosition.y + (width * Mathf.Sin(transform.localEulerAngles.x * Mathf.Deg2Rad)),
            bottomFrontNode.transform.localPosition.z - (width * Mathf.Cos(transform.localEulerAngles.x * Mathf.Deg2Rad)));

        topBackNode.transform.localPosition = new Vector3(topBackNode.transform.localPosition.x,
            topFrontNode.transform.localPosition.y + (width * Mathf.Sin(transform.localEulerAngles.x * Mathf.Deg2Rad)),
            topFrontNode.transform.localPosition.z - (width * Mathf.Cos(transform.localEulerAngles.x * Mathf.Deg2Rad)));

        // Update the forearm position and rotation
        Vector3 topFrontToTopBack = topFrontNode.transform.localPosition - topBackNode.transform.localPosition;
        topFrontToTopBack = topFrontToTopBack / 2;
        frontLinkageBeam.transform.localPosition = topFrontNode.transform.localPosition - topFrontToTopBack;
        frontLinkageBeam.transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            frontLinkageBeam.transform.localEulerAngles.y,
            frontLinkageBeam.transform.localEulerAngles.z);


        // Update the upper arm posiiton and rotation
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

        // Update bottom linkage position and rotation
        bottomLinkage.transform.localPosition = new Vector3(bottomBackNode.transform.localPosition.x,
            bottomFrontNode.transform.localPosition.y + ((width / 2) * Mathf.Sin(transform.localEulerAngles.x * Mathf.Deg2Rad)),
            bottomFrontNode.transform.localPosition.z - ((width / 2) * Mathf.Cos(transform.localEulerAngles.x * Mathf.Deg2Rad)));
        bottomLinkage.transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            bottomLinkage.transform.localEulerAngles.y,
            bottomLinkage.transform.localEulerAngles.z);

        MaxLimitPosition = localEulerAngleToEditorAngle(elbowMotor.transform.localEulerAngles.x + 60);
        MinLimitPosition = Mathf.Max(localEulerAngleToEditorAngle(elbowMotor.transform.localEulerAngles.x - 60), -30);

        // Record the previous angle for checking whether state changed
        // Elbow and shoulder only rotate around x axis, no need to check any other angles
        prevElbowAngle = elbowMotor.transform.localEulerAngles.x;
        prevShoulderAngle = transform.localEulerAngles.x;
    }

    // Euler angles cannot be negative, this function takes a angle that is greater than 180
    // and converts it to it's negative counterpart.
    private float localEulerAngleToEditorAngle(float angle)
    {
        if (angle >= 180)
        {
            return angle - 360;
        }
        return angle;
    }
}   
