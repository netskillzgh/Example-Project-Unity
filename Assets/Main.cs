using UnityEngine;
using Rollo.Client;

class Fake
{
    public int age = 0;

    public Fake(byte[] v)
    {
        age = 10;
    }
}

public class Main : MonoBehaviour
{
    async void Start()
    {
        // Subscribe to server notifications
        RolloNotificationCenter.Instance.AddEventListener(OpCodeList.Hello, (message) =>
        {
            // Handle packet here (in the game thread).
            var fake = message.GetObject<Fake>();
            Debug.Log("Hello from server " + fake.age);
        }, (v) =>
        {
            // Deserialize packet here (in the network thread to avoid blocking the game thread).
            return new Fake(v);
        });

        var result = await NetworkManager.Instance.Client.ConnectToServer("127.0.0.1", 6666);

        if (result)
        {
            var bytes = new byte[0]; // can be null as well, you can send empty packets.
                                     // NetworkManager.Instance.Client.Send(bytes, OpCodeList.Hello);

            // or for heavy operations
            NetworkManager.Instance.Client.SendWithThread(() =>
            {
                Debug.Log("Send from network thread");
                // prepare packet here to avoid blocking the game thread.
                var sent = NetworkManager.Instance.Client.Send(new byte[0], OpCodeList.Hello);

                if (!sent)
                {
                    Debug.LogError("Failed to send packet");
                }
                else
                {
                    Debug.Log("Packet sent");
                }
            });
        }
        else
        {
            Debug.LogError("Failed to connect to server");
        }
    }
}
