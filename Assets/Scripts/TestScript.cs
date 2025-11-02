using UnityEngine;

public class TestScript : MonoBehaviour
{
    [SerializeField] private InputReader inputReader;

    void Start()
    {
        if (inputReader != null)
            inputReader.MoveEvent += HandleMove;
    }

    void OnDestroy()
    {
        if (inputReader != null)
            inputReader.MoveEvent -= HandleMove;
    }

    private void HandleMove(Vector2 move)
    {
        Debug.Log($"Move: {move}");
    }
}
