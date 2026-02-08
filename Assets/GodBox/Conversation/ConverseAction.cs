using System.Collections;
using UnityEngine;
using GodBox.UtilityAI;
using GodBox.Pathfinding;
using GodBox.Conversation;

namespace GodBox.UtilityAI
{
    [CreateAssetMenu(fileName = "ConverseAction", menuName = "GodBox/UtilityAI/Actions/Converse")]
    public class ConverseAction : UtilityAction
    {
        public float SearchRadius = 10f;
        public float TalkRange = 3f;
        public float Cooldown = 10f; // Time before talking again

        public override float Evaluate(UtilityAIComponent context)
        {
            // Locking mechanism: If we are already committed (Moving, Talking, or Listening),
            // return a score higher than max possible (1.0) to prevent interruption.
            int state = context.GetData<int>("ConversationState"); 
            if (state > 0) return 2.0f; 

            // Check Cooldown
            float cooldownUntil = context.GetData<float>("ConversationCooldownTime");
            if (Time.time < cooldownUntil)
            {
                // Debug.Log($"[ConverseAction] {context.name} on cooldown until {cooldownUntil} (Current: {Time.time})");
                return 0f;
            }

            return base.Evaluate(context);
        }

        public override void Execute(UtilityAIComponent context)
        {
            // State: 0=Searching, 1=Initiator Moving, 2=Initiator Talking, 3=Listener Listening
            int state = context.GetData<int>("ConversationState");
            GameObject target = context.GetData<GameObject>("ConversationTarget");
            
            var agent = context.GetAgentComponent<PathfindingAgent>();
            var myDialogue = context.GetAgentComponent<DialogueComponent>();

            // 0. Searching (Initiate)
            if (state == 0)
            {
                // Check Cooldown before searching. 
                // This prevents re-entry in the same frame Execute() is called after Reset(), 
                // before the AI Component has a chance to Tick() and switch actions.
                float cooldownUntil = context.GetData<float>("ConversationCooldownTime");
                if (Time.time < cooldownUntil) return;

                // Safety: If we somehow have a target but state 0, clear it
                if (target != null) context.SetData("ConversationTarget", null);

                target = FindAvailablePartner(context);
                if (target != null)
                {
                    // Lock myself
                    context.SetData("ConversationTarget", target);
                    context.SetData("ConversationState", 1); // Moving

                    // Lock partner
                    // We assume partner has UtilityAIComponent because FindAvailablePartner checks it
                    var partnerAI = target.GetComponent<UtilityAIComponent>();
                    partnerAI.SetData("ConversationTarget", context.gameObject);
                    partnerAI.SetData("ConversationState", 3); // Listener
                    
                    Debug.Log($"[ConverseAction] {context.name} initiated conversation with {target.name}");
                }
                else
                {
                    // No one found. Log a warning periodically if we expect to find someone but don't.
                    // This helps debug missing colliders/layers.
                    // Using Time.time to limit spam (every 2 seconds)
                    float lastWarn = context.GetData<float>("ConversationWarnTime");
                    if (Time.time > lastWarn + 2f)
                    {
                        Debug.LogWarning($"[ConverseAction] {context.name} wants to converse but cannot find any partner with a Collider2D & DialogueComponent within {SearchRadius}m.");
                        context.SetData("ConversationWarnTime", Time.time);
                    }
                }
                return;
            }

            // If we are here, we have a target or we are in a state that implies a target.
            // Safety check: Is target still valid?
            if (target == null)
            {
                ResetConversation(context, null);
                return;
            }

            // 1. Initiator Moving
            if (state == 1)
            {
                float dist = Vector2.Distance(context.transform.position, target.transform.position);
                if (dist <= TalkRange)
                {
                    agent.Stop();
                    context.SetData("ConversationState", 2); // Start talking
                    context.StartCoroutine(ConversationRoutine(context, target, myDialogue));
                }
                else
                {
                    if (!agent.IsMoving)
                        agent.SetDestination(target.transform.position);
                }
            }
            
            // 2. Initiator Talking
            // Coroutine handles it. We just wait.

            // 3. Listener Waiting
            if (state == 3)
            {
                agent.Stop(); // Ensure we don't walk away
                
                // We rely on the Initiator to reset our state when done.
                // But we should have a failsafe? 
                // If target (Initiator) is too far or destroyed, we should reset.
                // For now, let's assume happy path or Initiator handles reset.
            }
        }

        private void ResetConversation(UtilityAIComponent context, GameObject target)
        {
            context.SetData("ConversationTarget", null);
            context.SetData("ConversationState", 0);
            context.SetData("ConversationCooldownTime", Time.time + Cooldown);

            if (target != null)
            {
                var targetAI = target.GetComponent<UtilityAIComponent>();
                if (targetAI != null)
                {
                    targetAI.SetData("ConversationTarget", null);
                    targetAI.SetData("ConversationState", 0);
                    targetAI.SetData("ConversationCooldownTime", Time.time + Cooldown);
                }
            }
        }

        private GameObject FindAvailablePartner(UtilityAIComponent context)
        {
            var colliders = Physics2D.OverlapCircleAll(context.transform.position, SearchRadius);
            foreach (var col in colliders)
            {
                if (col.gameObject == context.gameObject) continue;
                
                var ai = col.GetComponent<UtilityAIComponent>();
                var diag = col.GetComponent<DialogueComponent>();

                // Must have components AND be free (State 0)
                if (ai != null && diag != null)
                {
                    int partnerState = ai.GetData<int>("ConversationState");
                    float partnerCooldown = ai.GetData<float>("ConversationCooldownTime");

                    // Only pick if State is 0 (Free) AND Cooldown has passed
                    if (partnerState == 0 && Time.time >= partnerCooldown)
                    {
                        return col.gameObject;
                    }
                }
            }
            return null;
        }

        private IEnumerator ConversationRoutine(UtilityAIComponent context, GameObject target, DialogueComponent myDialogue)
        {
            // Debug.Log($"[ConverseAction] Starting conversation between {context.name} and {target.name}");
            
            var otherDialogue = target.GetComponent<DialogueComponent>();
            
            if (myDialogue == null) Debug.LogError($"[ConverseAction] {context.name} has no DialogueComponent!");
            if (otherDialogue == null) Debug.LogError($"[ConverseAction] Target {target.name} has no DialogueComponent!");

            // Greets
            if (myDialogue != null) myDialogue.Say(DialogueDatabase.GetRandomGreeting());
            yield return new WaitForSeconds(2f);
            
            if (otherDialogue != null) {
                otherDialogue.Say(DialogueDatabase.GetRandomGreeting());
                yield return new WaitForSeconds(2f);
            }

            // Topics
            for (int i = 0; i < 2; i++) // Exchange 2 lines
            {
                string topic = DialogueDatabase.GetRandomTopic();
                string reply = DialogueDatabase.GetRandomReply(topic);

                if (myDialogue != null) myDialogue.Say("Let's talk about " + topic + ".");
                yield return new WaitForSeconds(3f);

                if (otherDialogue != null) {
                    otherDialogue.Say(reply);
                    yield return new WaitForSeconds(3f);
                }
            }

            // Parting
            if (myDialogue != null) myDialogue.Say(DialogueDatabase.GetRandomParting());
            yield return new WaitForSeconds(2f);
             if (otherDialogue != null) {
                otherDialogue.Say(DialogueDatabase.GetRandomParting());
                yield return new WaitForSeconds(2f);
            }

            // End
            ResetConversation(context, target);
        }
    }
}
