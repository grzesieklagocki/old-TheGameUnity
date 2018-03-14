using HexGameBoard;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static HexGameBoard.HexHelper;

public class GameBoardController : NetworkBehaviour
{
    [Range(0.5f, 5)]
    public float hexScale;

    [Range(0, 0.5f)]
    public float margin;

    public GameObject hexPrefab;

    private Camera mainCamera;
    private GameObject[,] fields;
    private Vector2Int size;
    private readonly Vector2 offset = new Vector2(1.5f, Mathf.Sqrt(3));
    private Vector3 globalOffset;

    #region Start

    void Start()
    {
        //Debug.Log("kąt " + Mathf.Atan(1) * Mathf.Rad2Deg);
        mainCamera = GameObject.Find("Main Camera").transform.GetComponent<Camera>();

        Initialize(31, 31);
        var a = FieldsInRange(new Vector2Int(10, 10), 10);
        //PriorityQueue<float> queue = new PriorityQueue<float>();

        foreach (var pos in fields)
        {
            var field = pos.GetComponent<FieldController>().index;
            if (HasFieldValidIndex(field))
                FieldAt(field).SetActive(true);
        }

        //FindPath(new Vector2Int(0, 0), new Vector2Int(20, 20));
        //StartCoroutine(FindRandom());
    }

    private IEnumerator FindRandom()
    {
        while (true)
        {
            var random = new System.Random();

            AddRandomSpace();
            ClearGameBoard();

            FindPath(new Vector2Int(random.Next(0, size.x), random.Next(0, size.y)), new Vector2Int(random.Next(0, size.x), random.Next(0, size.y)));
            
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ClearGameBoard()
    {
        var material = Resources.Load<Material>("Materials/HexFieldMaterial");
        var blocked = Resources.Load<Material>("Blocked");

        foreach (var field in fields)
        {
            if (!field.GetComponent<FieldController>().IsAvailable)
                field.GetComponent<FieldController>().gameObject.GetComponent<LineRenderer>().material = blocked;
            else
                field.GetComponent<FieldController>().gameObject.GetComponent<LineRenderer>().material = material;
        }
    }

    private void AddRandomSpace()
    {
        var random = new System.Random();

        var field = fields[random.Next(0, size.x), random.Next(0, size.y)];

        field.GetComponent<FieldController>().IsAvailable = false;
        field.GetComponent<FieldController>().gameObject.GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/Blocked");
    }

    private void FindPath(Vector2Int start, Vector2Int destination)
    {
        var f = GetIFieldArray(fields).Select(f1 => f1.Select(f2 => f2.isAvailable).ToArray()).ToArray();

        var watch = System.Diagnostics.Stopwatch.StartNew();

        var path = HexHelper.FindPath(f, start, destination);

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        if (elapsedMs > 0)
            Debug.Log($"[{elapsedMs}ms] Obliczona trasa={path.Count}, potrzebna={GetDistance(start, destination) + 1}");

        foreach (var field in path)
        {
            FieldAt(field).GetComponent<FieldController>().gameObject.GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/HexFieldHighlightedMaterial");
        }
    }

    private PathFindableField[][] GetIFieldArray(GameObject[,] fields)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        var array = new PathFindableField[fields.GetLength(0)][];
        var size = new Vector2Int(fields.GetLength(0), fields.GetLength(1));

        for (int x = 0; x < size.x; x++)
            array[x] = new PathFindableField[size.y];

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                var field = fields[x, y].GetComponent<FieldController>();

                //field.AvailabilityLevel = 2;

                array[x][y] = new PathFindableField(field.Position, field.IsAvailable);
            }
        }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;

        //Debug.Log($"[{elapsedMs}ms] Przekształcanie planszy {size.x} x {size.y}");

        return array;
    }

    #endregion

    #region Update

    void Update()
    {
        //if (!Input.anyKeyDown)
        //    return;

        //if (Input.GetKeyDown(KeyCode.Q))
        //{            
        //    inner--;
        //}
        //else if (Input.GetKeyDown(KeyCode.W))
        //{
        //    inner++;
        //}
        //else if (Input.GetKeyDown(KeyCode.E))
        //{
        //    outer--;
        //}
        //else if (Input.GetKeyDown(KeyCode.R))
        //{
        //    outer++;
        //}
        //else
        //{
        //    return;
        //}

        //var fieldsInRange = FieldsInRange(new Vector2Int(10, 10), outer, inner);

        //foreach (var field in fields)
        //{
        //    field.SetActive(false);
        //}

        //foreach (var field in fieldsInRange)
        //{
        //    if (HasFieldValidIndex(field))
        //        FieldAt(field).SetActive(true);
        //}

        var ray = new RaycastHit();

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out ray))
        {
            ClearGameBoard();

            Vector2Int selectedFieldIndex = PixelToHexIndex(ray.point.x - globalOffset.x, -(ray.point.z - globalOffset.z));

            if (selectedFieldIndex.x < 0 || selectedFieldIndex.x >= size.x || selectedFieldIndex.y < 0 || selectedFieldIndex.y >= size.x)
                return;

            //List<Vector2Int> line = DrawLine(selectedFieldIndex, Vector2Int.zero);

            bool[][] f = GetIFieldArray(fields).Select(f1 => f1.Select(f2 => f2.isAvailable).ToArray()).ToArray();
            Stack<Vector2Int> line = HexHelper.FindPath(f, selectedFieldIndex, Vector2Int.zero);

            foreach (var field in line)
                FieldAt(field).GetComponent<FieldController>().gameObject.GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/HexFieldHighlightedMaterial 1");

            GameObject selectedFIeld = FieldAt(selectedFieldIndex);
            //selectedFIeld.GetComponent<FieldController>().gameObject.GetComponent<LineRenderer>().material = Resources.Load<Material>("Materials/HexFieldHighlightedMaterial");
        }
    }

    #endregion

    #region Initialize

    public void Initialize(int sizeX, int sizeY)
    {
        Initialize(new Vector2Int(sizeX, sizeY));
    }

    public void Initialize(Vector2Int size)
    {
        this.size.x = size.x;
        this.size.y = size.y;

        fields = new GameObject[size.x, size.y];

        CenterGameBoard();

        for (int x = 0; x < size.x; x++)
        {
            float startY = (x % 2 == 0) ? 0 : -((offset.y + margin) / 2);
            float xPosition = x * (offset.x + margin);

            for (int y = 0; y < size.y; y++)
            {
                float yPosition = startY - (y * (offset.y + margin));

                var field = DrawField(xPosition, yPosition);
                AddIndexComponent(field, x, y);

                fields[x, y] = field;
            }
        }
    }

    #endregion

    public GameObject FieldAt(Vector2Int index)
    {
        return fields[index.x, index.y];
    }

    public bool HasFieldValidIndex(Vector2Int index)
    {
        return (index.x >= 0 && index.x < size.x && index.y >= 0 && index.y < size.y);
    }


    #region Draw Field

    private GameObject DrawField(float x, float y)
    {
        //var field = Instantiate(hexPrefab, transform, true);
        var field = Instantiate(new GameObject(), transform, true);

        field.transform.localPosition = new Vector3(x * hexScale, 0, y * hexScale); field.SetActive(false);
        field.transform.localScale = new Vector3(hexScale, hexScale, hexScale);

        var renderer = field.AddComponent<LineRenderer>();

        Vector3[] positions =
        {
            new Vector3(0.475f, 0.1f, 0.816025f),
            new Vector3(-0.475f, 0.1f, 0.816025f),
            new Vector3(-0.95f, 0.1f, 0),
            new Vector3(-0.475f, 0.1f, -0.816025f),
            new Vector3(0.475f, 0.1f, -0.816025f),
            new Vector3(0.95f, 0.1f, 0),
        };

        renderer.positionCount = 6;
        renderer.loop = true;
        renderer.useWorldSpace = true;
        renderer.material.color = Color.blue;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        for (int i = 0; i < 6; i++)
        {
            var pos = positions[i];
            renderer.SetPosition(i, pos + field.transform.position + new Vector3(0, Terrain.activeTerrain.SampleHeight(pos + field.transform.position)));
            var pos2 = renderer.GetPosition(i);
            var height = Terrain.activeTerrain.SampleHeight(pos);

        }

        renderer.widthMultiplier = 0.1f;

        //var line = field.AddComponent<LineRenderer>();

        //line.positionCount = 6;
        //line.widthMultiplier = 0.1f;
        //line.loop = true;

        //line.SetPosition(0, new Vector3(0.816025f, Terrain.activeTerrain.SampleHeight(new Vector3(0.816025f, 0, 0.475f)), 0.475f));
        //line.SetPosition(0, new Vector3(0.816025f, Terrain.activeTerrain.SampleHeight(new Vector3(0.816025f, 0, -0.475f)), -0.475f));

        return field;
    }

    #endregion

    private void AddIndexComponent(GameObject field, int x, int y)
    {
        var fieldController = field.AddComponent<FieldController>();
        fieldController.index.x = x;
        fieldController.index.y = y;
    }

    private void CenterGameBoard()
    {
        float pxXSize = (size.x - 1) * (offset.x + margin) + (offset.x * 4 / 3);
        float pxYSize = (size.y) * (offset.y + margin) + (offset.y / 2);

        globalOffset = transform.position -= new Vector3(pxXSize / 2, -0.1f, -pxYSize / 2)
            - (new Vector3(offset.x * 2 / 3, 0, -offset.y / 2));
    }

    public Vector3 GetCenterPositionOfField(Vector2Int indexes)
    {
        return GetCenterPositionOfField(indexes.x, indexes.y);
    }

    public Vector3 GetCenterPositionOfField(int x, int y)
    {
        var position = new Vector3(x * (offset.x + margin), 0, -y * (offset.y + margin)) + globalOffset;

        if (x % 2 == 1)
            position -= new Vector3(0, 0, offset.y + margin) / 2;

        return position;
    }

    private void OnGUI()
    {
        var ray = new RaycastHit();

        if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out ray))
        {
            //GUI.Label(new Rect(new Vector2(100, 100), new Vector2(200, 20)), Terrain.activeTerrain.SampleHeight(ray.point).ToString());
            GUI.Label(new Rect(new Vector2(100, 120), new Vector2(200, 20)), ray.point.ToString());
            GUI.Label(new Rect(new Vector2(100, 140), new Vector2(200, 20)), PixelToHexIndex(ray.point.x, ray.point.z).ToString());
            GUI.Label(new Rect(new Vector2(100, 160), new Vector2(200, 20)), globalOffset.ToString());
        }
    }       
}