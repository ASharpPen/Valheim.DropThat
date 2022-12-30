using System;

namespace DropThat.Core.Network;

[Serializable]
internal abstract class Dto
{
    public virtual void BeforePack()
    {
    }

    public virtual void AfterUnpack()
    {
    }
}