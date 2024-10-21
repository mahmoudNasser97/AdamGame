using System;
using System.Collections.Generic;
using UnityEngine;
using AISystem.Civil.Objects;

namespace AISystem.Civil.CivilAISystem
{
    public class DutyList : ScriptableObject
    {
        public Duty[] duties;

        public DutyList(Duty[] newDuties)
        {
            duties = newDuties;
        }

        public void setDuties(Duty[] newDuties)
        {
            duties = newDuties;
        }

        public List<Duty> GetDuties(Guid[] duties_to_lookup)
        {
            List<Duty> looked_up_duties = new List<Duty>();

            foreach (Guid candidate in duties_to_lookup)
            {
                Duty looked_up_entry = getDuty(candidate.ToString());

                looked_up_duties.Add(looked_up_entry);
            }

            return looked_up_duties;
        }

        public Duty getDuty(string current_duty)
        {
            foreach (Duty candidate in duties)
            {
                if (candidate.id == current_duty)
                {
                    return candidate;
                }
            }

            // Return IDLE if not found
            return duties[0];
        }

        public DutyList makeDutiesList(Job dutiesToLookup)
        {
            List<Duty> lookedUpList = new List<Duty>();

            foreach (var candidate in dutiesToLookup.duties)
            {
                foreach (Duty masterListCandidate in duties)
                {
                    if (candidate.GetGuid() == masterListCandidate.id)
                    {
                        lookedUpList.Add(masterListCandidate);
                    }
                }
            }

            DutyList newDutyList =  ScriptableObject.CreateInstance<DutyList>();

            newDutyList.setDuties(lookedUpList.ToArray());

            return newDutyList;
        }
    }
}
