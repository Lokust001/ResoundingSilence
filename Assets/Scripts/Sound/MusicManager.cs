using FMOD.Studio;
using UnityEngine;
using System.Threading.Tasks;

public class MusicManager : BaseManager
{
    public static MusicManager Instance;

    private EventInstance BGM;

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

        BGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.BGM);

        //the following if-else block could be changed to a Switch statement -Josh
        StopAll();


        await Task.CompletedTask;
    }

    void StopAll()
    {
        //levelBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        BGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
