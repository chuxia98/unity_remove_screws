using System.Collections.Generic;
using UnityEngine;

namespace Watermelon
{
    public class RaycastController : MonoBehaviour
    {
        private static bool isActive;

        public static event SimpleCallback OnInputActivated;

        private bool isPopupOpened;

        public void Init()
        {
            isActive = true;

            UIController.PopupOpened += OnPopupStateChanged;
            UIController.PopupClosed += OnPopupStateChanged;
        }

        private void OnPopupStateChanged(IPopupWindow popupWindow, bool state)
        {
            isPopupOpened = state;
        }

        private void Update()
        {
            if (!isActive || !LevelController.IsRaycastEnabled || !LevelController.IsLevelLoaded) return;

            if (Input.GetMouseButtonDown(0) && !IsRaycastBlockedByUI())
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit))
                {
                    IClickableObject clickableObject = hit.transform.GetComponent<IClickableObject>();
                    if (clickableObject != null)
                    {
                        ApplyPUOrClick(clickableObject, hit.point);
                    }
                }
                else
                {
                    // Collecting all 2D colliders
                    Collider2D[] colliders2D = Physics2D.OverlapPointAll(ray.origin);
                    List<IClickableObject> clickableObjects2D = ProcessRaycastColliders2D(colliders2D);

                    if (!clickableObjects2D.IsNullOrEmpty())
                    {
                        // Only BaseBehavior is clicked
                        if (clickableObjects2D.Count == 1)
                        {
                            ApplyPUOrClick(clickableObjects2D[0], ray.origin);
                            return;
                        }

                        // Bottom collider isn't base hole
                        if (!(clickableObjects2D[^2] is BaseHoleBehavior baseHole))
                        {
                            ApplyPUOrClick(clickableObjects2D[0], ray.origin);
                            return;
                        }

                        // recalculate Physics for the base hole exact position
                        colliders2D = Physics2D.OverlapCircleAll(baseHole.Position, baseHole.PhysicsRadius);
                        clickableObjects2D = ProcessRaycastColliders2D(colliders2D);

                        // if the top collider is the hole - checking if we can insert the screw
                        if (ScrewBehavior.SelectedScrew != null)
                        {
                            if (clickableObjects2D[0] is HoleBehavior)
                            {
                                LevelController.ProcessClick(clickableObjects2D);
                            }
                        }
                        else
                        {
                            ApplyPUOrClick(clickableObjects2D[0], ray.origin);
                        }
                    }
                }
            }
        }

        private static List<IClickableObject> ProcessRaycastColliders2D(Collider2D[] colliders)
        {
            List<IClickableObject> clickableObjects2D = new List<IClickableObject>();

            // Sorting only for clickables
            for (int i = 0; i < colliders.Length; i++)
            {
                IClickableObject clickableObject = colliders[i].transform.GetComponent<IClickableObject>();
                if (clickableObject != null)
                {
                    clickableObjects2D.Add(clickableObject);
                }
            }

            clickableObjects2D.Sort((first, second) => (int)((first.Position.z - second.Position.z) * 100));

            return clickableObjects2D;
        }

        private void ApplyPUOrClick(IClickableObject clickableObject, Vector3 clickPosition)
        {
            if (PUController.SelectedPU != null)
            {
                PUController.ApplyToElement(clickableObject, clickPosition);
            }
            else
            {
                clickableObject.OnObjectClicked(clickPosition);
            }
        }

        public static List<IClickableObject> HasOverlapCircle2D(Vector2 point, float radius, int mask)
        {
            Collider2D[] colliders2D = Physics2D.OverlapCircleAll(point, radius, mask);
            List<IClickableObject> clickableObjects2D = ProcessRaycastColliders2D(colliders2D);

            return clickableObjects2D;
        }

        private bool IsRaycastBlockedByUI()
        {
            return isPopupOpened;
        }

        public static void Disable()
        {
            isActive = false;
        }

        public static void Enable()
        {
            isActive = true;

            OnInputActivated?.Invoke();
        }

        public void ResetControl()
        {

        }
    }
}
