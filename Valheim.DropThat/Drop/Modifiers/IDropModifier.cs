﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Valheim.DropThat.Caches;

namespace Valheim.DropThat.Drop.Modifiers
{
    public interface IDropModifier
    {
        void Modify(DropContext context);
    }
}
