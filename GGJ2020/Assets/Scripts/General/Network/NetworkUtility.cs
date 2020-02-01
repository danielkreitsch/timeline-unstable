using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using GGJ2020.Game;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;
using Object = UnityEngine.Object;

public class NetworkUtility : MonoBehaviour
{
	public static object FromNetwork(string message)
	{
		var classname = message.Substring(0, message.IndexOf(':'));
		var data = message.Substring(message.IndexOf(':') + 1);
		Type t = Type.GetType(classname);
		return JsonUtility.FromJson(data, Type.GetType(classname));
	}


	public static string ToNetwork(object @object)
	{
		return @object.GetType().Name + ":" + JsonUtility.ToJson(@object);
	}
}
