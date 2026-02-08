using UnityEngine;
using GodBox.Interaction;
using GodBox.Pathfinding;
using GodBox.GameplayTags;
using System.Collections.Generic;

namespace GodBox.UtilityAI
{
    [CreateAssetMenu(fileName = "LookForFoodAction", menuName = "GodBox/UtilityAI/Actions/LookForFood")]
    public class LookForFoodAction : UtilityAction
    {
        public GameplayTag FoodTag;

        public override void Execute(UtilityAIComponent context)
        {
            var mover = context.GetAgentComponent<PathfindingAgent>();
            if (mover == null) return;

            FoodSource target = context.GetData<FoodSource>("TargetFood");

            // Look for food if we don't have a valid target
            if (target == null)
            {
                target = FindNearestFood(context.transform.position);
                if (target != null)
                {
                    context.SetData("TargetFood", target);
                    mover.SetDestination(target.transform.position);
                }
                else
                {
                    // Optionally log, but limit spam. For now, let's log once if not finding anything.
                    // Debug.Log($"[{context.name}] No food found with tag {FoodTag?.TagName}");
                }
            }

            // If we have a target, check status
            if (target != null)
            {
                float dist = Vector3.Distance(context.transform.position, target.transform.position);
                if (dist <= target.InteractionRange)
                {
                    target.Interact(context.gameObject);
                    context.SetData("TargetFood", null); // Clear target after eating
                    mover.Stop();
                }
                else if (!mover.IsMoving)
                {
                     // Recalculate if we stopped but haven't reached (e.g. pushed off path)
                     mover.SetDestination(target.transform.position);
                     
                     if (!mover.IsMoving) {
                        // Pathfinding failed?
                        // Debug.Log($"[{context.name}] Pathfinding failed to food at {target.transform.position}");
                     }
                }
            }
        }

        private FoodSource FindNearestFood(Vector3 position)
        {
            if (FoodTag == null)
            {
                Debug.LogWarning("FoodTag not assigned in LookForFoodAction");
                return null;
            }

            // Use Tag System
            HashSet<GameObject> foodObjects = GameplayTagManager.Instance.GetObjectsWithTag(FoodTag);
            
            FoodSource nearest = null;
            float minDst = float.MaxValue;

            foreach (var obj in foodObjects)
            {
                if (obj == null) continue; // Safety check
                
                var food = obj.GetComponent<FoodSource>();
                if (food == null) continue;

                float dst = Vector3.Distance(position, food.transform.position);
                if (dst < minDst)
                {
                    minDst = dst;
                    nearest = food;
                }
            }
            return nearest;
        }
    }
}
