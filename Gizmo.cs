using System;
using UnityEngine;

public class Gizmo : MonoBehaviour
{
	private void OnDrawGizmos()
	{
		if (Camera.current && Vector3.Distance(Camera.current.transform.position, base.transform.position) < 7.5f)
		{
			Gizmos.DrawIcon(base.transform.position, this.Icons[(int)this.icon]);
		}
	}

	private string[] Icons = new string[]
	{
		"obsolete.tga",
		"choreo_scene.tga",
		"env_shake.tga",
		"env_fade.tga",
		"env_soundscape.tga",
		"env_spark.tga",
		"env_cubemap.tga",
		"env_texturetoggle.tga",
		"game_text.tga",
		"info_landmark.tga",
		"logic_auto.tga",
		"logic_branch.tga",
		"logic_case.tga",
		"logic_compare.tga",
		"logic_relay.tga",
		"logic_script.tga",
		"logic_timer.tga",
		"math_counter.tga",
		"phys_ballsocket.tga"
	};

	public Gizmo.SourceIcons icon;

	public enum SourceIcons
	{
		obsolete,
		choreo_scene,
		env_shake,
		env_fade,
		env_soundscape,
		env_spark,
		env_cubemap,
		env_texturetoggle,
		game_text,
		info_landmark,
		logic_auto,
		logic_branch,
		logic_case,
		logic_compare,
		logic_relay,
		logic_script,
		logic_timer,
		math_counter,
		phys_ballsocket
	}
}
