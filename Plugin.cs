using BepInEx;
using Photon.Pun;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using System.IO;

namespace SelfTracker
{
    [BepInPlugin("org.elixir.self.tracker", "Self Tracker", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public void Start()
        {
            string folder = Path.Combine(Paths.GameRootPath, "SelfTracker");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string namePath = Path.Combine(folder, "name.txt");
            if (!File.Exists(namePath))
            {
                File.WriteAllText(namePath, "[REPLACE WITH NAME]");
            }
            else
                name = File.ReadAllText(namePath).Trim();

            string webhookPath = Path.Combine(folder, "webhook.txt");
            if (!File.Exists(webhookPath))
            {
                File.WriteAllText(webhookPath, "[REPLACE WITH WEBHOOK URL]");
            }
            else
                webhook = File.ReadAllText(webhookPath).Trim();

            SendWeb($"**{name}** has loaded into the game!");
        }

        int i = 0;
        public void Update()
        {
            if (PhotonNetwork.InRoom && i < 1)
            {
                i++;
                SendWeb($"**{name}** has joined code: **{PhotonNetwork.CurrentRoom.Name}**, Players In Lobby: " + PhotonNetwork.CurrentRoom.PlayerCount + "/10");
            }
            if (!PhotonNetwork.InRoom && i >= 1)
            {
                i = 0;
                SendWeb($"**{name}** has left the previous code");
            }
        }

        #region Webhook Sending
        public static void SendWeb(string str)
        {
            string jsonPayload = $"{{\"content\": \"{str}\"}}";

            GorillaTagger.Instance.StartCoroutine(SendWebhook(jsonPayload));
        }
        private static IEnumerator SendWebhook(string jsonPayload)
        {
            using (UnityWebRequest request = new UnityWebRequest(webhook, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                }
                else
                {
                }
            }
        }
        #endregion

        static string webhook;
        static string name;
    }
}
