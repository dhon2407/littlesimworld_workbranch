using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelQuest : Mission

     
{
    public int RequieredStrLevel = 2;
    public int RequiredIntLevel = 2;
    public int RequiredFitLevel = 2;
    public int RequiredChrLevel = 2;
    public int RequiredCookingLevel = 2;
    public int RequiredRepairLevel = 2;

    // Start is called before the first frame update
    private void Start()
    {
        GetRewardButton.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerStatsManager.Instance.playerSkills[SkillType.Strength].Level >= RequieredStrLevel ||
            PlayerStatsManager.Instance.playerSkills[SkillType.Intelligence].Level >= RequiredIntLevel ||
            PlayerStatsManager.Instance.playerSkills[SkillType.Fitness].Level >= RequiredFitLevel ||
            PlayerStatsManager.Instance.playerSkills[SkillType.Charisma].Level >= RequiredChrLevel||
            PlayerStatsManager.Instance.playerSkills[SkillType.Cooking].Level >= RequiredCookingLevel||
         PlayerStatsManager.Instance.playerSkills[SkillType.Repair].Level >= RequiredRepairLevel)

        {
            if (accomplished == false)
            {
              
                    

                Accomplish();
                

               
               
               
            }
        }
        
    }
}
