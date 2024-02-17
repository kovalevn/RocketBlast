using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrewMember : CrewMember
{
    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!CrewMemberEjected && !gameController.RocketDestroyed && gameController.isGameStarted)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EjectCrewMember();
                EjectTime = gameController.TimeFromStart;
            }
        }
    }
}
