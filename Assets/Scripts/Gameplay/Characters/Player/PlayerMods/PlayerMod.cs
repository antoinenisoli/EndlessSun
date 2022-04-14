using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMod : CharacterMod
{
    protected PlayerController2D player => entity as PlayerController2D;
    protected PlayerProfile playerProfile => player.CharacterProfile as PlayerProfile;
}
