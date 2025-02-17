using UnityEngine;
using DG.Tweening;

namespace IML.Gaze
{
    public class RotatingObject : MonoBehaviour
    {
        [Header("Rotation Settings")]
        [SerializeField] private bool randomizeRotation = true;
        [SerializeField, Range(0f, 1f)] private float rotationSpeed = 0.5f;
        [SerializeField] private Vector3 rotationDirection = Vector3.up;

        [Header("Hover Settings")]
        [SerializeField] private float hoverSpeedChange = 0.0f;
        [SerializeField] private float hoverScaleChange = 0.5f;
        [SerializeField] private float hoverTransitionDuration = 0.5f;

        [Header("Select Settings")]
        [SerializeField] private float selectTransitionDuration = 0.2f;

        private Vector3 originalScale;
        private float originalSpeed;
        private float currentSpeed;
        private Tween hoverTween;
        private bool selected = false;

        void Start()
        {
            originalScale = transform.localScale;

            if (randomizeRotation)
            {
                rotationSpeed = Random.Range(0.1f, 0.9f);
                rotationDirection = Random.onUnitSphere;
            }

            originalSpeed = rotationSpeed;
            currentSpeed = rotationSpeed;

            // Start rotating the object
            RotateObject();
        }

        void RotateObject()
        {
            // Rotate the object continuously
            transform.DORotate(rotationDirection * 360, 1 / currentSpeed, RotateMode.FastBeyond360)
                     .SetEase(Ease.Linear)
                     .SetLoops(-1, LoopType.Incremental);
        }

        public void OnHoverEnter()
        {
            if (selected)
            {
                return;
            }

            // Increase rotation speed and reduce size gradually
            float newSpeed = originalSpeed * hoverSpeedChange;
            Vector3 newScale = originalScale * hoverScaleChange;

            hoverTween?.Kill();

            hoverTween = DOTween.Sequence()
                .Append(DOTween.To(() => currentSpeed, x => currentSpeed = x, newSpeed, hoverTransitionDuration)
                                .OnUpdate(() => UpdateRotationSpeed()))
                .Join(transform.DOScale(newScale, hoverTransitionDuration).SetEase(Ease.OutQuad));
        }

        public void OnHoverExit()
        {
            if (selected)
            {
                return;
            }

            // Reset rotation speed and size to original gradually
            hoverTween?.Kill();

            hoverTween = DOTween.Sequence()
                .Append(DOTween.To(() => currentSpeed, x => currentSpeed = x, originalSpeed, hoverTransitionDuration)
                                .OnUpdate(() => UpdateRotationSpeed()))
                .Join(transform.DOScale(originalScale, hoverTransitionDuration).SetEase(Ease.OutQuad));
        }

        void UpdateRotationSpeed()
        {
            // Update the rotation speed dynamically
            transform.DOKill();
            RotateObject();
        }

        public void OnSelect()
        {
            selected = true;
            hoverTween?.Kill();

            // Gradually reduce scale to 0 and set inactive
            transform.DOScale(Vector3.zero, selectTransitionDuration)
                     .SetEase(Ease.InQuad)
                     .OnComplete(() => gameObject.SetActive(false));
        }
    }
}
