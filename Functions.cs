using System;
using CoupDeGrace;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Blueprints.Classes.Selection;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components;
using Kingmaker.UnitLogic.Mechanics.Actions;


namespace CoupFix
{
	public class Functions
	{
		private static LibraryScriptableObject Library
		{
			get
			{
				return Main.Library;
			}
		}

		
		public static void FixCoupDeGrace()
		{
			ContextActionProvokeAttackOfOpportunity Action = Helpers.Create<ContextActionProvokeAttackOfOpportunity>(null);
			Action.ApplyToCaster = true;
			AbilityEffectCoupDeGrace replacement = Helpers.Create<AbilityEffectCoupDeGrace>(delegate (AbilityEffectCoupDeGrace a)
			{
				a.Actions = Helpers.CreateActionList(new GameAction[]
				{
					Action
				});
			});
			Functions.coup_de_grace.SetDescription("As a full-round action, you can use a melee weapon to deliver a coup de grace to a helpless opponent.\nYou automatically hit and score a critical hit.If the defender survives the damage, he must make a Fortitude save (DC 10 + damage dealt) or die.\nDelivering a coup de grace provokes attacks of opportunity from threatening opponents.\nYou can't deliver a coup de grace against a creature that is immune to critical hits.");
			Functions.coup_de_grace.ReplaceComponent<AbilityEffectRunAction>(replacement);
		}
		
		private static BlueprintAbility coup_de_grace = Library.Get<BlueprintAbility>("32280b137ca642c45be17e2d92898758");
    }


}