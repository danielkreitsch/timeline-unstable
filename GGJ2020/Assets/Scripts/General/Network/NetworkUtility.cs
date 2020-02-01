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

namespace GGJ2020
{
	public class NetworkUtility : MonoBehaviour
	{
		public static object FromNetwork(string message)
		{
			var classname = message.Substring(0, message.IndexOf(':'));
			var data = message.Substring(message.IndexOf(':') + 1);
			Type t = Type.GetType("GGJ2020." + classname);
			Debug.Log("Class name: " + classname + ", type: " + t);
			return JsonUtility.FromJson(data, t);
		}


		public static string ToNetwork(object @object)
		{
			return @object.GetType().Name + ":" + JsonUtility.ToJson(@object);
		}
	}
}