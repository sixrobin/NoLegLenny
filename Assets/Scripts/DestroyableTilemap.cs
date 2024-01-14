namespace JuiceJam
{
    using RSLib.Extensions;
    using UnityEngine;

    public class DestroyableTilemap : MonoBehaviour, IDamageable, IExplodable, IRespawnable
    {
        [System.Serializable]
        public struct TileDestroyedFeedback
        {
            public GameObject ObjectToInstantiate;
            public Vector2 TileCenterOffset;
        }

        [Header("REFERENCES")]
        [SerializeField] private UnityEngine.Tilemaps.Tilemap _tilemap = null;
        [SerializeField] private TileDestroyedFeedback[] _tileDestroyedCenterFeedback = null;
        [SerializeField] private GameObject[] _tileDestroyedContactPointFeedback = null;
        
        [Header("SETTINGS")]
        [SerializeField, Min(0f)] private float _tilesAboveDestroyRate = 0.1f;
        [SerializeField, Min(0f)] private float _tilesExplosionDestructionDelay = 0.1f;
        [SerializeField, Min(0f)] private float _tilesExplosionDestructionRate = 0.08f;

        [Header("AUDIO")]
        [SerializeField] private RSLib.Audio.ClipProvider _destroyedTileClip = null;

        private readonly System.Collections.Generic.Dictionary<Vector3Int, UnityEngine.Tilemaps.TileBase> _destroyedTiles = new System.Collections.Generic.Dictionary<Vector3Int, UnityEngine.Tilemaps.TileBase>();

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
                StartCoroutine(DestroyTilesAbove(tilePosition, -1));

                break;
            }
        }

        public void Explode(ExplosionData explosionData)
        {
            int radius = Mathf.FloorToInt(explosionData.Radius);
            StartCoroutine(ExplodeTiles(_tilemap.WorldToCell(explosionData.Source), radius));
        }

        public void Respawn()
        {
            _destroyedTiles.ForEach((k, v) => _tilemap.SetTile(k, v));
            _destroyedTiles.Clear();
        }

        private void DestroyTile(Vector3Int tilePosition, Vector3 hitPosition)
        {
            if (!_tilemap.HasTile(tilePosition))
                return;

            _destroyedTiles.Add(tilePosition, _tilemap.GetTile(tilePosition));
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

            RSLib.Audio.AudioManager.PlayNextPlaylistSound(_destroyedTileClip);
        }

        private System.Collections.IEnumerator DestroyTilesAbove(Vector3Int startPosition, int count)
        {
            for (int i = 0; i < count || count == -1; ++i)
            {
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_tilesAboveDestroyRate);

                Vector3Int tilePosition = startPosition + new Vector3Int(0, i + 1);
                if (!_tilemap.HasTile(tilePosition))
                    yield break;

                DestroyTile(tilePosition, _tilemap.CellToWorld(tilePosition) + Vector3.one * 0.5f);
            }
        }

        private System.Collections.IEnumerator ExplodeTiles(Vector3Int startPosition, int radius)
        {
            yield return RSLib.Yield.SharedYields.WaitForSeconds(_tilesExplosionDestructionDelay);

            for (int i = 0; i < radius; ++i)
            {
                yield return RSLib.Yield.SharedYields.WaitForSeconds(_tilesExplosionDestructionRate);

                for (int x = i; x >= -i; --x)
                {
                    for (int y = i; y >= -i; --y)
                    {
                        Vector3Int tilePosition = _tilemap.LocalToCell(startPosition + new Vector3(x, y));
                        if (!_tilemap.HasTile(tilePosition))
                            continue;

                        DestroyTile(tilePosition, tilePosition);
                        if (i == radius - 1)
                            StartCoroutine(DestroyTilesAbove(tilePosition, -1));
                    }
                }
            }
        }
    }
}