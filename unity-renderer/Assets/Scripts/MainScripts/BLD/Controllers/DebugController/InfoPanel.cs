using TMPro;
using UnityEngine;

namespace BLD
{
    public class InfoPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI networkText;
        [SerializeField] private TextMeshProUGUI realmText;

        public void SetVisible(bool active)
        {
            gameObject.SetActive(active);
        }

        public void Setup(string network, string realm)
        {
            SetNetwork(network);
            SetRealm(realm);
        }

        public void SetNetwork(string network)
        {
            networkText.text = network;
        }

        public void SetRealm(string realm)
        {
            realmText.text = realm;
        }
    }
}