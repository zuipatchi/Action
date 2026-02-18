using System.Collections.Generic;
using UnityEngine;
using VContainer;
using Main.Player.Action.Model;

namespace Main.Player.Action.Dash
{

    public sealed class DashAfterImageMesh : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private Transform _meshRoot;               // SkinnedMeshRenderer群のルート（未指定なら自分）
        [SerializeField] private Material _ghostMaterial;

        [Header("Spawn")]
        private float _spawnInterval = 0.04f;
        private float _lifeTime = 0.3f;
        private int _poolSize = 16;

        private PlayerModel _playerModel;

        private float _timer;
        private readonly List<SkinnedMeshRenderer> _skinned = new();
        private GhostPool _pool;

        [Inject]
        public void Construct(PlayerModel playerModel)
        {
            _playerModel = playerModel;
        }

        private void Awake()
        {
            if (_meshRoot == null) _meshRoot = transform;

            _meshRoot.GetComponentsInChildren(true, _skinned);

            _pool = new GhostPool(_ghostMaterial, _poolSize);
        }

        private void Update()
        {
            if (!_playerModel.IsDashing) { _timer = 0f; return; }

            _timer -= Time.deltaTime;
            if (_timer > 0f) return;

            _timer = _spawnInterval;
            SpawnGhosts();
        }

        private void SpawnGhosts()
        {
            for (int i = 0; i < _skinned.Count; i++)
            {
                var smr = _skinned[i];
                if (smr == null || !smr.enabled) continue;

                // 表示されてない/メッシュが無い等はスキップ
                if (smr.sharedMesh == null) continue;

                var ghost = _pool.Rent();
                ghost.SetupFromSkinned(smr, _lifeTime);
            }
        }

        // -------------------------
        // Pool + Ghost
        // -------------------------
        private sealed class GhostPool
        {
            private readonly Material _mat;
            private readonly Queue<Ghost> _pool = new();
            private readonly Queue<Mesh> _meshPool = new(); // Bake用 Mesh の再利用

            public GhostPool(Material mat, int size)
            {
                _mat = mat;

                for (int i = 0; i < size; i++)
                {
                    _meshPool.Enqueue(NewMesh());
                    _pool.Enqueue(Create());
                }
            }

            public Ghost Rent() => _pool.Count > 0 ? _pool.Dequeue() : Create();

            public void Return(Ghost g) => _pool.Enqueue(g);

            public Mesh RentMesh() => _meshPool.Count > 0 ? _meshPool.Dequeue() : NewMesh();

            public void ReturnMesh(Mesh m)
            {
                // 念のため中身をクリア（参照が残ってるとデバッグしづらい）
                if (m != null) m.Clear();
                _meshPool.Enqueue(m);
            }

            private static Mesh NewMesh()
            {
                var m = new Mesh
                {
                    name = "GhostBakedMesh"
                };
                // 16bit index だと頂点数で詰むことがあるので保険（必要なら）
                // m.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
                return m;
            }

            private Ghost Create()
            {
                var go = new GameObject("GhostSkinnedMesh");
                go.SetActive(false);

                var mf = go.AddComponent<MeshFilter>();
                var mr = go.AddComponent<MeshRenderer>();
                mr.sharedMaterial = _mat;

                var ghost = go.AddComponent<Ghost>();
                ghost.Init(this, mf, mr);
                return ghost;
            }
        }

        private sealed class Ghost : MonoBehaviour
        {
            private GhostPool _owner;
            private MeshFilter _mf;
            private MeshRenderer _mr;

            private float _t;
            private float _life;

            private Mesh _bakedMesh; // そのフレームの焼きメッシュ（プールから借りる）
            private MaterialPropertyBlock _mpb;

            public void Init(GhostPool owner, MeshFilter mf, MeshRenderer mr)
            {
                _owner = owner;
                _mf = mf;
                _mr = mr;
            }

            public void SetupFromSkinned(SkinnedMeshRenderer src, float lifeTime)
            {
                gameObject.SetActive(true);

                _life = Mathf.Max(0.0001f, lifeTime);
                _t = 0f;

                // 焼きメッシュを借りる（毎回 new Mesh しない！）
                _bakedMesh ??= _owner.RentMesh();

                // 重要：SkinnedMeshRenderer の現在の変形結果を焼く
                // 旧Unityだと BakeMesh(mesh) のみ、新しめだと BakeMesh(mesh, true) 等あります
                src.BakeMesh(_bakedMesh);

                _mf.sharedMesh = _bakedMesh;

                // Transform 同期（SkinnedMeshRenderer の transform を基準）
                var tr = transform;
                var sTr = src.transform;
                tr.SetPositionAndRotation(sTr.position, sTr.rotation);
                tr.localScale = sTr.lossyScale;

                SetAlpha(1f);

                _mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                _mr.receiveShadows = false;
            }

            private void Update()
            {
                _t += Time.deltaTime;
                var a = 1f - Mathf.Clamp01(_t / _life);
                SetAlpha(a);

                if (_t < _life) return;

                gameObject.SetActive(false);

                // Mesh をプールに返す
                if (_bakedMesh != null)
                {
                    _owner.ReturnMesh(_bakedMesh);
                    _bakedMesh = null;
                    _mf.sharedMesh = null;
                }

                _owner.Return(this);
            }

            private void SetAlpha(float a)
            {
                _mpb ??= new MaterialPropertyBlock();

                _mr.GetPropertyBlock(_mpb);

                if (_mr.sharedMaterial != null && _mr.sharedMaterial.HasProperty("_BaseColor"))
                {
                    var c = _mr.sharedMaterial.GetColor("_BaseColor");
                    c.a = a;
                    _mpb.SetColor("_BaseColor", c);
                }
                else
                {
                    var c = _mr.sharedMaterial != null ? _mr.sharedMaterial.color : Color.white;
                    c.a = a;
                    _mpb.SetColor("_Color", c);
                }

                _mr.SetPropertyBlock(_mpb);
            }
        }
    }
}
