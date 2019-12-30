using System;
using GoogleMobileAds.Api;
using UnityEngine;

[CreateAssetMenu(fileName = "ApiData", menuName = "API/ApiData")]
public class ApiData : ScriptableObject
{
    public AdsInfor AndroidInfor;
    public AdsInfor IosInfor;
}

[Serializable]
public class AdmobInfor
{
    public string AdmobBannerId;
    public string AdmobFullId;
    public string AdmobVideoId;

    public bool IsUseBanner = true;
    public bool IsUseFull = true;
    public bool IsUseVideo = true;

    public AdSize BannerSize = AdSize.Banner;
    public AdPosition BannerPosition = AdPosition.Bottom;
}

[Serializable]
public class AdsInfor
{
    public string StoreLink;
    public string GameLink;
    public AdmobInfor Admob;
    public string UnityVideoId;
}
