using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class Waypoint : MonoBehaviour
{
    [SerializeField] private float waitTime = 1f;
    
    private SphereCollider _collider;

    public float Radius => _collider.radius * GetMaxScale();
    public float GetWaitTime() => waitTime;

    void Awake()
    {
        _collider = GetComponent<SphereCollider>();
    }

    private float GetMaxScale()
    {
        Vector3 scale = transform.lossyScale;
        return Mathf.Max(scale.x, scale.y, scale.z);
    }

    void OnDrawGizmos()
    {
        if(_collider == null) _collider = GetComponent<SphereCollider>();
        
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawSphere(transform.position, _collider.radius * GetMaxScale());
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _collider.radius * GetMaxScale());
    }
}