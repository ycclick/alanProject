using UnityEngine;
using UnityEngine.UI;

namespace Project
{
    public class UIComponent : MonoBehaviour
    {

        private Image barImage;
        /// <summary>
        /// bar底圖
        /// </summary>
        private Image barImageBack;

        [SerializeField]
        private int barValue;
        [SerializeField]
        private int bar;
        private void Awake()
        {
            
        }
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}

