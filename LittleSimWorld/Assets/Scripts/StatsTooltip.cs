using UnityEngine;
using UnityEngine.UI;
using System.Text;
using TMPro;

namespace CharacterStats
{

[DefaultExecutionOrder(99999)]
    public class StatsTooltip : MonoBehaviour

    {
        public static StatsTooltip Instance;
        public UIPopUp PopUpScript;
        public Image image;
        public float MinWidth = 0.1f;
        public float MaxWidth = 0.92f;
        public float MinHeight = 0.2f;
        public float MaxHeight = 1;
        [SerializeField] TextMeshProUGUI statsText = null;


        private void Awake()
        {
           
        }
        private void Start()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            
            if (!PopUpScript)
            {
                PopUpScript = gameObject.GetComponent<UIPopUp>();
            }

        }

        private void LateUpdate()
        {
            transform.position = new Vector2 (Mathf.Clamp(Input.mousePosition.x , Screen.width * MinWidth, Screen.width * MaxWidth) ,
                Mathf.Clamp(Input.mousePosition.y, Screen.height * MinHeight, Screen.height * MaxHeight));
        }


        public void ShowDescription(string Description)
        {
            transform.position = new Vector2(Mathf.Clamp(Input.mousePosition.x, Screen.width * MinWidth, Screen.width * MaxWidth),
                 Mathf.Clamp(Input.mousePosition.y, Screen.height * MinHeight, Screen.height * MaxHeight));

            PopUpScript.Open();

            statsText.text = Description;

        }

        public void HideDescription()
        {
            PopUpScript.Close();
        }
    }
}