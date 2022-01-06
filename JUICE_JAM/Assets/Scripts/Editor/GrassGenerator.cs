namespace JuiceJam.Editor
{
    using RSLib.Extensions;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Tilemaps;

    public sealed class GrassGeneratorMenu
    {
        [MenuItem("JuiceJam/Grass Generator")]
        public static void LaunchGrassGenerator()
        {
            GrassGeneratorEditor.LaunchGenerator();
        }
    }

    public sealed class GrassGeneratorEditor : EditorWindow
    {
        private Tilemap _groundTilemap;
        private Transform _grassParent;
        private GameObject _grass;

        private float _grassChance = 0.65f;
        private float _offsetMax = 0.36f;
        private int _grassPerTileMax = 1;

        public static void LaunchGenerator()
        {
            EditorWindow window = GetWindow<GrassGeneratorEditor>("Grass Generator");
            window.Show();
        }

        private void GenerateGrass()
        {
            Debug.Log($"Generating grass (chance:{_grassChance}, offset max:{_offsetMax}, max per tile:{_grassPerTileMax})");

            foreach (Vector3Int tilePosition in _groundTilemap.cellBounds.allPositionsWithin)
            {
                Vector3Int localPlace = new Vector3Int(tilePosition.x, tilePosition.y, tilePosition.z);

                if (!_groundTilemap.HasTile(localPlace))
                    continue;

                if (_groundTilemap.HasTile(localPlace + new Vector3Int(0, 1, 0)))
                    continue;

                if (Random.Range(0f, 1f) > _grassChance)
                    continue;

                int grassCount = Random.Range(1, _grassPerTileMax + 1);
                for (int i = 0; i < grassCount; ++i)
                {
                    Vector3 worldPosition = _groundTilemap.CellToWorld(localPlace)
                                            + new Vector3(0.5f, 1f)
                                            + new Vector3(Random.Range(-_offsetMax, _offsetMax), 0f);

                    Object grassInstance = PrefabUtility.InstantiatePrefab(_grass, _grassParent);
                    (grassInstance as GameObject).transform.position = worldPosition;
                }
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.BeginVertical();
            GUILayout.Space(10f);

            EditorGUILayout.LabelField("Ground Tilemap", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(5f);

            _groundTilemap = EditorGUILayout.ObjectField(_groundTilemap, typeof(Tilemap), true, null) as Tilemap;
            _grass = EditorGUILayout.ObjectField(_grass, typeof(GameObject), true, null) as GameObject;
            _grassParent = EditorGUILayout.ObjectField(_grassParent, typeof(Transform), true, null) as Transform;

            GUILayout.Space(5f);

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grass Chance per tile");
            _grassChance = EditorGUILayout.Slider(_grassChance, 0f, 1f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grass X Offset Max");
            _offsetMax = EditorGUILayout.Slider(_offsetMax, 0f, 0.5f);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grass per tile Max");
            _grassPerTileMax = EditorGUILayout.IntSlider(_grassPerTileMax, 1, 3);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5f);

            if (GUILayout.Button("Generate Grass", GUILayout.Height(45f), GUILayout.ExpandWidth(true)))
            {
                if (_groundTilemap == null)
                {
                    EditorUtility.DisplayDialog("Grass Generator Warning", "You must provide a ground Tilemap for grass positions!", "OK");
                    return;
                }

                if (_grass == null)
                {
                    EditorUtility.DisplayDialog("Grass Generator Warning", "You must provide a grass asset!", "OK");
                    return;
                }

                GenerateGrass();
            }

            if (GUILayout.Button("Clear Grass Parent", GUILayout.Height(45f), GUILayout.ExpandWidth(true)))
            {
                if (_grassParent == null)
                    return;

                _grassParent.DestroyImmediateChildren();
            }

            GUILayout.Space(2f);
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();
            GUILayout.Space(10f);
            EditorGUILayout.EndHorizontal();

            Repaint();
        }
    }
}