namespace Valheim.DropThat.Drop.DropTableSystem.Conditions
{
    public interface IDropTableCondition
    {
        bool ShouldFilter(DropSourceTemplateLink context, DropTemplate template);
    }
}
