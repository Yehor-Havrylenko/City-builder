using BuildingSystems.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private GameController gameController;
        [SerializeField] private Button selectSmallBuildButton;
        [SerializeField] private Button selectBigBuildButton;
        [SerializeField] private Button selectRoadButton;
        [SerializeField] private Button resetButton;

        private void Awake()
        {
            selectSmallBuildButton.onClick.AddListener(OnSelectSmallBuild);
            selectBigBuildButton.onClick.AddListener(OnSelectBigBuild);
            selectRoadButton.onClick.AddListener(OnSelectRoad);
            resetButton.onClick.AddListener(OnReset);
        }

        private void OnSelectSmallBuild()
        {
            gameController.CreateObject(Vector3.zero,0, PlacerType.Build);
        }

        private void OnSelectBigBuild()
        {
            gameController.CreateObject(Vector3.zero, 1, PlacerType.Build);
        }

        private void OnSelectRoad()
        {
            gameController.CreateObject(Vector3.zero,0, PlacerType.Road);
        }

        private void OnReset()
        {
            gameController.ResetToDefault();
        }
    }
}