using DG.Tweening;
using UnityEngine;

namespace BuildingSystems.Base
{
    public class ObjectAnimation : MonoBehaviour
    {
        [SerializeField] private Vector3 flyheight;
        [SerializeField] private SpriteRenderer spriteRenderer;
        private Sequence _select;
        private Sequence _drag;
        private Vector3 _startLocalPosition;

        private void Awake()
        {
            _startLocalPosition = transform.localPosition;
            _drag = DOTween.Sequence()
                .Append(transform.DOLocalMove(_startLocalPosition + flyheight, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo))
                .Pause();
            _select = DOTween.Sequence()
                .Append(transform.DOLocalMove(_startLocalPosition + flyheight, 0.5f))
                .Append(spriteRenderer.DOColor(Color.gray, 0.5f)
                    .SetLoops(-1, LoopType.Yoyo))
                .Pause();
        }

        public void PlayDrag()
        {
            _drag.Restart();
        }

        public void StopDrag()
        {
            if (_drag != null)
            {
                _drag.Pause();
            }

            transform.localPosition = _startLocalPosition;
        }

        public void PlaySelect()
        {
            _select.Restart();
        }

        public void StopSelect()
        {
            if (_select != null)
            {
                _select.Pause();
            }

            spriteRenderer.color = Color.white;
            transform.localPosition = _startLocalPosition;
        }
    }
}