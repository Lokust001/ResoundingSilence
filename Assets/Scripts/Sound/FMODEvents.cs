using FMODUnity;
using UnityEngine;
public class FMODEvents : MonoBehaviour
{
    public static FMODEvents Instance;

    protected virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Start()
    {
        Debug.Log("penis");
    }

    [field: Header("BGM")]
    [field: SerializeField] public EventReference BGM { get; private set; }

    [field: Header("Enemy SFX")]
    [field: SerializeField] public EventReference step { get; private set; }

    [field: Header("Weapon SFX")]
    [field: SerializeField] public EventReference abilityRecharged { get; private set; }
    [field: SerializeField] public EventReference abilitySelect { get; private set; }
    [field: SerializeField] public EventReference crossbowFire { get; private set; }
    [field: SerializeField] public EventReference crossbowHit { get; private set; }
    [field: SerializeField] public EventReference crossbowReload { get; private set; }
    [field: SerializeField] public EventReference LifeSteal { get; private set; }
    [field: SerializeField] public EventReference ScytheSlash1 { get; private set; }
    [field: SerializeField] public EventReference ScytheSlash2 { get; private set; }
    [field: SerializeField] public EventReference SwapWeapons { get; private set; }

    [field: Header("Environment SFX")]
    [field: SerializeField] public EventReference Coin { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference Walk { get; private set; }
    [field: SerializeField] public EventReference Dash { get; private set; }
    [field: SerializeField] public EventReference TakeDamage { get; private set; }
    [field: SerializeField] public EventReference Heal { get; private set; }
    [field: SerializeField] public EventReference Interact { get; private set; }
    [field: SerializeField] public EventReference Levelup { get; private set; }
}
