using Valheim.DropThat.Utilities.Valheim;

namespace Valheim.DropThat.Creature.DamageRecords
{
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
    }
}
