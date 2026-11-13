using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class ItemCollectedBannerController : MenuBase
{
    [SerializeField]
    private string item = string.Empty;
    private Queue<string> BannerQueue = new();

    private Coroutine bannerCoroutine = null;

    [SerializeField]
    private TMP_Text bannerText;

    RectTransform trans;

    private bool bannerIsMoving = false;

    [SerializeField]
    private AnimationClip animClip;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        trans = GetComponent<RectTransform>();
    }

    [Button]
    private void UpdateBannerQueue(string update = "TestUpdate")
    {
        if (update == "TestUpdate")
        {
            update = item;
        }

        BannerQueue.Enqueue(update);
        if (!bannerIsMoving)
        {
            StartBannerAnimPlaying();
        }
    }

    private void StartBannerAnimPlaying()
    {
        bannerIsMoving = true;
        UpdateBannerText(BannerQueue.Peek());
        anim.Play(animClip.name, 0, 0.0f);
    }

    private void bannerAnimDonePlaying()
    {
        if (BannerQueue.Count > 0)
        {
            BannerQueue.Dequeue();
        }

        if (BannerQueue.Count > 0)
        {
            StartBannerAnimPlaying();
            return;
        }

        bannerIsMoving = false;
    }

    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        UIPublicEvents.UpdateCollectionBanner += UpdateBannerQueue;
    }

    protected override void MenuClosedSucessfully()
    {
        base.MenuClosedSucessfully();
        UIPublicEvents.UpdateCollectionBanner -= UpdateBannerQueue;
    }
    private void UpdateBannerText(string s)
    {
        bannerText.text = $"Collected item: {s}";
    }
}
