using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// The strip that holds cells of 60 zones
    /// Also indicates next safe and next super zones
    /// </summary>
    
    public class ZoneBarView : MonoBehaviour
    {
        #region Reference Variables

        [Header("Reference")] 
        [SerializeField] private RectTransform _content;
        [SerializeField] private ZoneCellView _cellPrefab;
        [SerializeField] private TMP_Text _nextSafeText;
        [SerializeField] private TMP_Text _nextSuperText;

        #endregion

        #region Look Variables

        [Header("Look")] 
        [SerializeField] private Sprite _normalSprite;
        [SerializeField] private Sprite _safeSprite;
        [SerializeField] private Sprite _superSprite;
        [SerializeField] private Sprite _currentSprite;
        [SerializeField] private Color _passedTint = new Color(1f, 1f, 1f, 0.35f);
        [SerializeField, Min(0f)] private float _spacing = 12f;
        [SerializeField, Min(0f)] private float _scrollDuration = 0.4f;

        #endregion

        #region Unity Runtime

        private void OnDestroy()
        {
            _content.DOKill();
        }

        #endregion

        #region Rule Variables

        private readonly List<ZoneCellView> _cells = new List<ZoneCellView>();
        private ZoneRules _rules;
        private float _cellWidth;

        #endregion

        #region Builder

        //Build the cells one by one and add them to _cells
        public void Build(ZoneRules zoneRules)
        {
            _rules = zoneRules;
            _cellWidth = _cellPrefab.RectTransform.rect.width;

            for (int zone = 1; zone <= _rules.TotalZones; zone++)
            {
                var cell = Instantiate(_cellPrefab, _content);
                cell.name = $"ui_cell_zone_{zone}";
                cell.RectTransform.anchoredPosition = new Vector2(CellCenter(zone), 0f);
                cell.SetNumber(zone);
                _cells.Add(cell);
            }
        }

        #endregion

        #region Zone Methods

        //Adjust the zone numebrs on the cells of the zone progress bar
        public void ShowZone(int currentZone)
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                var zone = i + 1;
                var sprite = zone == currentZone ? _currentSprite : SpriteFor(_rules.GetZoneType(zone));
                _cells[i].SetLook(sprite, zone < currentZone ? _passedTint : Color.white);               
            }

            _content.DOKill();
            _content.DOAnchorPosX(-CellCenter(currentZone), _scrollDuration).SetEase(Ease.OutCubic);
            
            _nextSafeText.text = NextZoneText("SAFE", _rules.FindNextZone(currentZone, ZoneType.Safe));
            _nextSuperText.text = NextZoneText("SUPER", _rules.FindNextZone(currentZone, ZoneType.Super));
        }
        
        private static string NextZoneText(string label, int zone) =>
            zone > 0 ? $"NEXT {label}: {zone}" : $"NO MORE {label}";

        #endregion

        #region Cell & Sprites

        private Sprite SpriteFor(ZoneType type) => type switch
        {
            ZoneType.Safe => _safeSprite,
            ZoneType.Super => _superSprite,
            _ => _normalSprite
        };
        
        private float CellCenter(int zone) => (zone - 1) * (_cellWidth + _spacing) + _cellWidth * 0.5f;

        #endregion
    }
}
