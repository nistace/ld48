using UnityEngine;
using UnityEngine.Events;

namespace LD48.Game.Data.Dwarfs {
	public class DwarfAnimator : MonoBehaviour {
		[SerializeField] protected Animator       _animator;
		[SerializeField] protected ParticleSystem _miningImpactParticleSystem;
		[SerializeField] protected GameObject     _pickax;

		private static readonly int miningAnimParam       = Animator.StringToHash("Mining");
		private static readonly int movingAnimParam       = Animator.StringToHash("Moving");
		private static readonly int directionAnimParam    = Animator.StringToHash("Direction");
		private static readonly int startFallingAnimParam = Animator.StringToHash("StartFalling");
		private static readonly int fallingAnimParam      = Animator.StringToHash("Falling");
		private static readonly int dieAnimParam          = Animator.StringToHash("Die");
		private static readonly int takePickaxAnimParam   = Animator.StringToHash("TakePickax");

		public GameObject pickax => _pickax;

		public UnityEvent onMiningImpact { get; } = new UnityEvent();

		public void SetMining(bool value) => _animator.SetBool(miningAnimParam, value);
		public void SetMoving(bool value) => _animator.SetBool(movingAnimParam, value);

		public void SetFalling(bool value) {
			if (value) _animator.SetTrigger(startFallingAnimParam);
			_animator.SetBool(fallingAnimParam, value);
		}

		public void TriggerDie() => _animator.SetTrigger(dieAnimParam);
		public void TriggerTakePickax() => _animator.SetTrigger(takePickaxAnimParam);
		public void SetDirection(int value) => _animator.SetInteger(directionAnimParam, value);

		public void ResetSpeed() => SetSpeed(1);
		public void SetSpeed(float speed) => _animator.speed = speed;

		public void DoMiningImpact() {
			_miningImpactParticleSystem.Play();
			onMiningImpact.Invoke();
		}

		public void MakePickAxAppear() => _pickax.gameObject.SetActive(true);
	}
}