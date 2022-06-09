using BLD;
using BLD.Helpers;
using BLD.Interface;
using System;
using System.Collections;
using UnityEngine;
using Environment = BLD.Environment;
using WaitUntil = UnityEngine.WaitUntil;

public class ProfileHUDController : IHUD
{
    private readonly IUserProfileBridge userProfileBridge;

    [Serializable]
    public struct Configuration
    {
        public bool connectedWallet;
    }

    private const string URL_CLAIM_NAME = "https://builder.beland.io/claim-name";
    private const string URL_BEAN_INFO = "https://docs.beland.io/examples/get-a-wallet";
    private const string URL_BEAN_PURCHASE = "https://account.beland.io";
    private const string URL_TERMS_OF_USE = "https://beland.io/terms";
    private const string URL_PRIVACY_POLICY = "https://beland.io/privacy";
    private const float FETCH_BEAN_INTERVAL = 60;

    public readonly ProfileHUDView view;
    internal AvatarEditorHUDController avatarEditorHud;

    private UserProfile ownUserProfile => UserProfile.GetOwnUserProfile();
    private IMouseCatcher mouseCatcher;
    private Coroutine fetchBeanIntervalRoutine = null;

    public RectTransform tutorialTooltipReference { get => view.tutorialTooltipReference; }

    public event Action OnOpen;
    public event Action OnClose;

    public ProfileHUDController(IUserProfileBridge userProfileBridge)
    {
        this.userProfileBridge = userProfileBridge;
        mouseCatcher = SceneReferences.i?.mouseCatcher;


        view = UnityEngine.Object.Instantiate(GetViewPrefab()).GetComponent<ProfileHUDView>();
        view.name = "_ProfileHUD";

        CommonScriptableObjects.builderInWorldNotNecessaryUIVisibilityStatus.OnChange += ChangeVisibilityForBuilderInWorld;
        DataStore.i.exploreV2.profileCardIsOpen.OnChange += SetAsFullScreenMenuMode;

        view.connectedWalletSection.SetActive(false);
        view.nonConnectedWalletSection.SetActive(false);
        view.ActivateDescriptionEditionMode(false);

        view.buttonLogOut.onClick.AddListener(WebInterface.LogOut);
        view.buttonSignUp.onClick.AddListener(WebInterface.RedirectToSignUp);
        view.buttonClaimName.onClick.AddListener(() => WebInterface.OpenURL(URL_CLAIM_NAME));
        view.buttonTermsOfServiceForConnectedWallets.onPointerDown += () => WebInterface.OpenURL(URL_TERMS_OF_USE);
        view.buttonPrivacyPolicyForConnectedWallets.onPointerDown += () => WebInterface.OpenURL(URL_PRIVACY_POLICY);
        view.buttonTermsOfServiceForNonConnectedWallets.onPointerDown += () => WebInterface.OpenURL(URL_TERMS_OF_USE);
        view.buttonPrivacyPolicyForNonConnectedWallets.onPointerDown += () => WebInterface.OpenURL(URL_PRIVACY_POLICY);
        view.inputName.onSubmit.AddListener(UpdateProfileName);
        view.descriptionEditionInput.onSubmit.AddListener(UpdateProfileDescription);
        view.OnOpen += () =>
        {
            WebInterface.RequestOwnProfileUpdate();
            OnOpen?.Invoke();
        };
        view.OnClose += () => OnClose?.Invoke();

        if (view.beanCounterView)
        {
            view.beanCounterView.buttonBeanInfo.onPointerDown += () => WebInterface.OpenURL(URL_BEAN_INFO);
            view.beanCounterView.buttonBeanPurchase.onClick.AddListener(() => WebInterface.OpenURL(URL_BEAN_PURCHASE));
        }

        if (view.polygonBeanCounterView)
        {
            view.polygonBeanCounterView.buttonBeanInfo.onPointerDown += () => WebInterface.OpenURL(URL_BEAN_INFO);
            view.polygonBeanCounterView.buttonBeanPurchase.onClick.AddListener(() => WebInterface.OpenURL(URL_BEAN_PURCHASE));
        }

        ownUserProfile.OnUpdate += OnProfileUpdated;
        if (mouseCatcher != null)
            mouseCatcher.OnMouseLock += OnMouseLocked;

        if (!BLD.Configuration.EnvironmentSettings.RUNNING_TESTS)
        {
            KernelConfig.i.EnsureConfigInitialized().Then(config => OnKernelConfigChanged(config, null));
            KernelConfig.i.OnChange += OnKernelConfigChanged;
        }

        DataStore.i.exploreV2.isInitialized.OnChange += ExploreV2Changed;
        ExploreV2Changed(DataStore.i.exploreV2.isInitialized.Get(), false);
    }

    protected virtual GameObject GetViewPrefab()
    {
        return Resources.Load<GameObject>("ProfileHUD");
    }

    public void ChangeVisibilityForBuilderInWorld(bool current, bool previus) { view.gameObject.SetActive(current); }

    public void SetVisibility(bool visible)
    {
        view?.SetVisibility(visible);

        if (visible && fetchBeanIntervalRoutine == null)
        {
            fetchBeanIntervalRoutine = CoroutineStarter.Start(BeanIntervalRoutine());
        }
        else if (!visible && fetchBeanIntervalRoutine != null)
        {
            CoroutineStarter.Stop(fetchBeanIntervalRoutine);
            fetchBeanIntervalRoutine = null;
        }
    }

    public void Dispose()
    {
        if (fetchBeanIntervalRoutine != null)
        {
            CoroutineStarter.Stop(fetchBeanIntervalRoutine);
            fetchBeanIntervalRoutine = null;
        }

        if (view)
        {
            GameObject.Destroy(view.gameObject);
        }

        ownUserProfile.OnUpdate -= OnProfileUpdated;
        CommonScriptableObjects.builderInWorldNotNecessaryUIVisibilityStatus.OnChange -= ChangeVisibilityForBuilderInWorld;
        if (mouseCatcher != null)
            mouseCatcher.OnMouseLock -= OnMouseLocked;

        if (!BLD.Configuration.EnvironmentSettings.RUNNING_TESTS)
        {
            KernelConfig.i.OnChange -= OnKernelConfigChanged;
        }

        view.descriptionPreviewInput.onSubmit.RemoveListener(UpdateProfileDescription);
        DataStore.i.exploreV2.profileCardIsOpen.OnChange -= SetAsFullScreenMenuMode;

        DataStore.i.exploreV2.isInitialized.OnChange -= ExploreV2Changed;
    }

    void OnProfileUpdated(UserProfile profile) { view?.SetProfile(profile); }

    void OnMouseLocked() { HideProfileMenu(); }

    IEnumerator BeanIntervalRoutine()
    {
        while (true)
        {
            WebInterface.FetchBalanceOfBEAN();
            yield return WaitForSecondsCache.Get(FETCH_BEAN_INTERVAL);
        }
    }

    /// <summary>
    /// Set an amount of BEAN on the HUD.
    /// </summary>
    /// <param name="balance">Amount of BEAN.</param>
    public void SetBeanBalance(string balance) { view.beanCounterView?.SetBalance(balance); }
    /// <summary>
    /// Close the Profile menu.
    /// </summary>
    public void HideProfileMenu() { view?.HideMenu(); }

    private void UpdateProfileName(string newName)
    {
        if (view.inputName.wasCanceled)
            return;

        if (!view.IsValidAvatarName(newName))
        {
            view.inputName.ActivateInputField();
            return;
        }

        if (view != null)
        {
            view.SetProfileName(newName);
            view.ActivateProfileNameEditionMode(false);
        }

        userProfileBridge.SaveUnverifiedName(newName);
    }

    private void OnKernelConfigChanged(KernelConfigModel current, KernelConfigModel previous) { view?.SetNameRegex(current.profiles.nameValidRegex); }

    private void UpdateProfileDescription(string description)
    {
        if (view.descriptionEditionInput.wasCanceled
            || !ownUserProfile.hasConnectedWeb3
            || description.Length > view.descriptionEditionInput.characterLimit)
        {
            view.ActivateDescriptionEditionMode(false);
            return;
        }

        view.SetDescription(description);
        view.ActivateDescriptionEditionMode(false);
        userProfileBridge.SaveDescription(description);
    }

    private void SetAsFullScreenMenuMode(bool currentIsFullScreenMenuMode, bool previousIsFullScreenMenuMode)
    {
        view.SetCardAsFullScreenMenuMode(currentIsFullScreenMenuMode);

        if (currentIsFullScreenMenuMode != CommonScriptableObjects.isProfileHUDOpen.Get())
            view.ToggleMenu();
    }

    private void ExploreV2Changed(bool current, bool previous) { view.SetStartMenuButtonActive(current); }
}