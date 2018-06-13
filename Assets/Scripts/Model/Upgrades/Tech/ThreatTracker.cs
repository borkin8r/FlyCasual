using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using UpgradesList.Abilities;
using Abilities;
using Ship;
using BoardTools;
using ActionsList;
using Tokens;

namespace UpgradesList
{
    public class ThreatTracker : Upgrade.GenericUpgrade
    {
        public ThreatTracker() : base()
        {
            Types.Add(UpgradeType.Tech);
            Name = "Threat Tracker";
            ImageUrl = "";
            Cost = 3;

            UpgradeAbilities.Add(new ThreatTrackerAbility());
        }
    }
    
    namespace Abilities
    {
        public class ThreatTrackerAbility : GenericAbility
        {
            public override void ActivateAbility()
            {
                GenericShip.OnCombatActivationGlobal += RegisterThreatTrackerOpportunity;
            }

            public override void DeactivateAbility()
            {
                GenericShip.OnCombatActivationGlobal -= RegisterThreatTrackerOpportunity;
            }

            private void RegisterThreatTrackerOpportunity(GenericShip ship)
            {
                var activatedShip = Combat.Attacker;
                if (activatedShip.Owner != ship.Owner && //When an enemy ship activates during combat,
                        (ship.CanPerformAction(new BarrelRollAction()) || ship.CanPerformAction(new BoostAction())) && //check boost and/or barrel roll in action bar
                        (HostShip.Tokens.GetTargetLockLetterPair(activatedShip) != ' '))  //and you have activated enemy ship target locked
                {
                    ShotInfo arcInfo = new ShotInfo(ship, activatedShip, ship.PrimaryWeapon);
                    if (arcInfo.InArc && arcInfo.Range <= 2) //inside your firing arc at range 1-2 activates
                    {
                        RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, PerformFreeBoostOrBarrelRoll);
                    }
                }
            }

            private void PerformFreeBoostOrBarrelRoll(object sender, System.EventArgs e)
            {
                List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction>() { new ActionsList.BoostAction(), new ActionsList.BarrelRollAction() };

                HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);

                //TODO spend target lock only if action accepted
                HostShip.Tokens.SpendToken(typeof(BlueTargetLockToken), null, 'A');
            }
        }
    }

}


