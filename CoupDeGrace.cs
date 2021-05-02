using System;
using System.Collections.Generic;
using Kingmaker.Blueprints;
using JetBrains.Annotations;
using Kingmaker.Blueprints.Classes;
using Kingmaker.Controllers.Combat;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Items.Slots;
using Kingmaker.PubSubSystem;
using Kingmaker.RuleSystem.Rules;
using Kingmaker.RuleSystem.Rules.Damage;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Abilities.Blueprints;
using Kingmaker.UnitLogic.Abilities.Components.Base;
using Kingmaker.Utility;

namespace CoupDeGrace
{
	[ComponentName("AbilityEffectNewCoup")]
	[AllowedOn(typeof(BlueprintAbility))]
	public class AbilityEffectCoupDeGrace : AbilityCustomLogic
	{
		[UsedImplicitly]
		public override IEnumerator<AbilityDeliveryTarget> Deliver(AbilityExecutionContext context, TargetWrapper target)
		{
			UnitEntityData caster = context.MaybeCaster;
			bool flag = caster == null;
			if (flag)
			{
				UberDebug.LogError(this, "Caster is missing", Array.Empty<object>());
				yield break;
			}
			WeaponSlot threatHand = caster.GetThreatHand();
			bool flag2 = threatHand == null;
			if (flag2)
			{
				UberDebug.LogError("Caster can't attack", Array.Empty<object>());
				yield break;
			}
			UnitEntityData targetUnit = target.Unit;
			bool flag3 = targetUnit == null;
			if (flag3)
			{
				UberDebug.LogError("Can't be applied to point", Array.Empty<object>());
				yield break;
			}
			int attackPenalty = 0;
			AbilityEffectCoupDeGrace.EventHandlers handlers = new AbilityEffectCoupDeGrace.EventHandlers();
			handlers.Add(new AbilityEffectCoupDeGrace.Coup(caster));
			RuleAttackWithWeapon rule = new RuleAttackWithWeapon(caster, targetUnit, threatHand.Weapon, attackPenalty)
			{
				AutoHit = true,
				AutoCriticalConfirmation = true,
				AutoCriticalThreat = true
			};
			using (handlers.Activate())
			{
				context.TriggerRule<RuleAttackWithWeapon>(rule);
			}
			AbilityEffectCoupDeGrace.EventHandlers eventHandlers = null;
			yield return new AbilityDeliveryTarget(target);		
			RuleSavingThrow rule3 = new RuleSavingThrow(targetUnit, SavingThrowType.Fortitude, AbilityEffectCoupDeGrace.m_coupDamage + 10);
			context.TriggerRule<RuleSavingThrow>(rule3);
			bool flag6 = !rule3.IsPassed;
				if (flag6)
				{
					targetUnit.Descriptor.State.MarkedForDeath = true;
				}
				using (context.GetDataScope(target))
				{
					this.Actions.Run();
				}
				ElementsContextData elementsContextData = null;
				rule3 = null;
			yield break;
		}


		public override void Cleanup(AbilityExecutionContext context)
		{
		}

		private static int m_coupDamage;
		public ActionList Actions;

		private class EventHandlers : IDisposable
		{

			public void Add(object handler)
			{
				this.m_Handlers.Add(handler);
			}

			public AbilityEffectCoupDeGrace.EventHandlers Activate()
			{
				foreach (object subscriber in this.m_Handlers)
				{
					EventBus.Subscribe(subscriber);
				}
				return this;
			}

			public void Dispose()
			{
				foreach (object subscriber in this.m_Handlers)
				{
					EventBus.Unsubscribe(subscriber);
				}
			}

			private readonly List<object> m_Handlers = new List<object>();
		}


		public class Coup : IInitiatorRulebookHandler<RuleDealDamage>, IRulebookHandler<RuleDealDamage>, IInitiatorRulebookSubscriber
		{
	
			public Coup(UnitEntityData unit)
			{
				this.m_Unit = unit;
			}

			public UnitEntityData GetSubscribingUnit()
			{
				return this.m_Unit;
			}

			public void OnEventAboutToTrigger(RuleDealDamage evt)
			{
			}

			public void OnEventDidTrigger(RuleDealDamage evt)
			{
				AbilityEffectCoupDeGrace.m_coupDamage = evt.Damage;
			}

			private readonly UnitEntityData m_Unit;
		}
	}
}