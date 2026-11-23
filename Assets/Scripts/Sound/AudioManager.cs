using System.Threading.Tasks;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : BaseManager
{
    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus vocalsBus;

    private float getPausedVolumeMultiplier = 1.0f;

    public static AudioManager Instance;

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

    //Sets AudioManager Instance in the scene
    protected void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        validateBus(masterBus);
        musicBus = RuntimeManager.GetBus("bus:/Music");
        validateBus(musicBus);
        sfxBus = RuntimeManager.GetBus("bus:/SoundEffects");
        validateBus(sfxBus);
        vocalsBus = RuntimeManager.GetBus("bus:/Vocals");
        validateBus(vocalsBus);

        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            //SettingsProfile.ReadSavedSettings();
            UpdateAllVolumes();
        }
    }

    public async override Awaitable InitManager()
    {
        await base.InitManager();

        UpdateAllVolumes();

        await setupPublicEvents();
    }

    public async Awaitable setupPublicEvents()
    {
        //updateAllVolumes += closemenu event at some point idk if we have one I coulnd't tell at first glace

        await Task.CompletedTask;
    }

    #region Volume Update Handling

    public void UpdateAllVolumes()
    {
        UpdateMasterVolume();
        UpdateMusicVolume();
        UpdateSFXVolume();
        UpdateVocalsVolume();
    }
    public void UpdateMasterVolume()
    {
        //masterBus.setVolume(SettingsProfile.MasterVolumeScaled * getPausedVolumeMultiplier);
    }

    public void UpdateMusicVolume()
    {
        //musicBus.setVolume(SettingsProfile.MusicVolumeScaled * getPausedVolumeMultiplier);
    }
    public void UpdateSFXVolume()
    {
        //sfxBus.setVolume(SettingsProfile.SFXVolumeScaled * getPausedVolumeMultiplier);
    }
    public void UpdateVocalsVolume()
    {
        //vocalsBus.setVolume(SettingsProfile.VocalsVolumeScaled * getPausedVolumeMultiplier);
    }

    #endregion

    //Plays a non-looping event WITHOUT 3d Attributes
    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    //Plays a non-looping event WITH 3d Attributes (has a spacializer)
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }

    //Sets EventInstance e's 3d attributes to a gameObject's transform + rigidbody
    //Should be called in e's update function
    public static void SetEventParameters(ref EventInstance e, Transform t, Rigidbody r)
    {
        RuntimeManager.AttachInstanceToGameObject(e, t.gameObject, r);
    }

    //Starts a looping sound effect from an ALREADY EXISTING INSTANCE
    //Music will have it's own script and functions for transitions
    //Not needed? Just play start function in each script
    /*public void StartLoopingSFX(EventInstance sound)
    {
        sound.start();
    }*/

    //Stops a looping sound effect
    //When fadeOut is true, allow for sound to finish
    //When fadeOut is false, stop sound immediately
    public void StopSFX(EventInstance sound, bool fadeOut)
    {
        if (fadeOut)
        {
            sound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
        else
        {
            sound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    private void OnDestroy()
    {

    }

    private void validateBus(Bus bus)
    {
        if (!bus.isValid())
        {
            Debug.LogError("Bus is not valid");
        }
    }
}

