namespace RSLib.Extensions
{
    using UnityEngine;

    public static class RectTransformExtensions
    {
        /// <summary>
        /// Clamps the RectTransform position to another RectTransform position.
        /// </summary>
        /// <param name="other">Reference RectTransform to clamp to.</param>
        public static void ClampTo(this RectTransform rectTransform, RectTransform other)
        {
            Vector3 localPos = rectTransform.localPosition;

            Vector3 minPos = other.rect.min - rectTransform.rect.min;
            Vector3 maxPos = other.rect.max - rectTransform.rect.max;

            localPos.x = Mathf.Clamp(rectTransform.localPosition.x, minPos.x, maxPos.x);
            localPos.y = Mathf.Clamp(rectTransform.localPosition.y, minPos.y, maxPos.y);

            rectTransform.localPosition = localPos;
        }

        /// <summary>
        /// Clamps the RectTransform position to its parent RectTransform.
        /// </summary>
        public static void ClampToParent(this RectTransform rectTransform)
        {
            UnityEngine.Assertions.Assert.IsNotNull(
                rectTransform.parent,
                $"Cannot clamp {rectTransform.name} RectTransform to its parent since it has no parent.");

            RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();

            UnityEngine.Assertions.Assert.IsNotNull(
                parentRectTransform,
                $"{rectTransform.name} RectTransform parent's {parentRectTransform.name} has no RectTransform component to do a clamp.");

            rectTransform.ClampTo(parentRectTransform);
        }
    }
}