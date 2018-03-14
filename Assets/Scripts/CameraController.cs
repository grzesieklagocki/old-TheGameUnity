using UnityEngine;
using static UnityEngine.Mathf;

public class CameraController : MonoBehaviour
{
    public GameObject target;
    public GameObject trackedGameObject;

    [Range(0.1f, 1)]
    public float mouseSensivity;

    [Range(1, 5)]
    public float scrollSensivity;

    [Range(1, 50)]
    public float minDistance;

    [Range(1, 50)]
    public float maxDistance;

    private Vector3 position;
    private float distance = 5f;
    private Vector2 rotation = new Vector2(0, 45);

    private Vector3 previousMousePosition;


	void Start()
    {
        if (minDistance > maxDistance)
            throw new System.Exception("minDistance > maxDistance");

        target.transform.rotation = Quaternion.Euler(rotation.y, 0, 0);
        Zoom();
    }
	
	void LateUpdate()
    {
        Zoom();

        if (Input.GetKey(KeyCode.Mouse2))
        {
            MainController.Mode = MainController.Modes.CameraManipulation;

            Rotate();
        }
        else
        {
            MainController.Mode = MainController.Modes.Normal;
        }

        FollowTarget();

        previousMousePosition = Input.mousePosition;
    }


    private void Zoom()
    {
        distance = Clamp(distance - Input.mouseScrollDelta.y * scrollSensivity, minDistance, maxDistance);
        transform.localPosition = new Vector3(0, 0, Lerp(transform.localPosition.z, -distance, 0.1f));
    }

    private void Rotate() //
    {
        Vector3 actualMousePosition = Input.mousePosition;
        Vector3 delta = previousMousePosition - actualMousePosition;

        rotation.x += delta.x * mouseSensivity;
        rotation.y = Clamp(rotation.y + (delta.y * mouseSensivity), 15, 85);

        target.transform.rotation = Quaternion.Euler(rotation.y, -rotation.x, 0);
    }

    private void FollowTarget()
    {
        if (target.transform.position == trackedGameObject.transform.position)
            return;

        position.x = Lerp(target.transform.position.x, trackedGameObject.transform.position.x, 0.1f);
        position.y = Lerp(target.transform.position.y, trackedGameObject.transform.position.y, 0.1f);
        position.z = Lerp(target.transform.position.z, trackedGameObject.transform.position.z, 0.1f);

        target.transform.position = position;
    }
}
