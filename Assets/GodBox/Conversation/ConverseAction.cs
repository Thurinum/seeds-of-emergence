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

        public override void Execute(UtilityAIComponent context)
        {
            // Do we have a target?
            GameObject target = context.GetData<GameObject>("ConversationTarget");
            
            // State: 0=Searching, 1=Moving, 2=Talking
            int state = context.GetData<int>("ConversationState");

            var agent = context.GetAgentComponent<PathfindingAgent>();
            var myDialogue = context.GetAgentComponent<DialogueComponent>();

            // 1. Find Target
            if (target == null)
            {
                target = FindAvailablePartner(context);
                if (target != null)
                {
                    context.SetData("ConversationTarget", target);
                    context.SetData("ConversationState", 1); // Moving
                }
                else
                {
                    // No one found, maybe wander or fail out
                    return;
                }
            }

            // 2. Move to Target
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
            
            // 3. Talking handled by Coroutine. 
            // While Talking, this Execute() keeps getting called. We should just wait.
            // When routine finishes, it clears the target.
        }

        private GameObject FindAvailablePartner(UtilityAIComponent context)
        {
            var colliders = Physics2D.OverlapCircleAll(context.transform.position, SearchRadius);
            foreach (var col in colliders)
            {
                if (col.gameObject == context.gameObject) continue;
                
                // Must have DialogueComponent and AI
                if (col.GetComponent<DialogueComponent>() != null && col.GetComponent<UtilityAIComponent>() != null)
                {
                    return col.gameObject;
                }
            }
            return null;
        }

        private IEnumerator ConversationRoutine(UtilityAIComponent context, GameObject target, DialogueComponent myDialogue)
        {
            var otherDialogue = target.GetComponent<DialogueComponent>();
            
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
            context.SetData("ConversationTarget", null);
            context.SetData("ConversationState", 0);
            
            // Add cooldown so we don't spam talk immediately?
            // Need a Blackboard cooldown key ideally.
        }
    }
}
