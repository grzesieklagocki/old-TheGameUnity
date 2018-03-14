using UnityEngine;

public class MainController : MonoBehaviour
{

    public enum Modes
    {
        Normal,
        CameraManipulation
    }

    private static Modes mode;
    public static Modes Mode
    {
        get
        {
            return mode;
        }
        set
        {
            if (value == mode)
                return;

            ActivateMode(mode = value);
        }
    }


    private void Start()
    {

    }


    private static void ActivateMode(Modes mode)
    {
        switch (mode)
        {
            case Modes.Normal:
                Cursor.visible = true;
                break;
            case Modes.CameraManipulation:
                Cursor.visible = false;
                break;
        }
    }

}