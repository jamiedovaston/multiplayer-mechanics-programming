using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineTargetGroup m_TargetGroup;

    public void Awake() =>
        instance = this;

    public void AssignTargetToTargetGroup(Transform target) =>
        m_TargetGroup.AddMember(target, 1.0f, 1.0f);

    public void RemoveTargetFromTargetGroup(Transform target) =>
        m_TargetGroup.RemoveMember(target);
}
