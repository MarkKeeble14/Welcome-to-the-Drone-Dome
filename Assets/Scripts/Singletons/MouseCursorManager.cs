using UnityEngine;

public class MouseCursorManager : MonoBehaviour
{
    public static MouseCursorManager _Instance { get; private set; }
    private bool cursorLocked;
    public bool Locked => cursorLocked;

    private void Awake()
    {
        if (_Instance != null)
        {
            Destroy(_Instance.gameObject);
        }
        _Instance = this;
        SetCursor(CursorType.DEFAULT, false);
    }

    [SerializeField]
    private SerializableDictionary<CursorType, CursorTypeInfo> cursorTextures
        = new SerializableDictionary<CursorType, CursorTypeInfo>();

    public void SetCursor(CursorType type, bool lockCursor)
    {
        cursorLocked = lockCursor;
        CursorTypeInfo typeInfo = cursorTextures.GetEntry(type).Value;
        Cursor.SetCursor(typeInfo.Texture, typeInfo.HotSpot, typeInfo.Mode);
    }
}
