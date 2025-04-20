using UnityEngine;

public class ObjectsSpawnOrbit : MonoBehaviour
{
    [Header("References")]
    public GameObject planet;         // Объект планеты
    public GameObject[] prefabs;     // Массив префабов для спавна

    [Header("Parameters")]
    public float planetRadius = 5f;  // Радиус планеты
    public float orbitRadius = 10f;  // Радиус орбиты
    public float orbitMultiplier; 
    public int spawnCount = 20;      // Количество объектов
    public float safetyMargin = 0.5f; // Защитный отступ от планеты
    public bool planetObjSpawner = false; 

    void Start()
    {
        SpawnObjectsAroundPlanet();
    }

    void SpawnObjectsAroundPlanet()
    {
        if (planet == null || prefabs.Length == 0)
        {
            Debug.LogError("Не назначены необходимые объекты!");
            return;
        }

        Vector3 center = planet.transform.position;
        planetRadius = GetComponent<Waypoint>().Radius; 
        orbitRadius = planetRadius * orbitMultiplier; 
        float minRadius = planetRadius + safetyMargin;

        for (int i = 0; i < spawnCount; i++)
        {
            // Генерация случайной точки в сферических координатах
            Vector3 randomDirection = Random.onUnitSphere;
            float randomRadius = Random.Range(minRadius, orbitRadius);
            
            // Расчет позиции
            Vector3 spawnPosition = center + randomDirection * randomRadius;
            
            // Выбор случайного префаба
            GameObject randomPrefab = prefabs[Random.Range(0, prefabs.Length)];
            
            // Создание объекта
            GameObject item = Instantiate(randomPrefab, spawnPosition, Quaternion.identity);
            if(planetObjSpawner)
                item.GetComponent<FakeGravityBody>().attractor = GetComponent<FakeGravity>(); 
        }
    }
}
