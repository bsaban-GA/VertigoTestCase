using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.TestCase.UI
{
    public class RewardFlyView : MonoBehaviour
    {
        [SerializeField, Tooltip("Inactive child copied for each flying icon")] private Image _iconTemplate;
        [SerializeField, Min(0f)] private float _duration = 0.6f;

        private readonly List<Image> _pool = new List<Image>();

        public void Fly(Sprite sprite, RectTransform from, RectTransform to)
        {
            var icon = TakeIcon();
            icon.sprite = sprite;

            var rect = icon.rectTransform;
            rect.position = from.position;
            rect.localScale = Vector3.one;
            icon.gameObject.SetActive(true);

            DOTween.Sequence()
                .Append(rect.DOScale(1.3f, _duration * 0.25f).SetEase(Ease.OutQuad))
                .Append(rect.DOMove(to.position, _duration * 0.75f).SetEase(Ease.InCubic))
                .Join(rect.DOScale(0.6f, _duration * 0.75f).SetEase(Ease.InCubic))
                .OnComplete(() => icon.gameObject.SetActive(false))
                .SetLink(icon.gameObject);
        }

        private Image TakeIcon()
        {
            foreach (var icon in _pool)
            {
                if (!icon.gameObject.activeSelf)
                    return icon;
            }

            var created = Instantiate(_iconTemplate, transform);
            created.name = "ui_image_fly_icon_value";
            _pool.Add(created);
            return created;
        }
    }
}
