using GameSettings;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.Rendering.Universal;

namespace GameCamera
{

    [RequireComponent(typeof(PixelPerfectCamera))]
    public class PixelPerfectZoomManager : MonoBehaviour
    {
		new PixelPerfectCamera camera;

		const int defaultMinPPU = 105;
		const int defaultMaxPPU = 170;
		const float defaultScreenHeight = 1080f;

		int minPPU = 105;
		int maxPPU = 170;



		void Awake() {
			camera = GetComponent<PixelPerfectCamera>();
		}
		void Start() {
			UpdateValues();
			Settings.Display.onChangeResolution.AddListener(delegate { UpdateValues(); });
		}
		void LateUpdate() {
			if (CameraFollow.Instance != null && 
                !EventSystem.current.IsPointerOverGameObject() &&
                Application.isFocused && Input.GetAxis("Mouse ScrollWheel") != 0) {
				camera.assetsPPU += Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel"));
				ForceUpdateCamera();
			}
		}

		void ForceUpdateCamera() {
            if (CameraFollow.Instance != null) {
                camera.assetsPPU = Mathf.Clamp(camera.assetsPPU, minPPU, maxPPU);

                CameraFollow.Instance.SetLimits();
                CameraFollow.Instance.UpdateCamera();
            }
		}

		private void UpdateValues()
        {
#if !UNITY_EDITOR
            camera.refResolutionX = Settings.Display.CurrentGameResolution.width;
            camera.refResolutionY = Settings.Display.CurrentGameResolution.height;

			float ratio = Settings.Display.CurrentGameResolution.height / defaultScreenHeight;

			// Update value range
			minPPU = (int) (defaultMinPPU * ratio);
			maxPPU = (int) (defaultMaxPPU * ratio);

			// Set zoom to be the middle of the zoom range allowed
			var medium = minPPU + (minPPU - maxPPU) / 2;
			camera.assetsPPU = (int) medium;

			ForceUpdateCamera();
#endif
		}

		private void Reset()
        {
            camera = GetComponent<PixelPerfectCamera>();
        }
    }
}