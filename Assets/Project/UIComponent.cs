using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class UIComponent : MonoBehaviour
    {
        public enum AnchorPreset
        {
            Custom,
            TopLeft,
            Top,
            TopRight,
            Left,
            Center,
            Right,
            BottomLeft,
            Bottom,
            BottomRight,
            StretchHorizontal,
            StretchVertical,
            StretchAll
        }
        [Serializable]
        public struct Anchors
        {
            public AnchorPreset preset;
            public Vector2 Min;
            public Vector2 Max;
        }
        [Serializable]
        public class RectTransformData
        {
            public Anchors anchors;
            public Vector2 sizeDelta;
            public Vector3 localScale;
            public Quaternion localRotation;
            public Vector3 scale;

        }
        [Serializable]
        public class UIBar
        {
            public UIBar(string name, Sprite image, Sprite imageBack, int value, int bar)
            {
                barName = name;
                barImage = image;
                barImageBack = imageBack;
                maxBar = value;
                this.nowBar = bar;
                rectTransform.anchors.preset = AnchorPreset.Custom;
            }
            public string barName;
            [SerializeField]
            public Sprite barImage;
            /// <summary>
            /// bar底圖
            /// </summary>
            [SerializeField]
            private Sprite barImageBack;

            [SerializeField]
            private int nowBar;
            [SerializeField]
            private int maxBar;
            [SerializeField]
            public RectTransformData rectTransform;
        }
        [SerializeField]
        private List<UIBar> barList = new List<UIBar>();
        [SerializeField]
        private GameObject uiObj;

        //private void OnValidate()
        //{
        //    UnityEditor.EditorApplication.delayCall += GenerateObjects;
        //}
        //[ContextMenu("Generate Objects")]
        public void GenerateObjects()
        {
            foreach (var bar in barList)
            {
                GameObject barObj = new GameObject(bar.barName);
                barObj.transform.SetParent(uiObj.transform);
                Image barImage = barObj.AddComponent<Image>();
                barImage.sprite = bar.barImage;
                RectTransform rectTransform = barObj.GetComponent<RectTransform>();
                rectTransform.sizeDelta = new Vector2(100, 20); // Set the size of the bar
                rectTransform.anchoredPosition = new Vector2(0, 0); // Set the position of the bar
                //bar.rectTransform = rectTransform;
            }
        }

        public void AnchorPresetEditor(AnchorPreset preset, RectTransform rectTransform)
        {
            switch (preset)
            {
                case AnchorPreset.TopLeft:
                    rectTransform.anchorMin = new Vector2(0, 1);
                    rectTransform.anchorMax = new Vector2(0, 1);
                    break;
                case AnchorPreset.Top:
                    rectTransform.anchorMin = new Vector2(0.5f, 1);
                    rectTransform.anchorMax = new Vector2(0.5f, 1);
                    break;
                case AnchorPreset.TopRight:
                    rectTransform.anchorMin = new Vector2(1, 1);
                    rectTransform.anchorMax = new Vector2(1, 1);
                    break;
                case AnchorPreset.Left:
                    rectTransform.anchorMin = new Vector2(0, 0.5f);
                    rectTransform.anchorMax = new Vector2(0, 0.5f);
                    break;
                case AnchorPreset.Center:
                    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                case AnchorPreset.Right:
                    rectTransform.anchorMin = new Vector2(1, 0.5f);
                    rectTransform.anchorMax = new Vector2(1, 0.5f);
                    break;
                case AnchorPreset.BottomLeft:
                    rectTransform.anchorMin = new Vector2(0, 0);
                    rectTransform.anchorMax = new Vector2(0, 0);
                    break;
                case AnchorPreset.Bottom:
                    rectTransform.anchorMin = new Vector2(0.5f, 0);
                    rectTransform.anchorMax = new Vector2(0.5f, 0);
                    break;
                case AnchorPreset.BottomRight:
                    rectTransform.anchorMin = new Vector2(1, 0);
                    rectTransform.anchorMax = new Vector2(1, 0);
                    break;
                case AnchorPreset.StretchHorizontal:
                    rectTransform.anchorMin = new Vector2(0, 0.5f);
                    rectTransform.anchorMax = new Vector2(1, 0.5f);
                    break;
                case AnchorPreset.StretchVertical:
                    rectTransform.anchorMin = new Vector2(0.5f, 0);
                    rectTransform.anchorMax = new Vector2(0.5f, 1);
                    break;
            }
        }
    }
}


