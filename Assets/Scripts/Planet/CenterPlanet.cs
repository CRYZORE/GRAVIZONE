using UnityEngine;


[System.Serializable]
public class OrbitSettings
{
    public float radius;        // Радиус орбиты
    public int minObjectCount, maxObjectCount; 
    public int objectCount;     // Количество объектов на орбите
    public float minHeight;     // Минимальная высота относительно плоскости орбиты
    public float maxHeight;     // Максимальная высота относительно плоскости орбиты
}
public class CenterPlanet : MonoBehaviour
{
    [Header("References")]
    public GameObject planet;
    public GameObject[] planetPrefabs;
    [SerializeField] Transform player; 
    [SerializeField] GameObject botPrefab; 

    [Header("Parameters")]
    public OrbitSettings[] orbits;
    public float planetRadius = 5f;
    public float safetyMargin = 1f;
    public bool playerSpawned = false; 

    void Start()
    {
        SpawnOrbitalObjects();
        planetRadius = GetComponent<Waypoint>().Radius;
    }

    void SpawnOrbitalObjects()
    {
        if (planet == null || planetPrefabs.Length == 0)
        {
            Debug.LogError("Не назначены необходимые префабы!");
            return;
        }

        Vector3 center = planet.transform.position;

        foreach (var orbit in orbits)
        {
            if (orbit.radius < planetRadius + safetyMargin)
            {
                Debug.LogWarning($"Орбита с радиусом {orbit.radius} слишком близко к планете!");
                continue;
            }
            orbit.objectCount = Random.Range(orbit.minObjectCount, orbit.maxObjectCount); 
            for (int i = 0; i < orbit.objectCount; i++)
            {
                // Рассчет угла для равномерного распределения
                float angle = i * Mathf.PI * 2 / orbit.objectCount;
                
                // Базовые координаты на плоскости орбиты (XZ)
                Vector3 position = new Vector3(
                    Mathf.Cos(angle) * orbit.radius,
                    0,
                    Mathf.Sin(angle) * orbit.radius
                );

                // Добавляем случайную высоту
                position.y = Random.Range(orbit.minHeight, orbit.maxHeight);

                // Выбор и поворот префаба
                GameObject prefab = planetPrefabs[Random.Range(0, planetPrefabs.Length)];
                Quaternion rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

                // Создаем объект с учетом позиции планеты
                GameObject newPlanet = Instantiate(prefab, center + position, rotation);
            }
        }
    }

    // Для визуализации орбит в редакторе
    void OnDrawGizmosSelected()
    {
        if (planet == null) return;
        
        Gizmos.color = Color.cyan;
        foreach (var orbit in orbits)
        {
            Gizmos.DrawWireSphere(planet.transform.position, orbit.radius);
        }
    }
}
