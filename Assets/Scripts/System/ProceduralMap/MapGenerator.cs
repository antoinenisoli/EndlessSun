using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Pathfinding;
using System.IO;
using System.Linq;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;

public class MapGenerator : MonoBehaviour
{
	GridManager gridManager => GridManager.Instance;
	public MapInfo mainMap => gridManager.MainMap;
	[SerializeField] Vector2Int gridSize;
	GridLayout gridLayout;

	[Header("Debug")]
	[SerializeField] bool debugMode;
	[SerializeField] Text debugText;
	[SerializeField] Transform debugCube;
	float elapsedFrames;
	bool stopFrameCount;

	[Header("Generate Map")]
	[SerializeField] CellularAutomata cellularAutomata;

	[Header("Generate Props")]
	[SerializeField] RuleTile propsTiles;
	[SerializeField] float propsProb = 30f;

	[Header("Draw tiles")]
	[SerializeField] IslandProfile[] islandProfiles;
	[SerializeField] RuleTile waterTile, bridgeRuleTile;
	Dictionary<IslandBiome, IslandProfile> storedIslandProfiles = new Dictionary<IslandBiome, IslandProfile>();

	private void OnDrawGizmos()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(transform.position + new Vector3(gridSize.x/2, gridSize.y/2), new Vector3(gridSize.x, gridSize.y));
    }

    private void Awake()
    {
		gridLayout = FindObjectOfType<GridLayout>();
		cellularAutomata = GetComponentInChildren<CellularAutomata>();
		StoreProfiles();
	}

    private void Start()
    {
		EventManager.Instance.onMapCreated.AddListener(FinishedMap);
    }

    public void StartNewMap()
    {
		mainMap.map = new int[gridSize.x, gridSize.y];
		mainMap.biomeFlags = new int[gridSize.x, gridSize.y];

		cellularAutomata.Init(gridSize, gridManager);
		if (gridSize.sqrMagnitude != 0)
			StartCoroutine(MainGeneration());
	}

	public IEnumerator MainGeneration()
	{
		float delay = 1f;
		if (UIManager.Instance)
        {
			UIManager.Instance.BlackScreen(1, delay);
			yield return new WaitForSeconds(delay);
		}
		if (debugText)
			StartCoroutine(DebugCoroutine());

		GenerateMap();
		SetupMap();
		yield return new WaitForSeconds(0.001f);
		ProcessMap();

		yield return new WaitForSeconds(0.001f);
		GenerateEnemies();

		yield return new WaitForSeconds(0.001f);
		EventManager.Instance.onMapCreated.Invoke();
	}

	IEnumerator DebugCoroutine()
    {
		stopFrameCount = false;
		elapsedFrames = Time.unscaledTime;
		yield return new WaitUntil(() => stopFrameCount = true);
		debugText.text = "Time : " + (Time.unscaledTime - elapsedFrames) + " frames";
	}

	void GenerateEnemies()
    {
		if (AstarPath.active)
			AstarPath.active.Scan();

		foreach (Island island in mainMap.islands)
			island.SpawnEnemies();
	}

	void FinishedMap()
    {
		stopFrameCount = true;
		gridManager.TeleportPlayer();
		if (UIManager.Instance)
			UIManager.Instance.BlackScreen(0, 0.5f);

		//debugCube.transform.position = c.transform.position;
	}

	void SetupMap()
    {
		CreateCells();
		AssignIslands();
		cellularAutomata.ConnectClosestIslands(mainMap.islands);
		CreateCells();
		foreach (var ocean in mainMap.seas)
			ocean.Update();
	}

	void ProcessMap()
    {
		DrawOtherTiles();
		DrawGroundTiles();
		GenerateProps();
		GridManager.MapText("mapText.txt", gridManager.GetMap());
	}

	void GenerateMap()
	{
		gridManager.groundTilemap.ClearAllTiles();
		gridManager.propsTilemap.ClearAllTiles();
		mainMap.map = new int[gridSize.x, gridSize.y];
		mainMap.biomeFlags = new int[gridSize.x, gridSize.y];
		foreach (var item in mainMap.allCells.Values)
		{
			if (item)
				Destroy(item.gameObject);
		}

		mainMap.allCells.Clear();
		cellularAutomata.NewMap();
	}

	void CreateCells()
	{
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
			{
				Vector2Int coords = new Vector2Int(x, y);
				if (!mainMap.allCells.ContainsKey(coords) && mainMap.map[x, y] != 1)
				{
					Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(coords.x, coords.y, 0));
					GameObject cellGameobject = Instantiate(gridManager.cellPrefab, worldToCell, Quaternion.identity, gridManager.cellParent);
					Cell cellScript = cellGameobject.GetComponent<Cell>();
					cellScript.Initialize(coords, worldToCell);
					cellScript.GetNeighbours();

					if (string.IsNullOrEmpty(cellScript.myRegion.name))
                    {
						cellScript.SetRegion(new Region(null, -1));
						if (debugMode)
                        {
							Color c = GameDevHelper.RandomColor();
							c.a = 0.5f;
							cellScript.SetColor(c);
						}
					}

					mainMap.allCells.Add(coords, cellScript);
				}
			}
	}

	public void AssignIslands()
	{
		foreach (var island in mainMap.islands)
			island.GetCellList();

		for (int i = 0; i < mainMap.islands.Count; i++)
		{
			Island island = mainMap.islands[i];
			Color color = GameDevHelper.RandomColor();
			IslandBiome newBiome = Island.RandomBiome();
			IslandProfile newProfile = null;

			if (storedIslandProfiles.TryGetValue(newBiome, out IslandProfile profile))
            {
				newProfile = profile;
				color = profile.islandColor;
			}
			else
				newProfile = islandProfiles[0];

			island.SetProfile(newProfile, newBiome, i);
			foreach (var cell in island.Cells)
			{
				cell.GetNeighbours();
				cell.SetRegion(island);
				mainMap.biomeFlags[cell.coordinates.x, cell.coordinates.y] = i + 1;
				if (debugMode)
                {
					color.a = 0.5f;
					cell.SetColor(color);
				}
			}

			island.GetEdgeTiles();
			foreach (var cell in island.edgeTiles)
			{
				mainMap.biomeFlags[cell.coordinates.x, cell.coordinates.y] = -1;
				if (debugMode)
                {
					color.a = 0.9f;
					cell.SetColor(color);
				}
			}
		}
	}

	void StoreProfiles()
    {
		System.Array array = System.Enum.GetValues(typeof(IslandBiome));
		foreach (var islandProfile in islandProfiles)
            for (int i = 0; i < array.Length; i++)
            {
				IslandBiome biome = (IslandBiome)array.GetValue(i);
				if (islandProfile.name.Contains(biome.ToString()) && !storedIslandProfiles.ContainsValue(islandProfile))
				{
					storedIslandProfiles.Add(biome, islandProfile);
					break;
				}
			}
	}

	public void DrawGroundTiles()
	{
		foreach (var island in mainMap.islands)
			if (island.profile)
				foreach (var coord in island.CoordinateList)
				{
					Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(coord.x, coord.y, 0));
					gridManager.groundTilemap.SetTile(worldToCell, island.profile.ruleTile);
				}
	}

	public void DrawOtherTiles()
	{
		for (int x = 0; x < gridSize.x; x++)
			for (int y = 0; y < gridSize.y; y++)
            {
				Vector2Int item = new Vector2Int(x, y);
				Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(item.x, item.y, 0));

				if (mainMap.map[x,y] == 2)
					gridManager.groundTilemap.SetTile(worldToCell, bridgeRuleTile);
				else if (mainMap.map[x, y] == 1)
					gridManager.groundTilemap.SetTile(worldToCell, waterTile);
			}
	}

	void GenerateProps()
    {
		if (!propsTiles)
			return;

		foreach (Island island in mainMap.islands)
            foreach (Cell cell in island.Cells)
				if (!gridManager.IsShore(cell))
				{
					float r = Random.Range(0, 100);
					if (r < propsProb)
					{
						Vector3Int worldToCell = gridLayout.WorldToCell(new Vector3Int(cell.coordinates.x, cell.coordinates.y, 0));
						gridManager.propsTilemap.SetTile(worldToCell, propsTiles);
					}
				}
	}
}
