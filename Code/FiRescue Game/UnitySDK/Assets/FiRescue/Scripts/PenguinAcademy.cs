using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PenguinAcademy : Academy
{
    private ForestArea[] forestAreas;
    public override void AcademyReset()
    {
        //Get the penguin Areas
        if(forestAreas == null)
        {
            forestAreas = FindObjectsOfType<ForestArea>();
        }

        //Set up areas
        foreach(ForestArea forestArea in forestAreas)
        {
            forestArea.feedRadius = resetParameters["feed_radius"];
            forestArea.ResetArea();
        }
    }
}
