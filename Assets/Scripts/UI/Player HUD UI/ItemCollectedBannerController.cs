/*
* Author: Tyler
* Contributors:
* Last Modified: 11/18/2026
* Summary: Controls the banner that pops up when the player collects an item 
* To Do:   N/A
*/

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

    [SerializeField]
    private TMP_Text bannerText;

    RectTransform trans;

    private bool bannerIsMoving = false;

    [SerializeField]
    private AnimationClip animClip;

    private Animator anim;

    private float timeInAnimationSave;
    private int animHash;

    private float animSpeed;

    /// <summary>
    /// sets the right refs
    /// </summary>
    public override void InitMenu()
    {
        base.InitMenu();
        anim = GetComponent<Animator>();
        trans = GetComponent<RectTransform>();
    }

    /// <summary>
    /// Updates the banner queue with the param. Triggers the banner to start moving.
    /// </summary>
    /// <param name="update"></param>
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

    /// <summary>
    /// starts the banner moving
    /// </summary>
    private void StartBannerAnimPlaying()
    {
        bannerIsMoving = true;
        UpdateBannerText(BannerQueue.Peek());
        anim.Play(animClip.name, 0, 0.0f);
        animSpeed = anim.speed;
    }

    /// <summary>
    /// triggers when the animation is done playing
    /// </summary>
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

    /// <summary>
    /// plays the anim clip if it was in the middle of one
    /// </summary>
    protected override void MenuOpenedSucessfully()
    {
        base.MenuOpenedSucessfully();
        if (bannerIsMoving)
        {
            anim.Play(animHash, 0, timeInAnimationSave);
            anim.Update(0);
        }
    }

    /// <summary>
    /// public event subscription
    /// </summary>
    protected override void SetUpPublicEvents()
    {
        base.SetUpPublicEvents();
        UIPublicEvents.UpdateCollectionBanner += UpdateBannerQueue;
    }

    /// <summary>
    /// public event unsubscrtiption
    /// </summary>
    protected override void MenuClosedSucessfully()
    {
        base.MenuClosedSucessfully();
        UIPublicEvents.UpdateCollectionBanner -= UpdateBannerQueue;
        if (bannerIsMoving)
        {
            AnimatorStateInfo currentAnim = anim.GetCurrentAnimatorStateInfo(0);
            timeInAnimationSave = currentAnim.normalizedTime % 1f;
            animHash = currentAnim.fullPathHash;

        }
    }

    /// <summary>
    /// updates the text on the banner
    /// </summary>
    /// <param name="s"></param>
    private void UpdateBannerText(string s)
    {
        bannerText.text = $"Collected item: {s}";
    }
}
