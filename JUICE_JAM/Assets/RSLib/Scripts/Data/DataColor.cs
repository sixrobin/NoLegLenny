namespace RSLib
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Data Color", menuName = "RSLib/Data/Color")]
    public class DataColor : ScriptableObject
    {
        [SerializeField] private Color _color = Color.white;

        public static Color Default => Color.red;

        public Color Color => _color;
        public string HexCode => _color != null ? ColorUtility.ToHtmlStringRGBA(_color) : null;
    }
}