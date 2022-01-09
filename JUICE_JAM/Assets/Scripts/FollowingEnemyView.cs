using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingEnemyView : MonoBehaviour
{
    private static readonly Vector3[] _lookAtDirections = new Vector3[]
    {
        Vector3.up,
        Vector3.right,
        Vector3.down,
        Vector3.left,
        Vector3.up + Vector3.right,
        Vector3.down + Vector3.right,
        Vector3.up + Vector3.left,
        Vector3.down + Vector3.left
    };

    [SerializeField] private SpriteRenderer _faceSpriteRenderer = null;

    [Header("FACES")]
    [SerializeField] private Sprite _normalFace = null;
    [SerializeField] private Sprite _angryFace = null;
    [SerializeField] private Sprite _hurtFace = null;

    [Header("LOOK AT")]
    [SerializeField] private float _lookAtOffset = 0.04f;

    private Vector3 _faceInitLocalPosition;

    public void SetNormalFace()
    {
        _faceSpriteRenderer.sprite = _normalFace;
    }

    public void SetAngryFace()
    {
        _faceSpriteRenderer.sprite = _angryFace;
    }

    public void SetHurtFace()
    {
        _faceSpriteRenderer.sprite = _hurtFace;
    }

    public void LookAt(Vector2 direction)
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

        _faceSpriteRenderer.transform.localPosition = _faceInitLocalPosition + closestDirection * _lookAtOffset;
    }

    private void Awake()
    {
        _faceInitLocalPosition = _faceSpriteRenderer.transform.localPosition;
    }
}