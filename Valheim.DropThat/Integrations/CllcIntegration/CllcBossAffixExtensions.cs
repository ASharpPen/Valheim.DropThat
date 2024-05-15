using CreatureLevelControl;

namespace DropThat.Integrations.CllcIntegration;

internal static class CllcBossAffixExtensions
{
    public static BossAffix Convert(this CllcBossAffix affix)
    {
        return affix switch
        {
            CllcBossAffix.Reflective => BossAffix.Reflective,
            CllcBossAffix.Shielded => BossAffix.Shielded,
            CllcBossAffix.Mending => BossAffix.Mending,
            CllcBossAffix.Summoner => BossAffix.Summoner,
            CllcBossAffix.Elementalist => BossAffix.Elementalist,
            CllcBossAffix.Enraged => BossAffix.Enraged,
            CllcBossAffix.Twin => BossAffix.Twin,
            _ => BossAffix.None,
        };
    }
}
