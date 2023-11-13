using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using YVR.Core;

public class HandPoseDebug : MonoBehaviour
{
    public HandType handType;
    private List<Vector3> jointPos = new List<Vector3>(new Vector3[(int)HandJoint.JointMax]);
    private HandJointLocations handJointLocations;

    private Vector3 thumb0, thumb1, thumb2, thumb3;
    private Vector3 index0, index1, index2, index3;
    private Vector3 middle0, middle1, middle2, middle3;
    private Vector3 ring0, ring1, ring2, ring3;
    private Vector3 pinky0, pinky1, pinky2, pinky3;
    private List<float> flexionAnglelList = new List<float>();
    private List<float> curlAngleList = new List<float>();
    private List<float> abductionAngleList = new List<float>();

    private Quaternion wirstRot;
    private Vector3 wirstRight;
    private Vector3 wirstForward;
    private Vector3 wristUp;
    public Text outPutSkeleton;

    private void SetSekletonAngle()
    {
        string str = handType.ToString()+"\n";
        str += "\n";
        for (int i = 0; i < flexionAnglelList.Count; i++)
        {
            str += ((XRHandFingerID)i).ToString() + " flexion angle: "+flexionAnglelList[i] +"\n";
        }

        str += "\n";
        for (int i = 0; i < curlAngleList.Count; i++)
        {
            str += ((XRHandFingerID)i).ToString() + " curl angle: "+curlAngleList[i] +"\n";
        }

        str += "\n";
        for (int i = 0; i < abductionAngleList.Count; i++)
        {
            str += ((XRHandFingerID)i).ToString() + " abduction angle: "+abductionAngleList[i] +"\n";
        }

        outPutSkeleton.text = str;
    }

    private void UpdateJointPose()
    {
        handJointLocations = handType == HandType.HandLeft
            ? YVRHandManager.instance.leftHandData
            : YVRHandManager.instance.rightHandData;

        if (handJointLocations.jointLocations == null) return;

        for (int i = 0; i < handJointLocations.jointLocations.Length; i++)
        {
            jointPos[i] = handJointLocations.jointLocations[i].pose.position;
            if (i == (int)HandJoint.JointWrist)
            {
                wirstRot = handJointLocations.jointLocations[i].pose.orientation;
            }
        }

        thumb0 = jointPos[(int)HandJoint.JointThumbTip];
        thumb1 = jointPos[(int)HandJoint.JointThumbDistal];
        thumb2 = jointPos[(int)HandJoint.JointThumbProximal];
        thumb3 = jointPos[(int)HandJoint.JointThumbMetacarpal];

        index0 = jointPos[(int)HandJoint.JointIndexTip];
        index1 = jointPos[(int)HandJoint.JointIndexDistal];
        index2 = jointPos[(int)HandJoint.JointIndexIntermediate];
        index3 = jointPos[(int)HandJoint.JointIndexProximal];

        middle0 = jointPos[(int)HandJoint.JointMiddleTip];
        middle1 = jointPos[(int)HandJoint.JointMiddleDistal];
        middle2 = jointPos[(int)HandJoint.JointMiddleIntermediate];
        middle3 = jointPos[(int)HandJoint.JointMiddleProximal];

        ring0 = jointPos[(int)HandJoint.JointRingTip];
        ring1 = jointPos[(int)HandJoint.JointRingDistal];
        ring2 = jointPos[(int)HandJoint.JointRingIntermediate];
        ring3 = jointPos[(int)HandJoint.JointRingProximal];

        pinky0 = jointPos[(int)HandJoint.JointLittleTip];
        pinky1 = jointPos[(int)HandJoint.JointLittleDistal];
        pinky2 = jointPos[(int)HandJoint.JointLittleIntermediate];
        pinky3 = jointPos[(int)HandJoint.JointLittleProximal];
    }

    public void Update()
    {
        UpdateJointPose();
        if (handType == HandType.HandLeft)
        {
            wirstRight = wirstRot * Vector3.right;
            wirstForward = wirstRot * Vector3.forward;
        }

        if (handType == HandType.HandRight)
        {
            wirstRight = wirstRot * Vector3.left;
            wirstForward = wirstRot * Vector3.forward;
        }

        wristUp = wirstRot * Vector3.up;
        SkeletonFlexionAngle();
        SkeletonCurlAngle();
        AbductionAngle();

        SetSekletonAngle();
    }

    private void SkeletonFlexionAngle()
    {
        flexionAnglelList.Clear();
        Vector3 thumb23 = (thumb2 - thumb3);
        Vector3 thumb23_project = Vector3.ProjectOnPlane(thumb23, wristUp);
        float flexThumbAngle = Vector3.Angle(thumb23_project, wirstRight);
        Vector3 index23 = (index2 - index3);
        Vector3 index_project = Vector3.ProjectOnPlane(index23, wirstRight);
        float flexIndexAngle = Vector3.Angle(index_project, wirstForward);
        Vector3 middle23 = (middle2 - middle3);
        Vector3 middle_project = Vector3.ProjectOnPlane(middle23, wirstRight);
        float flexMiddleAngle = Vector3.Angle(middle_project, wirstForward);
        Vector3 ring23 = (ring2 - ring3);
        Vector3 ring_project = Vector3.ProjectOnPlane(ring23, wirstRight);
        float flexRingAngle = Vector3.Angle(ring_project, wirstForward);
        Vector3 pinky23 = (pinky2 - pinky3);
        Vector3 pinky_project = Vector3.ProjectOnPlane(pinky23, wirstRight);
        float flexPinkyAngle = Vector3.Angle(pinky_project, wirstForward);
        flexionAnglelList.Add(flexThumbAngle);
        flexionAnglelList.Add(flexIndexAngle);
        flexionAnglelList.Add(flexMiddleAngle);
        flexionAnglelList.Add(flexRingAngle);
        flexionAnglelList.Add(flexPinkyAngle);
    }

    private void SkeletonCurlAngle()
    {
        curlAngleList.Clear();
        Vector3 thumb01 = (thumb0 - thumb1);
        Vector3 thumb32 = (thumb3 - thumb2);
        float curlThumbAngle = Vector3.Angle(thumb01, thumb32);
        Vector3 index01 = (index0 - index1);
        Vector3 index32 = (index3 - index2);
        float curlIndexAngle = Vector3.Angle(index32, index01);
        Vector3 middle01 = (middle0 - middle1);
        Vector3 middle32 = (middle3 - middle2);
        float curlMiddleAngle = Vector3.Angle(middle32, middle01);
        Vector3 ring01 = (ring0 - ring1);
        Vector3 ring32 = (ring3 - ring2);
        float curlRingAngle = Vector3.Angle(ring32, ring01);
        Vector3 pinky01 = (pinky0 - pinky1);
        Vector3 pinky32 = (pinky3 - pinky2);
        float curlPinkyAngle = Vector3.Angle(pinky32, pinky01);
        curlAngleList.Add(curlThumbAngle);
        curlAngleList.Add(curlIndexAngle);
        curlAngleList.Add(curlMiddleAngle);
        curlAngleList.Add(curlRingAngle);
        curlAngleList.Add(curlPinkyAngle);
    }

    private void AbductionAngle()
    {
        abductionAngleList.Clear();
        Vector3 thumb12 = (thumb1 - thumb2);
        Vector3 index23 = (index2 - index3);
        Vector3 middle23 = (middle2 - middle3);
        Vector3 ring23 = (ring2 - ring3);
        Vector3 pinky23 = (pinky2 - pinky3);

        float abducThumbAngle = Vector3.Angle(thumb12, index23);
        float abducIndexAngle = Vector3.Angle(index23, middle23);
        float abducMiddleAngle = Vector3.Angle(middle23, ring23);
        float abducRingAngle = Vector3.Angle(ring23, pinky23);
        abductionAngleList.Add(abducThumbAngle);
        abductionAngleList.Add(abducIndexAngle);
        abductionAngleList.Add(abducMiddleAngle);
        abductionAngleList.Add(abducRingAngle);
    }
}