using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions 
{
	public static Dictionary<T,U> InitializeDefaultValues<T, U>(this Dictionary<T, U> dictionary, bool InitializeNew) where T : System.Enum where U : new() {

		var enumValues = System.Enum.GetValues(typeof(T));

		if (dictionary == null) {
			Debug.Log("Dictionary is null. Make sure you assign the value somewhere.");
			dictionary = new Dictionary<T, U>();
		}
		foreach (var enumValue in enumValues) {
			if (InitializeNew) { dictionary.Add((T) enumValue, new U()); }
			else { dictionary.Add((T) enumValue, default(U)); }
		}

		return dictionary;
	}

	public static Dictionary<T, U> InitializeDefaultValues<T, U>(this Dictionary<T, U> dictionary) where T : System.Enum {

		var enumValues = System.Enum.GetValues(typeof(T));

		if (dictionary == null) {
			Debug.Log("Dictionary is null. Make sure you assign the value somewhere.");
			dictionary = new Dictionary<T, U>();
		}
		foreach (var enumValue in enumValues) { dictionary.Add((T) enumValue, default(U)); }

		return dictionary;
	}

	public static Dictionary<T, U> InitializeDefaultValues<T, U>(this Dictionary<T, U> dictionary, U defaultValue) where T : System.Enum {

		var enumValues = System.Enum.GetValues(typeof(T));

		if (dictionary == null) {
			Debug.Log("Dictionary is null. Make sure you assign the value somewhere.");
			dictionary = new Dictionary<T, U>();
		}
		foreach (var enumValue in enumValues) { dictionary.Add((T) enumValue, defaultValue); }

		return dictionary;
	}
}
