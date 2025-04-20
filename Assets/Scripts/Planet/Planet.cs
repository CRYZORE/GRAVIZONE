using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 

public class Planet : MonoBehaviour
{
    Renderer myRenderer; 
    [SerializeField] List<Material> mats; 
    [SerializeField] bool scalable = true; 
    [Header("Size Settings")]
    [SerializeField] private float minSize = 0.8f;
    [SerializeField] private float maxSize = 1.2f;
    public Transform spawnPos; 
    [SerializeField] GameObject botPrefab; 
    [SerializeField] CenterPlanet midPlanet; 
    [SerializeField] Transform player; 
    [SerializeField] bool isSpawning = true; 
    [SerializeField] GameManager gameManager; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myRenderer = GetComponent<Renderer>(); 
        midPlanet = GameObject.FindObjectOfType<CenterPlanet>(); 
        gameManager = GameObject.FindObjectOfType<GameManager>(); 
        player = GameObject.FindObjectOfType<Player>().transform; 
        if(midPlanet.playerSpawned && isSpawning) {
            GameObject newBot = Instantiate(botPrefab, spawnPos.position, Quaternion.identity); 
            gameManager.enemies.Add(newBot);
        }
        else if(!midPlanet.playerSpawned && isSpawning){
            player.position = spawnPos.position;
            midPlanet.playerSpawned = true; 
        }
        myRenderer.material = mats[Random.Range(0, mats.Count)]; 
        if(scalable)
            ApplyRandomSize(); 
    }
    public void ApplyRandomSize()
    {
        float randomScale = Random.Range(minSize, maxSize);
        transform.localScale = Vector3.one * randomScale;
    }
    [ContextMenu("Randomize Size")]
    private void RandomizeSizeEditor()
    {
        ApplyRandomSize();
    }
}
