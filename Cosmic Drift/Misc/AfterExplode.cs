using UnityEngine;
using System.Collections;

public class AfterExplode : StateMachineBehaviour {
	GameManager gameAudio;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		gameAudio = GameObject.FindGameObjectWithTag ("AudioManager").GetComponent<GameManager>();
		if (animator.CompareTag ("Debris")) gameAudio.PlaySfx ("debris");
		else gameAudio.PlaySfx ("enemy");
		if (animator.gameObject.transform.parent != null && animator.CompareTag("Debris")){
			animator.gameObject.transform.parent.gameObject.GetComponent<Animator>().enabled = false;
			animator.gameObject.transform.parent.gameObject.GetComponent<Collider2D>().enabled = false;
			animator.gameObject.transform.parent.gameObject.GetComponent<SpriteRenderer> ().enabled = false;
			animator.gameObject.transform.parent.gameObject.GetComponent<Debris> ().enableBlink = false;
		}
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	//override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (animator.gameObject.transform.parent != null && animator.CompareTag("Debris"))
			GameObject.Destroy (animator.gameObject.transform.parent.gameObject);
		else
			GameObject.Destroy (animator.gameObject);
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
