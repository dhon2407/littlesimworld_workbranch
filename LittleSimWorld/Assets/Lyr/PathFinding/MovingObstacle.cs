using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
namespace PathFinding {
    [RequireComponent(typeof(Collider2D))]
    public class MovingObstacle : MonoBehaviour {
        Collider2D col;

        [Tooltip("How often the Obstacle updates its collider's position (0 = each frame)")]
        [SuffixLabel("seconds"), SerializeField]
        float UpdateFrequency = 0;

        float currentT;

        void Awake() => col = GetComponent<Collider2D>();
        void Update() {
            if (UpdateFrequency != 0) {
                currentT += Time.deltaTime;
                if (currentT <= UpdateFrequency) { return; }
                currentT = 0;
            }

            NodeGridManager.RegisterUnwalkable(col);
        }

    }
}