using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// One cell in the zone bar, a background and a number
    /// </summary>
    
    public class ZoneCellView : MonoBehaviour
    {
        #region Cell Variables

        [SerializeField] private Image _background;
        [SerializeField] private TMP_Text _number;
        public RectTransform RectTransform => (RectTransform)transform;
        
        #endregion
        
        public void SetNumber(int ZoneNumber) => _number.text = ZoneNumber.ToString();

        public void SetLook(Sprite sprite, Color tint)
        {
            _background.sprite = sprite;
            _background.color = tint;
            _number.color = tint;
        }
    }
}
