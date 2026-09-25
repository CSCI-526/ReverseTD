# Defense side: hand-off

The board, path, and towers, by Ryoonki (`earl-kayy`). This page covers what you need to plug soldiers and rewards into them.

- `Assets/Shared/`: the combat contract your soldiers implement.
- `Assets/Defense/`: the board, path, and towers.

Please don't edit these two folders. If you need a change, ask me.

## Try it

Run `Tools > RK > Build Defense Sandbox`, then press Play. Dummy soldiers walk the path, towers shoot them, and the dummies hit the towers back.

## Put the board in your scene

1. Add an empty GameObject at the origin, and add a `BoardView` to it.
2. Set its Layout to `Assets/Defense/Data/DefaultStage.asset`.
3. Optional: add a `BoardCamera` to your camera and set its Board, so the whole board fits any screen.

The board builds itself when Play starts.

## Make a soldier

```csharp
using ReverseTD.Defense;
using ReverseTD.Shared;
using UnityEngine;

public class Soldier : MonoBehaviour, IDamageable, IPathProgress
{
    [SerializeField] private float speed = 1.5f;
    [SerializeField] private float hp = 60f;

    private BoardView board;
    private float distance;
    private bool isAlive = true;

    public Team Team => Team.Attacker;
    public bool IsAlive => isAlive;
    public Vector3 Position => transform.position;
    public float DistanceTraveled => distance;

    private void Awake() { board = FindAnyObjectByType<BoardView>(); } // or have your spawner pass it in

    // Towers only see registered soldiers.
    private void OnEnable() { CombatRegistry.Register(this); }
    private void OnDisable() { CombatRegistry.Unregister(this); }

    private void Update()
    {
        if (!isAlive) return;

        distance += speed * Time.deltaTime;
        transform.position = board.Path.GetPointAtDistance(distance);
        if (distance >= board.Path.TotalLength)
        {
            // Broke through: your game logic goes here.
        }
    }

    public void TakeDamage(float amount)
    {
        if (!isAlive) return;

        hp -= amount;
        if (hp <= 0f)
        {
            isAlive = false; // before Destroy, so towers stop targeting it right away
            Destroy(gameObject);
        }
    }
}
```

- `IPathProgress` is optional. Without it, towers target the soldier by distance instead of by how far along the path it is.
- Give soldier sprites `sortingOrder = SortingOrders.Soldier`, or they can end up hidden under the path.
- A complete working example: `Assets/Defense/Scripts/Testing/DummySoldier.cs`.

## Hit towers

Towers are `IDamageable` too (`Team.Defender`). Find them with `CombatRegistry.GetAlive(Team.Defender, list)`, then call `TakeDamage`.

If you keep a reference to a tower, check `CombatRegistry.IsAlive(tower)` before using it. Towers get destroyed, and `== null` on an interface doesn't notice.

## Rewards

- `Tower.Destroyed` (`Action<Tower>`): raised when a tower hits 0 HP, just before it's removed. Its position and `Definition` are still readable.
- `BoardView.AllTowersDestroyed` (`Action<BoardView>`): raised after the last tower's `Destroyed`.

Both events are static: subscribe in `OnEnable`, unsubscribe in `OnDisable`. Example: `Assets/Defense/Scripts/Testing/DefenseEventLogger.cs`.

## Tower stats

The tower stats in `Assets/Defense/Data/DefaultTower.asset` are placeholders (HP 100, range 3, damage 10, 1 shot per second). Tell me if they need to change.
