namespace JuiceJam
{
    using UnityEngine;

    public class DestroyableTilemap : MonoBehaviour, IDamageable
    {
        [System.Serializable]
        public struct TileDestroyedFeedback
        {
            public GameObject ObjectToInstantiate;
            public Vector2 TileCenterOffset;
        }

        [SerializeField] private UnityEngine.Tilemaps.Tilemap _tilemap = null;
        [SerializeField] private TileDestroyedFeedback[] _tileDestroyedCenterFeedback = null;
        [SerializeField] private GameObject[] _tileDestroyedContactPointFeedback = null;
        [SerializeField] private float _tilesAboveDestroyRate = 0.1f;

        public bool CanBeDamaged => true;
        public bool DontDestroyDamageSource => false;

        public void TakeDamage(DamageData damageData)
        {
            Vector2[] checkDirections = new Vector2[]
            {
                damageData.HitDirection,
                new Vector2(damageData.HitDirection.x, -damageData.HitDirection.y),
                new Vector2(damageData.HitDirection.y, damageData.HitDirection.x),
                new Vector2(-damageData.HitDirection.y, damageData.HitDirection.x),
                new Vector2(damageData.HitDirection.y, -damageData.HitDirection.x)
            };

            for (int i = checkDirections.Length - 1; i >= 0; --i)
            {
                Vector3Int tilePosition = _tilemap.WorldToCell(damageData.HitPoint + checkDirections[i].normalized * 0.2f);
                if (!_tilemap.HasTile(tilePosition))
                    continue;

                DestroyTile(tilePosition, damageData.HitPoint);
                StartCoroutine(DestroyTilesAbove(tilePosition, 10));

                break;
            }
        }

        private void DestroyTile(Vector3Int tilePosition, Vector3 hitPosition)
        {
            if (!_tilemap.HasTile(tilePosition))
                return;

            _tilemap.SetTile(tilePosition, null);

            Vector3 tileWorldCenter = _tilemap.CellToWorld(tilePosition);

            for (int i = _tileDestroyedCenterFeedback.Length - 1; i >= 0; --i)
            {
                Instantiate(_tileDestroyedCenterFeedback[i].ObjectToInstantiate,
                    tileWorldCenter + new Vector3(_tileDestroyedCenterFeedback[i].TileCenterOffset.y, _tileDestroyedCenterFeedback[i].TileCenterOffset.x),
                    _tileDestroyedCenterFeedback[i].ObjectToInstantiate.transform.rotation);
            }

            for (int i = _tileDestroyedContactPointFeedback.Length - 1; i >= 0; --i)
            {
                Instantiate(_tileDestroyedContactPointFeedback[i],
                    hitPosition,
                    _tileDestroyedContactPointFeedback[i].transform.rotation);
            }
        }

        private System.Collections.IEnumerator DestroyTilesAbove(Vector3Int startPosition, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_tilesAboveDestroyRate);

                Vector3Int tilePosition = startPosition + new Vector3Int(0, i + 1);
                DestroyTile(tilePosition, _tilemap.CellToWorld(tilePosition) + Vector3.one * 0.5f);
            }
        }
    }
}