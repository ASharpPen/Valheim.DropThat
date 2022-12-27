using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DropThat.Drop.Options;

public interface IItemModifier
{
    void Modify(GameObject drop);
}
