using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;
using WebSocketSharp.Net;

public class ServerCMD : MonoBehaviour
{
	const string CMDNAME_SERVERPORT = "-port";
	public static WebSocketServer wssv;

	void Awake()
	{
		#if UNITY_EDITOR
		string fromPort_str = "8080";
		#else
		string fromPort_str = GetArg(CMDNAME_SERVERPORT);
		#endif

		int fromPort;
		if (fromPort_str == null)
		{
			Debug.LogError("Error while starting websocket. Missing parameter \"" + CMDNAME_SERVERPORT + "\".");
			Application.Quit();
			return;
		}
		if (!int.TryParse(fromPort_str, out fromPort))
		{
			Debug.LogError("Error while starting websocket. Invalid port number: " + fromPort_str);
			Application.Quit();
			return;
		}

		// Websocket starten.
		wssv = new WebSocketServer(fromPort);
		wssv.AddWebSocketService<Connection>("/towerdefense");
		wssv.Start();
		Debug.Log("Server started on port: " + fromPort);
		wssv.WebSocketServices.TryGetServiceHost("/towerdefense", out Connection.service);
	}

	private static string GetArg(string name)
	{
		var args = System.Environment.GetCommandLineArgs();
		for (int i = 0; i < args.Length; i++) if (args[i] == name && args.Length > i + 1) return args[i + 1];
		return null;
	}

	void OnApplicationQuit()
	{
		if (wssv != null) wssv.Stop();
		Debug.Log("Server closed.");
	}
}