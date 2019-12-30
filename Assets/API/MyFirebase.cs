using System.Collections;
using System.Collections.Generic;
using Firebase.Analytics;
using UnityEngine;

public class MyFirebase
{
    public enum EventButtonName
    {
        click_reward__get_rotate,
        click_play_in_home,
        click_replay_pause,
        click_replay_gameover,
        click_menu_pause,
        click_menu_gameover,
        click_on_sound,
        click_off_sound,
        click_on_music,
        click_off_music,
        click_moregame,
        click_rate_home,
        click_continue_pause,
        click_pause_button,
    }
    
    public enum UserProfile
    {
        get_level_bonus,
        get_new_highscore,
    }

    public static void LogButtonEvent(EventButtonName action)
    {
        FirebaseAnalytics.LogEvent("button_event", "click_button", action.ToString());
    }

    public static void LogUserProfile(UserProfile action)
    {
        FirebaseAnalytics.LogEvent("user_profile", "action", action.ToString());
    }
}