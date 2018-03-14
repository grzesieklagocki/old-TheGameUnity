using HexGameBoard;
using UnityEngine;

public class FieldController : MonoBehaviour
{
    public Vector2Int index = new Vector2Int();

    private static Materials materials;
    private static GameBoardController gameBoardController;

    private Animator animator;

    private struct Materials
    {
        public Material normalMaterial;
        public Material highlightedMaterial;
        public bool areLoaded;
    }

    public bool IsSelected
    {
        get
        {
            return animator.GetBool("IsSelected");
        }
        set
        {
            animator.SetBool("IsSelected", value);
        }
    }

    public bool IsAvailable { get; set; } = true;

    public Vector2Int Position { get { return index; } set { index = value; } }

    private void Start()
    {
        animator = gameObject.GetComponent<Animator>();

        if (!materials.areLoaded)
            materials = new Materials()
            {
                areLoaded = true,
                normalMaterial = Resources.Load<Material>("Materials/HexFieldMaterial"),
                highlightedMaterial = Resources.Load<Material>("Materials/HexFieldHighlightedMaterial")
            };

        if (!gameBoardController)
            gameBoardController = GameObject.Find("GameBoard").GetComponent<GameBoardController>();
    }

    private void OnMouseDown()
    {
        Debug.Log(index);
    }

    private void OnMouseEnter()
    {
        if (MainController.Mode != MainController.Modes.Normal)
            return;

        gameObject.GetComponent<LineRenderer>().material = materials.highlightedMaterial;
        //gameObject.transform.localScale = new Vector3(1, 1, 1) * 1.1f;
        //gameObject.transform.Translate(0, 0, 0.1f);

        IsSelected = true;
    }

    private void OnMouseExit()
    {

        gameObject.GetComponent<LineRenderer>().material = materials.normalMaterial;
        //gameObject.transform.localScale = new Vector3(1, 1, 1);
        //gameObject.transform.Translate(0, 0, -0.1f);

        IsSelected = false;
    }
}
