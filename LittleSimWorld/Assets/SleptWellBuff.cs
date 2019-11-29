﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleptWellBuff : Buffs

    
{
    

    void Start()
    {
        PlayerStatsManager.Instance.BonusXPMultiplier += XPmodifier;
    }

    void FixedUpdate()
    {
        CooldownImage.fillAmount = 1 - currentDuration / maxDuration ;
        currentDuration += Time.deltaTime * GameTime.Clock.TimeMultiplier;
        if(currentDuration >= maxDuration)
        {
            Destroy(gameObject);
        }

    }
    private void OnDestroy()
    {
        PlayerStatsManager.Instance.BonusXPMultiplier -= XPmodifier;
    }
}
