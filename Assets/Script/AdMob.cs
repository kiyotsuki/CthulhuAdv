using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;

public class AdMob : MonoBehaviour
{
	private BannerView bannerView;
	// Use this for initialization
	void Start()
	{
		// 広告ユニット ID を記述します
		string adUnitId = "ca-app-pub-8131978555342020/3842877758"; // テスト用ID

		// Create a 320x50 banner at the top of the screen.
		bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);
		// Create an empty ad request.
		AdRequest request = new AdRequest.Builder().AddTestDevice("868135021361006").AddTestDevice("868135021392001").Build();
		
		// Load the banner with the request.
		bannerView.LoadAd(request);


	}
}