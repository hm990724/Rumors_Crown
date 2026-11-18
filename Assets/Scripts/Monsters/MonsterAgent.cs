using UnityEngine;
using Rumors.Core;

namespace Rumors.Monsters
{
    /// <summary>
    /// Unity MonoBehaviour that moves a monster on the world map.
    /// Uses legacy Input API (activeInputHandler = Both).
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class MonsterAgent : MonoBehaviour
    {
        public MonsterData Data { get; private set; }
        public bool IsAtWork => _atWork;

        private SpriteRenderer   _sr;
        private Vector3          _target;
        private float            _moveSpeed  = 1.4f;
        private float            _wanderCD;
        private float            _animTimer;
        private bool             _facingLeft;
        private Vector3          _baseScale;

        // Work state
        private float _workTimer;
        private bool  _atWork;

        public void Init(MonsterData data, Vector3 pos)
        {
            Data   = data;
            _sr    = GetComponent<SpriteRenderer>();
            _sr.sortingOrder = 20;
            if (SpriteRegistry.DefaultMaterial != null)
                _sr.material = SpriteRegistry.DefaultMaterial;
            _sr.sprite = SpriteRegistry.GetMonster(data.Species);
            if (_sr.sprite != null && _sr.sprite.texture != null)
                _sr.sprite.texture.filterMode = FilterMode.Point;

            transform.position = pos;
            transform.localScale = new Vector3(0.85f, 0.85f, 1f);
            _target    = pos;
            _baseScale = transform.localScale;

            // collider for click detection
            var col = gameObject.AddComponent<BoxCollider2D>();
            col.size     = new Vector2(0.9f, 0.9f);
            col.isTrigger = true;

            _wanderCD = Random.Range(1f, 3f);
        }

        private void Update()
        {
            _wanderCD -= Time.deltaTime;
            if (_wanderCD <= 0f)
            {
                _wanderCD = Random.Range(2f, 5f);
                PickTarget();
            }

            MoveToTarget();
            AnimateProcedural();
        }

        private void PickTarget()
        {
            Vector3 center;
            float   radius;

            if (Data.AssignedBuildingPos.HasValue && Rumors.World.WorldGrid.Instance != null)
            {
                center = Rumors.World.WorldGrid.Instance.GridToWorld(Data.AssignedBuildingPos.Value);
                radius = 0.7f;
                _atWork = true;
            }
            else
            {
                center  = Rumors.World.WorldGrid.Instance != null
                        ? Rumors.World.WorldGrid.Instance.GridToWorld(Rumors.World.WorldGrid.CastleGridPos)
                        : Vector3.zero;
                radius  = 2.5f;
                _atWork = false;
            }

            _target = center + new Vector3(
                Random.Range(-radius, radius),
                Random.Range(-radius, radius), 0f);
        }

        private void MoveToTarget()
        {
            float dist = Vector3.Distance(transform.position, _target);
            if (dist < 0.06f)
            {
                HandleStationary();
                return;
            }

            Vector3 dir = (_target - transform.position).normalized;
            transform.position = Vector3.MoveTowards(transform.position, _target, _moveSpeed * Time.deltaTime);

            if (Mathf.Abs(dir.x) > 0.05f)
            {
                _facingLeft = dir.x < 0;
                _sr.flipX   = _facingLeft;
            }
        }

        private void HandleStationary()
        {
            if (Data.CurrentJob == JobType.Training)
            {
                _workTimer += Time.deltaTime;
                if (_workTimer >= 2f)
                {
                    _workTimer = 0f;
                    StartCoroutine(LungeCR());
                }
            }
        }

        private void AnimateProcedural()
        {
            _animTimer += Time.deltaTime;
            float dist = Vector3.Distance(transform.position, _target);

            if (_sr != null && Data != null)
            {
                if (dist > 0.08f)
                {
                    // Walk sequence (4 frames)
                    int frame = (int)(_animTimer * 6f) % 4;
                    _sr.sprite = SpriteRegistry.GetMonsterFrame(Data.Species, "Walk", frame);

                    // Walk bounce
                    float sy = 1f + Mathf.Abs(Mathf.Sin(_animTimer * 10f)) * 0.08f;
                    transform.localScale = new Vector3(_baseScale.x, _baseScale.y * sy, 1f);
                }
                else
                {
                    // Idle sequence (2 frames)
                    int frame = (int)(_animTimer * 2f) % 2;
                    _sr.sprite = SpriteRegistry.GetMonsterFrame(Data.Species, "Idle", frame);

                    // Idle breathe
                    float sy = 1f + Mathf.Sin(_animTimer * 3f) * 0.04f;
                    transform.localScale = new Vector3(_baseScale.x, _baseScale.y * sy, 1f);
                }
            }
        }

        private System.Collections.IEnumerator LungeCR()
        {
            var orig = transform.position;
            var dir  = (_facingLeft ? Vector3.left : Vector3.right) * 0.35f;
            if (_sr != null && Data != null)
                _sr.sprite = SpriteRegistry.GetMonsterFrame(Data.Species, "Attack", 1);

            float t  = 0f;
            while (t < 0.1f)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(orig, orig + dir, t / 0.1f);
                yield return null;
            }
            t = 0f;
            while (t < 0.15f)
            {
                t += Time.deltaTime;
                transform.position = Vector3.Lerp(orig + dir, orig, t / 0.15f);
                yield return null;
            }
            transform.position = orig;
        }

        public void Flash(Color c)
        {
            if (_sr) StartCoroutine(FlashCR(c));
        }

        private System.Collections.IEnumerator FlashCR(Color c)
        {
            var orig = _sr.color;
            _sr.color = c;
            yield return new WaitForSeconds(0.12f);
            if (_sr) _sr.color = orig;
        }
    }
}
