using DropThat.Caches;
using DropThat.Drop.CharacterDropSystem.Models;
using DropThat.Utilities.Valheim;
using ThatCore.Extensions;
using UnityEngine;

namespace DropThat.Creature.DamageRecords;

public class DamageRecord
{
    public HitData Hit { get; set; }

    private HitData.DamageType? damageType;

    public HitData.DamageType DamageType 
    {
        get
        {
            return damageType ??= Hit?.GetCombinedDamageType() ?? 0;
        }
    }

    public Skills.SkillType SkillType => Hit?.m_skill ?? Skills.SkillType.None;

    /// <summary>
    /// Znet time when hit was recorded.
    /// </summary>
    public double Timestamp { get; set; }

    public EntityType AttackerType { get; set; } = EntityType.Other;

    public EntityType GetHitterType()
    {
        EntityType hitBy = EntityType.Other;

        if (Hit?.HaveAttacker() == true)
        {
            GameObject attacker = ZNetScene.instance.FindInstance(Hit.m_attacker);

            var attackerCharacter = ComponentCache.Get<Character>(attacker);

            if (attackerCharacter.IsNotNull())
            {
                if (attackerCharacter.IsPlayer())
                {
                    hitBy = EntityType.Player;
                }
                else
                {
                    hitBy = EntityType.Creature;
                }
            }
        }

        return hitBy;
    }
}
