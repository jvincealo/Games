using UnityEngine;
using System.Collections;

public class AttackBehavior : StateMachineBehaviour {
	bool attackSuccess = false;
	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	//override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Enemy enemy = animator.GetComponent<Enemy>();
		EnemyAI enemyAI = animator.GetComponent<EnemyAI>();
		GameObject target = GameObject.FindGameObjectWithTag("Player");
		Player player = target.GetComponent<Player>();
		if(stateInfo.normalizedTime >= .4f && !attackSuccess && enemyAI.state == EnemyAI.State.IN_RANGE){
			player.TakeDamage(enemy.damage);
			attackSuccess = true;
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		attackSuccess = false;
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
