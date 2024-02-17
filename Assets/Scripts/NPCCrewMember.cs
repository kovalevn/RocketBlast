using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCrewMember : CrewMember
{
    private bool ejectTimeChanged = false;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        FindCrewMemberEjectTime();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!CrewMemberEjected && !gameController.RocketDestroyed)
        {
            if (gameController.TimeFromStart >= EjectTime)
            {
                EjectCrewMember();
            }
        }
    }
    private void FindCrewMemberEjectTime()
    {
        EjectTime = Random.Range(1f, 10f);
    }

    public void ChangeEjectTime()
    {
        if (!ejectTimeChanged)
        {
            EjectTime = gameController.TimeFromStart + Random.Range(0.3f, 1f);
            ejectTimeChanged = true;
        }
    }
}
