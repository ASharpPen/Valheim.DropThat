using UnityEngine;
using ThatCore.Extensions;
using DropThat.Caches;

namespace DropThat.Drop.CharacterDropSystem.Models;

public class DropContext
{
    private Heightmap _heightmap;
    private Character _character = null;
    private ZDO _zdo;

    public DropContext(CharacterDrop characterDrop)
    {
        CharacterDrop = characterDrop;
    }

    public DropConfigInfo DropInfo { get; set; }

    public CharacterDrop CharacterDrop { get; }

    // TODO: Move this to a new context for drop modifications.
    public GameObject Item { get; set; }

    public Vector3 Pos { get; set; }

    public ZDO ZDO
    {
        get
        {
            if (_zdo is null)
            {
                var character = Character;

                if (character.IsNotNull() &&
                    character.m_nview.IsNotNull())
                {
                    _zdo = character.m_nview.m_zdo;
                }
            }

            return _zdo;
        }
    }

    public Character Character
    {
        get
        {
            if (_character.IsNull())
            {
                _character = ComponentCache.Get<Character>(CharacterDrop);
            }

            return _character;
        }
    }

    public Heightmap Heightmap
    {
        get
        {
            if (_heightmap.IsNull())
            {
                _heightmap = Heightmap.FindHeightmap(Pos);
            }

            return _heightmap;
        }
    }
}
