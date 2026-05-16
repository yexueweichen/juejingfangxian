using UnityEngine;

[RequireComponent(typeof(Animator))]
public class IKBoneSync : MonoBehaviour
{
    [Header("IK手骨")]
    public Transform rightIKHand;
    public Transform leftIKHand;

    // 存储武器相对于手骨的初始偏移
    private Quaternion _rightHandLocalRot;
    private Quaternion _leftHandLocalRot;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        
        if (rightIKHand != null)
        {
            _rightHandLocalRot = rightIKHand.localRotation;
        }
        if (leftIKHand != null)
        {
            _leftHandLocalRot = leftIKHand.localRotation;
        }
    }

   
    private void OnAnimatorIK(int layerIndex)
    {
        // 同步右手
        if (rightIKHand != null)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1f);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, rightIKHand.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, rightIKHand.rotation);
        }

        // 同步左手
        if (leftIKHand != null)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, leftIKHand.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, leftIKHand.rotation);
        }
    }
}
