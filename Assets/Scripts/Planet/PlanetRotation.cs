using UnityEngine; 
public class PlanetRotation : MonoBehaviour
{
    [SerializeField] private Vector3 minRotationSpeed;
    [SerializeField] private Vector3 maxRotationSpeed;

    private Vector3 rotationSpeed;

    void Start()
    {
        rotationSpeed = new Vector3(
            Random.Range(minRotationSpeed.x, maxRotationSpeed.x),
            Random.Range(minRotationSpeed.y, maxRotationSpeed.y),
            Random.Range(minRotationSpeed.z, maxRotationSpeed.z)
        );
    }

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}