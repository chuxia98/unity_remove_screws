using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class ScrewBehavior : MonoBehaviour, IClickableObject
    {
        private static readonly int UNSCREW_TRIGGER = Animator.StringToHash("Unscrew");
        private static readonly int UNSCREW_PU_TRIGGER = Animator.StringToHash("Unscrew PU");
        private static readonly int SCREW_IN_TRIGGER = Animator.StringToHash("Screw In");

        private static readonly int UNSCREW_SPEED_MULTIPLIER_FLOAT = Animator.StringToHash("Unscrew Speed Multiplier");
        private static readonly int UNSCREW_PU_SPEED_MULTIPLIER_FLOAT = Animator.StringToHash("Unscrew PU Speed Multiplier");
        private static readonly int SCREW_IN_SPEED_MULTIPLIER_FLOAT = Animator.StringToHash("Screw In Speed Multiplier");

        private static readonly Vector3 DEFAULT_HIGHLIGHT_SIZE = Vector3.one;
        private const float HIGHLIGHT_BREATHING_SPEED = 1.2f;

        [SerializeField] Collider2D screwCollider;
        [SerializeField] SpriteRenderer shadowSpriteRenderer;
        [SerializeField] Transform visuals;

        [SerializeField] Animator animator;

        private List<HoleBehavior> connectedHoles = new List<HoleBehavior>();

        public Vector3 Position => transform.position;

        private static ScrewBehavior selectedScrew;
        public static ScrewBehavior SelectedScrew => selectedScrew;

        private bool isHighlighted;
        private TweenCase moveTweenCase;

        private float highlightBreathingState;

        private Vector3 cachedVisualsLocalPos;

        public event SimpleCallback Unscrewed;
        public event SimpleCallback Selected;
        public event SimpleCallback Deselected;

        private void Awake()
        {
            cachedVisualsLocalPos = visuals.localPosition;

            screwCollider.enabled = false;
        }

        public void Init(List<BaseHoleBehavior> baseHoles, List<PlankBehavior> planks)
        {
            isHighlighted = false;

            Unscrewed = null;
            Selected = null;
            Deselected = null;

            shadowSpriteRenderer.color = GameController.Data.ScrewShadowColor;
            shadowSpriteRenderer.transform.localScale = DEFAULT_HIGHLIGHT_SIZE;

            highlightBreathingState = 0;

            for (int i = 0; i < baseHoles.Count; i++)
            {
                BaseHoleBehavior hole = baseHoles[i];

                if (Vector2.Distance(hole.transform.position, transform.position) <= 0.1f)
                {
                    hole.SetActive(true);
                    connectedHoles.Add(hole);

                    break;
                }
            }

            for (int i = 0; i < planks.Count; i++)
            {
                PlankBehavior plank = planks[i];

                for (int j = 0; j < plank.Holes.Count; j++)
                {
                    PlankHoleBehavior plankHole = plank.Holes[j];

                    if (Vector2.Distance(plankHole.transform.position, transform.position) <= 0.1f)
                    {
                        connectedHoles.Add(plankHole);
                        plankHole.ActivateJoint(screwCollider, transform.position);

                        break;
                    }
                }
            }
        }

        public void EnableCollider()
        {
            screwCollider.enabled = true;
        }

        private void Update()
        {
            if (isHighlighted && GameController.Data.HighlightBreathingEffect)
            {
                highlightBreathingState = Mathf.PingPong(HIGHLIGHT_BREATHING_SPEED * Time.time, 1);

                shadowSpriteRenderer.color = Color.Lerp(GameController.Data.ScrewHighlightColor, GameController.Data.ScrewHighlightColor.SetAlpha(0.5f), highlightBreathingState);
            }
        }

        private void Unscrew()
        {
            for (int i = 0; i < connectedHoles.Count; i++)
            {
                if (connectedHoles[i] is PlankHoleBehavior plankHole)
                {
                    plankHole.DisableJoint(screwCollider, true);
                }
                else
                {
                    (connectedHoles[i] as BaseHoleBehavior).SetActive(false);
                }
            }

            Unscrewed?.Invoke();
        }

        public void UnscrewAndDiscard()
        {
            animator.SetFloat(UNSCREW_PU_SPEED_MULTIPLIER_FLOAT, GameController.Data.UnscrewPUAnimationSpeedMultiplier);
            animator.SetTrigger(UNSCREW_PU_TRIGGER);
        }

        public void OnUnscrewPUAnimEnded()
        {
            Unscrew();

            Discard();
        }

        public void ChangeHoles(List<HoleBehavior> newHoles)
        {
            Unscrew();

            connectedHoles = newHoles;

            var newPosition = newHoles[^1].transform.position.SetZ(transform.position.z);

            var difference = newPosition - transform.position;

            visuals.localPosition = visuals.localPosition - difference;

            transform.position = newPosition;

            for (int i = 0; i < connectedHoles.Count; i++)
            {
                if (connectedHoles[i] is PlankHoleBehavior plankHole)
                {
                    plankHole.ActivateJoint(screwCollider, newPosition);
                }
                else
                {
                    (connectedHoles[i] as BaseHoleBehavior).SetActive(true);
                }
            }

#if MODULE_HAPTIC
            Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif

            moveTweenCase.KillActive();
            moveTweenCase = visuals.DOLocalMove(cachedVisualsLocalPos, GameController.Data.ScrewMovementDuration).SetEasing(Ease.Type.SineInOut).OnComplete(() => {
                Deselect();
            });
        }

        public void OnObjectClicked(Vector3 clickPosition)
        {
            if (moveTweenCase.ExistsAndActive()) return;

            if (SelectedScrew != null && SelectedScrew == this)
            {
                Deselect();
            }
            else
            {
                Select();
            }
        }

        public void Select()
        {
            if (selectedScrew != null) selectedScrew.Deselect();

            selectedScrew = this;

            animator.SetFloat(UNSCREW_SPEED_MULTIPLIER_FLOAT, GameController.Data.UnscrewAnimationSpeedMultiplier);
            animator.SetTrigger(UNSCREW_TRIGGER);

            AudioController.PlaySound(AudioController.AudioClips.screwPick);

#if MODULE_HAPTIC
            Haptic.Play(Haptic.HAPTIC_LIGHT);
#endif

            Selected?.Invoke();
        }

        public void Deselect()
        {
            animator.SetFloat(SCREW_IN_SPEED_MULTIPLIER_FLOAT, GameController.Data.ScrewInAnimationSpeedMultiplier);
            animator.SetTrigger(SCREW_IN_TRIGGER);

            AudioController.PlaySound(AudioController.AudioClips.screwPlace);

            selectedScrew = null;

            Deselected?.Invoke();
        }

        public void Discard()
        {
            connectedHoles.Clear();
            gameObject.SetActive(false);

            moveTweenCase.KillActive();
            visuals.localPosition = cachedVisualsLocalPos;

            screwCollider.enabled = false;
        }

        public void Highlight()
        {
            if (isHighlighted) return;

            isHighlighted = true;

            shadowSpriteRenderer.color = GameController.Data.ScrewHighlightColor;
            shadowSpriteRenderer.transform.localScale = DEFAULT_HIGHLIGHT_SIZE * GameController.Data.HighlightSizeMultiplier;
        }

        public void Unhighlight()
        {
            if (!isHighlighted) return;

            isHighlighted = false;

            shadowSpriteRenderer.color = GameController.Data.ScrewShadowColor;
            shadowSpriteRenderer.transform.localScale = DEFAULT_HIGHLIGHT_SIZE;

            highlightBreathingState = 0;
        }
    }
}