using System;
using TMPro;
using System.Collections.Generic;
using BLD.Controllers;
using BLD.Helpers;
using UnityEngine;
using UnityEngine.UI;
using BLD.TransactionHUDModel;
using UnityEngine.SocialPlatforms.Impl;
using Type = BLD.TransactionHUDModel.Type;

public class TransactionHUD : MonoBehaviour, ITransactionHUD
{
    [SerializeField] private Button acceptButton;
    
    [SerializeField] private Button rejectButton;
    
    [SerializeField] private TMP_Text messageLabel;

    public Model model { get; private set; } = new Model();

    public event Action<ITransactionHUD> OnTransactionAccepted;
    
    public event Action<ITransactionHUD> OnTransactionRejected;

    private void OnEnable()
    {
        acceptButton.onClick.AddListener(AcceptTransaction);
        
        rejectButton.onClick.AddListener(RejectTransaction);
    }
    
    public IParcelScene FindScene(string sceneId)
    {
        if (BLD.Environment.i.world?.state?.scenesSortedByDistance != null)
        {
            foreach (IParcelScene scene in BLD.Environment.i.world.state.scenesSortedByDistance)
            {
                if (scene.sceneData.id == sceneId)
                    return scene;
            }
        }

        return null;
    }

    private void OnDisable()
    {
        acceptButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();
    }

    private static string ShortAddress(string address)
    {
        if (address == null)
            return "Null";

        if (address.Length >= 12)
            return $"{address.Substring(0, 6)}...{address.Substring(address.Length - 4)}";
        
        return address;
    }

    private void ShowSignMessage(Model model)
    {
        var scene = FindScene(model.sceneId);

        if (scene != null)
        {
            messageLabel.text = $"This scene {scene.sceneData.basePosition.ToString()} wants you to sign a message. Press ALLOW and then check your mobile wallet to confirm the transaction.";
        }
    }

    public void Show(Model model)
    {
        if (Utils.IsCursorLocked)
            Utils.UnlockCursor();

        this.model = model;
        
        ShowSignMessage(model);
    }

    public void AcceptTransaction()
    {
        OnTransactionAccepted?.Invoke(this);
        Destroy(gameObject);
    }

    public void RejectTransaction()
    {
        OnTransactionRejected?.Invoke(this);
        Destroy(gameObject);
    }
}