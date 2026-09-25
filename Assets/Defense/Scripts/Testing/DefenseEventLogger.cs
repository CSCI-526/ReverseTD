// TEST ONLY: logs tower events to the Console. Also an example of subscribing to them.

using UnityEngine;

namespace ReverseTD.Defense.Testing
{
    /// <summary>
    /// Logs <see cref="Tower.Destroyed"/> and <see cref="BoardView.AllTowersDestroyed"/> to the Console.
    /// Static events keep their subscribers when scenes change, so subscribe in OnEnable and unsubscribe in OnDisable.
    /// </summary>
    public class DefenseEventLogger : MonoBehaviour
    {
        private void OnEnable()
        {
            Tower.Destroyed += LogTowerDestroyed;
            BoardView.AllTowersDestroyed += LogAllTowersDestroyed;
        }

        private void OnDisable()
        {
            Tower.Destroyed -= LogTowerDestroyed;
            BoardView.AllTowersDestroyed -= LogAllTowersDestroyed;
        }

        private static void LogTowerDestroyed(Tower tower)
        {
            Debug.Log($"Tower destroyed: {tower.name} at {(Vector2)tower.transform.position}");
        }

        private static void LogAllTowersDestroyed(BoardView board)
        {
            Debug.Log($"All towers destroyed on {board.name}.");
        }
    }
}
