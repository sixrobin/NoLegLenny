namespace JuiceJam
{
    using System.Linq;
    using UnityEngine;

    public class FollowingEnemyView : MonoBehaviour
    {
        private static readonly Vector2[] _lookAtDirections = new Vector2[]
        {
            Vector2.up,
            Vector2.right,
            Vector2.down,
            Vector2.left,
            Vector2.up + Vector2.right,
            Vector2.down + Vector2.right,
            Vector2.up + Vector2.left,
            Vector2.down + Vector2.left
        };

        [System.Serializable]
        public struct FaceByDirection
        {
            public Vector2 Direction;
            public Sprite FaceSprite;
        }

        [SerializeField] private SpriteRenderer _faceSpriteRenderer = null;

        [Header("FACES")]
        [SerializeField] private System.Collections.Generic.List<FaceByDirection> _normalFaces = new();
        [SerializeField] private System.Collections.Generic.List<FaceByDirection> _angryFaces = new();
        [SerializeField] private Sprite _hurtFace = null;

        [Header("LOOK AT")]
        [SerializeField] private float _lookAtOffset = 0.04f;

        [Header("IDLE")]
        [SerializeField] private float _idleRotationSpeed = 3f;

        private System.Collections.Generic.Dictionary<Vector2, Sprite> _normalFacesByDirection = new();
        private System.Collections.Generic.Dictionary<Vector2, Sprite> _angryFacesByDirection = new();

        private Vector3 _faceInitLocalPosition;
        private Vector3 _idleDirection = Vector3.right;
        private bool _useIdleFace = true;

        public bool IsVisible => _faceSpriteRenderer.isVisible;

        public void UseIdleFace()
        {
            _useIdleFace = true;
        }

        public void SetNormalFace(Vector2 direction)
        {
            _useIdleFace = false;
            _faceSpriteRenderer.sprite = _normalFacesByDirection[GetClosestDirection(direction)];
        }

        public void SetAngryFace(Vector2 direction)
        {
            _useIdleFace = false;
            _faceSpriteRenderer.sprite = _angryFacesByDirection[GetClosestDirection(direction)];
        }

        public void SetHurtFace()
        {
            _useIdleFace = false;
            _faceSpriteRenderer.sprite = _hurtFace;
        }

        public void LookAt(Vector2 direction)
        {
            _faceSpriteRenderer.transform.localPosition = direction == Vector2.zero
                                                        ? _faceInitLocalPosition
                                                        : _faceInitLocalPosition + GetClosestDirection(direction) * _lookAtOffset;
        }

        private Vector3 GetClosestDirection(Vector2 direction)
        {
            Vector3 closestDirection = Vector3.zero;
            float closestAngle = Mathf.Infinity;

            for (int i = _lookAtDirections.Length - 1; i >= 0; --i)
            {
                float angle = Vector3.Angle(_lookAtDirections[i], direction);
                if (angle < closestAngle)
                {
                    closestAngle = angle;
                    closestDirection = _lookAtDirections[i];
                }
            }

            return closestDirection;
        }

        private void Awake()
        {
            _faceInitLocalPosition = _faceSpriteRenderer.transform.localPosition;

            _normalFaces.ForEach(o => _normalFacesByDirection.Add(o.Direction, o.FaceSprite));
            _angryFaces.ForEach(o => _angryFacesByDirection.Add(o.Direction, o.FaceSprite));
        }

        private void Update()
        {
            if (_useIdleFace)
            {
                _idleDirection = new Vector2(Mathf.Cos(Time.time * _idleRotationSpeed), Mathf.Sin(Time.time * _idleRotationSpeed));
                SetNormalFace(GetClosestDirection(_idleDirection));
            }
        }

        private void Reset()
        {
            _normalFaces = new();
            _angryFaces = new();

            for (int i = _lookAtDirections.Length - 1; i >= 0; --i)
            {
                _normalFaces.Add(new FaceByDirection() { Direction = _lookAtDirections[i] });
                _angryFaces.Add(new FaceByDirection() { Direction = _lookAtDirections[i] });
            }
        }
    }
}