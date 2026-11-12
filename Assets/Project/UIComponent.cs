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

            public float width;
            public float height;
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
                rectTransform = new RectTransformData();
                rectTransform.width = 100;
                rectTransform.height = 100;
                rectTransform.anchors = new Anchors();
                rectTransform.anchors.preset = AnchorPreset.Custom;
                rectTransform.anchors.Min = new Vector2(0.5f, 0.5f);
                rectTransform.anchors.Max = new Vector2(0.5f, 0.5f);

            }
            public GameObject uibarObject;
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
            public RectTransformData rectTransform = new RectTransformData();
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
                if (bar.uibarObject != null)
                {


                    return;
                }
                bar.uibarObject = new GameObject(bar.barName);
                bar.uibarObject.transform.SetParent(uiObj.transform);
                Image barImage = bar.uibarObject.AddComponent<Image>();
                barImage.sprite = bar.barImage;
                RectTransform rectTransform = bar.uibarObject.GetComponent<RectTransform>();
                rectTransform.localPosition = Vector3.zero;
                rectTransform.anchorMax = bar.rectTransform.anchors.Max;
                rectTransform.anchorMin = bar.rectTransform.anchors.Min;
                rectTransform.sizeDelta = new Vector2(bar.rectTransform.width, bar.rectTransform.height);
              
                //bar.rectTransform = rectTransform;
            }
        }
        public void ClearObjects()
        {
            foreach (var bar in barList)
            {
                if (bar.uibarObject != null)
                {
                    DestroyImmediate(bar.uibarObject);
                    bar.uibarObject = null;
                }
            }
        }

    }
}


