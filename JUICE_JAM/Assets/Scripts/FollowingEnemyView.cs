using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingEnemyView : MonoBehaviour
{
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
        _faceSpriteRenderer.transform.localPosition = _faceInitLocalPosition + (Vector3)direction.normalized * _lookAtOffset;
    }

    private void Awake()
    {
        _faceInitLocalPosition = _faceSpriteRenderer.transform.localPosition;
    }
}