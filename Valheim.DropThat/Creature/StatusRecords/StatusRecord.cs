﻿using System.Collections.Generic;
using System.Linq;

namespace DropThat.Creature.StatusRecords;

public sealed class StatusRecord
{
    private HashSet<string> statusSet = null;
    private List<string> statusList;

    public List<string> Statuses {
        get
        {
            statusSet = null;
            return statusList;
        }
        set
        {
            statusList = value;
        }
    }

    public bool HasStatus(string status)
    {
        if( statusSet is null)
        {
            statusSet = statusList.ToHashSet();
        }

        return statusSet.Contains(status);
    }
}
