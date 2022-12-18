using System;
using System.Collections.Generic;
using System.Linq;

namespace DropThat.Utilities.Valheim
{
    public static class HitDataExtensions
    {
        public static HitData.DamageType GetCombinedDamageType(this HitData hit)
        {
            HitData.DamageType result = 0;

            if (hit.m_damage.m_blunt > 0)
            {
                result |= HitData.DamageType.Blunt;
            }
            if (hit.m_damage.m_slash > 0)
            {
                result |= HitData.DamageType.Slash;
            }
            if (hit.m_damage.m_pierce > 0)
            {
                result |= HitData.DamageType.Pierce;
            }
            if (hit.m_damage.m_chop > 0)
            {
                result |= HitData.DamageType.Chop;
            }
            if (hit.m_damage.m_pickaxe > 0)
            {
                result |= HitData.DamageType.Pickaxe;
            }
            if (hit.m_damage.m_fire > 0)
            {
                result |= HitData.DamageType.Fire;
            }
            if (hit.m_damage.m_frost > 0)
            {
                result |= HitData.DamageType.Frost;
            }
            if (hit.m_damage.m_lightning > 0)
            {
                result |= HitData.DamageType.Lightning;
            }
            if (hit.m_damage.m_poison > 0)
            {
                result |= HitData.DamageType.Poison;
            }
            if (hit.m_damage.m_spirit > 0)
            {
                result |= HitData.DamageType.Spirit;
            }

            return result;
        }
    }
}
