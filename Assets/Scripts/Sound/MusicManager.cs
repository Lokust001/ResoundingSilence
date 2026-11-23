using FMOD.Studio;
using UnityEngine;
using System.Threading.Tasks;

public class MusicManager : BaseManager
{
    public static MusicManager Instance;

    private EventInstance combatBGM;
    private EventInstance roamBGM;

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

    public async override Awaitable InitManager()
    {
        await base.InitManager();

        combatBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.CombatBGM);
        roamBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.RoamBGM);

        //the following if-else block could be changed to a Switch statement -Josh
        StopAll();


        await Task.CompletedTask;
    }

    void StopAll()
    {
        //levelBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        combatBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        roamBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
