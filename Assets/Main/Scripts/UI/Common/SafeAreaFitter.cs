using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// Shrinks a RectTransform to the phone's safe area su UI isn't hidden behind (especially for 20:9 phones)
    /// </summary>
    
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        #region Rect Variables

        private RectTransform _rectTransform;
        private Rect _appliedArea;

        #endregion

        #region Unity Runtime

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
            Apply();
        }

        private void Update()
        {
            //In order to catch when phoen rotates between two landscape sides
            if(Screen.safeArea != _appliedArea)
                Apply();
        }

        #endregion

        #region Resizing Methods

        private void Apply()
        {
            _appliedArea = Screen.safeArea;

            var min = _appliedArea.position;
            var max = _appliedArea.position + _appliedArea.size;
            
            _rectTransform.anchorMin = new Vector2(min.x / Screen.width, min.y / Screen.height);
            _rectTransform.anchorMax = new Vector2(max.x / Screen.width, max.y / Screen.height);
        }

        #endregion
    }
}
