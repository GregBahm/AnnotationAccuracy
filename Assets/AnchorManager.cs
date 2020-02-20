using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorManager : MonoBehaviour
{
    private Anchor theAnchor;

    public static AnchorManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public void ParentToAnchor(Transform obj)
    {
        if(theAnchor == null)
        {
            Pose anchorPose = new Pose(obj.position, obj.rotation);
            theAnchor = Session.CreateAnchor(anchorPose);
        }
        obj.SetParent(theAnchor.transform, true);
    }
}