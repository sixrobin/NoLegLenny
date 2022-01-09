namespace JuiceJam
{
    using UnityEngine;

    public class CloudsEnd : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer[] _playerRelatedSprites = null;
        [SerializeField] private Material _cloudedMaterial = null;
        
        [Header("VFX")]
        [SerializeField] private GameObject _transitionParticles = null;
        [SerializeField] private float _transitionTrauma = 0f;

        private System.Collections.Generic.Dictionary<SpriteRenderer, Material> _playerBaseMaterials = new();

        private void SpawnTransitionParticles(Vector3 position)
        {
            GameObject transitionParticles = Instantiate(_transitionParticles, position, _transitionParticles.transform.rotation);
            Destroy(transitionParticles, 5f);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out PlayerController playerController) || _playerBaseMaterials.Count > 0)
                return;

            for (int i = _playerRelatedSprites.Length - 1; i >= 0; --i)
            {
                _playerBaseMaterials.Add(_playerRelatedSprites[i], _playerRelatedSprites[i].material);
                _playerRelatedSprites[i].material = _cloudedMaterial;
            }

            playerController.IsClouded = true;

            SpawnTransitionParticles(collision.gameObject.transform.position);
            CameraShake.AddTrauma(_transitionTrauma);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.TryGetComponent(out PlayerController playerController))
                return;

            for (int i = _playerRelatedSprites.Length - 1; i >= 0; --i)
                _playerRelatedSprites[i].material = _playerBaseMaterials[_playerRelatedSprites[i]];

            _playerBaseMaterials.Clear();
            playerController.IsClouded = false;

            SpawnTransitionParticles(collision.gameObject.transform.position);
            CameraShake.AddTrauma(_transitionTrauma);
        }
    }
}
