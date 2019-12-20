using CharacterData;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using static PlayerStats.Skill;
using CharacterInfo = CharacterData.CharacterInfo;

namespace GameFile
{
    public class LoadDataPreview : MonoBehaviour
    {
        [SerializeField]
        new private TMPro.TextMeshProUGUI name = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI job = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI playtime = null;
        [SerializeField]
        private TMPro.TextMeshProUGUI totalSkillLevel = null;

        [Header("Gender Icons")]
        [SerializeField]
        private GameObject maleIcon = null;
        [SerializeField]
        private GameObject femaleIcon = null;
        [Header("Character Preview")]
        [SerializeField]
        private CharacterPreview characterPreview = null;
        [Space]
        [SerializeField]
        private UnityEngine.UI.Button backButton = null;

        private SaveData saveData = null;

        public void Initialize(string filename, SaveData data)
        {
            saveData = data;
            name.text = filename;
            playtime.text = $"{System.TimeSpan.FromSeconds(data.RealPlayTime).Hours.ToString("00")}:" +
                            $"{System.TimeSpan.FromSeconds(data.RealPlayTime).Minutes.ToString("00")}";

            int totalLvl = 0;

            if (data.playerSkillsData != null)
            {
                totalLvl = data.playerSkillsData[Type.Charisma].level +
                               data.playerSkillsData[Type.Fitness].level +
                               data.playerSkillsData[Type.Intelligence].level +
                               data.playerSkillsData[Type.Strength].level +
                               data.playerSkillsData[Type.Cooking].level +
                               data.playerSkillsData[Type.Repair].level;
            }

            totalSkillLevel.text = (totalLvl <= 0) ? "-" : totalLvl.ToString("0");

            job.text = "Unemployed"; //NOT IMPLEMENTED YET

            CharacterInfo charInfo = data.characterVisuals.GetVisuals();

            maleIcon.SetActive(charInfo.Gender == Gender.Male);
            femaleIcon.SetActive(charInfo.Gender == Gender.Female);

            characterPreview.SetHair(charInfo.SpriteSets[CharacterPart.Hair]);
            characterPreview.SetBody(charInfo.SpriteSets[CharacterPart.Body]);
            characterPreview.SetHead(charInfo.SpriteSets[CharacterPart.Head]);
            characterPreview.SetShirt(charInfo.SpriteSets[CharacterPart.Top]);
            characterPreview.SetPants(charInfo.SpriteSets[CharacterPart.Bottom]);
            characterPreview.SetHands(charInfo.SpriteSets[CharacterPart.Hands]);
        }

        public void SetBackAction(UnityAction backAction)
        {
            backButton.onClick.AddListener(backAction);
        }

        public void LoadData()
        {
            Data.Set(saveData, name.text);
            MainMenu.LoadGameScene();
        }

        public void DeleteThisSave()
        {
            DirectoryInfo dir = new DirectoryInfo(Application.persistentDataPath);
            var files = dir.GetFiles().Where(obj => obj.Name.EndsWith(name.text + Save.fileExtension));
            foreach (FileInfo file in files)
                file.Delete();
            
            MainMenu.LoadSaves();
        }
    }
}