using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class PlankBehavior : MonoBehaviour, IClickableObject
    {
        [SerializeField] SpriteRenderer spriteRenderer;
        public SpriteRenderer SpriteRenderer => spriteRenderer;

        [SerializeField] ColliderType colliderType;
        [SerializeField, HideIf("IsColliderTypePolygon")] List<Collider2D> colliders;
        [SerializeField, ShowIf("IsColliderTypePolygon")] PolygonCollider2D polygonCollider;

        public Vector3 Position => transform.position;
        public int SortingOrder => spriteRenderer.sortingOrder;
        public Color Color => spriteRenderer.color;

        public bool IsBeingDisabled { get; private set; }

        private Rigidbody2D rb;
        public Rigidbody2D Rigidbody
        {
            get
            {
                if (rb == null)
                    rb = GetComponent<Rigidbody2D>();
                return rb;
            }
        }

        private List<PlankHoleBehavior> holes;
        public List<PlankHoleBehavior> Holes => holes;

        private PlankLevelData data;
        public int Layer => data.PlankLayer;

        private TweenCase disableCase;

        private void Awake()
        {
            DisableColliders();
            StopSimulation();
        }

        public void Init(PlankLevelData data, Color color, int id)
        {
            this.data = data;

            transform.position = data.Position.SetZ(-0.01f * (data.PlankLayer + 1));
            transform.eulerAngles = data.Rotation;
            transform.localScale = data.Scale;
            spriteRenderer.color = color;
            spriteRenderer.sortingOrder = id * 2;

            disableCase.KillActive();
            IsBeingDisabled = false;

            rb.totalTorque = 0;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.totalForce = Vector2.zero;
        }

        public void SetHoles(List<Vector3> holePostions, SkinsManager skinsManager)
        {
            holes = new List<PlankHoleBehavior>();
            for (int i = 0; i < holePostions.Count; i++)
            {
                Vector3 holePosition = holePostions[i];
                holePosition.z = -0.005f;

                PlankHoleBehavior hole = skinsManager.GetPlankHole();
                hole.transform.SetParent(transform);
                hole.transform.localPosition = holePosition.Divide(transform.localScale);

                hole.Init(this);

                holes.Add(hole);
            }
        }

        public void EnableColliders()
        {
            if (colliderType == ColliderType.Primitive)
            {
                for (int i = 0; i < colliders.Count; i++)
                {
                    colliders[i].enabled = true;
                }
            } else
            {
                polygonCollider.enabled = true;
            }
        }

        public void DisableColliders()
        {
            if (colliderType == ColliderType.Primitive)
            {
                for (int i = 0; i < colliders.Count; i++)
                {
                    colliders[i].enabled = false;
                }
            }
            else
            {
                polygonCollider.enabled = false;
            }

            Rigidbody.totalTorque = 0;
            Rigidbody.velocity = Vector2.zero;
            Rigidbody.angularVelocity = 0;
            Rigidbody.totalForce = Vector2.zero;
        }

        public void StopSimulation()
        {
            Rigidbody.simulated = false;
        }

        public void StartSimulation()
        {
            Rigidbody.simulated = true;
        }

        public void IgnorePlank(PlankBehavior plankToIgnore)
        {
            if (plankToIgnore.colliderType == ColliderType.Primitive)
            {
                for (int i = 0; i < plankToIgnore.colliders.Count; i++)
                {
                    IgnoreCollider(plankToIgnore.colliders[i]);
                }
            }
            else
            {
                IgnoreCollider(plankToIgnore.polygonCollider);
            }
        }

        public void CollideWithPlank(PlankBehavior plankToIgnore)
        {
            if (plankToIgnore.colliderType == ColliderType.Primitive)
            {
                for (int i = 0; i < plankToIgnore.colliders.Count; i++)
                {
                    StopIgnoringCollider(plankToIgnore.colliders[i]);
                }
            }
            else
            {
                StopIgnoringCollider(plankToIgnore.polygonCollider);
            }
        }

        public void IgnoreCollider(Collider2D collider)
        {
            if (colliderType == ColliderType.Primitive)
            {
                for (int i = 0; i < colliders.Count; i++)
                {
                    Physics2D.IgnoreCollision(collider, colliders[i], true);
                }
            }
            else
            {
                Physics2D.IgnoreCollision(collider, polygonCollider, true);
            }
        }

        public void StopIgnoringCollider(Collider2D collider)
        {
            if (colliderType == ColliderType.Primitive)
            {
                for (int i = 0; i < colliders.Count; i++)
                {
                    Physics2D.IgnoreCollision(collider, colliders[i], false);
                }
            }
            else
            {
                Physics2D.IgnoreCollision(collider, polygonCollider, false);
            }
        }

        public void OnHoleAboutToBeSnapped(PlankHoleBehavior hole, Vector3 desiredHolePosition)
        {
            if (Time.fixedTime < 0.1f) return;

            PlankHoleBehavior activeHole = null;
            for (int i = 0; i < holes.Count; i++)
            {
                PlankHoleBehavior plankHole = holes[i];

                if (plankHole.IsActive)
                {
                    if (activeHole != null) return;
                    activeHole = plankHole;
                }
            }

            if (activeHole == null)
            {
                return;
            }

            float step = 0.5f;
            float distance = Vector2.Distance(desiredHolePosition, hole.Position);
            if (distance < 0.05f) return;

            while (Mathf.Abs(step) > 0.02f)
            {
                transform.RotateAround(activeHole.transform.position, Vector3.forward, step);

                float newDistance = Vector2.Distance(desiredHolePosition, hole.Position);

                if (newDistance > distance)
                {
                    step /= -2f;
                }

                distance = newDistance;
            }
        }

        public void OnHoleUnscrewed()
        {
            int counter = 0;
            for (int i = 0; i < holes.Count; i++)
            {
                PlankHoleBehavior plankHole = holes[i];

                if (plankHole.IsActive)
                {
                    counter++;
                }
            }

            Rigidbody.isKinematic = counter > 1;

            Tween.NextFrame(OnHoleUnscrewedNextFrame);
        }

        private void OnHoleUnscrewedNextFrame()
        {
            int counter = 0;

            for (int i = 0; i < holes.Count; i++)
            {
                PlankHoleBehavior plankHole = holes[i];

                if (plankHole.IsActive)
                {
                    counter++;
                }
            }

            if (counter == 1)
            {
                float sign = (Random.Range(0, 2) * 2 - 1);
                Rigidbody.AddTorque(100f * sign, ForceMode2D.Force);
            }
        }

        public void OnObjectClicked(Vector3 clickPosition)
        {

        }

        public void DestroyPlank()
        {
            LevelController.UpdateDestroyedPlanks();
            Discard(false);
        }

        public void Disable()
        {
            IsBeingDisabled = true;
            LevelController.UpdateDestroyedPlanks();
            disableCase = Tween.DelayedCall(0.3f, () =>
            {
                Discard(true);

                AudioController.PlaySound(AudioController.AudioClips.plankComplete);
            });
        }

        public void Discard(bool withParticle)
        {
            if (withParticle)
            {
                ParticleCase particleCase = ParticlesController.PlayParticle("Confetti");

                Transform particleTransform = particleCase.ParticleSystem.transform;
                particleTransform.position = transform.position;
                particleTransform.rotation = Quaternion.FromToRotation(Vector3.up, (Vector3.up * 10 - transform.position.SetZ(0)).normalized);
            }

            for (int i = 0; i < holes.Count; i++)
            {
                holes[i].Discard();
            }

            holes.Clear();

            data = null;

            gameObject.SetActive(false);
            DisableColliders();
            StopSimulation();

            transform.position = Vector3.zero;

            disableCase.KillActive();

            Tween.NextFrame(() => IsBeingDisabled = false);
        }

        public bool IsFlying()
        {
            for (int i = 0; i < holes.Count; i++)
            {
                PlankHoleBehavior hole = holes[i];

                if (hole.IsActive) return false;
            }

            return true;
        }

        public bool DoesPointOverlapsCollider(Vector3 point)
        {
            if (colliderType == ColliderType.Primitive)
            {
                bool result = false;

                for (int i = 0; i < colliders.Count; i++)
                {
                    result = colliders[i].OverlapPoint(point);

                    if (result)
                        return true;
                }

                return false;
            }
            else
            {
                return polygonCollider.OverlapPoint(point);
            }
        }

        protected bool IsColliderTypePolygon() => colliderType == ColliderType.Polygon;

        private enum ColliderType
        {
            Primitive = 0,
            Polygon = 1,
        }
    }
}