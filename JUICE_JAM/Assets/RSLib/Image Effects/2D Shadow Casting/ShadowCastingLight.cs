namespace RSLib.ImageEffects
{
	using System;
	using UnityEngine;

	[ExecuteInEditMode]
    [DisallowMultipleComponent]
    [AddComponentMenu("RSLib/Image Effects/2D Shadow Casting Light")]
	public class ShadowCastingLight : MonoBehaviour
    {
        private struct ObstacleDatas
		{
			public readonly Vector2[] Vertices;
			public readonly float Precision;

			public ObstacleDatas(Vector2[] vertices, float precision)
			{
				Vertices = vertices;
				Precision = precision;
			}
		}

        private struct AngledVertex
		{
			public Vector3 Vertex;
			public Vector2 UV;
			public float Angle;
		}

		private enum CircleResolution : int
		{
			[InspectorName("4")] Four = 4,
			[InspectorName("8")] Eight = 8,
			[InspectorName("16")] Sixteen = 16,
			[InspectorName("32")] ThirtyTwo = 32
		}

		[Header("DETECTION")]
		[SerializeField] private LayerMask _obstaclesMask = 0;

		[Tooltip("Colliders to consider, whatever their start range from the light.")]
		[SerializeField] private Collider2D[] _autoConsidered = null;

		[Header("SETTINGS")]
		[SerializeField, Range(1f, 100f)] private float _range = 20;
		[SerializeField, Range(0.001f, 0.1f)] private float _boxPrecision = 0.001f;
		[SerializeField, Range(0.001f, 0.1f)] private float _circlePrecision = 0.01f;

		[Tooltip ("Circles that are close to the light will require higher resolution.")]
		[SerializeField] private CircleResolution _circlesResolution = CircleResolution.Sixteen;

        [Header("DEBUG")]
        [SerializeField] private bool _showRange = true;
        [SerializeField] private bool _showCircleResolution = false;
        [SerializeField] private bool _showRays = false;
        [SerializeField] private bool _showDiagonals = false;

        private Collider2D[] _inRangeObstacles;
		private Transform _lightTransform;
		private Vector3 _lightPosition;
		private Mesh _mesh;
		private ObstacleDatas[] _obstaclesDatas;
		private AngledVertex[] _angledVertices;
		private Vector3[] _vertices;
        private Vector2[] _uvs;
        private Vector2[] _allCirclesPoints;

		public Collider2D[] AutoConsidered
        {
            get => _autoConsidered;
            set => _autoConsidered = value;
        }

		private T[] ConcatenateArrays<T>(T[] first, T[] second)
		{
			T[] concatted = new T[first.Length + second.Length];
			Array.Copy(first, concatted, first.Length);
			Array.Copy(second, 0, concatted, first.Length, second.Length);
			return concatted;
		}

        private ObstacleDatas GetObstacleDatas(Collider2D collider)
		{
			if (collider is BoxCollider2D box)
				return GetBoxDatas(box);
			else if (collider is CircleCollider2D circle)
				return GetCircleDatas(circle);
			else if (collider is CompositeCollider2D composite)
				return GetCompositeDatas(composite);

			return new ObstacleDatas();
		}

        private ObstacleDatas GetBoxDatas(BoxCollider2D box)
		{
			Vector2[] corners = new Vector2[4];

			Vector2 boxPosition = box.transform.position;
			Quaternion boxAngle = box.transform.localRotation;
			Vector2 boxScale = box.transform.localScale;
			boxPosition += new Vector2(box.offset.x * boxScale.x, box.offset.y * boxScale.y);
			Vector2 boxSize = box.size;

			Vector2 diagonal = new Vector2(boxSize.x * boxScale.x, boxSize.y * boxScale.y) * 0.5f;
			Vector2 angledDiagonal = boxAngle * diagonal;
			Vector2 angledOppositeDiagonal = boxAngle * new Vector2(-diagonal.x, diagonal.y);

			corners[0] = boxPosition + new Vector2(+angledDiagonal.x, +angledDiagonal.y);
			corners[1] = boxPosition + new Vector2(-angledDiagonal.x, -angledDiagonal.y);
			corners[2] = boxPosition + new Vector2(+angledOppositeDiagonal.x, +angledOppositeDiagonal.y);
			corners[3] = boxPosition + new Vector2(-angledOppositeDiagonal.x, -angledOppositeDiagonal.y);

			if (_showDiagonals)
				foreach (Vector2 diag in corners)
					Debug.DrawLine(boxPosition, diag, Color.blue);

			return new ObstacleDatas(corners, _boxPrecision);
		}

        private ObstacleDatas GetCircleDatas(CircleCollider2D circle)
		{
			Vector2[] points = new Vector2[(int)_circlesResolution / 2 + 1];

			Vector2 circlePosition = circle.transform.position;
			float circleRadius = circle.radius * circle.transform.localScale.x;

			for (int i = 0; i < points.Length; ++i)
			{
				float angle = i * (360 / (float)_circlesResolution) * Mathf.Deg2Rad;
				Vector2 circlePoint = new Vector3(circleRadius * Mathf.Sin(angle), circleRadius * Mathf.Cos(angle));

				Vector2 direction = (Vector2)_lightPosition - circlePosition;
				float theta = Vector2.Angle(new Vector2(1f, 0f), direction);
				theta *= direction.y > 0 ? 1 : -1;
				circlePoint = circlePosition + (Vector2)(Quaternion.Euler(0f, 0f, theta) * circlePoint);

				points[i] = circlePoint;
			}

			if (_showCircleResolution)
			{
				if (_allCirclesPoints == null)
					_allCirclesPoints = new Vector2[0];
				_allCirclesPoints = ConcatenateArrays(_allCirclesPoints, points);
			}

			return new ObstacleDatas(points, _circlePrecision);
		}

        private ObstacleDatas GetCompositeDatas(CompositeCollider2D composite)
		{
			Vector2[] points = new Vector2[composite.pointCount];

			for (int i = 0; i < composite.pathCount; ++i)
			{
				Vector2[] path = new Vector2[composite.GetPathPointCount (i)];
				composite.GetPath(i, path);
				points = ConcatenateArrays(points, path);
			}

			return new ObstacleDatas(points, _boxPrecision);
		}

        private void GetInRangeObstacles()
		{
			_inRangeObstacles = Physics2D.OverlapCircleAll(_lightTransform.position, _range, _obstaclesMask);
			_inRangeObstacles = ConcatenateArrays(_inRangeObstacles, AutoConsidered);

			if (_inRangeObstacles.Length == 0)
			{
				Debug.LogWarning ("ShadowsCastingLight WARNING: No obstacle found to the light, disabling component!", gameObject);
				enabled = false;
			}
		}

        private void Setup()
		{
			if (_showCircleResolution)
				_allCirclesPoints = null;

			_lightPosition = _lightTransform.position;

			_obstaclesDatas = new ObstacleDatas[_inRangeObstacles.Length];
			for (int i = 0; i < _inRangeObstacles.Length; i++)
				_obstaclesDatas[i] = GetObstacleDatas(_inRangeObstacles[i]);

			Vector2[] allVertices = _obstaclesDatas[0].Vertices;
			for (int i = 1; i < _inRangeObstacles.Length; ++i)
				allVertices = ConcatenateArrays(allVertices, _obstaclesDatas[i].Vertices);

			_angledVertices = new AngledVertex[allVertices.Length * 2];
			_vertices = new Vector3[_angledVertices.Length + 1];
			_uvs = new Vector2[_vertices.Length];

			_vertices[0] = _lightTransform.worldToLocalMatrix.MultiplyPoint3x4(_lightPosition);
			_uvs[0] = new Vector2(_vertices[0].x, _vertices[0].y);
		}

        private void RaycastAllVertices()
		{
			int h = 0;

			for (int i = 0; i < _obstaclesDatas.Length; ++i)
			{
				ObstacleDatas datas = _obstaclesDatas[i];

				for (int j = 0; j < datas.Vertices.Length; ++j)
				{
					Vector2 ray = new Vector2(datas.Vertices[j].x - _lightPosition.x, datas.Vertices[j].y - _lightPosition.y);
					Vector2 offset = new Vector2(-ray.y, ray.x) * datas.Precision;
					Vector2 rayLeft = ray + offset;
					Vector2 rayRight = ray - offset;

					float angle1 = Mathf.Atan2(rayLeft.y, rayLeft.x);
					float angle2 = Mathf.Atan2(rayRight.y, rayRight.x);

					RaycastHit2D hit1 = Physics2D.Raycast(_lightPosition, rayLeft, 100f, _obstaclesMask);
					RaycastHit2D hit2 = Physics2D.Raycast(_lightPosition, rayRight, 100f, _obstaclesMask);

					if (_showRays)
					{
						Debug.DrawLine(_lightTransform.position, hit1.point, Color.red);
						Debug.DrawLine(_lightTransform.position, hit2.point, Color.green);
					}

					_angledVertices[h * 2].Vertex = _lightTransform.worldToLocalMatrix.MultiplyPoint3x4(hit1.point);
					_angledVertices[h * 2].Angle = angle1;
					_angledVertices[h * 2].UV = new Vector2(_angledVertices[h * 2].Vertex.x, _angledVertices[h * 2].Vertex.y);

					_angledVertices[h * 2 + 1].Vertex = _lightTransform.worldToLocalMatrix.MultiplyPoint3x4(hit2.point);
					_angledVertices[h * 2 + 1].Angle = angle2;
					_angledVertices[h * 2 + 1].UV = new Vector2(_angledVertices[h * 2 + 1].Vertex.x, _angledVertices[h * 2 + 1].Vertex.y);

					h++;
				}
			}
		}

		private void ConstructLightMesh()
		{
			Array.Sort(_angledVertices, delegate(AngledVertex one, AngledVertex two)
			{
				return one.Angle.CompareTo(two.Angle);
			});

			for (int i = 0; i < _angledVertices.Length; ++i)
			{
				_vertices[i + 1] = _angledVertices[i].Vertex;
				_uvs[i + 1] = _angledVertices[i].UV;
			}

			for (int i = 0; i < _uvs.Length; ++i)
				_uvs[i] = new Vector2(_uvs[i].x + 0.5f, _uvs[i].y + 0.5f);

			int[] triangles = { 0, 1, _vertices.Length - 1 };
			for (int i = _vertices.Length - 1; i > 0; --i)
				triangles = ConcatenateArrays(triangles, new int[] { 0, i, i - 1 });

			_mesh.Clear();
			_mesh.vertices = _vertices;
			_mesh.triangles = triangles;
			_mesh.uv = _uvs;
		}

		private void Awake()
		{
			_lightTransform = transform;

#if UNITY_EDITOR
			MeshFilter meshFilter = GetComponentInChildren<MeshFilter>();
			Mesh meshCopy = Instantiate(meshFilter.sharedMesh);
			_mesh = meshCopy;
			meshFilter.mesh = meshCopy;
#else
			_mesh = GetComponentInChildren<MeshFilter>().mesh;
#endif

			GetInRangeObstacles();
		}

        private void Update()
		{
			Setup();
			RaycastAllVertices();
			ConstructLightMesh();
		}

        private void OnDrawGizmosSelected()
		{
			Gizmos.color = Color.yellow;
			if (_showRange)
				Gizmos.DrawWireSphere(transform.position, _range);

			Gizmos.color = Color.red;
			if (_showCircleResolution && _allCirclesPoints != null)
				foreach (Vector3 p in _allCirclesPoints)
					Gizmos.DrawSphere(p, 0.1f - 0.002f * (int)_circlesResolution);
		}
	}
}