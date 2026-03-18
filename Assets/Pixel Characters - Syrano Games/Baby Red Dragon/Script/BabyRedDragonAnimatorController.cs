using UnityEngine;

namespace SyranoGames
{
    [RequireComponent(typeof(Animator))]
    public class BabyRedDragonAnimatorController : MonoBehaviour
    {
        public int value;
        Animator animator; // Cached Animator component

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        private void Start()
        {
            SetInt();
        }

        public void SetInt()
        {
            animator.SetInteger("Animation", value);
        }
    }
}